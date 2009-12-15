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
using Microsoft.Xna.Framework.Net;
using VexRuntime.Network;
using VexRuntime.Game;
using Microsoft.Xna.Framework.Content;

namespace DDW.Display
{
    public class Screen : DisplayObjectContainer
    {
        public V2DWorld v2dWorld;
        public float MillisecondsPerFrame = 1000f / 12f;
        public Dictionary<string, Texture2D> textures = new Dictionary<string,Texture2D>();
        private SymbolImport symbolImport;
        public bool isActive = false;

        protected MoveList moveList;
        InputManager[] inputManagers;
        Move[] playerMoves;
        TimeSpan[] playerMoveTimes;
		readonly TimeSpan MoveTimeOut = TimeSpan.FromSeconds(1.0);

		protected PacketWriter packetWriter = new PacketWriter();
		protected PacketReader packetReader = new PacketReader();
		protected int framesBetweenPackets = 4;
		protected int framesSinceLastSend;
		protected bool enablePrediction = true;
		protected bool enableSmoothing = true;
        
        public Screen()
        {
        }
        public Screen(SymbolImport symbolImport)
        {
            if (symbolImport != null)
            {
                this.SymbolImport = symbolImport;
                InstanceName = symbolImport.instanceName;
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
                catch (Exception) 
				{
				}
            }

            return result;
        }


        public virtual void SetValidInput()
        {
            moveList = new MoveList(new Move[]
            {
                Move.Up,
                Move.Down,
                Move.Left,
                Move.Right,
                Move.Start,
                Move.Back,
                Move.ButtonA,
                Move.ButtonB,
                Move.ButtonX,
                Move.ButtonY,
                Move.LeftShoulder,
                Move.RightShoulder,
                Move.LeftTrigger,
                Move.RightTrigger,
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
            if (inputManagers != null && isActive)
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
						BroadcastMove(i, playerMoves[i], playerMoveTimes[i]);
                    }
                }
            }
		}

#region network
		public virtual void BroadcastMove(int playerIndex, Move move, TimeSpan time)
        {
		}
		public virtual void WriteNetworkPacket(PacketWriter packetWriter, GameTime gameTime)
		{
		}
		public virtual void ReadNetworkPacket(PacketReader packetReader,GameTime gameTime, TimeSpan latency)
		{
		}
		void UpdateNetworkSession(GameTime gameTime)
		{
			bool sendPacketThisFrame = false;
			framesSinceLastSend++;
			if (framesSinceLastSend >= framesBetweenPackets)
			{
				sendPacketThisFrame = true;
				framesSinceLastSend = 0;
			}

			if (NetworkManager.Session.SessionState == NetworkSessionState.Playing)
			{
				foreach (LocalNetworkGamer gamer in NetworkManager.Session.LocalGamers)
				{
					UpdateLocalGamer(gamer, gameTime, sendPacketThisFrame);
				}
			}

			try
			{
				NetworkManager.Session.Update();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				NetworkManager.Instance.LeaveSession();
			}

			// Make sure the session has not ended.
			if (NetworkManager.Session != null && NetworkManager.Session.SessionState == NetworkSessionState.Playing)
			{
				// Read any packets telling us the state of remotely controlled tanks.
				foreach (LocalNetworkGamer gamer in NetworkManager.Session.LocalGamers)
				{
					ReadIncomingPackets(gamer, gameTime);
				}

				// Apply prediction and smoothing to the remotely controlled tanks.
				foreach (NetworkGamer gamer in NetworkManager.Session.RemoteGamers)
				{
					Player p = gamer.Tag as Player;
					if (p != null)
					{
						p.UpdateRemotePlayer(framesBetweenPackets, enablePrediction);
					}
				}

				// Update the latency and packet loss simulation options.
				//UpdateOptions();
			}
		}
		protected virtual void ReadIncomingPackets(LocalNetworkGamer gamer, GameTime gameTime)
		{
			while (gamer.IsDataAvailable)
			{
				NetworkGamer sender;
				gamer.ReceiveData(packetReader, out sender);
				if (!sender.IsLocal && sender.Tag != null)
				{
					Player p = sender.Tag as Player;
					TimeSpan latency = NetworkManager.Session.SimulatedLatency +
									   TimeSpan.FromTicks(sender.RoundtripTime.Ticks / 2);

					// Read the state of this tank from the network packet.
					p.ReadNetworkPacket(packetReader, gameTime, latency);
				}
			}
		}
		protected virtual void UpdateLocalGamer(LocalNetworkGamer gamer, GameTime gameTime, bool sendPacketThisFrame)
		{
			Player p = gamer.Tag as Player;
			if (p != null)
			{
				p.UpdateLocalPlayer(gameTime);

				// Periodically send our state to everyone in the session.
				if (sendPacketThisFrame)
				{
					p.WriteNetworkPacket(packetWriter, gameTime);
					gamer.SendData(packetWriter, SendDataOptions.InOrder);
				}
			}
		}

#endregion

        public override void Update(GameTime gameTime)
        {
            if (isActive)
            {
				ManageInput(gameTime);
				if (NetworkManager.Session != null)
				{
					UpdateNetworkSession(gameTime);
				}
                base.Update(gameTime);
            }
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
