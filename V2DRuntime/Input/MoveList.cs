#region File Description
//-----------------------------------------------------------------------------
// MoveList.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
#endregion

namespace DDW.Input // these are ms supplied classes, namespace changed to fit
{
    /// <summary>
    /// Represents a set of available moves for matching. This internal storage of this
    /// class is optimized for efficient match searches.
    /// </summary>
    public class MoveList
    {
        private Move[] moves;

        public MoveList(IEnumerable<Move> moves)
        {
            // Store the list of moves in order of decreasing sequence length.
            // This greatly simplifies the logic of the DetectMove method.
            this.moves = moves.OrderByDescending(m => m.Sequence.Length).ToArray();
        }

        public Move MatchButtons(Buttons[] b)
        {
            Move result = null;
            for (int i = 0; i < moves.Length; i++)
            {
                Move m = moves[i];
                for (int j = 0; j < b.Length; j++)
                {
                    if (j >= m.Sequence.Length || m.Sequence[j] != b[j])
                    {
                        m = null;
                        break;
                    }
                }

                if (m != null)
                {
                    result = m;
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// Finds the longest Move which matches the given input, if any.
        /// </summary>
        public Move DetectMove(InputManager input)
        {
            if (input.Buffer.Count > 0)
            {
                // Perform a linear search for a move which matches the input. This relies
                // on the moves array being in order of decreasing sequence length.
                foreach (Move move in moves)
                {
                    if (input.Matches(move))
                    {
                        return move;
                    }
                }
            }
            return null;
        }

        public int LongestMoveLength
        {
            get
            {
                // Since they are in decreasing order,
                // the first move is the longest.
                return moves[0].Sequence.Length;
            }
        }
    }
}
