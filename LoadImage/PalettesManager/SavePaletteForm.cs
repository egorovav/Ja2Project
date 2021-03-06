﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Resources = LocalizerNameSpace.Localizer;

namespace dotNetStiEditor
{
	public partial class SavePaletteForm : Form
	{
		public SavePaletteForm()
		{
			Initialize();
		}

		void Initialize()
		{
			InitializeComponent();
			this.cancelButton.Text = Resources.GetString("Cancel");
			this.textBox1.Text = Resources.GetString("MyPalette");
			this.label1.Text = Resources.GetString("PaletteName");
			this.okButton.Text = Resources.GetString("Save");
			this.Text = Resources.GetString("SavePalette");
		}

		public string PaletteName
		{
			get { return this.textBox1.Text; }
		}
	}
}