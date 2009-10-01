/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using DDW.Vex;
using System.IO;

namespace DDW.Xaml
{
	public class WPFRenderer : XamlRenderer
	{
		public override void GenerateXaml(VexObject v, out string xamlFileName)
		{
			this.v = v;
			this.Log = new StringBuilder();
			xamlFileName = Directory.GetCurrentDirectory() + "/" + v.Name + ".xaml";
			xw = new XamlWriter(xamlFileName, Encoding.UTF8);

			xw.WriteComment(headerComment);

#if(IS_TRIAL)
			// turn off watermarking for small files
			isWatermarking = true;
			xw.WriteComment(trialComment);
			if (v.Definitions.Count < 15)
			{
				isWatermarking = false;
			}
#else
			isWatermarking = false;
#endif

			xw.OpenHeaderTag(v.ViewPort.Size.Width, v.ViewPort.Size.Height, v.BackgroundColor);

			WriteDefinitions(v.Definitions, true, false);

			WriteTimelineDefiniton(v.Root, true);

			xw.CloseHeaderTag();

			xw.Close();
		}
		public override void GenerateXamlPart(VexObject v, IDefinition def, out string xamlFileName)
		{
			this.v = v;
			this.Log = new StringBuilder();
			xamlFileName = Directory.GetCurrentDirectory() + "/" + v.Name + "_" + def.Id + ".xaml";
			xw = new XamlWriter(xamlFileName, Encoding.UTF8);

			xw.WriteComment(headerComment);

#if(IS_TRIAL)
			// turn off watermarking for small files
			isWatermarking = true;
			xw.WriteComment(trialComment);
			if (v.Definitions.Count < 15)
			{
				isWatermarking = false;
			}
#else
			isWatermarking = false;
#endif

			xw.OpenHeaderTag(def.StrokeBounds.Size.Width, def.StrokeBounds.Size.Height, v.BackgroundColor);

			Dictionary<uint, IDefinition> defList = new Dictionary<uint, IDefinition>();
			defList.Add(1, def);
			WriteDefinitions(defList, true, true);

			//WriteTimelineDefiniton(v.Root, true);
			// Write a rectangle to hold this shape
			Instance inst = new Instance();
			inst.Name = instancePrefix + def.Id;
			inst.InstanceID = 1;
			inst.DefinitionId = def.Id;
            inst.Transformations.Add(new Transform(0, 1000, Matrix.Identitiy, 1, ColorTransform.Identity));
			WriteInstance(def, inst);

			xw.CloseHeaderTag();

			xw.Close();
		}

