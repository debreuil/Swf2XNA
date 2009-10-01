namespace DDW.XamlViewer
{
	partial class XamlViewer
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XamlViewer));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.splitMain = new System.Windows.Forms.SplitContainer();
			this.tabControlFile = new System.Windows.Forms.TabControl();
			this.tabControlContent = new System.Windows.Forms.TabControl();
			this.tabXamlView = new System.Windows.Forms.TabPage();
			this.webBrowserXaml = new System.Windows.Forms.WebBrowser();
			this.tabXamlCode = new System.Windows.Forms.TabPage();
			this.textBoxXaml = new System.Windows.Forms.TextBox();
			this.tabSilverlightView = new System.Windows.Forms.TabPage();
			this.webBrowserSilverlight = new System.Windows.Forms.WebBrowser();
			this.tabSilverlightCode = new System.Windows.Forms.TabPage();
			this.textBoxSilverlight = new System.Windows.Forms.TextBox();
			this.tabSwfView = new System.Windows.Forms.TabPage();
			this.webBrowserSwf = new System.Windows.Forms.WebBrowser();
			this.tabSwfDump = new System.Windows.Forms.TabPage();
			this.textBoxSwfDump = new System.Windows.Forms.TextBox();
			this.menuStrip1.SuspendLayout();
			this.splitMain.Panel1.SuspendLayout();
			this.splitMain.Panel2.SuspendLayout();
			this.splitMain.SuspendLayout();
			this.tabControlContent.SuspendLayout();
			this.tabXamlView.SuspendLayout();
			this.tabXamlCode.SuspendLayout();
			this.tabSilverlightView.SuspendLayout();
			this.tabSilverlightCode.SuspendLayout();
			this.tabSwfView.SuspendLayout();
			this.tabSwfDump.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(888, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
			this.closeToolStripMenuItem.Text = "Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(97, 6);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(97, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem1});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// copyToolStripMenuItem1
			// 
			this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
			this.copyToolStripMenuItem1.Size = new System.Drawing.Size(99, 22);
			this.copyToolStripMenuItem1.Text = "Copy";
			this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyToolStripMenuItem1_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(100, 6);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Location = new System.Drawing.Point(0, 616);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(888, 22);
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// splitMain
			// 
			this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitMain.Location = new System.Drawing.Point(0, 24);
			this.splitMain.Name = "splitMain";
			// 
			// splitMain.Panel1
			// 
			this.splitMain.Panel1.Controls.Add(this.tabControlFile);
			// 
			// splitMain.Panel2
			// 
			this.splitMain.Panel2.Controls.Add(this.tabControlContent);
			this.splitMain.Size = new System.Drawing.Size(888, 592);
			this.splitMain.SplitterDistance = 296;
			this.splitMain.TabIndex = 3;
			// 
			// tabControlFile
			// 
			this.tabControlFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlFile.Location = new System.Drawing.Point(0, 0);
			this.tabControlFile.Name = "tabControlFile";
			this.tabControlFile.SelectedIndex = 0;
			this.tabControlFile.Size = new System.Drawing.Size(296, 592);
			this.tabControlFile.TabIndex = 0;
			// 
			// tabControlContent
			// 
			this.tabControlContent.Controls.Add(this.tabXamlView);
			this.tabControlContent.Controls.Add(this.tabXamlCode);
			this.tabControlContent.Controls.Add(this.tabSilverlightView);
			this.tabControlContent.Controls.Add(this.tabSilverlightCode);
			this.tabControlContent.Controls.Add(this.tabSwfView);
			this.tabControlContent.Controls.Add(this.tabSwfDump);
			this.tabControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlContent.Location = new System.Drawing.Point(0, 0);
			this.tabControlContent.Name = "tabControlContent";
			this.tabControlContent.SelectedIndex = 0;
			this.tabControlContent.Size = new System.Drawing.Size(588, 592);
			this.tabControlContent.TabIndex = 0;
			this.tabControlContent.SelectedIndexChanged += new System.EventHandler(this.tabControlContent_SelectedIndexChanged);
			// 
			// tabXamlView
			// 
			this.tabXamlView.Controls.Add(this.webBrowserXaml);
			this.tabXamlView.Location = new System.Drawing.Point(4, 22);
			this.tabXamlView.Name = "tabXamlView";
			this.tabXamlView.Padding = new System.Windows.Forms.Padding(3);
			this.tabXamlView.Size = new System.Drawing.Size(580, 566);
			this.tabXamlView.TabIndex = 1;
			this.tabXamlView.Text = "Xaml View";
			this.tabXamlView.UseVisualStyleBackColor = true;
			// 
			// webBrowserXaml
			// 
			this.webBrowserXaml.AllowWebBrowserDrop = false;
			this.webBrowserXaml.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowserXaml.Location = new System.Drawing.Point(3, 3);
			this.webBrowserXaml.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserXaml.Name = "webBrowserXaml";
			this.webBrowserXaml.Size = new System.Drawing.Size(574, 560);
			this.webBrowserXaml.TabIndex = 0;
			// 
			// tabXamlCode
			// 
			this.tabXamlCode.Controls.Add(this.textBoxXaml);
			this.tabXamlCode.Location = new System.Drawing.Point(4, 22);
			this.tabXamlCode.Name = "tabXamlCode";
			this.tabXamlCode.Size = new System.Drawing.Size(580, 566);
			this.tabXamlCode.TabIndex = 2;
			this.tabXamlCode.Text = "Xaml Code";
			this.tabXamlCode.UseVisualStyleBackColor = true;
			// 
			// textBoxXaml
			// 
			this.textBoxXaml.AcceptsReturn = true;
			this.textBoxXaml.AcceptsTab = true;
			this.textBoxXaml.CausesValidation = false;
			this.textBoxXaml.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxXaml.Location = new System.Drawing.Point(0, 0);
			this.textBoxXaml.Multiline = true;
			this.textBoxXaml.Name = "textBoxXaml";
			this.textBoxXaml.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxXaml.Size = new System.Drawing.Size(580, 566);
			this.textBoxXaml.TabIndex = 0;
			this.textBoxXaml.WordWrap = false;
			// 
			// tabSilverlightView
			// 
			this.tabSilverlightView.Controls.Add(this.webBrowserSilverlight);
			this.tabSilverlightView.Location = new System.Drawing.Point(4, 22);
			this.tabSilverlightView.Name = "tabSilverlightView";
			this.tabSilverlightView.Size = new System.Drawing.Size(580, 566);
			this.tabSilverlightView.TabIndex = 5;
			this.tabSilverlightView.Text = "Silverlight View";
			this.tabSilverlightView.UseVisualStyleBackColor = true;
			// 
			// webBrowserSilverlight
			// 
			this.webBrowserSilverlight.AllowWebBrowserDrop = false;
			this.webBrowserSilverlight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowserSilverlight.Location = new System.Drawing.Point(0, 0);
			this.webBrowserSilverlight.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserSilverlight.Name = "webBrowserSilverlight";
			this.webBrowserSilverlight.Size = new System.Drawing.Size(580, 566);
			this.webBrowserSilverlight.TabIndex = 0;
			// 
			// tabSilverlightCode
			// 
			this.tabSilverlightCode.Controls.Add(this.textBoxSilverlight);
			this.tabSilverlightCode.Location = new System.Drawing.Point(4, 22);
			this.tabSilverlightCode.Name = "tabSilverlightCode";
			this.tabSilverlightCode.Size = new System.Drawing.Size(580, 566);
			this.tabSilverlightCode.TabIndex = 6;
			this.tabSilverlightCode.Text = "Silverlight Code";
			this.tabSilverlightCode.UseVisualStyleBackColor = true;
			// 
			// textBoxSilverlight
			// 
			this.textBoxSilverlight.AcceptsReturn = true;
			this.textBoxSilverlight.AcceptsTab = true;
			this.textBoxSilverlight.CausesValidation = false;
			this.textBoxSilverlight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxSilverlight.Location = new System.Drawing.Point(0, 0);
			this.textBoxSilverlight.Multiline = true;
			this.textBoxSilverlight.Name = "textBoxSilverlight";
			this.textBoxSilverlight.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxSilverlight.Size = new System.Drawing.Size(580, 566);
			this.textBoxSilverlight.TabIndex = 1;
			this.textBoxSilverlight.WordWrap = false;
			// 
			// tabSwfView
			// 
			this.tabSwfView.Controls.Add(this.webBrowserSwf);
			this.tabSwfView.Location = new System.Drawing.Point(4, 22);
			this.tabSwfView.Name = "tabSwfView";
			this.tabSwfView.Padding = new System.Windows.Forms.Padding(3);
			this.tabSwfView.Size = new System.Drawing.Size(580, 566);
			this.tabSwfView.TabIndex = 0;
			this.tabSwfView.Text = "Swf View";
			this.tabSwfView.UseVisualStyleBackColor = true;
			// 
			// webBrowserSwf
			// 
			this.webBrowserSwf.AllowWebBrowserDrop = false;
			this.webBrowserSwf.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowserSwf.Location = new System.Drawing.Point(3, 3);
			this.webBrowserSwf.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserSwf.Name = "webBrowserSwf";
			this.webBrowserSwf.Size = new System.Drawing.Size(574, 560);
			this.webBrowserSwf.TabIndex = 0;
			// 
			// tabSwfDump
			// 
			this.tabSwfDump.Controls.Add(this.textBoxSwfDump);
			this.tabSwfDump.Location = new System.Drawing.Point(4, 22);
			this.tabSwfDump.Name = "tabSwfDump";
			this.tabSwfDump.Size = new System.Drawing.Size(580, 566);
			this.tabSwfDump.TabIndex = 3;
			this.tabSwfDump.Text = "Swf Dump";
			this.tabSwfDump.UseVisualStyleBackColor = true;
			// 
			// textBoxSwfDump
			// 
			this.textBoxSwfDump.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxSwfDump.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxSwfDump.Location = new System.Drawing.Point(0, 0);
			this.textBoxSwfDump.Multiline = true;
			this.textBoxSwfDump.Name = "textBoxSwfDump";
			this.textBoxSwfDump.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxSwfDump.Size = new System.Drawing.Size(580, 566);
			this.textBoxSwfDump.TabIndex = 2;
			this.textBoxSwfDump.WordWrap = false;
			// 
			// XamlViewer
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(888, 638);
			this.Controls.Add(this.splitMain);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "XamlViewer";
			this.Text = "theConverted - Xaml Viewer";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitMain.Panel1.ResumeLayout(false);
			this.splitMain.Panel2.ResumeLayout(false);
			this.splitMain.ResumeLayout(false);
			this.tabControlContent.ResumeLayout(false);
			this.tabXamlView.ResumeLayout(false);
			this.tabXamlCode.ResumeLayout(false);
			this.tabXamlCode.PerformLayout();
			this.tabSilverlightView.ResumeLayout(false);
			this.tabSilverlightCode.ResumeLayout(false);
			this.tabSilverlightCode.PerformLayout();
			this.tabSwfView.ResumeLayout(false);
			this.tabSwfDump.ResumeLayout(false);
			this.tabSwfDump.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.SplitContainer splitMain;
		private System.Windows.Forms.TabControl tabControlContent;
		private System.Windows.Forms.TabPage tabSwfView;
		private System.Windows.Forms.TabPage tabXamlView;
		private System.Windows.Forms.TabPage tabXamlCode;
		private System.Windows.Forms.TabPage tabSwfDump;
		private System.Windows.Forms.TabPage tabSilverlightView;
		private System.Windows.Forms.TabPage tabSilverlightCode;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.WebBrowser webBrowserSwf;
		private System.Windows.Forms.TabControl tabControlFile;
		private System.Windows.Forms.WebBrowser webBrowserXaml;
		private System.Windows.Forms.TextBox textBoxXaml;
		private System.Windows.Forms.TextBox textBoxSwfDump;
		private System.Windows.Forms.TextBox textBoxSilverlight;
		private System.Windows.Forms.WebBrowser webBrowserSilverlight;
	}
}

