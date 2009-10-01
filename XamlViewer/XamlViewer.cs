/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.CodeDom.Compiler;

using DDW.Swf;
using DDW.Vex;
using DDW.Xaml;

namespace DDW.XamlViewer
{
	public partial class XamlViewer : Form
	{
		private List<SwfCompilationUnit> swfs = new List<SwfCompilationUnit>();
		private List<VexObject> vexObjects = new List<VexObject>();
		private List<string> xamlFileLocations = new List<string>();
		private List<string> silverlightFileLocations = new List<string>();

		private System.ComponentModel.BackgroundWorker bw;

		private StringBuilder sb = new StringBuilder();
		private string msg;
		private string fileName;
		public bool isConverting;
		public int fileIndex = -1;
		private uint selectedFileName = uint.MaxValue;
		string partName = "";

		public XamlViewer()
		{
			InitializeComponent();
			InitializeCustom();
		}
		private void InitializeCustom()
		{
			this.bw = new System.ComponentModel.BackgroundWorker();
			this.bw.WorkerSupportsCancellation = false;
			this.bw.DoWork += new DoWorkEventHandler(this.ConvertFileWorker);
			this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.ConvertComplete);

			this.tabControlFile.Selected += new TabControlEventHandler(tabControlFile_Selected);
		}
		private void ConvertFiles(string[] fileNames)
		{
			this.Cursor = Cursors.WaitCursor;
			if (fileNames.Length > 0)
			{
				string pl = fileNames.Length > 1 ? "s" : "";
				AddStatusText("Begin converting " + fileNames.Length + " file" + pl + ".");

				for (int i = 0; i < fileNames.Length; i++)
				{
					try
					{
						fileName = fileNames[i];
						AddStatusText("Converting: " + fileName + ".");
						string fileDir = Path.GetDirectoryName(fileName);
						Directory.SetCurrentDirectory(fileDir);
						//this.Refresh();

						this.bw.RunWorkerAsync();

						//fileConvertThread = new Thread(new ThreadStart(this.ConvertFile));
						//fileConvertThread.Name = "File Convert";
						//fileConvertThread.Start();

						AddStatusText(msg);
					}
					catch (Exception e)
					{
						AddStatusText("Unsupported file element -- conversion failure. " + e.Message);
					}
				}
				AddStatusText("Complete.");
			}
		}
		#region Drag and Drop
		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			string[] fileNames = (string[])drgevent.Data.GetData("FileDrop");
			ConvertFiles(fileNames);
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
		#endregion

		private void ConvertFileWorker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker bw = sender as BackgroundWorker;
			e.Result = ConvertFile(bw);

