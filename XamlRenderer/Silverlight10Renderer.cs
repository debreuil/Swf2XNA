/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using DDW.Vex;
using System.IO;

namespace DDW.Xaml
{
	public class Silverlight10Renderer : XamlRenderer
	{
		public const string SILVERLIGHT_SUFFIX = "_sl";

		public override void GenerateXaml(VexObject v, out string resultFileName)
		{
			this.v = v;
			this.Log = new StringBuilder();
			string baseFileName = Directory.GetCurrentDirectory() +
				"/" +
				v.Name +
				SILVERLIGHT_SUFFIX;
			baseFileName.Replace('\\', '/');

			resultFileName = baseFileName + ".html";
			string xamlFileName = baseFileName + ".xaml";

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

			AddImages(v.Definitions, true);

			WriteTimelineDefiniton(v.Root, true);

			xw.CloseHeaderTag();

			xw.Close();

			WriteSilverlightHtml(v, resultFileName);
		}
		public override void GenerateXamlPart(VexObject v, IDefinition def, out string resultFileName)
		{
			this.v = v;
			this.Log = new StringBuilder();
			string baseFileName = Directory.GetCurrentDirectory() +
				"/" +
				v.Name +
				SILVERLIGHT_SUFFIX +
				"_" + def.Id;

			resultFileName = baseFileName + ".html";
			string xamlFileName = baseFileName + ".xaml";

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
			AddImagesPart(defList, true);

			//WriteTimelineDefiniton(v.Root, true);
			// Write a rectangle to hold this shape
			Instance inst = new Instance();
			inst.Name = instancePrefix + def.Id;
			inst.InstanceId = 1;
			inst.DefinitionId = def.Id;
			inst.Transformations.Add(new Transform(0, 1000, Matrix.Identitiy, 1, ColorTransform.Identity));
			WriteInstance(def, inst, true);

			xw.CloseHeaderTag();

			xw.Close();

			WriteSilverlightHtml(v, resultFileName);
		}
		public override void WriteTimelineDefiniton(Timeline timeline, bool isRoot)
		{
			if (isRoot)
			{
				xw.OpenRootTag();
			}
			else
			{
				string tlName = timelinePrefix + instName;
				xw.OpenTimelineTag(tlName);//timelinePrefix + timeline.Id);
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
				WriteTransformsDefs(timeline, inst, tlName);
				WriteStoryboard(timeline, inst, tlName);
			}
			timeline.Instances.Sort();
			NumberInstances(timeline.Instances, isRoot);
			WriteInstances(timeline.Instances, isRoot);

			xw.CloseFrameTag();
		}
		public override void WriteInstance(IDefinition s, Instance inst)
		{
			WriteInstance(s, inst, false);
		}
		public void WriteInstance(IDefinition s, Instance inst, bool isPart)
		{
			if (s is Symbol)
			{
				Symbol symbol = (Symbol)s;
				string fullName = GetInstanceName(inst);

				OpenSymbolDefTag(fullName, symbol.StrokeBounds, isPart);

				// don't write transforms when just displaying part of file
				if (!isPart) 
				{
					// write transform defs
					WriteTransformsDefs(symbol, inst, fullName);

					// write Storyboard
					WriteStoryboard(symbol, inst, fullName);
				}
				// write paths
				WriteSymbolDefinition(symbol);

				CloseSymbolDefTag();
			}
			else if (s is SoundInstance)
			{
				WriteSoundInstance((SoundInstance)s);
			}
		}
		public override void WriteSoundInstance(SoundInstance sound)
		{
			// note: Silverlight doesn't support media element, 
			// so starting sounds at certain times is not possible without code

			xw.WriteStartElement("MediaElement");

			xw.WriteStartAttribute("Source");
			xw.WriteValue(sound.Path);
			xw.WriteEndAttribute();

			xw.WriteStartAttribute("Name");
			xw.WriteValue(VexObject.SoundPrefix + sound.DefinitionId);
			xw.WriteEndAttribute();

			xw.WriteEndElement();
		}
		public override void WriteTextBlock(Text txt, Instance inst)
		{
			xw.WriteStartElement("Canvas");

			if (inst != null)
			{
				string name = (inst.Name != null && inst.Name != "") ? inst.Name : instancePrefix + instName;
				string fullName = instancePrefix + instName;
				xw.WriteStartAttribute("x:Name");
				xw.WriteValue(fullName);
				xw.WriteEndAttribute();

				// write transform defs
				WriteTransformsDefs(v.Definitions[inst.DefinitionId], inst, fullName);

				// write Storyboard
				WriteStoryboard(v.Definitions[inst.DefinitionId], inst, fullName);
			}


			// ***** Can't use spans as that loses centering

			if (txt.TextRuns.Count != 0)
			{
				xw.WriteStartElement("TextBlock");
				for (int i = 0; i < txt.TextRuns.Count; i++)
				{
					TextRun tr = txt.TextRuns[i];

					if (i > 0 && tr.isContinuous)
					{
						xw.WriteStartElement("Span");

						WriteTextAttributes(tr);
						xw.WriteValue(tr.Text);

						xw.WriteEndElement();
					}
					else
					{
						if (i > 0)
						{
							xw.WriteEndElement();
							xw.WriteStartElement("TextBlock");
						}
						xw.WriteAttributeString("Canvas.Left", tr.Left.ToString());
						xw.WriteAttributeString("Canvas.Top", tr.Top.ToString());

						WriteTextAttributes(tr);
						xw.WriteValue(tr.Text);

					}
				}
				xw.WriteEndElement();
			}
			xw.WriteEndElement(); // Canvas
		}
		public override void WriteSymbolDefinition(Symbol symbol)
		{
			for (int i = 0; i < symbol.Shapes.Count; i++)
			{
				Shape sh = symbol.Shapes[i];
				RenderPath(sh.Fill, sh.Stroke, sh.ShapeData, true);
			}
		}
		private void WriteTransformsDefs(IDefinition s, Instance inst, string fullName)
		{
			//<Canvas.RenderTransform>
			//<TransformGroup>
			//  <RotateTransform x:Name="inst_rt_0r" Angle="0" />
			//  <SkewTransform x:Name="inst_rt_0k" AngleX="0" AngleY="0"/>
			//  <ScaleTransform x:Name="inst_rt_0s" ScaleX="1" ScaleY="1" />
			//  <TranslateTransform x:Name="inst_rt_0t" X="35" Y="26" />
			//</TransformGroup>
			//</Canvas.RenderTransform>  

			IDefinition def = v.Definitions[s.Id];
			//string defName = instancePrefix + GetInstanceName(inst);

			Rectangle r = s.StrokeBounds;
			Matrix m = inst.Transformations[0].Matrix;
			MatrixComponents mt = m.GetMatrixComponents();

			bool multiTransform = true;// inst.Transformations.Count > 1;
			bool hasRot = !(mt.Rotation == 0);
			bool hasSkew = !(mt.Shear == 0);
			bool hasScale = !(mt.ScaleX == 1 && mt.ScaleY == 1);
			bool hasTranslate = !(mt.TranslateX == 0 && mt.TranslateY == 0 && r.Point.X == 0 && r.Point.Y == 0);

			if (multiTransform || hasRot || hasSkew || hasScale || hasTranslate)
			{
				xw.WriteStartElement("Canvas.RenderTransform");
				xw.WriteStartElement("TransformGroup");

				if (multiTransform || hasRot)
				{
					xw.WriteStartElement("RotateTransform");
					xw.WriteStartAttribute("x:Name");
					xw.WriteValue(fullName + "r");
					xw.WriteEndAttribute();
					xw.WriteStartAttribute("Angle");
					//xw.WriteValue(mt.Rotation);
					xw.WriteValue(0);
					xw.WriteEndAttribute();
					xw.WriteEndElement();
				}

				if (multiTransform || hasSkew)
				{
					xw.WriteStartElement("SkewTransform");
					xw.WriteStartAttribute("x:Name");
					xw.WriteValue(fullName + "k");
					xw.WriteEndAttribute();
					xw.WriteStartAttribute("AngleX");
					//xw.WriteValue(mt.Shear);
					xw.WriteValue(0);
					xw.WriteEndAttribute();
					xw.WriteStartAttribute("AngleY");
					xw.WriteValue(0);
					xw.WriteEndAttribute();
					xw.WriteEndElement();
				}

				if (multiTransform || hasScale)
				{
					xw.WriteStartElement("ScaleTransform");
					xw.WriteStartAttribute("x:Name");
					xw.WriteValue(fullName + "s");
					xw.WriteEndAttribute();
					xw.WriteStartAttribute("ScaleX");
					//xw.WriteValue(mt.ScaleX);
					xw.WriteValue(1);
					xw.WriteEndAttribute();
					xw.WriteStartAttribute("ScaleY");
					//xw.WriteValue(mt.ScaleY);
					xw.WriteValue(1);
					xw.WriteEndAttribute();
					xw.WriteEndElement();
				}

				if (multiTransform || hasTranslate)
				{
					xw.WriteStartElement("TranslateTransform");
					xw.WriteStartAttribute("x:Name");
					xw.WriteValue(fullName + "t");
					xw.WriteEndAttribute();
					xw.WriteStartAttribute("X");
					//xw.WriteValue(mt.TranslateX + r.Point.X);
					xw.WriteValue(0);
					xw.WriteEndAttribute();
					xw.WriteStartAttribute("Y");
					//xw.WriteValue(mt.TranslateY + r.Point.Y);
					xw.WriteValue(0);
					xw.WriteEndAttribute();
					xw.WriteEndElement();
				}

				xw.WriteEndElement(); //TransformGroup
				xw.WriteEndElement(); //Canvas.RenderTransform
			}
		}
		public void NumberInstances(List<IInstance> instances, bool isRoot)
		{
			for (int i = 0; i < instances.Count; i++)
			{
				if (instances[i] == null)
				{
					continue;
				}
				instances[i].InstanceId = (uint)i;
			}
		}
		public void WriteStoryboard(IDefinition s, Instance inst, string fullName)
		{
			//<Canvas.Triggers>
			//  <EventTrigger RoutedEvent="Canvas.Loaded">
			//    <BeginStoryboard>
			//      <Storyboard>
			//        <DoubleAnimationUsingKeyFrames Storyboard.TargetName="def_6t" Storyboard.TargetProperty="X" Duration="0:0:10">
			//          <DiscreteDoubleKeyFrame Value="0" KeyTime="0:0:0" />
			//          <DiscreteDoubleKeyFrame Value="350" KeyTime="0:0:2" />
			//          <DiscreteDoubleKeyFrame Value="50" KeyTime="0:0:7" />
			//          <DiscreteDoubleKeyFrame Value="200" KeyTime="0:0:8" /> 
			//        </DoubleAnimationUsingKeyFrames>
			//      </Storyboard>
			//    </BeginStoryboard>
			//  </EventTrigger>
			//</Canvas.Triggers> 

			xw.WriteStartElement("Canvas.Triggers");

			xw.WriteStartElement("EventTrigger");
			xw.WriteStartAttribute("RoutedEvent");
			xw.WriteValue("Canvas.Loaded"); // MediaElement.Loaded
			xw.WriteEndAttribute();

			xw.WriteStartElement("BeginStoryboard");
			xw.WriteStartElement("Storyboard");

			// first write appear and disappear info
			// This is now done as opacity is written as Silverlight doesn't support 'Visible'
			// WriteVisibility(inst, fullName);

			// now the transform matrixes - note: silverlight, ugh, does not support matrixes : (
			Rectangle r = s.StrokeBounds;
			Matrix m;
			MatrixComponents mc;
			//string fullName = instancePrefix + GetInstanceName(inst);


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
				// <DoubleAnimationUsingKeyFrames Storyboard.TargetName="def_6t" Storyboard.TargetProperty="X" Duration="0:0:10">
				//   <DiscreteDoubleKeyFrame Value="0" KeyTime="0:0:0" />
				//   <DiscreteDoubleKeyFrame Value="350" KeyTime="0:0:2" />
				//   <DiscreteDoubleKeyFrame Value="50" KeyTime="0:0:7" />
				//   <DiscreteDoubleKeyFrame Value="200" KeyTime="0:0:8" /> 
				// </DoubleAnimationUsingKeyFrames>

				#region ScaleX
				// write scaleX
				xw.WriteStartElement("DoubleAnimationUsingKeyFrames");

				xw.WriteStartAttribute("Storyboard.TargetName");
				xw.WriteValue(fullName + "s");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Storyboard.TargetProperty");
				xw.WriteValue("ScaleX");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("RepeatBehavior");
				xw.WriteValue("Forever");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Duration");
				xw.WriteMilliseconds(curTL.Duration);
				xw.WriteEndAttribute();

				for (int i = 0; i < inst.Transformations.Count; i++)
				{
					Transform t = inst.Transformations[i];

					if (t.HasAlpha())
					{
						hasOpacity = true;
					}

					if (t.HasMatrix())
					{
						xw.WriteStartElement("DiscreteDoubleKeyFrame");

						xw.WriteStartAttribute("KeyTime");
						xw.WriteMilliseconds(t.StartTime);
						xw.WriteEndAttribute();

						m = t.Matrix;
						mc = m.GetMatrixComponents();

						xw.WriteStartAttribute("Value");
						xw.WriteFloat(mc.ScaleX);
						xw.WriteEndAttribute();

						xw.WriteEndElement();
					}
				}
				xw.WriteEndElement();
				#endregion
				#region ScaleY
				// write scaleY
				xw.WriteStartElement("DoubleAnimationUsingKeyFrames");

				xw.WriteStartAttribute("Storyboard.TargetName");
				xw.WriteValue(fullName + "s");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Storyboard.TargetProperty");
				xw.WriteValue("ScaleY");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("RepeatBehavior");
				xw.WriteValue("Forever");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Duration");
				xw.WriteMilliseconds(curTL.Duration);
				xw.WriteEndAttribute();

				for (int i = 0; i < inst.Transformations.Count; i++)
				{
					Transform t = inst.Transformations[i];

					if (t.HasAlpha())
					{
						hasOpacity = true;
					}

					if (t.HasMatrix())
					{
						xw.WriteStartElement("DiscreteDoubleKeyFrame");

						xw.WriteStartAttribute("KeyTime");
						xw.WriteMilliseconds(t.StartTime);
						xw.WriteEndAttribute();

						m = t.Matrix;
						mc = m.GetMatrixComponents();

						xw.WriteStartAttribute("Value");
						xw.WriteFloat(mc.ScaleY);
						xw.WriteEndAttribute();

						xw.WriteEndElement();
					}
				}
				xw.WriteEndElement();
				#endregion
				#region shear
				// write shears
				xw.WriteStartElement("DoubleAnimationUsingKeyFrames");

				xw.WriteStartAttribute("Storyboard.TargetName");
				xw.WriteValue(fullName + "k");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Storyboard.TargetProperty");
				xw.WriteValue("AngleX");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("RepeatBehavior");
				xw.WriteValue("Forever");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Duration");
				xw.WriteMilliseconds(curTL.Duration);
				xw.WriteEndAttribute();

				for (int i = 0; i < inst.Transformations.Count; i++)
				{
					Transform t = inst.Transformations[i];

					if (t.HasAlpha())
					{
						hasOpacity = true;
					}

					if (t.HasMatrix())
					{
						xw.WriteStartElement("DiscreteDoubleKeyFrame");

						xw.WriteStartAttribute("KeyTime");
						xw.WriteMilliseconds(t.StartTime);
						xw.WriteEndAttribute();

						m = t.Matrix;
						mc = m.GetMatrixComponents();

						xw.WriteStartAttribute("Value");
						xw.WriteFloat(mc.Shear);
						xw.WriteEndAttribute();

						xw.WriteEndElement();
					}
				}
				xw.WriteEndElement();
				#endregion
				#region rotate
				// write rotates
				xw.WriteStartElement("DoubleAnimationUsingKeyFrames");

				xw.WriteStartAttribute("Storyboard.TargetName");
				xw.WriteValue(fullName + "r");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Storyboard.TargetProperty");
				xw.WriteValue("Angle");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("RepeatBehavior");
				xw.WriteValue("Forever");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Duration");
				xw.WriteMilliseconds(curTL.Duration);
				xw.WriteEndAttribute();

				for (int i = 0; i < inst.Transformations.Count; i++)
				{
					Transform t = inst.Transformations[i];

					if (t.HasAlpha())
					{
						hasOpacity = true;
					}

					if (t.HasMatrix())
					{
						xw.WriteStartElement("DiscreteDoubleKeyFrame");

						xw.WriteStartAttribute("KeyTime");
						xw.WriteMilliseconds(t.StartTime);
						xw.WriteEndAttribute();

						m = t.Matrix;
						mc = m.GetMatrixComponents();

						xw.WriteStartAttribute("Value");
						xw.WriteFloat(mc.Rotation);
						xw.WriteEndAttribute();

						xw.WriteEndElement();
					}
				}
				xw.WriteEndElement();
				#endregion
				#region TranslateX
				// write TranslateX
				xw.WriteStartElement("DoubleAnimationUsingKeyFrames");

				xw.WriteStartAttribute("Storyboard.TargetName");
				xw.WriteValue(fullName + "t");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Storyboard.TargetProperty");
				xw.WriteValue("X");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("RepeatBehavior");
				xw.WriteValue("Forever");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Duration");
				xw.WriteMilliseconds(curTL.Duration);
				xw.WriteEndAttribute();

				for (int i = 0; i < inst.Transformations.Count; i++)
				{
					Transform t = inst.Transformations[i];

					if (t.HasAlpha())
					{
						hasOpacity = true;
					}

					if (t.HasMatrix())
					{
						xw.WriteStartElement("DiscreteDoubleKeyFrame");

						xw.WriteStartAttribute("KeyTime");
						xw.WriteMilliseconds(t.StartTime);
						xw.WriteEndAttribute();

						m = t.Matrix;
						mc = m.GetMatrixComponents();

						xw.WriteStartAttribute("Value");
						xw.WriteFloat(mc.TranslateX);// + r.Point.X);
						xw.WriteEndAttribute();

						xw.WriteEndElement();
					}
				}
				xw.WriteEndElement();
				#endregion
				#region TranslateY
				// write TranslateY
				xw.WriteStartElement("DoubleAnimationUsingKeyFrames");

				xw.WriteStartAttribute("Storyboard.TargetName");
				xw.WriteValue(fullName + "t");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Storyboard.TargetProperty");
				xw.WriteValue("Y");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("RepeatBehavior");
				xw.WriteValue("Forever");
				xw.WriteEndAttribute();

				xw.WriteStartAttribute("Duration");
				xw.WriteMilliseconds(curTL.Duration);
				xw.WriteEndAttribute();

				for (int i = 0; i < inst.Transformations.Count; i++)
				{
					Transform t = inst.Transformations[i];

					if (t.HasAlpha())
					{
						hasOpacity = true;
					}

					if (t.HasMatrix())
					{
						xw.WriteStartElement("DiscreteDoubleKeyFrame");

						xw.WriteStartAttribute("KeyTime");
						xw.WriteMilliseconds(t.StartTime);
						xw.WriteEndAttribute();

						m = t.Matrix;
						mc = m.GetMatrixComponents();

						xw.WriteStartAttribute("Value");
						xw.WriteFloat(mc.TranslateY);// + r.Point.Y);
						xw.WriteEndAttribute();

						xw.WriteEndElement();
					}
				}
				xw.WriteEndElement();
				#endregion
			}
			WriteVisibility(inst, fullName, hasOpacity);

