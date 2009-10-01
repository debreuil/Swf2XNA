namespace DDW.SwfToXaml
{
	partial class TrialExpired
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
			this.pbSiteVisit = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbSiteVisit)).BeginInit();
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
			this.pbClose.TabIndex = 6;
			this.pbClose.TabStop = false;
			this.pbClose.MouseLeave += new System.EventHandler(this.pbClose_Leave);
			this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
			this.pbClose.MouseEnter += new System.EventHandler(this.pbClose_Enter);
			// 
			// pbSiteVisit
			// 
			this.pbSiteVisit.BackColor = System.Drawing.Color.Transparent;
			this.pbSiteVisit.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pbSiteVisit.Image = null;
			this.pbSiteVisit.InitialImage = null;
			this.pbSiteVisit.Location = new System.Drawing.Point(549, 447);
			this.pbSiteVisit.Name = "pbSiteVisit";
			this.pbSiteVisit.Size = new System.Drawing.Size(200, 68);
			this.pbSiteVisit.TabIndex = 7;
			this.pbSiteVisit.TabStop = false;
			this.pbSiteVisit.MouseEnter += new System.EventHandler(this.pbSiteVisit_Enter);
			this.pbSiteVisit.MouseLeave += new System.EventHandler(this.pbSiteVisit_Leave);
			this.pbSiteVisit.Click += new System.EventHandler(this.pbSiteVisit_Click);
			// 
			// TrialExpired
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackgroundImage = global::SwfToXaml.Properties.Resources.trialExpired;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.CausesValidation = false;
			this.ClientSize = new System.Drawing.Size(800, 600);
			this.ControlBox = false;
			this.Controls.Add(this.pbSiteVisit);
			this.Controls.Add(this.pbClose);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "TrialExpired";
			this.Text = "Trial Expired";
			((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbSiteVisit)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pbClose;
		private System.Windows.Forms.PictureBox pbSiteVisit;

	}
}
