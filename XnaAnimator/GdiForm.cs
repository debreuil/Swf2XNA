/* Copyright (C) 2008 Robin Debreuil -- Released under the GNU General Public License (GPL) v2 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DDW.Vex;


namespace DDW
{
	public class GdiForm : Form
	{
		Dictionary<string, Bitmap> bmps = new Dictionary<string,Bitmap>();
		private PictureBox picture;
		private SplitContainer splitContainer1;
		private ListBox list;
		Bitmap curBmp = null;

		public GdiForm(List<Bitmap> bs)
		{
			InitializeComponent();

			for (int i = 0; i < bs.Count; i++)
			{
				Bitmap b = bs[i];
				this.AddBitmap("Symbol " + i, b);
			}
		}
		public void AddBitmap(string name, Bitmap b)
		{
			this.bmps.Add(name, b);
			this.list.Items.Add(name);

			if (list.SelectedItem == null)
			{
				list.SetSelected(0, true);
			}
		}

		public void PaintEventHandler()
		{

		}

		private void InitializeComponent()
		{
			this.picture = new System.Windows.Forms.PictureBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.list = new System.Windows.Forms.ListBox();
			((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// picture
			// 
			this.picture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.picture.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picture.Location = new System.Drawing.Point(0, 0);
			this.picture.Name = "picture";
			this.picture.Size = new System.Drawing.Size(811, 689);
			this.picture.TabIndex = 0;
			this.picture.TabStop = false;
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.list);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.picture);
			this.splitContainer1.Size = new System.Drawing.Size(969, 693);
			this.splitContainer1.SplitterDistance = 150;
			this.splitContainer1.TabIndex = 2;
			// 
			// list
			// 
			this.list.Dock = System.Windows.Forms.DockStyle.Fill;
			this.list.FormattingEnabled = true;
			this.list.Location = new System.Drawing.Point(0, 0);
			this.list.Name = "list";
			this.list.Size = new System.Drawing.Size(146, 680);
			this.list.TabIndex = 0;
			this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
			// 
			// GdiForm
			// 
			this.ClientSize = new System.Drawing.Size(969, 693);
			this.Controls.Add(this.splitContainer1);
			this.Name = "GdiForm";
			((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private void list_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.list.SelectedIndices.Count > 0)
			{
				int index = this.list.SelectedIndices[0];
				string item = (string)this.list.Items[index];
				this.curBmp = this.bmps[item];

				if (this.curBmp != null && this.picture.BackgroundImage != curBmp)
				{
					this.picture.BackgroundImage = curBmp;
				}
			}

		}
	}
}
