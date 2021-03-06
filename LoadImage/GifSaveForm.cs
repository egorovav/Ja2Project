﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using StiLib;

namespace dotNetStiEditor
{
	public partial class GifSaveForm : Form
	{
		public GifSaveForm(List<ExtendedBitmap> movie)
		{
			InitializeComponent();
			this.movie = movie;
			this.openFileDialog.AddExtension = true;
			this.openFileDialog.DefaultExt = "gif";
			this.openFileDialog.Filter = "Image Files (*.gif)|*.gif";
		}

		List<ExtendedBitmap> movie = new List<ExtendedBitmap>();

		private void button3_Click(object sender, EventArgs e)
		{
			GIF.ConvertBitmapsToGif(this.movie, this.openFileDialog.FileName, 
				(UInt16)this.nudTimeOut.Value, this.chbTransparent.Checked, !this.chbGlobalPalette.Checked);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (this.openFileDialog.ShowDialog() == DialogResult.OK)
			{
				this.tbxFileName.Text = this.openFileDialog.FileName;
			}
		}
	}
}