			if (bw.CancellationPending)
			{
				e.Cancel = true;
			}
		}
		private void ConvertComplete(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
			}
			else if (e.Error != null)
			{
			}
			else
			{
				ConversionObjects co = (ConversionObjects)e.Result;
				AddTab(co);
			}
			this.Cursor = Cursors.Default;
		}

		void tv_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeView tv = (TreeView)sender;
			if (tv.SelectedNode != null)
			{
				if (tv.SelectedNode.Name == "root")
				{
					partName = "";
					selectedFileName = uint.MaxValue;
				}
				else
				{
					uint nm = uint.Parse(tv.SelectedNode.Name);
					if (selectedFileName != nm)
					{
						partName = tv.SelectedNode.Name;
						selectedFileName = nm;
						IDefinition def = vexObjects[this.fileIndex].Definitions[selectedFileName];
						WPFRenderer wpfr = new WPFRenderer();
						string xamlFileName;
						wpfr.GenerateXamlPart(vexObjects[this.fileIndex], def, out xamlFileName);

						Silverlight10Renderer slfr = new Silverlight10Renderer();
						string slFileName;
						slfr.GenerateXamlPart(vexObjects[this.fileIndex], def, out slFileName);
					}
				}
				RefreshContentPane();
			}
		}
		private ConversionObjects ConvertFile(BackgroundWorker bw)
		{
			SwfCompilationUnit scu;
			VexObject v;
			string xamlFileName;

			isConverting = true;
			msg = DDW.SwfToXaml.SwfToXaml.Convert(fileName, false, out scu, out v, out xamlFileName);
			isConverting = false;

			swfs.Add(scu);
			vexObjects.Add(v);
			xamlFileLocations.Add(xamlFileName);

			ConversionObjects co = new ConversionObjects(scu, v, xamlFileName, msg);

			string silverlightFileName;
			string svMsg = DDW.SwfToXaml.SwfToXaml.Convert(true, scu, v, out silverlightFileName);
			silverlightFileLocations.Add(silverlightFileName);

			return co;
		}
		private void AddTab(ConversionObjects co)
		{
			//string endName = co.xamlFileName.Remove(0, co.xamlFileName.LastIndexOf("/") + 1);
			//string tabName = endName.Substring(0, endName.LastIndexOf('.'));
			TabPage tp = new TabPage(co.scu.Name);
			this.tabControlFile.TabPages.Add(tp);
			ActivateTab(this.tabControlFile.TabPages.Count - 1);
		}
		private void ActivateTab(int index)
		{
			this.fileIndex = index;
			if (index == -1)
			{
				return;
			}
			TabPage tp = this.tabControlFile.TabPages[index];
			this.tabControlFile.SelectTab(index);

			TreeView tv = new TreeView();
			tv.SuspendLayout();
			tp.Controls.Add(tv);
			tv.Dock = DockStyle.Fill;
			AddSwfToTreeView(swfs[fileIndex], tv);
			tv.AfterSelect += new TreeViewEventHandler(tv_AfterSelect);
			tv.ResumeLayout();

			RefreshContentPane();
		}


		void tabControlFile_Selected(object sender, TabControlEventArgs e)
		{
			ActivateTab(e.TabPageIndex);
		}
		private void AddSwfToTreeView(SwfCompilationUnit scu, TreeView tv)
		{
			// temp: move this to subclasss of treeview
			tv.Nodes.Add("root", scu.Name);
			TreeNode root = tv.Nodes[0];

			for (int i = 0; i < scu.Tags.Count; i++)
			{
				switch (scu.Tags[i].TagType)
				{
					case TagType.DefineShape:
					case TagType.DefineShape2:
						DefineShape2Tag ds2 = (DefineShape2Tag)scu.Tags[i];
						root.Nodes.Add(ds2.ShapeId.ToString(), ds2.ShapeId + ": DefineShape2");
						break;

					case TagType.DefineShape3:
						DefineShape3Tag ds3 = (DefineShape3Tag)scu.Tags[i];
						root.Nodes.Add(ds3.ShapeId.ToString(), ds3.ShapeId + ": DefineShape3");
						break;

					case TagType.DefineShape4:
						DefineShape4Tag ds4 = (DefineShape4Tag)scu.Tags[i];
						root.Nodes.Add(ds4.ShapeId.ToString(), ds4.ShapeId + ": DefineShape4");
						break;
				}
			}
		}
		private void RefreshContentPane()
		{
			if (this.fileIndex == -1)
			{
				return;
			}

			switch ((TabWindow)this.tabControlContent.SelectedIndex)
			{
				case TabWindow.XamlView:
					string org = xamlFileLocations[fileIndex];
					if (partName != "")
					{
						org = org.Replace(".xaml", "_" + partName + ".xaml");
					}
					Uri xamlUrl = new Uri(@"file://" + org);
					webBrowserXaml.Navigate(xamlUrl);
					break;
				case TabWindow.XamlCode:
					string orgCode = xamlFileLocations[fileIndex];
					if (partName != "")
					{
						orgCode = orgCode.Replace(".xaml", "_" + partName + ".xaml");
					}
					using (TextReader streamReader = new StreamReader(orgCode))
					{
						textBoxXaml.Text = streamReader.ReadToEnd();
					}
					break;
				case TabWindow.SilverlightView:
					string orgSl = silverlightFileLocations[fileIndex];
					if (partName != "")
					{
						orgSl = orgSl.Replace(".html", "_" + partName + ".html");
					}
					Uri slUrl = new Uri(@"file://" + orgSl);
					webBrowserSilverlight.Navigate(slUrl);
					break;
				case TabWindow.SilverlightCode:
					string orgSlCode = silverlightFileLocations[fileIndex];
					if (partName != "")
					{
						orgSlCode = orgSlCode.Replace(".html", "_" + partName + ".html");
					}

					string slXaml = orgSlCode.Replace(".html", ".xaml");
					using (TextReader streamReader = new StreamReader(slXaml))
					{
						textBoxSilverlight.Text = streamReader.ReadToEnd();
					}
					break;
				case TabWindow.SwfView:
					Uri swfUrl = new Uri(@"file://" + swfs[fileIndex].FullPath);
					webBrowserSwf.Navigate(swfUrl);
					break;
				case TabWindow.SwfDump:
					StringWriter sw = new StringWriter();
					IndentedTextWriter w = new IndentedTextWriter(sw);
					swfs[fileIndex].Dump(w);

					textBoxSwfDump.Text = sw.ToString();
					break;
				case TabWindow.VexCode:
					break;
				default:
					break;
			}
		}
		private void AddStatusText(string text)
		{
			sb.Append(text + System.Environment.NewLine);
		}

		void tabControlContent_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			RefreshContentPane();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Multiselect = true;
			ofd.ShowDialog();
			if (ofd.FileNames.Length > 0)
			{
				ConvertFiles(ofd.FileNames);
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			switch ((TabWindow)this.tabControlContent.SelectedIndex)
			{
				case TabWindow.XamlView:
				case TabWindow.XamlCode:
					Clipboard.SetText(textBoxXaml.Text);
					break;
				case TabWindow.SilverlightView:
				case TabWindow.SilverlightCode:
					Clipboard.SetText(textBoxSilverlight.Text);
					break;
				case TabWindow.SwfView:
					break;
				case TabWindow.SwfDump:
					Clipboard.SetText(textBoxSwfDump.Text);
					break;
				case TabWindow.VexCode:
					break;
				default:
					break;
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch ((TabWindow)this.tabControlContent.SelectedIndex)
			{
				case TabWindow.XamlView:
				case TabWindow.XamlCode:
					string org = xamlFileLocations[fileIndex];
					if (partName != "")
					{
						org = org.Replace(".xaml", "_" + partName + ".xaml");
					}
					using (StreamWriter writer = new StreamWriter(org))
					{
						writer.Write(textBoxXaml.Text);
					}
					break;
				case TabWindow.SilverlightView:
				case TabWindow.SilverlightCode:
					string orgSlCode = silverlightFileLocations[fileIndex];
					if (partName != "")
					{
						orgSlCode = orgSlCode.Replace(".html", "_" + partName + ".html");
					}
					string slXaml = orgSlCode.Replace(".html", ".xaml");
					using (StreamWriter writer = new StreamWriter(slXaml))
					{
						writer.Write(textBoxSilverlight.Text);
					}
					break;
				case TabWindow.SwfView:
					break;
				case TabWindow.SwfDump:
					Clipboard.SetText(textBoxSwfDump.Text);
					break;
				case TabWindow.VexCode:
					break;
				default:
					break;
			}

		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("http://theconverted.ca\n\n© 2007 Debreuil Digital Works", "theConverted Xaml Viewer");
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CloseFile(fileIndex);
		}
		private void CloseFile(int index)
		{
			//fileIndex = -1;
			//if (index < this.tabControlFile.TabPages.Count - 1)
			//{
			//    fileIndex = index;
			//}
			//else if (index > 0)
			//{
			//    fileIndex = index - 1;
			//}
			this.tabControlFile.TabPages.RemoveAt(index);

			xamlFileLocations.RemoveAt(index);
			silverlightFileLocations.RemoveAt(index);

			webBrowserXaml.Navigate("");
			textBoxXaml.Text = "";
			webBrowserSilverlight.Navigate("");
			textBoxSilverlight.Text = "";
			textBoxSwfDump.Text = "";

			RefreshContentPane();
		}
	}
}