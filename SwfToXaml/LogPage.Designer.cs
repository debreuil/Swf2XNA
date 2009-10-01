namespace DDW.SwfToXaml
{
	partial class LogPage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogPage));
			this.logText = new System.Windows.Forms.Label();
			this.pbClose = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
			this.SuspendLayout();
			// 
			// logText
			// 
			this.logText.BackColor = System.Drawing.Color.Transparent;
			this.logText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.logText.ForeColor = System.Drawing.Color.Silver;
			this.logText.Location = new System.Drawing.Point(37, 43);
			this.logText.Name = "logText";
			this.logText.Size = new System.Drawing.Size(717, 522);
			this.logText.TabIndex = 0;
			this.logText.Text = "Log Page";
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
			// LogPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.CausesValidation = false;
			this.ClientSize = new System.Drawing.Size(800, 600);
			this.ControlBox = false;
			this.Controls.Add(this.pbClose);
			this.Controls.Add(this.logText);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "LogPage";
			this.Text = "LogPage";
			((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label logText;
		private System.Windows.Forms.PictureBox pbClose;
	}
}