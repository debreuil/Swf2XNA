/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DDW.SwfToXaml
{
	public partial class ConvertedUI : Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConvertedUI));
			this.pbArms = new System.Windows.Forms.PictureBox();
			this.pbSettings = new System.Windows.Forms.PictureBox();
			this.pbLog = new System.Windows.Forms.PictureBox();
			this.pbMin = new System.Windows.Forms.PictureBox();
			this.pbWin = new System.Windows.Forms.PictureBox();
			this.pbClose = new System.Windows.Forms.PictureBox();
			this.pb_website = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pbArms)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbSettings)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbLog)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbWin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb_website)).BeginInit();
			this.SuspendLayout();
			// 
			// pbArms
			// 
			this.pbArms.BackColor = System.Drawing.Color.Transparent;
			this.pbArms.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pbArms.Location = new System.Drawing.Point(24, 131);
			this.pbArms.Name = "pbArms";
			this.pbArms.Size = new System.Drawing.Size(661, 334);
			this.pbArms.TabIndex = 1;
			this.pbArms.TabStop = false;
			this.pbArms.Visible = false;
			// 
			// pbSettings
			// 
			this.pbSettings.BackColor = System.Drawing.Color.Transparent;
			this.pbSettings.Image = ((System.Drawing.Image)(resources.GetObject("pbSettings.Image")));
			this.pbSettings.Location = new System.Drawing.Point(24, 561);
			this.pbSettings.Name = "pbSettings";
			this.pbSettings.Size = new System.Drawing.Size(74, 27);
			this.pbSettings.TabIndex = 2;
			this.pbSettings.TabStop = false;
			this.pbSettings.MouseLeave += new System.EventHandler(this.pbSettings_Leave);
			this.pbSettings.Click += new System.EventHandler(this.pbSettings_Click);
			this.pbSettings.MouseEnter += new System.EventHandler(this.pbSettings_Enter);
			// 
			// pbLog
			// 
			this.pbLog.BackColor = System.Drawing.Color.Transparent;
			this.pbLog.Image = global::SwfToXaml.Properties.Resources.btnLog;
			this.pbLog.Location = new System.Drawing.Point(117, 561);
			this.pbLog.Name = "pbLog";
			this.pbLog.Size = new System.Drawing.Size(43, 27);
			this.pbLog.TabIndex = 3;
			this.pbLog.TabStop = false;
			this.pbLog.MouseLeave += new System.EventHandler(this.pbLog_Leave);
			this.pbLog.Click += new System.EventHandler(this.pbLog_Click);
			this.pbLog.MouseEnter += new System.EventHandler(this.pbLog_Enter);
			// 
			// pbMin
			// 
			this.pbMin.BackColor = System.Drawing.Color.Transparent;
			this.pbMin.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pbMin.Location = new System.Drawing.Point(725, 5);
			this.pbMin.Name = "pbMin";
			this.pbMin.Size = new System.Drawing.Size(20, 21);
			this.pbMin.TabIndex = 4;
			this.pbMin.TabStop = false;
			this.pbMin.MouseLeave += new System.EventHandler(this.pbMin_Leave);
			this.pbMin.Click += new System.EventHandler(this.pbMin_Click);
			this.pbMin.MouseEnter += new System.EventHandler(this.pbMin_Enter);
			// 
			// pbWin
			// 
			this.pbWin.BackColor = System.Drawing.Color.Transparent;
			this.pbWin.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pbWin.Location = new System.Drawing.Point(750, 5);
			this.pbWin.Name = "pbWin";
			this.pbWin.Size = new System.Drawing.Size(20, 21);
			this.pbWin.TabIndex = 4;
			this.pbWin.TabStop = false;
			this.pbWin.MouseLeave += new System.EventHandler(this.pbWin_Leave);
			this.pbWin.Click += new System.EventHandler(this.pbWin_Click);
			this.pbWin.MouseEnter += new System.EventHandler(this.pbWin_Enter);
			// 
			// pbClose
			// 
			this.pbClose.BackColor = System.Drawing.Color.Transparent;
			this.pbClose.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pbClose.Location = new System.Drawing.Point(775, 5);
			this.pbClose.Name = "pbClose";
			this.pbClose.Size = new System.Drawing.Size(19, 20);
			this.pbClose.TabIndex = 4;
			this.pbClose.TabStop = false;
			this.pbClose.MouseLeave += new System.EventHandler(this.pbClose_Leave);
			this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
			this.pbClose.MouseEnter += new System.EventHandler(this.pbClose_Enter);
			// 
			// pb_website
			// 
			this.pb_website.BackColor = System.Drawing.Color.Transparent;
			this.pb_website.Image = global::SwfToXaml.Properties.Resources.btn_website;
			this.pb_website.Location = new System.Drawing.Point(177, 561);
			this.pb_website.Name = "pb_website";
			this.pb_website.Size = new System.Drawing.Size(79, 27);
			this.pb_website.TabIndex = 5;
			this.pb_website.TabStop = false;
			this.pb_website.MouseLeave += new System.EventHandler(this.pbWebsite_Leave);
			this.pb_website.Click += new System.EventHandler(this.pbWebsite_Click);
			this.pb_website.MouseEnter += new System.EventHandler(this.pbWebsite_Enter);
			// 
			// ConvertedUI
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.BackgroundImage = global::SwfToXaml.Properties.Resources.main;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ClientSize = new System.Drawing.Size(800, 600);
			this.ControlBox = false;
			this.Controls.Add(this.pb_website);
			this.Controls.Add(this.pbClose);
			this.Controls.Add(this.pbWin);
			this.Controls.Add(this.pbMin);
			this.Controls.Add(this.pbLog);
			this.Controls.Add(this.pbSettings);
			this.Controls.Add(this.pbArms);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConvertedUI";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "The Converted";
			this.Load += new System.EventHandler(this.ConvertedUI_Load);
			((System.ComponentModel.ISupportInitialize)(this.pbArms)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbSettings)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbLog)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbWin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb_website)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private PictureBox pbArms;
		private PictureBox pbSettings;
		private PictureBox pbLog;
		private PictureBox pbMin;
		private PictureBox pbWin;
		private PictureBox pbClose;
		private PictureBox pb_website;


	}
}