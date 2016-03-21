using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using Resources = LocalizerNameSpace.Localizer;

namespace dotNetStiEditor
{
    public partial class PaletteForm : Form
    {
        const int grid_size = 16;

        public PaletteForm(Bitmap bm)
        {
            InitializeComponent();
            this.dataGridView1.ReadOnly = true;

            this.FColors = bm.Palette.Entries;
            for(int i = 0; i < grid_size; i++)
            {
                this.dataGridView1.Rows.Add();

                for (int j = 0; j < grid_size; j++)
                {
                    int index = i * grid_size + j;
                    this.dataGridView1.Rows[i].Cells[j].Style.BackColor = this.FColors[index];
                    this.dataGridView1.Rows[i].Cells[j].Value = index;
                }
            }

            this.pictureBox1.Image = (Bitmap)bm.Clone();

            #region SET LOCAL STRINGS
            this.btnSave.Text = Resources.GetString("Save");
            this.Text = Resources.GetString("Palette");
            this.editToolStripMenuItem.Text = Resources.GetString("Edit");
            #endregion
        }

        Color[] FColors;
        public Color[] Colors
        {
            get { return this.FColors; }
        }

        Color FSelectedColor;

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if(this.dataGridView1.SelectedCells.Count > 0)
            {
                this.FSelectedColor = this.dataGridView1.SelectedCells[0].Style.BackColor;
            }

            ColorPalette _pal = this.pictureBox1.Image.Palette;

            for(int i = 0; i < this.FColors.Length; i++)
            {
                _pal.Entries[i] = this.FColors[i];
            }

            foreach(DataGridViewCell _cell in this.dataGridView1.SelectedCells)
            {
                int index = _cell.RowIndex * grid_size + _cell.ColumnIndex;
                _pal.Entries[index] = Color.Yellow;
            }

            this.pictureBox1.Image.Palette = _pal;
            this.pictureBox1.Refresh();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog _cd = new ColorDialog();
            _cd.Color = this.FSelectedColor;
            _cd.FullOpen = true;
            if (_cd.ShowDialog() == DialogResult.OK)
            {
                int dA = this.FSelectedColor.A - _cd.Color.A;
                int dR = this.FSelectedColor.R - _cd.Color.R;
                int dG = this.FSelectedColor.G - _cd.Color.G;
                int dB = this.FSelectedColor.B - _cd.Color.B;

                foreach(DataGridViewCell _cell in this.dataGridView1.SelectedCells)
                {
                    int _a = _cell.Style.BackColor.A - dA;
                    int _r = _cell.Style.BackColor.R - dR;
                    int _g = _cell.Style.BackColor.G - dG;
                    int _b = _cell.Style.BackColor.B - dB;
                    _cell.Style.BackColor = Color.FromArgb(IntToByte(_a), IntToByte(_r), IntToByte(_g), IntToByte(_b));
                    int index = _cell.RowIndex * grid_size + _cell.ColumnIndex;
                    this.FColors[index] = _cell.Style.BackColor;
                }
            }
        }

        private byte IntToByte(int i)
        {
            if (i < 0)
                return 0;
            else if (i > 255)
                return 255;
            else
                return (byte)i;
        }
    }
}
