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
	public partial class TrialExpired : Form
	{
		public TrialExpired()
		{
			InitializeComponent();
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

		private void pbSiteVisit_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(@"http://theconverted.ca/get.html");
		}
		private void pbSiteVisit_Enter(object sender, EventArgs e)
		{
			this.pbSiteVisit.Image = global::SwfToXaml.Properties.Resources.visitSiteOver;
		}
		private void pbSiteVisit_Leave(object sender, EventArgs e)
		{
			this.pbSiteVisit.Image = null;
		}
	}
}

