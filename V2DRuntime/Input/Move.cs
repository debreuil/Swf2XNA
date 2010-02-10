#region File Description
//-----------------------------------------------------------------------------
// Move.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework.Input;
#endregion

namespace DDW.Input // these are ms supplied classes, namespace changed to fit
{
    /// <summary>
    /// Describes a sequences of buttons which must be pressed to active the move.
    /// A real game might add a virtual PerformMove() method to this class.
    /// </summary>
    public class Move
    {
        public string Name;
        // The sequence of button presses required to activate this move.
        public Buttons[] Sequence;
        // Set this to true if the input used to activate this move may
        // be reused as a component of longer moves.
        public bool IsSubMove;
        public Buttons Releases;
        
        public static Move Empty = new Move("");
        public static Move Up = new Move("Up",  Direction.Up);
        public static Move Down = new Move("Down", Direction.Down);
        public static Move Left = new Move("Left", Direction.Left);
        public static Move Right = new Move("Right", Direction.Right);

        public static Move Start = new Move("Start", Buttons.Start);
        public static Move Back = new Move("Back", Buttons.Back);
        public static Move ButtonA = new Move("A", Buttons.A);
        public static Move ButtonB = new Move("B", Buttons.B);
        public static Move ButtonX = new Move("X", Buttons.X);
        public static Move ButtonY = new Move("Y", Buttons.Y);
        public static Move LeftShoulder = new Move("LeftShoulder", Buttons.LeftShoulder);
        public static Move RightShoulder = new Move("RightShoulder", Buttons.RightShoulder);
        public static Move LeftTrigger = new Move("LeftTrigger", Buttons.LeftTrigger);
        public static Move RightTrigger = new Move("RightTrigger", Buttons.RightTrigger);

        public Move(string name, params Buttons[] sequence)
        {
            Name = name;
            Sequence = sequence;
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is Move && ((Move)obj).Name == Name)
            {
                result = true;
            }
            return result;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(Move a, Move b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Name == b.Name;
        }

        public static bool operator !=(Move a, Move b)
        {
            return !(a == b);
        }
    }
}
