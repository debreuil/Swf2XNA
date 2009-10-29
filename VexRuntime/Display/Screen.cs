using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DDW.V2D;
using DDW.V2D.Serialization;
using DDW.Input;
using Microsoft.Xna.Framework.Input;

namespace DDW.Display
{
    public class Screen : DisplayObjectContainer
    {
        public V2DWorld v2dWorld;
        public float MillisecondsPerFrame = 1000f / 12f;
        public Dictionary<string, Texture2D> textures = new Dictionary<string,Texture2D>();
        private SymbolImport symbolImport;

        protected MoveList moveList;
        InputManager[] inputManagers;
        Move[] playerMoves;
        TimeSpan[] playerMoveTimes;
        readonly TimeSpan MoveTimeOut = TimeSpan.FromSeconds(1.0);
        
        public Screen()
        {
        }
        public Screen(SymbolImport symbolImport)
        {
            if (symbolImport != null)
            {
                this.SymbolImport = symbolImport;
            }
        }
        public Screen(V2DContent v2dContent)
        {
            this.v2dWorld = v2dContent.v2dWorld;
            this.textures = v2dContent.textures;
        }

        public SymbolImport SymbolImport
        {
            get
            {
                return symbolImport;
            }
            set
            {
                symbolImport = value;
            }
        }
        public Texture2D GetTexture(string linkageName)
        {
            Texture2D result = null;
            if (this.textures.ContainsKey(linkageName))
            {
                result = this.textures[linkageName];
            }
            else
            {
                try
                {
                    result = V2DGame.contentManager.Load<Texture2D>(linkageName);
                }
                catch (Exception) { }
            }

            return result;
        }

        public virtual void SetValidInput()
        {
            moveList = new MoveList(new Move[]
            {
                new Move("Up",  Direction.Up),
                new Move("Down", Direction.Down),
                new Move("Left", Direction.Left),
                new Move("Right", Direction.Right),
            });
            // Create an InputManager for each player with a sufficiently large buffer.
            inputManagers = new InputManager[1];
            for (int i = 0; i < inputManagers.Length; ++i)
            {
                inputManagers[i] = new InputManager((PlayerIndex)i, moveList.LongestMoveLength);
            }

            // Give each player a location to store their most recent move.
            playerMoves = new Move[inputManagers.Length];
            playerMoveTimes = new TimeSpan[inputManagers.Length];
        }
        protected void ManageInput(GameTime gameTime)
        {
            if (inputManagers != null)
            {
                for (int i = 0; i < inputManagers.Length; ++i)
                {
                    // Expire old moves.
                    if (gameTime.TotalRealTime - playerMoveTimes[i] > MoveTimeOut)
                    {
                        playerMoves[i] = null;
                    }

                    // Get the updated input manager.
                    InputManager inputManager = inputManagers[i];
                    inputManager.Update(gameTime);

                    // Allows the game to exit.
                    if (inputManager.GamePadState.Buttons.Back == ButtonState.Pressed ||
                        inputManager.KeyboardState.IsKeyDown(Keys.Escape))
                    {
                        //Exit();
                    }

                    // Detection and record the current player's most recent move.
                    Move newMove = moveList.DetectMove(inputManager);
                    if (newMove != null)
                    {
                        playerMoves[i] = newMove;
                        playerMoveTimes[i] = gameTime.TotalRealTime;
                        OnPlayerInput(i, playerMoves[i], playerMoveTimes[i]);
                    }
                }
            }
        }
        public virtual void OnPlayerInput(int playerIndex, Move move, TimeSpan time)
        {
        }
        public override void Update(GameTime gameTime)
        {
            ManageInput(gameTime);
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
        }

        public override void Initialize()
        {
            base.Initialize();
            SetValidInput();
        }
    }
}
