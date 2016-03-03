using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace dotNetStiEditor
{
	public partial class DeletePaletteForm : Form
	{
		public DeletePaletteForm(List<PaletteRecord> palettes)
		{
			Initialize();
			this.comboBox1.DisplayMember = "Name";
		//	this.comboBox1.ValueMember = "ColorListNames";
			this.comboBox1.DataSource = palettes;
		}
		void Initialize()
		{
			InitializeComponent();
            this.cancelButton.Text = LocalizerNameSpace.Localizer.GetString("Cancel");
            this.label1.Text = LocalizerNameSpace.Localizer.GetString("PaletteName");
            this.okButton.Text = LocalizerNameSpace.Localizer.GetString("Remove");
            this.Text = LocalizerNameSpace.Localizer.GetString("RemovePalette");
		}

		public int PaletteIndex
		{
			get { return this.comboBox1.SelectedIndex; }
		}
	}
}