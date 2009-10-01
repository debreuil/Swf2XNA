/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace DDW.SwfToXaml
{
	public partial class SettingsPage : Form
	{
		private Bitmap[] knobs;
		public SettingsPage()
		{
			InitializeComponent();
			int len = 11;
			knobs = new Bitmap[len];

			Bitmap org = global::SwfToXaml.Properties.Resources.knob;
			int w = org.Width;
			int h = org.Height;
			float step = 360F / len;
			for (int i = 0; i < len; i++)
			{
				knobs[i] = new Bitmap(org);
				Graphics g = Graphics.FromImage(knobs[i]);// new Bitmap(org.Width, org.Height, g);
				g.Clear(Color.FromArgb(0,0,0,0));
				Matrix m = new Matrix();
				int ang = (int)(step * i);
				m.RotateAt(ang, new Point(w / 2, h / 2));
				g.Transform = m;
				g.DrawImage(org,0,0,180,180);
			}
			pbKnob.Image = knobs[knobIndex];
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
			this.isKnobTurning = false;
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

		bool isKnobTurning = false;
		Point knobStartMouse;
		Point knobCenter;
		double knobStartAngle;
		static int knobIndex = 0;
		int knobStartIndex = 0;

		private void pbKnob_Down(object o, MouseEventArgs e)
		{
			if (!isKnobTurning)
			{
				this.isKnobTurning = true;
				this.knobCenter = new Point(pbKnob.Size.Width / 2, pbKnob.Size.Height / 2);
				this.knobStartMouse = new Point(e.X, e.Y);
				this.knobStartAngle = Math.Atan2(knobStartMouse.Y - knobCenter.Y, knobStartMouse.X - knobCenter.X);
				knobStartIndex = knobIndex;
			}
		}
		private void pbKnob_Up(object o, MouseEventArgs e)
		{
			isKnobTurning = false;
		}
		private void pbKnob_Move(object o, MouseEventArgs e)
		{
			if (isKnobTurning)
			{
				double curAngle = Math.Atan2(e.Y - knobCenter.Y, e.X - knobCenter.X);
				int turn = (int)((curAngle + Math.PI/2) / (Math.PI * 2) * knobs.Length) + knobs.Length;
				knobIndex = Math.Abs((knobStartIndex + turn) % knobs.Length);
				pbKnob.Image = knobs[knobIndex];
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			if (pbKnob != null)
			{
				pbKnob.Image = null;
			}

			if (knobs != null)
			{
				for (int i = 0; i < knobs.Length; i++)
				{
					knobs[i].Dispose();
				}
			}	
		}
	}
}