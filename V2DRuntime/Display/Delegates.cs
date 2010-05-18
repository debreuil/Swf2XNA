using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using V2DRuntime.Tween;
using DDW.Display;
using V2DRuntime.Components;

namespace V2DRuntime.Display
{
    public delegate void DisplayObjectEvent(DisplayObject sender);

    public delegate void AnimationEvent(DisplayObjectContainer sender);

    public delegate void ButtonEventHandler(Button sender, int playerIndex, TimeSpan time);
    public delegate void FocusChangedEventHandler(Button sender);

    public delegate void TweenEvent(DisplayObject sender, TweenWorker worker);
    public delegate float EasingFormula(float t, float start, float length);
}
