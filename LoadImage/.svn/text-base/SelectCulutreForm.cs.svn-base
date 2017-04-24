using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace dotNetStiEditor
{
	public partial class SelectCulutreForm : Form
	{
		public SelectCulutreForm()
		{
			InitializeComponent();
		}

		public string CultureInfo
		{
			get
			{
				if (this.russianRadioButton1.Checked)
					return "ru";
				else if (this.englishRadioButton2.Checked)
					return "en";
				else if (this.deutschRadioButton3.Checked)
					return "de";
				else
					return "ru";
			}
		}
	}
}