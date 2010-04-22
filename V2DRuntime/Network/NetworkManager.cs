
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;


namespace V2DRuntime.Network
{
	public class NetworkManager
	{
		public delegate void NewGamerDelegate(NetworkGamer gamer, int gamerIndex);
		public event NewGamerDelegate OnNewGamer;
		public delegate void GamerLeftDelegate(NetworkGamer gamer, int gamerIndex);
		public event GamerLeftDelegate OnGamerLeft;
		public delegate void GameStartedDelegate();
		public event GameStartedDelegate OnGameStarted;

		private static NetworkManager instance;

		public NetworkSession networkSession;
		public NetworkSessionType networkSessionType;
		protected const int maxGamers = 16;
		protected const int maxLocalGamers = 4;
		protected PacketWriter packetWriter = new PacketWriter();
		protected PacketReader packetReader = new PacketReader();
		private string errorMessage; 

		private NetworkManager()
		{
		}

		public static NetworkManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new NetworkManager();
				}

				return instance;
			}
		}
		public static NetworkSession Session
		{
			get
			{
				NetworkSession result = null;
				if (instance != null)
				{
					result = instance.networkSession;
				}
				return result;
			}
		}

        public void CreateSession(NetworkSessionType networkSessionType)
		{
            this.networkSessionType = networkSessionType;
			try
			{
                networkSession = NetworkSession.Create(networkSessionType, maxLocalGamers, maxGamers);
				networkSession.AllowHostMigration = true;
				networkSession.AllowJoinInProgress = true;
				HookSessionEvents();
			}
			catch (Exception e)
			{
				errorMessage = e.Message;
			}
		}

		public void JoinSession()
		{
			try
			{
				using (AvailableNetworkSessionCollection availableSessions = NetworkSession.Find(networkSessionType, maxLocalGamers, null))
				{
					if (availableSessions.Count == 0)
					{
						errorMessage = "No network sessions found.";
					}
					else
					{
						networkSession = NetworkSession.Join(availableSessions[0]);
						HookSessionEvents();
					}
				}
			}
			catch (Exception e)
			{
				errorMessage = e.Message;
			}
		}


		public void LeaveSession()
		{
			if(networkSession != null)
			{
				if (networkSession.SessionState == NetworkSessionState.Playing)
				{
					networkSession.EndGame();
				}

				if (networkSession.IsHost)
				{
					networkSession.Dispose();
				}

				networkSession = null;
			}
		}

		public void HookSessionEvents()
		{
			networkSession.GamerJoined += GamerJoinedEventHandler;
			networkSession.GamerLeft += GamerLeftEventHandler;
			networkSession.GameStarted += GameStartedEventHandler;
			networkSession.SessionEnded += SessionEndedEventHandler;
		}

		public void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
		{
			OnNewGamer(e.Gamer, GetGamerIndex(e.Gamer));
			//int gamerIndex = networkSession.AllGamers.IndexOf(e.Gamer);
			//e.Gamer.Tag = new Player();
		}
		public void GamerLeftEventHandler(object sender, GamerLeftEventArgs e)
		{
			OnGamerLeft(e.Gamer, GetGamerIndex(e.Gamer));
		}
		public void GameStartedEventHandler(object sender, GameStartedEventArgs e)
		{
			OnGameStarted();
		}
		public void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
		{
			errorMessage = e.EndReason.ToString();

			networkSession.Dispose();
			networkSession = null;
		}

		public int GetGamerIndex(NetworkGamer gamer)
		{
			int result = -1;
			if (networkSession != null)
			{
				result = networkSession.AllGamers.IndexOf(gamer);
			}
			return result;
		}
	}
}
