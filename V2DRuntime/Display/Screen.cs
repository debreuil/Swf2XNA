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
using V2DRuntime.Network;
using V2DRuntime.Game;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using V2DRuntime.Shaders;
using System.Reflection;
using V2DRuntime.V2D;
using Box2D.XNA.TestBed.Framework;
using Box2D.XNA;

namespace DDW.Display
{
	public delegate void ShaderEffect(Effect effect, int firstPass, params float[] parameters);

    public class Screen : DisplayObjectContainer
    {
        public V2DWorld v2dWorld;
        public float MillisecondsPerFrame = 1000f / 12f;
        public Dictionary<string, Texture2D> textures = new Dictionary<string,Texture2D>();
        private SymbolImport symbolImport;

		public Vector2 ClientSize = new Vector2(400, 300);

        protected MoveList moveList;
        public InputManager[] inputManagers;
        protected Move[] playerMoves;
        protected TimeSpan[] playerMoveTimes;
		readonly TimeSpan MoveTimeOut = TimeSpan.FromSeconds(1.0);

		protected PacketWriter packetWriter = new PacketWriter();
		protected PacketReader packetReader = new PacketReader();
		protected int framesBetweenPackets = 4;
		protected int framesSinceLastSend;
		protected bool enablePrediction = true;
		protected bool enableSmoothing = true;
		private List<DisplayObject> destructionList = new List<DisplayObject>();
        public Dictionary<int, V2DShader> shaderMap = new Dictionary<int, V2DShader>();

        public bool isFinalLevel = false;

        protected bool allowKeyboardOnly = true;

        public Screen()
        {
        }
        public Screen(SymbolImport symbolImport)
        {
            this.SymbolImport = symbolImport;

			EnsureV2DWorld();
			if (SymbolImport == null || SymbolImport.instanceName == V2DGame.currentRootName)
			{
				instanceDefinition = v2dWorld.RootInstance;
				instanceName = V2DGame.ROOT_NAME;
			}
			else
			{
				instanceDefinition = FindRootInstance(v2dWorld.RootInstance, SymbolImport.instanceName);
				instanceName = symbolImport.instanceName;
			}

			if (instanceDefinition != null)
			{
				definitionName = instanceDefinition.DefinitionName;
			}
        }
        public Screen(V2DContent v2dContent)
        {
            this.v2dWorld = v2dContent.v2dWorld;
            this.textures = v2dContent.textures;
        }

		public void DestroyAfterUpdate(DisplayObject obj)
		{
			destructionList.Add(obj);
		}

		public override void Activate()
		{
			V2DGame.currentRootName = instanceDefinition.InstanceName == null ? V2DGame.ROOT_NAME : instanceDefinition.InstanceName;
            base.Activate();
		}

        public override void Initialize()
        {
			base.Initialize(); 
			
			System.Reflection.MemberInfo inf = this.GetType();			
			System.Attribute[] attrs = System.Attribute.GetCustomAttributes(inf);  // reflection
			foreach (System.Attribute attr in attrs)
			{
				if (attr is ScreenAttribute)
				{
					ScreenAttribute sa = (ScreenAttribute)attr;
					if (sa.backgroundColor != 0x000000)
					{
						this.color = new Color(
							((sa.backgroundColor & 0xFF0000) >> 16) / 255f, 
							((sa.backgroundColor & 0x00FF00) >> 8)  / 255f, 
							((sa.backgroundColor & 0x0000FF) >> 0)  / 255f);
					}
				}
				if (attr is V2DShaderAttribute)
				{
					V2DShaderAttribute sa = (V2DShaderAttribute)attr;

					float[] parameters = new float[] { };
					ConstructorInfo ci = sa.shaderType.GetConstructor(new Type[] { parameters.GetType() });
					this.defaultShader = (V2DShader)ci.Invoke(
						new object[] 
							{ 
								new float[]{sa.param0, sa.param1, sa.param2, sa.param3, sa.param4} 
							});
				}
			}

            SetValidInput();
        }

		public override void Added(EventArgs e)
		{
            base.Added(e); 

			V2DGame.instance.SetSize(v2dWorld.Width, v2dWorld.Height);
			Activate();
		}
		public override void Removed(EventArgs e)
		{
			base.Removed(e);
			Deactivate();
		}
		protected override void OnAddToStageComplete()
		{
			base.OnAddToStageComplete();
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
		public virtual Sprite CreateDefaultObject(Texture2D texture, V2DInstance inst)
		{
			return new Sprite(texture, inst);
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
				this.textures[linkageName] = null;
				//string fullPath = Path.Combine(
				//   StorageContainer.TitleLocation,
				//   Path.Combine(V2DGame.contentManager.RootDirectory, linkageName) + ".png");

				//if (File.Exists(fullPath))
				//{
				//    result = V2DGame.contentManager.Load<Texture2D>(linkageName);
				//}
				//else
				//{
				//    try
				//    {
				//        result = V2DGame.contentManager.Load<Texture2D>(linkageName);
				//    }
				//    catch(ContentLoadException)
				//    {
				//        this.textures[linkageName] = null;
				//    }
				//}
            }

            return result;
        }

