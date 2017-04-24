using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StiToGif
{
    public partial class ExceptionForm : Form
    {
        public ExceptionForm(string aExceptionText)
        {
            InitializeComponent();

            this.rtbxExceptionText.Text = aExceptionText;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
