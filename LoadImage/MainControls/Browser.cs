using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Xml;
using StiLib;
using Localizer;

namespace dotNetStiEditor
{
	public partial class Browser : UserControl
	{
		public Browser(EditorMainForm editorForm, DirectoryInfo startDir)
		{
			Initialize();
			//this.toolStripComboBox1.SelectedIndex = 1;
			this.editorForm = editorForm;
			this.startDir = startDir;
			this.treeBrowser = new TreeBrowser();

			TreeNode startNode = new TreeNode(startDir.Name);
			startNode.Tag = startDir;
			startNode.Expand();
			this.treeView1.Nodes.Add(startNode);
			makeTreeNodes(this.treeView1.Nodes);

			this.treeView1.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);
			this.graphics = this.panel1.CreateGraphics();
		}

		void Initialize()
		{
			InitializeComponent();
            this.добавитьToolStripMenuItem.Text = LocalizerNameSpace.Localizer.GetString("AddToLibrary");
            this.toolStripComboBox1.Text = LocalizerNameSpace.Localizer.GetString("All");
            this.toolStripComboBox1.Items[0] = LocalizerNameSpace.Localizer.GetString("All");
            this.toolStripLabel1.Text = LocalizerNameSpace.Localizer.GetString("Foreshortening");
            this.refreshToolStripButton1.ToolTipText = LocalizerNameSpace.Localizer.GetString("Refresh");
		}