        V2DContent content;
		private void EnsureV2DWorld()
		{
			if (SymbolImport != null && v2dWorld == null)
			{
                // ** note: unnamed elements may actually fall out of scope and get gc/disposed, so need a ref
                // todo: use multiple content loaders per screen and unload where needed.
                //V2DContent content = V2DGame.instance.Content.Load<V2DContent>(SymbolImport.assetName);
                content = V2DGame.instance.Content.Load<V2DContent>(SymbolImport.assetName);
                v2dWorld = content.v2dWorld;
                textures = content.textures;
				v2dWorld.RootInstance.Definition = v2dWorld.GetDefinitionByName(V2DGame.ROOT_NAME);
			}
		}

		private V2DInstance FindRootInstance(V2DInstance inst, string rootName)
		{
			// look through higher level instances first
			V2DInstance result = null;

			if (inst.InstanceName == rootName)
			{
				result = inst;
			}
			else if (inst.Definition != null)
			{
				for (int i = 0; i < inst.Definition.Instances.Count; i++)
				{
					if (inst.Definition.Instances[i].InstanceName == rootName)
					{
						result = inst.Definition.Instances[i];
						break;
					}
				}
			}

			if (result == null && inst.Definition != null)
			{
				for (int i = 0; i < inst.Definition.Instances.Count; i++)
				{
					result = FindRootInstance(inst.Definition.Instances[i], rootName);
					if (result != null)
					{
						break;
					}
				}
			}

            if (result == null)
            {
                throw new Exception("Could not find root instance: " + rootName + " in " + inst.InstanceName + ".");
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

			int maxLocalGamers = 4;
			inputManagers = new InputManager[maxLocalGamers];

			int sessionGamerIndex = 0;
			for (int i = 0; i < maxLocalGamers; i++)
			{
				if (GamePad.GetState((PlayerIndex)i).IsConnected)
				{
					inputManagers[i] = new InputManager((PlayerIndex)i, moveList.LongestMoveLength);

					if (NetworkManager.Session != null && 
						NetworkManager.Session.LocalGamers.Count > sessionGamerIndex)
					{
						inputManagers[i].NetworkGamer = NetworkManager.Session.LocalGamers[sessionGamerIndex];
						sessionGamerIndex++;
					}
				}
			}

            if (allowKeyboardOnly && sessionGamerIndex == 0) // keyboard only
            {
				inputManagers[0] = new InputManager((PlayerIndex)0, moveList.LongestMoveLength);
            }

            // Give each player a location to store their most recent move.
            playerMoves = new Move[inputManagers.Length];
            playerMoveTimes = new TimeSpan[inputManagers.Length];
			SetKeyboardController();
        }
		protected void SetKeyboardController()
		{
			bool hasController = false;
			for (int i = 0; i < inputManagers.Length; i++)
			{
				if (inputManagers[i] != null)
				{
					inputManagers[i].IsActiveController = !hasController;
					hasController = true;
				}
			}
		}

        protected void ManageInput(GameTime gameTime)
        {
            if (inputManagers != null)
			{
				for (int i = 0; i < inputManagers.Length; ++i)
                {
					if (inputManagers[i] != null)
					{
						InputManager inputManager = inputManagers[i];

						// Expire old moves.
						if (gameTime.TotalRealTime - playerMoveTimes[i] > MoveTimeOut)
						{
							playerMoves[i] = null;
						}

						// Get the updated input manager.
						inputManager.Update(gameTime);

						// Detection and record the current player's most recent move.
						Move newMove = moveList.DetectMove(inputManager);
						if (inputManager.Releases != 0)
						{
							newMove = new Move("");
							newMove.Releases = inputManager.Releases;
						}

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
				//p.UpdateLocalPlayer(gameTime);

				// Periodically send our state to everyone in the session.
				if (sendPacketThisFrame)
				{
					p.WriteNetworkPacket(packetWriter, gameTime);
					gamer.SendData(packetWriter, SendDataOptions.InOrder);
				}
			}
		}

#endregion

		public virtual void SetBounds(float x, float y, float w, float h)
		{
		}
		public override void Update(GameTime gameTime)
        {
			ManageInput(gameTime);

            base.Update(gameTime);
            if (isActive)
            {
                if (NetworkManager.Session != null && NetworkManager.Session.SessionType != NetworkSessionType.Local)
				{
					UpdateNetworkSession(gameTime);
				}
			}
			OnUpdateComplete(gameTime);		
		}

		public virtual void OnUpdateComplete(GameTime gameTime)
		{
			if (destructionList.Count > 0)
			{
				foreach (DisplayObject ds in destructionList)
				{
					DestroyElement(ds);
				}
				destructionList.Clear();
			}	
		}
        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);

            // this needs to happen for screen (class) level shaders (vs depth group shaders)
            if (lastShader != null)
            {
                lastShader.End();
                lastShader = null;
            }
        }
		public virtual void DrawDebugData(SpriteBatch batch)
		{
		}

		//Stack<V2DShader> shaderStack = new Stack<V2DShader>();
		public V2DShader lastShader;
		public V2DShader defaultShader;
		public V2DShader currentShader;
        V2DShader shaderEffect;
		protected override void DrawChild(DisplayObject d, SpriteBatch batch)
		{
            shaderEffect = shaderMap.ContainsKey(d.DepthGroup) ? shaderMap[d.DepthGroup] : defaultShader;

            if (shaderEffect != lastShader)
            {
                if (lastShader != null)
                {
                    lastShader.End();
                }

                lastShader = shaderEffect;
                batch.End();
                //batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
                batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, Stage.SpriteBatchMatrix);

                if (shaderEffect != null)
                {
                    shaderEffect.Begin();
                    shaderEffect = null;
                }
            }

            base.DrawChild(d, batch);
        }

    }
}
