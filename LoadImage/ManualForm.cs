using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Resources = LocalizerNameSpace.Localizer;
using System.IO;

namespace dotNetStiEditor
{
	public partial class ManualForm : Form
	{
		public ManualForm()
		{
			InitializeComponent();
			this.Text = Resources.GetString("Help");
			string helpFile = Path.Combine(Application.StartupPath, "help.htm");
			if (File.Exists(helpFile))
			{
				this.helpWebBrowser.Url = new Uri(helpFile);
			}
			else
			{
				this.helpWebBrowser.DocumentStream = new MemoryStream(Properties.Resources.help);
			}
				//MessageBox.Show(String.Format("{0} help.htm", Resources.GetString("NoFile")),
				//  Resources.GetString("Attention"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

		}
	}
}