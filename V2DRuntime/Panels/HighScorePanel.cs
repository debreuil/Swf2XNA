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

        public HighScorePanel(Texture2D texture, V2DInstance inst) : base(texture, inst)
        {
            gameName = V2DGame.instance.GetType().Name;
            containerName = gameName + "Container";
        }

        public override void Initialize()
        {
            base.Initialize();

            LoadHighScores();
        }

        private Random r = new Random();
        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            InsertIfHighScore(new HighScoreDataItem("ZZZ", r.Next(50, 200), true));
            SaveHighScores();
        }

        public void LoadDefaultHighScores()
        {
            highScores = new List<HighScoreDataItem> { 
                    new HighScoreDataItem( "Sue", 67, false),
                    new HighScoreDataItem( "Bob", 37, false),
                    new HighScoreDataItem( "Kay", 38, false),
                    new HighScoreDataItem( "Tom", 127, false),
                    new HighScoreDataItem( "Blu", 142, false),
                    new HighScoreDataItem( "Zed", 97, false),
                    new HighScoreDataItem( "May", 34, false),
                    new HighScoreDataItem( "Zoi", 55, false), };
        }

        public void InsertIfHighScore(HighScoreDataItem score)
        {
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
        public void LoadHighScores()
        {
            if (!Guide.IsVisible && !isBusy)
            {
                isBusy = true;
                device = null;
                PlayerIndex playerIndex = GetDefaultPlayerIndex();
                stateobj = (Object)("Load for Player " + playerIndex);
                StorageDevice.BeginShowSelector(playerIndex, this.GetXBoxDeviceAndLoad, stateobj);
            }
        }
        private bool isBusy;
        public void SaveHighScores()
        {
            if (!Guide.IsVisible && !isBusy)
            {
                isBusy = true;
                device = null;
                PlayerIndex playerIndex = GetDefaultPlayerIndex();
                stateobj = (Object)("Save for Player " + playerIndex);
                StorageDevice.BeginShowSelector(playerIndex, this.GetXBoxDeviceAndSave, stateobj);
            }
        }

        protected PlayerIndex GetDefaultPlayerIndex()
        {
            PlayerIndex result = PlayerIndex.One;
            for (int i = 0; i < InputManager.Managers.Length; i++)
            {
                InputManager im = InputManager.Managers[i];
                if (im != null)
                {
                    result = im.PlayerIndex;
                    break;
                }
            }
            return result;
        }

        protected void GetXBoxDeviceAndLoad(IAsyncResult result)
        {
            device = StorageDevice.EndShowSelector(result);
            if (device != null && device.IsConnected)
            {
                LoadFromDevice(device);
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
