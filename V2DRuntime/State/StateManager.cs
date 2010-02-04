using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using V2DRuntime.Enums;
using DDW.Display;

namespace V2DRuntime.State
{
    public class StateManager
    {
        //TransitionKind TransitionKind = TransitionKind.Fade;
        public Screen PreviousState;
        public Screen CurrentState;

        public void TransitionTo()
        {
        }
        public void CreateCurrentState()
        {
        }
        public void DestroyCurrentState()
        {
        }
    }
}
