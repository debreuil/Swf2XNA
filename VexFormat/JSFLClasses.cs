using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDW.FlaFormat
{
    public class Document
    {
        public Library Library { get; set; }
        public List<Timeline> Root { get; set; }
        public List<Setting> Settings { get; set; }
    }
    public class Library
    {
        public List<SymbolItem> Items { get; set; }
    }
    public class Setting
    {
        // todo: Settings
        //public Array Object { get; set; }
    }
    public class SymbolItem
    {
        public Timeline Timeline { get; set; }
        public string SymbolType { get; set; }
        public string SourceFilePath { get; set; }
        public string SourceLibraryName { get; set; }
        public string SourceAutoUpdate { get; set; }
        public bool ScalingGrid { get; set; }
        public Rectangle ScalingGridRect { get; set; }
        public string ItemType { get; set; }
        public string Name { get; set; }
        public bool LinkageExportForAS { get; set; }
        public bool LinkageExportForRS { get; set; }
        public bool LinkageImportForRS { get; set; }
        public bool LinkageExportInFirstFrame { get; set; }
        public string LinkageIdentifier { get; set; }
        public string LinkageClassName { get; set; }
        public string LinkageBaseClass { get; set; }
        public string LinkageURL { get; set; }
    };

    public class Timeline
    {
        public string Name { get; set; }
        public List<Layer> Layers { get; set; }
        public int FrameCount { get; set; }
        public int CurrentFrame { get; set; }
        public int LayerCount { get; set; }
        public int CurrentLayer { get; set; }
    }

    public class Layer
    {
        public int LayerIndex { get; set; }
        public string Name { get; set; }
        public string LayerType { get; set; }
        public bool Visible { get; set; }
        public bool Locked { get; set; }
        public int FrameCount { get; set; }
        public List<Frame> Frames { get; set; }
        public int Color { get; set; }
        public int Height { get; set; }
        public bool Outline { get; set; }
    }

    public class Frame
    {
        public int LayerIndex { get; set; }
        public int FrameIndex { get; set; }
        public string Name { get; set; }
        public string ActionScript { get; set; }
        public List<Element> Elements { get; set; }
        public int StartFrame { get; set; }
        public double Duration { get; set; }
        public int SoundLibraryItem { get; set; }
        public string SoundEffect { get; set; }
        public string SoundName { get; set; }
        public string SoundSync { get; set; }
        public string SoundLoopMode { get; set; }
        public int SoundLoop { get; set; }
        public string TweenType { get; set; }
        public int TweenEasing { get; set; }
        public bool MotionTweenScale { get; set; }
        public string MotionTweenRotate { get; set; }
        public double MotionTweenRotateTimes { get; set; }
        public bool MotionTweenOrientToPath { get; set; }
        public bool MotionTweenSync { get; set; }
        public bool MotionTweenSnap { get; set; }
        public string ShapeTweenBlend { get; set; }
        public string LabelType { get; set; }
        public bool HasCustomEase { get; set; }
        public bool UseSingleEaseCurve { get; set; }
    }

    public class Element
    {
        public string Loop { get; set; }
        public int FirstFrame { get; set; }
        public bool Locked { get; set; }
        public string ElementType { get; set; }
        public List<Edge> Edges { get; set; }
        public bool IsGroup { get; set; }
        public bool IsDrawingObject { get; set; }
        public bool IsOvalObject { get; set; }
        public bool IsRectangleObject { get; set; }
        public double StartAngle { get; set; }
        public double EndAngle { get; set; }
        public double InnerRadius { get; set; }
        public bool ClosePath { get; set; }
        public double TopLeftRadius { get; set; }
        public double BottomLeftRadius { get; set; }
        public double TopRightRadius { get; set; }
        public double BottomRightRadius { get; set; }
        public bool LockFlag { get; set; }
        public string Name { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Matrix Matrix { get; set; }
        public int Depth { get; set; }
        public bool Selected { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double TransformX { get; set; }
        public double TransformY { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public double SkewX { get; set; }
        public double SkewY { get; set; }
        public double Rotation { get; set; }
        public List<Chain> Chains { get; set; }
    }
    public class Edge
    {
        public string Id { get; set; }
        public int IsLine { get; set; }
        public Vertex V0 { get; set; }
        public Vertex V1 { get; set; }
        public Vertex V2 { get; set; }
    }

    public class Vertex
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class Contour
    {
        public List<HalfEdge> HalfEdges { get; set; }
        public string Orientation { get; set; }
        public bool Interior { get; set; }
    }

    public class HalfEdge
    {
        public string Id { get; set; }
        public int Index { get; set; }
    }

    public class Chain
    {
        public List<Edge> Records { get; set; }
        public string Direction { get; set; } // "ccw" "cw"
        public Point Origin { get; set; }
        public bool Interior { get; set; }
        public int Checksum { get; set; }
        public Bounds BoundingBox { get; set; }
        public Point InnerPoint { get; set; }

        public Fill Fill { get; set; }
    }

    public class Record
    {
    }
    public class Matrix
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double D { get; set; }
        public double Tx { get; set; }
        public double Ty { get; set; }
    }
    public class Rectangle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
    public class Bounds // todo: make rect
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Bottom { get; set; }
        public double Right { get; set; }
    }

    public class Fill // ouch, need to make new json/C# system
    {
        public string Kind { get; set; }
        public int Color { get; set; }
        public string Bitmap { get; set; }
        public Matrix Matrix { get; set; }
        public List<uint> Colors { get; set; }
        public List<uint> Alphas { get; set; }
        public List<uint> Ratios { get; set; }
    }
}