		public override void WriteTimelineDefiniton(Timeline timeline, bool isRoot)
		{
			if (isRoot)
			{
				//Instance rootInst = new Instance();
				//rootInst.DefinitionId = timeline.Id;
				//rootInst.Depth = 0;
				//rootInst.EndTime = timeline.Duration;
				//rootInst.Name = "_root";
				//rootInst.InstanceID = 0;
				//timelineStack.Push(rootInst);
				xw.OpenRootTag();
			}
			else
			{
				xw.OpenTimelineTag(instancePrefix + instName);//timelinePrefix + timeline.Id);
				Instance inst = timelineStack.Peek();
				if (inst.Transformations.Count > 0 && inst.Transformations[0].Matrix == Matrix.Empty)
				{
					xw.WriteAttributeString("Canvas.Left", "0");
					xw.WriteAttributeString("Canvas.Top", "0");
				}
				else
				{
					xw.WriteAttributeString("Canvas.Left", (-timeline.StrokeBounds.Point.X).ToString());
					xw.WriteAttributeString("Canvas.Top", (-timeline.StrokeBounds.Point.Y).ToString());
				}
			}
			timeline.Instances.Sort();
			WriteStoryboards(timeline.Instances, isRoot);
			WriteInstances(timeline.Instances, isRoot);

			xw.CloseFrameTag();
		}
		public void WriteDefinitions(Dictionary<uint, IDefinition> defs, bool isRoot, bool insurePartialImages)
		{
			xw.OpenResourcesTag();
			if (insurePartialImages)
			{
				AddImagesPart(defs, isRoot);
			}
			// write defs
			foreach (uint key in defs.Keys)
			{
				IDefinition def = defs[key];
				if (def is Symbol)
				{
					WriteSymbolDefinition((Symbol)def);
				}
				else if (def is Timeline)
				{
					//WriteTimelineDefiniton((Timeline)def, false);
				}
				else if (def is Image)
				{
					DefineImage((Image)def);
				}
				else if (def is Text)
				{
					// text is inlined
				}
				else
				{
					if (def != null)
					{
						Console.WriteLine("Non supported Vex element in Xaml: " + def.GetType());
					}
				}
			}
			// write watermark
			if (isWatermarking && isRoot)
			{
				WriteWatermarkDefinitions();
			}
			xw.CloseResourcesTag();
		}		
		public void WriteStoryboards(List<IInstance> instances, bool isRoot)
		{
			// write storyboard info
			
		  //<Canvas.Triggers>
		  //  <EventTrigger RoutedEvent="FrameworkElement.Loaded">
		  //    <BeginStoryboard>
		  //      <Storyboard SlipBehavior="Slip">					
		  //		...	
			//      </Storyboard>
		  //    </BeginStoryboard>
		  //  </EventTrigger>
		  //</Canvas.Triggers>
			xw.WriteStartElement("Canvas.Triggers");

			xw.WriteStartElement("EventTrigger");
			xw.WriteStartAttribute("RoutedEvent");
			xw.WriteValue("FrameworkElement.Loaded"); // MediaElement.Loaded
			xw.WriteEndAttribute();

			xw.WriteStartElement("BeginStoryboard");
			xw.WriteStartElement("Storyboard");

			xw.WriteStartAttribute("SlipBehavior");
			xw.WriteValue("Slip");
			xw.WriteEndAttribute();

			for (int i = 0; i < instances.Count; i++)
			{
				if (instances[i] == null)
				{
					continue;
				}
				instances[i].InstanceID = (uint)i;
				instName = GetInstanceName(instances[i]);// i + timelineSeparator + timelineStack.Peek().InstanceID;
				if (instances[i] is Instance)
				{
					Instance instance = (Instance)instances[i];
					if(v.Definitions.ContainsKey(instance.DefinitionId))
					{
						IDefinition def = v.Definitions[instance.DefinitionId];
						if (def != null && !instance.IsMask)
						{
							// write appear and disappear code for instance lifetime	
							WriteVisibility(instance);
							WriteStoryboard(def, instance);
						}
					}
				}
				else if (instances[i] is SoundInstance)
				{
					SoundInstance sound = (SoundInstance)instances[i];
					WriteSoundStoryboard(sound);
				}
			}

			// write watermark
			if (isWatermarking && isRoot)
			{
				WriteWatermarkStoryboards();
			}

			xw.WriteEndElement();
			xw.WriteEndElement();
			xw.WriteEndElement();
			xw.WriteEndElement();
		}
		public void WriteStoryboard(IDefinition s, Instance inst)
		{
			// <MatrixAnimationUsingKeyFrames 
			//		Storyboard.TargetName="inst_1" 
			//		Storyboard.TargetProperty="RenderTransform.Matrix" 
			//		RepeatBehavior="Forever" >				
			//   <DiscreteMatrixKeyFrame KeyTime="0" Value="1 0 0 1 0 110"/>
			//   <DiscreteMatrixKeyFrame KeyTime="0:0:0.25" Value="1 0 0 1 5 110"/>
			//   <DiscreteMatrixKeyFrame KeyTime="0:0:0.35" Value="1 0 0 1 30 110"/>	
			//   <DiscreteMatrixKeyFrame KeyTime="0:0:0.50" Value="1 0 0 1 35 110"/>	
			// </MatrixAnimationUsingKeyFrames>	

			Timeline curTL = (timelineStack.Count == 0) ?
				v.Root :
				(Timeline)v.Definitions[timelineStack.Peek().DefinitionId];

			// only write transforms if they exist
			// at least the Yahoo avatar tool will write no transforms, 
			// and then modify on the instance only
			bool hasOpacity = false;
			bool hasMatrix = true;
			if (inst.Transformations.Count > 0 && inst.Transformations[0].Matrix == Matrix.Empty)
			{
				hasMatrix = false;
			}

			if (hasMatrix)
			{
				xw.WriteStartElement("MatrixAnimationUsingKeyFrames");

				xw.WriteStartAttribute("Storyboard.TargetName");
				xw.WriteValue(instancePrefix + instName);
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Storyboard.TargetProperty");
				xw.WriteValue("RenderTransform.Matrix");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("RepeatBehavior");
				xw.WriteValue("Forever");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Duration");
				xw.WriteMilliseconds(curTL.Duration);
				xw.WriteEndAttribute();

				Rectangle r = s.StrokeBounds;
				for (int i = 0; i < inst.Transformations.Count; i++)
				{
					// <DiscreteMatrixKeyFrame KeyTime="0:0:0.25" Value="1 0 0 1 5 110"/>

					Transform t = inst.Transformations[i];

					if (t.HasAlpha())
					{
						hasOpacity = true;
					}

					if (t.HasMatrix()) // todo: make vex format hold all matrices, even when not transforming?
					{
						xw.WriteStartElement("DiscreteMatrixKeyFrame");

						xw.WriteStartAttribute("KeyTime");
						xw.WriteMilliseconds(t.StartTime);
						xw.WriteEndAttribute();

						Matrix m = t.Matrix;
						m.TranslateX += r.Point.X;
						m.TranslateY += r.Point.Y;

						xw.WriteStartAttribute("Value");
						xw.WriteMatrix(m);
						xw.WriteEndAttribute();

						xw.WriteEndElement();
					}
				}
				xw.WriteEndElement();
			}

			if (hasOpacity)
			{
				//<DoubleAnimationUsingKeyFrames 
				//Storyboard.TargetName="inst_0" 
				//Storyboard.TargetProperty="Opacity" 
				//RepeatBehavior="Forever" Duration="0:0:0.166">
				//  <DiscreteDoubleKeyFrame KeyTime="0:0:0" Value=".5" />
				//</DoubleAnimationUsingKeyFrames>

				xw.WriteStartElement("DoubleAnimationUsingKeyFrames");

				xw.WriteStartAttribute("Storyboard.TargetName");
				xw.WriteValue(instancePrefix + instName);
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Storyboard.TargetProperty");
				xw.WriteValue("Opacity");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("RepeatBehavior");
				xw.WriteValue("Forever");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Duration");
				xw.WriteMilliseconds(curTL.Duration);
				xw.WriteEndAttribute();

				float prevAlpha = -1;
				for (int i = 0; i < inst.Transformations.Count; i++)
				{
					// <DiscreteMatrixKeyFrame KeyTime="0:0:0.25" Value="1 0 0 1 5 110"/>

					Transform t = inst.Transformations[i];
					if (t.Alpha != prevAlpha)
					{
						xw.WriteStartElement("DiscreteDoubleKeyFrame");

						xw.WriteStartAttribute("KeyTime");
						xw.WriteMilliseconds(t.StartTime);
						xw.WriteEndAttribute();

						xw.WriteStartAttribute("Value");
						xw.WriteFloat(t.Alpha);
						xw.WriteEndAttribute();

						xw.WriteEndElement();

						prevAlpha = t.Alpha;
					}
				}

				xw.WriteEndElement();

			}
		}
		public void WriteVisibility(Instance inst)
		{
			//<ObjectAnimationUsingKeyFrames 
			//    Storyboard.TargetName="inst3" 
			//    Storyboard.TargetProperty="Visibility" 
			//    Duration="0:0:.8"
			//    RepeatBehavior="Forever">			 
			//    <DiscreteObjectKeyFrame KeyTime="0:0:.35"   Value="{x:Static Visibility.Visible}" /> 
			//</ObjectAnimationUsingKeyFrames>

			Timeline curTL = (timelineStack.Count == 0) ?
				v.Root :
				(Timeline)v.Definitions[timelineStack.Peek().DefinitionId];

			xw.WriteStartElement("ObjectAnimationUsingKeyFrames");

			xw.WriteStartAttribute("Storyboard.TargetName");
			xw.WriteValue(instancePrefix + instName);
			xw.WriteEndAttribute();

			xw.WriteStartAttribute("Storyboard.TargetProperty");
			xw.WriteValue("Visibility");
			xw.WriteEndAttribute();

			xw.WriteStartAttribute("Duration");
			xw.WriteMilliseconds(curTL.Duration);
			xw.WriteEndAttribute();

			xw.WriteStartAttribute("RepeatBehavior");
			xw.WriteValue("Forever");
			xw.WriteEndAttribute();

			if (inst.StartTime > 0)
			{
				xw.WriteStartElement("DiscreteObjectKeyFrame");
				xw.WriteStartAttribute("KeyTime");
				xw.WriteMilliseconds(0);
				xw.WriteEndAttribute();
				xw.WriteStartAttribute("Value");
				xw.WriteValue("{x:Static Visibility.Hidden}");
				xw.WriteEndAttribute();
				xw.WriteEndElement();
			}

			xw.WriteStartElement("DiscreteObjectKeyFrame");
			xw.WriteStartAttribute("KeyTime");
			xw.WriteMilliseconds(inst.StartTime);
			xw.WriteEndAttribute();
			xw.WriteStartAttribute("Value");
			xw.WriteValue("{x:Static Visibility.Visible}");
			xw.WriteEndAttribute();
			xw.WriteEndElement();

			xw.WriteStartElement("DiscreteObjectKeyFrame");
			xw.WriteStartAttribute("KeyTime");
			xw.WriteMilliseconds(inst.EndTime);
			xw.WriteEndAttribute();
			xw.WriteStartAttribute("Value");
			xw.WriteValue("{x:Static Visibility.Hidden}");
			xw.WriteEndAttribute();
			xw.WriteEndElement();

			xw.WriteEndElement();
		}
		public override void WriteInstance(IDefinition s, Instance inst)
		{
			// 	<Rectangle x:Name="inst1" Fill="{StaticResource VisualBrush2}" Width="100" Height="100" Canvas.Left="5" Canvas.Top="5"/>
			//string name = (inst.Name != null && inst.Name != "") ? inst.Name : instancePrefix + instName;

			xw.WriteStartElement("Rectangle");
			xw.WriteStartAttribute("x:Name");
			xw.WriteValue(instancePrefix + instName);
			xw.WriteEndAttribute();

			xw.WriteStartAttribute("Fill");
			xw.WriteString("{StaticResource " + visualBrushPrefix + inst.DefinitionId + "}");
			xw.WriteEndAttribute();

			xw.WriteStartAttribute("Width");
			xw.WriteValue(s.StrokeBounds.Size.Width);
			xw.WriteEndAttribute();

			xw.WriteStartAttribute("Height");
			xw.WriteValue(s.StrokeBounds.Size.Height);
			xw.WriteEndAttribute();

			//RenderTransformOrigin="1,1" 
			// bug from yahoo, width and hieght can be zero, when line with no linestyle is applied
			float tx = s.StrokeBounds.Size.Width == 0 ? 
				-s.StrokeBounds.Point.X : 
				-s.StrokeBounds.Point.X / s.StrokeBounds.Size.Width;
			float ty =  s.StrokeBounds.Size.Height == 0 ?
				-s.StrokeBounds.Point.Y : 
				-s.StrokeBounds.Point.Y / s.StrokeBounds.Size.Height;
			xw.WriteStartAttribute("RenderTransformOrigin");
			xw.WriteValue(tx + "," + ty);
			xw.WriteEndAttribute();

			//xw.WriteStartAttribute("Visibility");
			//xw.WriteValue("Hidden");
			//xw.WriteEndAttribute();

			xw.WriteEndElement();
		}
		public override void WriteSoundInstance(SoundInstance sound)
		{
			//<MediaElement Name="sndInst_0"/>

			xw.WriteStartElement("MediaElement");

			xw.WriteStartAttribute("Name");
			xw.WriteValue(VexObject.SoundPrefix + sound.DefinitionId);
			xw.WriteEndAttribute();

			xw.WriteEndElement();
		}
		public override void WriteSymbolDefinition(Symbol symbol)
		{
			string defName = GetDefinitionName(symbol);

			xw.OpenSymbolDefTag(defName, (int)symbol.StrokeBounds.Size.Width, (int)symbol.StrokeBounds.Size.Height);
			for (int i = 0; i < symbol.Shapes.Count; i++)
			{
				Shape sh = symbol.Shapes[i];
				RenderPath(sh.Fill, sh.Stroke, sh.ShapeData, false);
			}
			xw.CloseSymbolDefTag();

			xw.WriteVisualBrushRef(visualBrushPrefix + symbol.Id.ToString(), defName);
		}
		private void DefineImage(Image img)
		{
			string brushName = imageBrushPrefix + img.Id.ToString();
			xw.WriteImageBrushRef(brushName, img.Path); 
			imageBrushes.Add(img.Path, brushName);
			images.Add(img.Path, img);
		}

		/// <summary>
		/// This ensures nested image refs get defined when displaying only part of file
		/// </summary>
		/// <param name="defs"></param>
		/// <param name="isRoot"></param>
		public void AddImagesPart(Dictionary<uint, IDefinition> defs, bool isRoot)
		{
			uint imgCount = 0;
			foreach (uint key in defs.Keys)
			{
				IDefinition def = defs[key];
				if (defs[key] is Symbol)
				{
					Symbol sym = (Symbol)defs[key];
					for (int i = 0; i < sym.Shapes.Count; i++)
					{
						Shape sh = sym.Shapes[i];
						if (sh.Fill is ImageFill)
						{
							ImageFill imf = (ImageFill)sh.Fill;
							Image img = new Image(imf.ImagePath, imgCount++);
							string brushName = imageBrushPrefix + img.Id.ToString();
							xw.WriteImageBrushRef(brushName, img.Path);
							imageBrushes.Add(img.Path, brushName);
							images.Add(img.Path, img);
						}
					}
				}
			}
		}
	

	}
}
