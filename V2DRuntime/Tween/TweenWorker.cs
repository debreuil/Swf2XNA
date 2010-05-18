using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using V2DRuntime.Display;
using Microsoft.Xna.Framework;

namespace V2DRuntime.Tween
{
    public class TweenWorker
    {
        public event TweenEvent TweenComplete;
        public DisplayObject target;
        public TimeSpan duration;
        public EasingFormula easingFormula;

        private TweenState startState;
        private TweenState endState;

        private bool isAnimating = true;
        private float t;
        private TimeSpan elapsed;
        
        private bool hasPosition;
        private bool hasScale;
        private bool hasRotation;
        private bool hasAlpha;
        private bool hasFrames;

        public TweenWorker(TweenState startState, TweenState endState,  int milliseconds)
        {
            this.StartState = startState;
            this.EndState = endState;
            this.duration = new TimeSpan(0, 0, 0, 0, milliseconds);
        }
        public TweenWorker(TweenState startState, TweenState endState,  int milliseconds, EasingFormula easingFormula)
        {
            this.StartState = startState;
            this.EndState = endState;
            this.duration = new TimeSpan(0, 0, 0, 0, milliseconds);
            this.easingFormula = easingFormula;
        }
        public TweenWorker(DisplayObject obj, TweenState endState,  int milliseconds)
        {
            this.target = obj;
            this.StartState = new TweenState(obj);
            this.EndState = endState;
            this.duration = new TimeSpan(0, 0, 0, 0, milliseconds);
        }
        public TweenWorker(DisplayObject obj, TweenState endState,  int milliseconds, EasingFormula easingFormula)
        {
            this.target = obj;
            this.StartState = new TweenState(obj);
            this.EndState = endState;
            this.duration = new TimeSpan(0, 0, 0, 0, milliseconds);
            this.easingFormula = easingFormula;
        }

        public TweenState StartState { get{return startState;} 
            set
            {
                startState = value;
                SetChangeable();
            } 
        }
        public TweenState EndState { get{return endState;} 
            set
            {
                endState = value;
                SetChangeable();
            } 
        }

        private void SetChangeable()
        {
            if(startState != null && endState != null)
            {
                hasPosition = endState.Position != startState.Position;
                hasRotation = endState.Rotation != startState.Rotation;
                hasScale = endState.Scale != startState.Scale;
                hasAlpha = endState.Alpha != startState.Alpha;
                hasFrames = endState.Frame != startState.Frame;
            }
        }

        public void Begin()
        {
            isAnimating = true;
        }
        public void End()
        {
            isAnimating = false;
            if (TweenComplete != null)
            {
                TweenComplete(target, this);
            }
            Reset();
        }
        public void Reset()
        {
            t = 0;
            isAnimating = false;
        }

        public void Update(GameTime gameTime)
        {
            if(isAnimating)
            {
                elapsed += gameTime.ElapsedGameTime;
                if (elapsed > duration)
                {
                    t = 1;
                    isAnimating = false;
                }
                else
                {
                    t = (float)(elapsed.TotalMilliseconds / duration.TotalMilliseconds);

                    if (easingFormula != null)
                    {
                        t = easingFormula(t, 0, 1);
                    }
                }
                ApplyMetrics();

                if(t == 1)
                {
                    End();
                }
            }
        }
        public void ApplyMetrics()
        {
            if(hasPosition) target.Position = (endState.Position - startState.Position) * t + startState.Position;
            if (hasRotation) target.Rotation = (endState.Rotation - startState.Rotation) * t + startState.Rotation;
            if(hasScale) target.Scale = (endState.Scale - startState.Scale) * t + startState.Scale;
            if(hasAlpha) target.Alpha = (endState.Alpha - startState.Alpha) * t + startState.Alpha;
            if (hasFrames && target is DisplayObjectContainer)
            {
                ((DisplayObjectContainer)target).CurChildFrame = (uint)(Math.Floor((endState.Frame - startState.Frame) * t) % ((DisplayObjectContainer)target).FrameCount);
            }
        }
    }
}
