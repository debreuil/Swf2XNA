/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using DDW.Vex;

namespace DDW.Xaml
{
	public class XamlWriter : XmlTextWriter
	{
		private string rootElement = "Canvas";

		public XamlWriter(string filename, Encoding encoding) : base(filename, encoding)
		{
			this.Formatting = Formatting.Indented;
			this.Indentation = 2;
			this.Namespaces = true;
		}
		public XamlWriter(TextWriter tw) : base(tw)
		{
			this.Formatting = Formatting.Indented;
			this.Indentation = 2;
			this.Namespaces = true;
		}

		public void OpenHeaderTag(float width, float height, Color background)
		{
			// <Canvas  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
			//			Width="100" Height="100" 
			//			xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
			//			Background="#000000">

			uint titlePixels = 0;// xaml window title bar

			rootElement = "Canvas";
			//titlePixels = 30;

			this.WriteStartElement(rootElement);

			this.WriteAttributeString("xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
			this.WriteAttributeString("xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml");

			this.WriteStartAttribute("Width");
			this.WriteValue(width);
			this.WriteEndAttribute();

			this.WriteStartAttribute("Height");
			this.WriteValue(height + titlePixels);
			this.WriteEndAttribute();

			this.WriteStartAttribute("Background");
			this.WriteColor(background);
			this.WriteEndAttribute();

			// add clipping rect
			//  <Canvas.Clip>
			//	  <RectangleGeometry Rect="0,0 720 570" />
			//  </Canvas.Clip>
			this.WriteStartElement(rootElement + ".Clip");

			this.WriteStartElement("RectangleGeometry");
			this.WriteStartAttribute("Rect");
			this.WriteValue("0,0 " + width + " " + (height - titlePixels));
			this.WriteEndAttribute();
			this.WriteEndElement();
			this.WriteEndElement();

		}
		public void CloseHeaderTag()
		{
			this.WriteEndElement();
			this.Flush();
		}

		public void OpenResourcesTag()
		{
			this.WriteStartElement(rootElement + ".Resources");
		}
		public void CloseResourcesTag()
		{
			this.WriteEndElement();
			this.Flush();
		}

		public void OpenSymbolDefTag(string key, int width, int height)
		{
			// <Canvas Width="10" Height="10" RenderTransformOrigin="0,0" Canvas.Left="0" Canvas.Top="0">
			this.WriteStartElement("Canvas");

			this.WriteStartAttribute("x:Key");
			this.WriteValue(key);
			this.WriteEndAttribute();

			this.WriteStartAttribute("Width");
			this.WriteValue(width);
			this.WriteEndAttribute();

			this.WriteStartAttribute("Height");
			this.WriteValue(height);
			this.WriteEndAttribute();

			this.WriteAttributeString("RenderTransformOrigin", "0,0");
			this.WriteAttributeString("Canvas.Left", "0");
			this.WriteAttributeString("Canvas.Top", "0");
		}
		public void CloseSymbolDefTag()
		{
			this.WriteEndElement();
		}

		public void OpenRootTag()
		{
			this.WriteStartElement("Canvas");

			this.WriteStartAttribute("x:Name");
			this.WriteValue("_root");
			this.WriteEndAttribute();
		}
		public void OpenTimelineTag(string sprite)
		{
			this.WriteStartElement("Canvas");

			this.WriteStartAttribute("x:Name");
			this.WriteValue(sprite);
			this.WriteEndAttribute();
		}
		public void CloseFrameTag()
		{
			this.WriteEndElement();
		}

		public void WriteVisualBrushRef(string key, string def)
		{
			// <VisualBrush x:Key="VisualBrush2" Visual="{Binding Source={StaticResource def2}}"/>
			this.WriteStartElement("VisualBrush");

			this.WriteStartAttribute("x:Key");
			this.WriteValue(key);
			this.WriteEndAttribute();

			this.WriteStartAttribute("Visual");
			this.WriteValue("{Binding Source={StaticResource " + def + "}}");
			this.WriteEndAttribute();

			this.WriteEndElement();
		}
		public void WriteImageBrushRef(string key, string name)
		{
			// <ImageBrush x:Key="ib_1" ImageSource="bmp_1.jpg"/>
			this.WriteStartElement("ImageBrush");

			this.WriteStartAttribute("x:Key");
			this.WriteValue(key);
			this.WriteEndAttribute();

			this.WriteStartAttribute("ImageSource");
			this.WriteValue(name);
			this.WriteEndAttribute();

			this.WriteEndElement();
		}

		public void WriteMilliseconds(uint ms)
		{
			// "0:0:0.25"   h/m/s/fs
			uint h = (uint)(ms / (60 * 60 * 1000));
			ms -= h * (60 * 60 * 1000);
			uint m = (uint)(ms / (60 * 1000));
			ms -= m * (60 * 1000);
			float s = ms / 1000F;
			this.WriteString(h + ":" + m + ":" + s);
		}

		public void WriteMatrix(Matrix mx)
		{
			this.WriteString(
				mx.ScaleX + " " + 
				mx.Rotate0 + " " + 
				mx.Rotate1 + " " + 
				mx.ScaleY + " " + 
				mx.TranslateX + " " +
				mx.TranslateY);
		}
		public void WriteColor(Color color)
		{
			this.WriteString("#" + color.ARGB.ToString("X8"));
		}
		public void WriteGradientColor(GradientFill gf)
		{
			//this.WriteString("#" + gf.Color.ARGB.ToString("X8"));
		}

		public void WriteFloat(float f)
		{
			this.WriteString(f.ToString("f3"));
		}

		public void WritePoint(Point pt)
		{
			WriteFloat(pt.X);
			this.WriteString(",");
			WriteFloat(pt.Y);
			this.WriteString(" ");
		}

		public void WriteMoveTo(Point pt)
		{
			this.WriteString("M ");
			this.WritePoint(pt);
		}
		public void WriteLineTo(Point end)
		{
			this.WriteString("L ");
			this.WritePoint(end);
		}
		public void WriteCubicCurveTo(Point a0, Point a1, Point end)
		{
			this.WriteString("C ");
			this.WritePoint(a0);
			this.WritePoint(a1);
			this.WritePoint(end);
		}
		public void WriteQuadraticCurveTo(Point a0, Point end)
		{
			this.WriteString("Q ");
			this.WritePoint(a0);
			this.WritePoint(end);
		}
	}
}
