namespace DDW.SwfToXaml
{
	partial class SettingsPage
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
			this.pbClose = new System.Windows.Forms.PictureBox();
			this.pbKnob = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbKnob)).BeginInit();
			this.SuspendLayout();
			// 
			// pbClose
			// 
			this.pbClose.BackColor = System.Drawing.Color.Transparent;
			this.pbClose.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pbClose.InitialImage = null;
			this.pbClose.Location = new System.Drawing.Point(775, 5);
			this.pbClose.Name = "pbClose";
			this.pbClose.Size = new System.Drawing.Size(20, 21);
			this.pbClose.TabIndex = 5;
			this.pbClose.TabStop = false;
			this.pbClose.MouseLeave += new System.EventHandler(this.pbClose_Leave);
			this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
			this.pbClose.MouseEnter += new System.EventHandler(this.pbClose_Enter);
			// 
			// pbKnob
			// 
			this.pbKnob.BackColor = System.Drawing.Color.Transparent;
			this.pbKnob.Image = global::SwfToXaml.Properties.Resources.knob;
			this.pbKnob.Location = new System.Drawing.Point(249, 177);
			this.pbKnob.Name = "pbKnob";
			this.pbKnob.Size = new System.Drawing.Size(180, 180);
			this.pbKnob.TabIndex = 6;
			this.pbKnob.TabStop = false;
			this.pbKnob.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbKnob_Down);
			this.pbKnob.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbKnob_Up);
			this.pbKnob.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbKnob_Move);
			// 
			// SettingsPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::SwfToXaml.Properties.Resources.settingsBkg;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.CausesValidation = false;
			this.ClientSize = new System.Drawing.Size(800, 600);
			this.ControlBox = false;
			this.Controls.Add(this.pbKnob);
			this.Controls.Add(this.pbClose);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "SettingsPage";
			this.Text = "SettingsPage";
			((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbKnob)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pbClose;
		private System.Windows.Forms.PictureBox pbKnob;
	}
}