			xw.WriteEndElement();
			xw.WriteEndElement();
			xw.WriteEndElement();
			xw.WriteEndElement();
		}
		public void WriteVisibility(Instance inst, string fullName, bool hasOpacity)
		{
			// Note: WriteVisibility visible and opacity in this pass

			//<DoubleAnimationUsingKeyFrames 
			//Storyboard.TargetName="inst_0" 
			//Storyboard.TargetProperty="Opacity" 
			//RepeatBehavior="Forever" Duration="0:0:0.166">
			//  <DiscreteDoubleKeyFrame KeyTime="0:0:0" Value=".5" />
			//</DoubleAnimationUsingKeyFrames>

			Timeline curTL;
			if (timelineStack.Count == 0)
			{
				curTL = v.Root;
			}
			else if (v.Definitions[inst.DefinitionId] is Timeline && timelineStack.Count == 1)
			{
				curTL = v.Root;
			}
			else if (v.Definitions[inst.DefinitionId] is Timeline && timelineStack.Count > 1)
			{
				Instance temp = timelineStack.Pop();
				curTL = (Timeline)v.Definitions[timelineStack.Peek().DefinitionId];
				timelineStack.Push(temp);
			}
			else
			{
				curTL = (Timeline)v.Definitions[timelineStack.Peek().DefinitionId];
			}

			xw.WriteStartElement("DoubleAnimationUsingKeyFrames");

			xw.WriteStartAttribute("Storyboard.TargetName");
			xw.WriteValue(fullName);
			xw.WriteEndAttribute();

			xw.WriteStartAttribute("Storyboard.TargetProperty");
			xw.WriteValue("Opacity");
			xw.WriteEndAttribute();

			xw.WriteStartAttribute("Duration");
			xw.WriteMilliseconds(curTL.Duration);
			xw.WriteEndAttribute();

			xw.WriteStartAttribute("RepeatBehavior");
			xw.WriteValue("Forever");
			xw.WriteEndAttribute();

			if (inst.StartTime > 0)
			{
				xw.WriteStartElement("DiscreteDoubleKeyFrame");
				xw.WriteStartAttribute("KeyTime");
				xw.WriteMilliseconds(0);
				xw.WriteEndAttribute();
				xw.WriteStartAttribute("Value");
				xw.WriteValue(0);
				xw.WriteEndAttribute();
				xw.WriteEndElement();
			}

			if (hasOpacity)
			{
				float prevAlpha = -1;
				for (int i = 0; i < inst.Transformations.Count; i++)
				{
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
			}
			else
			{
				xw.WriteStartElement("DiscreteDoubleKeyFrame");
				xw.WriteStartAttribute("KeyTime");
				xw.WriteMilliseconds(inst.StartTime);
				xw.WriteEndAttribute();
				xw.WriteStartAttribute("Value");
				xw.WriteValue(1);
				xw.WriteEndAttribute();
				xw.WriteEndElement();
			}

			xw.WriteStartElement("DiscreteDoubleKeyFrame");
			xw.WriteStartAttribute("KeyTime");
			xw.WriteMilliseconds(inst.EndTime);
			xw.WriteEndAttribute();
			xw.WriteStartAttribute("Value");
			xw.WriteValue(0);
			xw.WriteEndAttribute();
			xw.WriteEndElement();

			xw.WriteEndElement();
		}
		public void AddImages(Dictionary<uint, IDefinition> defs, bool isRoot)
		{
			foreach (uint key in defs.Keys)
			{
				IDefinition def = defs[key];
				if (def is Image)
				{
					Image img = (Image)def;
					string brushName = imageBrushPrefix + img.Id.ToString();
					imageBrushes.Add(img.Path, brushName);
					images.Add(img.Path, img);
				}
			}
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
							imageBrushes.Add(img.Path, brushName);
							images.Add(img.Path, img);
						}
					}
				}
			}
		}
		public void OpenSymbolDefTag(string key, Rectangle r, bool isPart)
		{
			// <Canvas Width="10" Height="10" RenderTransformOrigin="0,0" Canvas.Left="0" Canvas.Top="0">
			xw.WriteStartElement("Canvas");

			xw.WriteStartAttribute("x:Name");
			xw.WriteValue(key);
			xw.WriteEndAttribute();

			xw.WriteStartAttribute("Width");
			xw.WriteValue(r.Size.Width);
			xw.WriteEndAttribute();

			xw.WriteStartAttribute("Height");
			xw.WriteValue(r.Size.Height);
			xw.WriteEndAttribute();
			
			//string rt = (-r.Point.X).ToString() + "," + (-r.Point.Y).ToString();
			xw.WriteAttributeString("RenderTransformOrigin", "0,0");
			if (isPart)
			{
				xw.WriteAttributeString("Canvas.Left", (-r.Point.X).ToString());
				xw.WriteAttributeString("Canvas.Top", (-r.Point.Y).ToString());
			}
			else
			{
				xw.WriteAttributeString("Canvas.Left", "0");//r.Point.X.ToString());
				xw.WriteAttributeString("Canvas.Top", "0");//r.Point.Y.ToString());
			}
		}
		public void CloseSymbolDefTag()
		{
			xw.WriteEndElement();
		}
		protected void WriteSilverlightHtml(VexObject v, string fileName)
		{
			using (TextWriter tw = new StreamWriter(fileName))
			{
				tw.WriteLine(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
				tw.WriteLine(@"<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"">");
				tw.WriteLine(@" <head>");
				tw.WriteLine(@"   <title>Silverlight</title>");
				tw.WriteLine(@"   <script type=""text/javascript"">");
				tw.WriteLine(@"    if(!window.Silverlight)window.Silverlight={};Silverlight._silverlightCount=0;Silverlight.ua=null;Silverlight.available=false;Silverlight.fwlinkRoot=""http://go.microsoft.com/fwlink/?LinkID="";Silverlight.StatusText=""Get Microsoft Silverlight"";Silverlight.EmptyText="""";Silverlight.detectUserAgent=function(){var a=window.navigator.userAgent;Silverlight.ua={OS:""Unsupported"",Browser:""Unsupported""};if(a.indexOf(""Windows NT"")>=0)Silverlight.ua.OS=""Windows"";else if(a.indexOf(""PPC Mac OS X"")>=0)Silverlight.ua.OS=""MacPPC"";else if(a.indexOf(""Intel Mac OS X"")>=0)Silverlight.ua.OS=""MacIntel"";if(Silverlight.ua.OS!=""Unsupported"")if(a.indexOf(""MSIE"")>=0){if(navigator.userAgent.indexOf(""Win64"")==-1)if(parseInt(a.split(""MSIE"")[1])>=6)Silverlight.ua.Browser=""MSIE""}else if(a.indexOf(""Firefox"")>=0){var b=a.split(""Firefox/"")[1].split("".""),c=parseInt(b[0]);if(c>=2)Silverlight.ua.Browser=""Firefox"";else{var d=parseInt(b[1]);if(c==1&&d>=5)Silverlight.ua.Browser=""Firefox""}}else if(a.indexOf(""Safari"")>=0)Silverlight.ua.Browser=""Safari""};Silverlight.detectUserAgent();Silverlight.isInstalled=function(d){var c=false,a=null;try{var b=null;if(Silverlight.ua.Browser==""MSIE"")b=new ActiveXObject(""AgControl.AgControl"");else if(navigator.plugins[""Silverlight Plug-In""]){a=document.createElement(""div"");document.body.appendChild(a);a.innerHTML='<embed type=""application/x-silverlight"" />';b=a.childNodes[0]}if(b.IsVersionSupported(d))c=true;b=null;Silverlight.available=true}catch(e){c=false}if(a)document.body.removeChild(a);return c};Silverlight.createObject=function(l,g,m,j,k,i,h){var b={},a=j,c=k;a.source=l;b.parentElement=g;b.id=Silverlight.HtmlAttributeEncode(m);b.width=Silverlight.HtmlAttributeEncode(a.width);b.height=Silverlight.HtmlAttributeEncode(a.height);b.ignoreBrowserVer=Boolean(a.ignoreBrowserVer);b.inplaceInstallPrompt=Boolean(a.inplaceInstallPrompt);var e=a.version.split(""."");b.shortVer=e[0]+"".""+e[1];b.version=a.version;a.initParams=i;a.windowless=a.isWindowless;a.maxFramerate=a.framerate;for(var d in c)if(c[d]&&d!=""onLoad""&&d!=""onError""){a[d]=c[d];c[d]=null}delete a.width;delete a.height;delete a.id;delete a.onLoad;delete a.onError;delete a.ignoreBrowserVer;delete a.inplaceInstallPrompt;delete a.version;delete a.isWindowless;delete a.framerate;if(Silverlight.isInstalled(b.version)){if(Silverlight._silverlightCount==0)if(window.addEventListener)window.addEventListener(""onunload"",Silverlight.__cleanup,false);else window.attachEvent(""onunload"",Silverlight.__cleanup);var f=Silverlight._silverlightCount++;a.onLoad=""__slLoad""+f;a.onError=""__slError""+f;window[a.onLoad]=function(a){if(c.onLoad)c.onLoad(document.getElementById(b.id),h,a)};window[a.onError]=function(a,b){if(c.onError)c.onError(a,b);else Silverlight.default_error_handler(a,b)};slPluginHTML=Silverlight.buildHTML(b,a)}else slPluginHTML=Silverlight.buildPromptHTML(b);if(b.parentElement)b.parentElement.innerHTML=slPluginHTML;else return slPluginHTML};Silverlight.supportedUserAgent=function(){var a=Silverlight.ua,b=a.OS==""Unsupported""||a.Browser==""Unsupported""||a.OS==""Windows""&&a.Browser==""Safari""||a.OS.indexOf(""Mac"")>=0&&a.Browser==""IE"";return !b};Silverlight.buildHTML=function(c,d){var a=[],e,i,g,f,h;if(Silverlight.ua.Browser==""Safari""){a.push(""<embed "");e="""";i="" "";g='=""';f='""';h=' type=""application/x-silverlight""/>'+""<iframe style='visibility:hidden;height:0;width:0'/>""}else{a.push('<object type=""application/x-silverlight""');e="">"";i=' <param name=""';g='"" value=""';f='"" />';h=""</object>""}a.push(' id=""'+c.id+'"" width=""'+c.width+'"" height=""'+c.height+'"" '+e);for(var b in d)if(d[b])a.push(i+Silverlight.HtmlAttributeEncode(b)+g+Silverlight.HtmlAttributeEncode(d[b])+f);a.push(h);return a.join("""")};Silverlight.default_error_handler=function(e,b){var d,c=b.ErrorType;d=b.ErrorCode;var a=""\nSilverlight error message     \n"";a+=""ErrorCode: ""+d+""\n"";a+=""ErrorType: ""+c+""       \n"";a+=""Message: ""+b.ErrorMessage+""     \n"";if(c==""ParserError""){a+=""XamlFile: ""+b.xamlFile+""     \n"";a+=""Line: ""+b.lineNumber+""     \n"";a+=""Position: ""+b.charPosition+""     \n""}else if(c==""RuntimeError""){if(b.lineNumber!=0){a+=""Line: ""+b.lineNumber+""     \n"";a+=""Position: ""+b.charPosition+""     \n""}a+=""MethodName: ""+b.methodName+""     \n""}alert(a)};Silverlight.createObjectEx=function(b){var a=b,c=Silverlight.createObject(a.source,a.parentElement,a.id,a.properties,a.events,a.initParams,a.context);if(a.parentElement==null)return c};Silverlight.buildPromptHTML=function(i){var a=null,f=Silverlight.fwlinkRoot,c=Silverlight.ua.OS,b=""92822"",d;if(i.inplaceInstallPrompt){var h;if(Silverlight.available){d=""94376"";h=""94382""}else{d=""92802"";h=""94381""}var g=""93481"",e=""93483"";if(c==""Windows""){b=""92799"";g=""92803"";e=""92805""}else if(c==""MacIntel""){b=""92808"";g=""92804"";e=""92806""}else if(c==""MacPPC""){b=""92807"";g=""92815"";e=""92816""}a='<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""205px""><tr><td><img title=""Get Microsoft Silverlight"" onclick=""javascript:Silverlight.followFWLink({0});"" style=""border:0; cursor:pointer"" src=""{1}""/></td></tr><tr><td style=""background:#C7C7BD; text-align: center; color: black; font-family: Verdana; font-size: 9px; padding-bottom: 0.05cm; ;padding-top: 0.05cm"" >By clicking <b>Get Microsoft Silverlight</b> you accept the <a title=""Silverlight License Agreement"" href=""{2}"" target=""_top"" style=""text-decoration: underline; color: #36A6C6""><b>Silverlight license agreement</b></a>.</td></tr><tr><td style=""border-left-style: solid; border-right-style: solid; border-width: 2px; border-color:#c7c7bd; background: #817d77; color: #FFFFFF; text-align: center; font-family: Verdana; font-size: 9px"">Silverlight updates automatically, <a title=""Silverlight Privacy Statement"" href=""{3}"" target=""_top"" style=""text-decoration: underline; color: #36A6C6""><b>learn more</b></a>.</td></tr><tr><td><img src=""{4}""/></td></tr></table>';a=a.replace(""{2}"",f+g);a=a.replace(""{3}"",f+e);a=a.replace(""{4}"",f+h)}else{if(Silverlight.available)d=""94377"";else d=""92801"";if(c==""Windows"")b=""92800"";else if(c==""MacIntel"")b=""92812"";else if(c==""MacPPC"")b=""92811"";a='<div style=""width: 205px; height: 67px; background-color: #FFFFFF""><img onclick=""javascript:Silverlight.followFWLink({0});"" style=""border:0; cursor:pointer"" src=""{1}"" alt=""Get Microsoft Silverlight""/></div>'}a=a.replace(""{0}"",b);a=a.replace(""{1}"",f+d);return a};Silverlight.__cleanup=function(){for(var a=Silverlight._silverlightCount-1;a>=0;a--){window[""__slLoad""+a]=null;window[""__slError""+a]=null}if(window.removeEventListener)window.removeEventListener(""unload"",Silverlight.__cleanup,false);else window.detachEvent(""onunload"",Silverlight.__cleanup)};Silverlight.followFWLink=function(a){top.location=Silverlight.fwlinkRoot+String(a)};Silverlight.HtmlAttributeEncode=function(c){var a,b="""";if(c==null)return null;for(var d=0;d<c.length;d++){a=c.charCodeAt(d);if(a>96&&a<123||a>64&&a<91||a>43&&a<58&&a!=47||a==95)b=b+String.fromCharCode(a);else b=b+""&#""+a+"";""}return b}");

				tw.Write(@"    function createMySilverlightPlugin(){Silverlight.createObject(""");
				int index = fileName.LastIndexOf(v.Name);
				string xamlName = fileName.Substring(index);
				xamlName = xamlName.Replace(".html", ".xaml");
				tw.Write(xamlName); // name
				tw.Write(@""",parentElement,""mySilverlightPlugin"",{width:'");
				tw.Write(v.ViewPort.Size.Width);			// width
				tw.Write(@"',height:'");
				tw.Write(v.ViewPort.Size.Height);			// height
				tw.Write(@"',inplaceInstallPrompt:true,background:'#");
				tw.Write(v.BackgroundColor.Value.ToString("X6"));		// color
				tw.Write(@"',isWindowless:'false',framerate:'");
				tw.Write(v.FrameRate);			// frame rate
				tw.WriteLine(@"',version:'1.0'},{onError:null,onLoad:null},null);}");

				tw.WriteLine(@"   </script>");
				tw.WriteLine(@" </head>");
				tw.WriteLine(@" <body>");
				tw.WriteLine(@"	 <div id=""mySilverlightPluginHost""> </div>");
				tw.WriteLine(@"	 <script type=""text/javascript"">");
				tw.WriteLine(@"	 var parentElement = document.getElementById(""mySilverlightPluginHost"");");
				tw.WriteLine(@"	 createMySilverlightPlugin();");
				tw.WriteLine(@"	 </script>");
				tw.WriteLine(@" </body>");
				tw.WriteLine(@"</html>");
			}

		}

	}
}
