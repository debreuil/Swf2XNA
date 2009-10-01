/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Resources;

namespace DDW.SwfToXaml
{

	public partial class ConvertedUI : Form
	{
		private ResourceManager rm; 
		private Thread fileConvertThread;
		private StringBuilder sb = new StringBuilder();

		public ConvertedUI()
		{
			InitializeComponent();
			rm = global::SwfToXaml.Properties.Resources.ResourceManager;
			AddStatusText("Swf to Xaml Converter. " + DateTime.Now.ToLongTimeString()); 
		}
		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			base.OnDragEnter(drgevent);
		}
		protected override void OnDragLeave(EventArgs e)
		{
			base.OnDragLeave(e);
			Cursor.Current = Cursors.Default;
		}
		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			pbArms.Visible = true;
			//base.OnDragDrop(drgevent);
			string[] fileNames = (string[])drgevent.Data.GetData("FileDrop");
			if (fileNames.Length > 0)
			{
				string pl = fileNames.Length > 1 ? "s" : "";

				this.BackgroundImage = global::SwfToXaml.Properties.Resources.converting1;
				this.Invalidate();

				AddStatusText("Begin converting " + fileNames.Length + " file" + pl + "."); 

				for (int i = 0; i < fileNames.Length; i++)
				{
					try
					{
						fileName = fileNames[i];
						AddStatusText("Converting: " + fileName + "."); 
						string fileDir = Path.GetDirectoryName(fileName);
						Directory.SetCurrentDirectory(fileDir);
						this.Refresh();

                        fileConvertThread = new Thread(new ThreadStart(this.convertFile));
                        fileConvertThread.Name = "File Convert";

                        // this change courtesty of SeriousM, thanks!
                        // converts will be formated to decimal-dot (123.23)
                        fileConvertThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                        // resource lookups will be formated to decimal-dot (123.23)
                        fileConvertThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture; 

                        fileConvertThread.Start();

						AnimateHands();

						AddStatusText(msg); 
					}	
					catch (Exception e)
					{
						AddStatusText("Unsupported file element -- conversion failure. " + e.Message); 
					}
				}
				AddStatusText("Complete."); 
				isConverting = false;

				this.BackgroundImage = global::SwfToXaml.Properties.Resources.fin5;
				this.pbArms.BackgroundImage = null;

				AnimateEnd();
				this.pbArms.BackgroundImage = null;
				pbArms.Visible = false;
				this.Refresh();
			}
		}
		private string msg;
		private string fileName;
		private void convertFile()
		{
			isConverting = true;
			msg = SwfToXaml.Convert(fileName, false);
			isConverting = false;
		}

		private bool isConverting;
		private void AnimateHands()
		{
			this.pbArms.BackgroundImage = global::SwfToXaml.Properties.Resources.hand0;
			this.Refresh();

			int handCount = 1;
			while (isConverting)
			{
				if(isConverting)
				{
					Thread.Sleep(100);
				}
				else
				{
					break;
				}
				this.pbArms.BackgroundImage = (Bitmap)rm.GetObject("hand" + handCount);
				this.Refresh();
				handCount += 1;
				if(handCount > 3)
				{
					handCount = 0;
				}
			}
			this.pbArms.BackgroundImage = null;
		}

		private void AnimateEnd()
		{
			this.BackgroundImage = global::SwfToXaml.Properties.Resources.fin0;
			this.Refresh();
			Thread.Sleep(100);
			this.BackgroundImage = global::SwfToXaml.Properties.Resources.fin1;
			this.Refresh();
			Thread.Sleep(100);
			this.BackgroundImage = global::SwfToXaml.Properties.Resources.fin2;
			this.Refresh();
			Thread.Sleep(100);
			this.BackgroundImage = global::SwfToXaml.Properties.Resources.fin3;
			this.Refresh();
			Thread.Sleep(100);
			this.BackgroundImage = global::SwfToXaml.Properties.Resources.fin4;
			this.Refresh();
			Thread.Sleep(100);
			this.BackgroundImage = global::SwfToXaml.Properties.Resources.fin5;
			this.Refresh();
			Thread.Sleep(100);

		}
		private void AddStatusText(string text)
		{
			sb.Append(text + System.Environment.NewLine);
			//if(statusText.TextLength > 0)
			//{
			//    statusText.Select(statusText.TextLength - 1, 0);
			//    statusText.ScrollToCaret();
			//}
		}
		protected override void OnDragOver(DragEventArgs drgevent)
		{
			base.OnDragOver(drgevent);
			string[] formats = drgevent.Data.GetFormats();
			bool isFiles = false;
			for (int i = 0; i < formats.Length; i++)
			{
				if (formats[i] == "FileDrop")
				{
					isFiles = true;
				}
			}
			if (isFiles)
			{
				Cursor.Current = Cursors.UpArrow;
				drgevent.Effect = DragDropEffects.Copy;
			}
		}

		private bool isDragging = false;
		private Point startMouse;
		private Point startWindow;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!isDragging)
			{
				this.isDragging = true;
				this.startMouse = new Point(e.X + this.Location.X, e.Y + this.Location.Y);
				this.startWindow = this.Location;
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			this.isDragging = false;
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			this.isDragging = false;
			base.OnMouseLeave(e);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (this.isDragging)
			{
				int x = this.Location.X + e.X;
				int y = this.Location.Y + e.Y;
				int difX = x - this.startMouse.X;
				int difY = y - this.startMouse.Y;
				this.Location = new Point(this.startWindow.X + difX, this.startWindow.Y + difY);
			}
		}

		private void ConvertedUI_Load(object sender, EventArgs e)
		{

		}

		private void pbLog_Enter(object sender, EventArgs e)
		{
			this.pbLog.Image = global::SwfToXaml.Properties.Resources.logOver;
		}
		private void pbLog_Leave(object sender, EventArgs e)
		{
			this.pbLog.Image = global::SwfToXaml.Properties.Resources.btnLog;
		}
		private void pbLog_Click(object sender, EventArgs e)
		{
			LogPage lp = new LogPage(sb.ToString());
			lp.Show();
		}

		private void pbSettings_Enter(object sender, EventArgs e)
		{
			this.pbSettings.Image = global::SwfToXaml.Properties.Resources.settingsOver;
		}
		private void pbSettings_Leave(object sender, EventArgs e)
		{
			this.pbSettings.Image = global::SwfToXaml.Properties.Resources.btnSettings;
		}
		private void pbSettings_Click(object sender, EventArgs e)
		{
			SettingsPage sp = new SettingsPage();
			sp.Show();
		}

		private void pbWebsite_Enter(object sender, EventArgs e)
		{
			this.pb_website.Image = global::SwfToXaml.Properties.Resources.website_over;
		}
		private void pbWebsite_Leave(object sender, EventArgs e)
		{
			this.pb_website.Image = global::SwfToXaml.Properties.Resources.btn_website;
		}
		private void pbWebsite_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(@"http://theconverted.ca/get.html");
		}

		private void pbClose_Enter(object sender, EventArgs e)
		{
			this.pbClose.Image = global::SwfToXaml.Properties.Resources.closeOver;
		}
		private void pbClose_Leave(object sender, EventArgs e)
		{
			this.pbClose.Image = null;
		}
		private void pbClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void pbWin_Enter(object sender, EventArgs e)
		{
			this.pbWin.Image = global::SwfToXaml.Properties.Resources.winOver;
		}
		private void pbWin_Leave(object sender, EventArgs e)
		{
			this.pbWin.Image = null;
		}

		private void pbMin_Enter(object sender, EventArgs e)
		{
			this.pbMin.Image = global::SwfToXaml.Properties.Resources.minOver;
		}
		private void pbMin_Leave(object sender, EventArgs e)
		{
			this.pbMin.Image = null;
		}

		private void pbWin_Click(object sender, EventArgs e)
		{
			this.Location = new Point(0, 0);
		}

		private void pbMin_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
		}

	}
}




