		void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			List<TreeNode> nodes = new List<TreeNode>();
			fileNames = new List<string>();
			nodes.Add(e.Node);
			makeFileNames(nodes);
			if (fileNames.Count > 0)
			{
				string fileName = fileNames[0];
				if (fileName.EndsWith(".sti", StringComparison.InvariantCultureIgnoreCase))
				{
					try
					{
						StciData stciData = new StciData(fileName, 0);
						List<ExtendedBitmap> bm = new List<ExtendedBitmap>();
						if (stciData._Indexed != null)
						{
							ETRLEData data = IndexedConverter.LoadIndexedImageData(stciData);
							bm = IndexedConverter.ConvertEtrleDataToBitmaps(data, 0);

						}
						else
						{
							ExtendedBitmap exBm = RGBConverter.GetBitmap(stciData);
							exBm.ApplicationData = null;
							bm.Add(exBm);
						}
						this.currentSti = bm;
						this.splitContainer1_Panel2_Paint(this.splitContainer1.Panel2, null);
					}
					catch (Exception exc)
					{
						MessageBox.Show(String.Format("{3} {0}\n{1}\n{2}",
                            fileName, exc.Message, exc.StackTrace, LocalizerNameSpace.Localizer.GetString("LoadingError")));
					}
				}
			}
		}

		Graphics graphics;
		List<ExtendedBitmap> currentSti = new List<ExtendedBitmap>();
		DirectoryInfo startDir;
		TreeBrowser treeBrowser;
		EditorMainForm editorForm;

		void makeTreeNodes(TreeNodeCollection parentNodes)
		{
			foreach (TreeNode parentNode in parentNodes)
			{
				object tag = parentNode.Tag;
				string fileName = String.Format(@"{0}\{1}.xml", 
					Application.StartupPath, treeBrowser.Name(tag));
				if (fileName != null && File.Exists(fileName))
				{
					XmlDocument xDoc = new XmlDocument();
					xDoc.Load(fileName);
					//  xDoc.Save("ANIMS2.xml");
					tag = xDoc.FirstChild;
				}

				foreach (object dir in treeBrowser.Childs(tag))
				{
					string name = treeBrowser.Name(dir);
					if (name != null)
					{
						TreeNode node = new TreeNode(name);
						node.Tag = dir;
						parentNode.Nodes.Add(node);
					}
				}
				makeTreeNodes(parentNode.Nodes);
			}
		}

		List<string> fileNames = new List<string>();

		void makeFileNames(IList parentNodes)
		{
			foreach (TreeNode parentNode in parentNodes)
			{
				object tag = parentNode.Tag;
				if (tag is FileInfo)
					fileNames.Add(((FileInfo)tag).FullName);
				else if (tag is XmlNode && parentNode.Nodes.Count == 0)
				{
					TreeNode node = parentNode;
					while (!(node.Tag is DirectoryInfo))
						node = node.Parent;
					string fileName = Path.Combine(((DirectoryInfo)node.Tag).FullName, parentNode.Text);
					fileNames.Add(fileName);
				}
				makeFileNames(parentNode.Nodes);
			}
		}

		private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			fileNames = new List<string>();
			List<TreeNode> selectedNodes = this.treeView1.SelectedNodes;
			makeFileNames(selectedNodes);
			foreach (string fileName in fileNames)
			{
				if (File.Exists(fileName))
					AddList(fileName, Foreshortening);
			}

			using (FileStream fs = new FileStream("globalPalette.txt", FileMode.Create))
				fs.Write(IndexedConverter.GlobalPalette, 0, 768);
		}

		public int Foreshortening
		{
			get { return this.toolStripComboBox1.SelectedIndex; }
		}

		List<KeyValuePair<string, int>> names = new List<KeyValuePair<string, int>>();
		public void AddList(string fileName, int foreshortening)
		{
			KeyValuePair<string, int> newList = new KeyValuePair<string, int>(fileName, foreshortening);
			if (!names.Contains(newList))
			{
				names.Add(newList);
				editorForm.AddRowToLibrary(fileName, foreshortening);
			}
		}

		public void AddList(string[] bitmapsFileNames, int foreshortening)
		{
			KeyValuePair<string, int> newList = 
				new KeyValuePair<string, int>(bitmapsFileNames[0], foreshortening);
			if (!names.Contains(newList))
			{
				names.Add(newList);
				editorForm.AddRowToLibrary(bitmapsFileNames, foreshortening);
			}
		}

		public void RemoveList(int index)
		{
			names.RemoveAt(index);
		}

		private void refreshToolStripButton1_Click(object sender, EventArgs e)
		{
			this.treeView1.SelectedNodes = new List<TreeNode>();
			this.treeView1.Nodes.Clear();

			TreeNode startNode = new TreeNode(startDir.Name);
			startNode.Tag = startDir;
			this.treeView1.Nodes.Add(startNode);
			this.makeTreeNodes(this.treeView1.Nodes);
			startNode.Expand();
		}

		private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
		{
			this.graphics = this.panel1.CreateGraphics(); // this.splitContainer1.Panel2.CreateGraphics();
			this.graphics.Clear(this.panel1.BackColor);
			int offsetX = 0; int offsetY = 0;
			int maxWidth = 0; int maxHeight = 0;
			int length = 1;
			foreach (ExtendedBitmap exBm in currentSti)
			{
				if (exBm.Bm.Width > maxWidth)
					maxWidth = exBm.Bm.Width;
				if (exBm.Bm.Height > maxHeight)
					maxHeight = exBm.Bm.Height;
				if (exBm.ForeshorteningLength != 0)
					length = exBm.ForeshorteningLength;
			}

			this.panel1.Width = maxWidth * length;
			this.panel1.Height = maxHeight * currentSti.Count / length;

			foreach (ExtendedBitmap exBm in currentSti)
			{
				if (length != 0 && currentSti.IndexOf(exBm) % length == 0 &&
					currentSti.IndexOf(exBm) / length != 0)
				{
					offsetX = 0;
					offsetY += maxHeight;
				}
				graphics.DrawImage(exBm.Bm, new Point(offsetX, offsetY));
				offsetX += maxWidth;
			}
		}
	}


	public class TreeBrowser
	{
		DirectoryInfo currentDir;
		public string Name(object node)
		{
			if (node is DirectoryInfo)
			{
				currentDir = (DirectoryInfo)node;
				return currentDir.Name;
			}
			if (node is FileInfo)
				return ((FileInfo)node).Name;
			if (node is XmlNode)
			{
				XmlNode xNode = (XmlNode)node;
				string name = xNode.Name;
				if (name == "File")
				{
					name = xNode.ChildNodes[0].InnerText;
					if (currentDir != null)
					{
						string fileName = Path.Combine(currentDir.FullName, name);
						if (!File.Exists(fileName))
							name = null;
					}
				}
				return name;
			}
			return null;
		}
		public IEnumerable Childs(object node)
		{
			if (node is DirectoryInfo)
			{
				DirectoryInfo dir = (DirectoryInfo)node;
				List<object> childs = new List<object>();
				List<object> files = new List<object>();
				try
				{
					childs.AddRange(dir.GetDirectories());
					files.AddRange(dir.GetFiles("*.STI"));
					files.AddRange(dir.GetFiles("*.JSD"));
				}
				catch (Exception exc)
				{
					MessageBox.Show(exc.Message);
				}
					files.Sort(delegate(object left, object rigth)
					{
						return ((FileInfo)left).Name.CompareTo(((FileInfo)rigth).Name);
					});
					childs.AddRange(files);
				return childs;
			}
			if (node is FileInfo)
				return new List<object>();
			if (node is XmlNode)
			{
				XmlNode xNode = (XmlNode)node;
				if (xNode.Name == "File")
					return new List<object>();
				return ((XmlNode)node).ChildNodes;
			}
			return null;
		}
	}
}
