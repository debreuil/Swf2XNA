using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using V2DRuntime.Display;
using V2DRuntime.Components;
using DDW.V2D;
using V2DRuntime.Data;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using System.Xml.Serialization;
using System.IO;
using DDW.Input;

namespace V2DRuntime.Panels
{
    public class HighScorePanel : Panel
    {
        public TextBox[] txName;
        public TextBox[] txScore;
        public TextBox[] txLevel;

        protected int scoreCount= 20;
        protected List<HighScoreDataItem> highScores;
        private StorageDevice device;
        private Object stateobj;

        protected string gameName;
        protected string containerName;

        private PlayerIndex activePlayer = PlayerIndex.One;

        public HighScorePanel(Texture2D texture, V2DInstance inst) : base(texture, inst)
        {
            gameName = V2DGame.instance.GetType().Name;
            containerName = gameName + "Container";
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        public void SetActivePlayer(PlayerIndex activePlayer)
        {
            this.activePlayer = activePlayer;
        }

        private Random r = new Random();
        public override void Activate()
        {
            base.Activate();
            LoadHighScores(activePlayer);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            SaveHighScores();
        }

        public void LoadDefaultHighScores()
        {
            highScores = new List<HighScoreDataItem> { 
                    new HighScoreDataItem( "Sue", 670, false),
                    new HighScoreDataItem( "Bob", 642, false),
                    new HighScoreDataItem( "Kay", 580, false),
                    new HighScoreDataItem( "Tom", 699, false),
                    new HighScoreDataItem( "Blu", 703, false),
                    new HighScoreDataItem( "Zed", 620, false),
                    new HighScoreDataItem( "May", 546, false),
                    new HighScoreDataItem( "Zoi", 556, false), };
        }
        protected bool scoresChanged = false;
        public void InsertIfHighScore(HighScoreDataItem score)
        {
            scoresChanged = true;
            if (highScores != null)
            {
                highScores.Add(score);
                highScores.Sort();
                while (highScores.Count >= scoreCount && scoreCount > 0)
                {
                    highScores.RemoveAt(highScores.Count - 1);
                }
                Redraw();
            }
        }

        public void LoadHighScores(PlayerIndex playerIndex)
        {
            SignedInGamer sig = V2DGame.instance.GetSignedInGamer((int)(playerIndex));

            if (sig == null)
            {
                SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(OnSignedIn);
            }
            else
            {
                tryLoadHighScores = true;
            }
        }

        void OnSignedIn(object sender, SignedInEventArgs e)
        {
            if (e.Gamer != null && e.Gamer.PlayerIndex == activePlayer)
            {
                SignedInGamer.SignedIn -= new EventHandler<SignedInEventArgs>(OnSignedIn);
                tryLoadHighScores = true;
            }
        }

        private bool tryLoadHighScores = false;
        private void TryLoadHighScores()
        {
            try
            {
                if (!Guide.IsVisible && !isBusy)
                {
                    tryLoadHighScores = false;

                    isBusy = true;
                    device = null;
                    stateobj = (Object)("Load for Player " + activePlayer);
                    StorageDevice.BeginShowSelector((PlayerIndex)(activePlayer), this.GetXBoxDeviceAndLoad, stateobj);
                }
            }
            catch (Exception)
            {
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (tryLoadHighScores)
            {
                TryLoadHighScores();
            }
        }

        private bool isBusy;
        public void SaveHighScores()
        {
            try
            {
                if (scoresChanged && !Guide.IsVisible && !isBusy)
                {
                    isBusy = true;
                    device = null;
                    PlayerIndex playerIndex = activePlayer;
                    stateobj = (Object)("Save for Player " + playerIndex);
                    StorageDevice.BeginShowSelector(playerIndex, this.GetXBoxDeviceAndSave, stateobj);
                }
            }
            catch (Exception)
            {
            }
        }

        //protected PlayerIndex GetDefaultPlayerIndex()
        //{
        //    PlayerIndex result = PlayerIndex.One;
        //    for (int i = 0; i < InputManager.Managers.Length; i++)
        //    {
        //        InputManager im = InputManager.Managers[i];
        //        if (im != null)
        //        {
        //            result = im.PlayerIndex;
        //            break;
        //        }
        //    }
        //    return result;
        //}

        protected void GetXBoxDeviceAndLoad(IAsyncResult result)
        {
            try
            {
                device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    LoadFromDevice(device);
                }
            }
            catch (Exception)
            {
            }

            if (highScores == null)
            {
                LoadDefaultHighScores();
            }

            highScores.Sort();
            Redraw();

            isBusy = false;
        }

        protected void GetXBoxDeviceAndSave(IAsyncResult result)
        {
            device = StorageDevice.EndShowSelector(result);
            if (device != null && device.IsConnected)
            {
                SaveToDevice(device); 
                scoresChanged = false;
            }
            isBusy = false;
        }
        protected void SaveToDevice(StorageDevice device)
        {
            IAsyncResult result = device.BeginOpenContainer(containerName, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = device.EndOpenContainer(result);
            result.AsyncWaitHandle.Close();

            string filename = gameName + ".sav";
            if (container.FileExists(filename))
            {
                container.DeleteFile(filename);
            }

            Stream stream = container.CreateFile(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(List<HighScoreDataItem>));
            serializer.Serialize(stream, highScores);

            stream.Close();
            container.Dispose();
        }

        protected void LoadFromDevice(StorageDevice device)
        {
            IAsyncResult result = device.BeginOpenContainer(containerName, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = device.EndOpenContainer(result);
            result.AsyncWaitHandle.Close();

            string filename = gameName + ".sav";
            if (!container.FileExists(filename))
            {
                container.Dispose();
                return;
            }

            Stream stream = container.OpenFile(filename, FileMode.Open);
            if (stream.Length > 0)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<HighScoreDataItem>));
                highScores = (List<HighScoreDataItem>)serializer.Deserialize(stream);
            }

            stream.Close();
            container.Dispose();
        }

        protected void DeleteFile(StorageDevice device)
        {
            IAsyncResult result = device.BeginOpenContainer(containerName, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = device.EndOpenContainer(result);
            result.AsyncWaitHandle.Close();

            string filename = gameName + ".sav";

            if (container.FileExists(filename))
            {
                container.DeleteFile(filename);
            }

            container.Dispose();
        }

        protected void Redraw()
        {
            for (int i = 0; i < txName.Length; i++)
            {
                if (i < highScores.Count)
                {
                    txName[i].Visible = true;
                    txName[i].Text = highScores[i].name;

                    if (txScore != null)
                    {
                        txScore[i].Visible = true;
                        txScore[i].Text = highScores[i].score.ToString();
                    }

                    if (txLevel != null)
                    {
                        txLevel[i].Visible = true;
                        txLevel[i].Text = highScores[i].level.ToString();
                    }
                }
                else
                {
                    txName[i].Visible = false;

                    if (txScore != null)
                    {
                        txScore[i].Visible = false;
                    }

                    if (txLevel != null)
                    {
                        txLevel[i].Visible = false;
                    }
                }
            }
        }
    }
}
