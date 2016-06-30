using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            this.chbIndicatePixels.Text = Resources.GetString("IndicatePixels");
            this.fileToolStripMenuItem.Text = Resources.GetString("File");
            this.openToolStripMenuItem.Text = Resources.GetString("Open...");
            this.saveToolStripMenuItem.Text = Resources.GetString("Save...");

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

            if (this.chbIndicatePixels.Checked)
            {
                foreach (DataGridViewCell _cell in this.dataGridView1.SelectedCells)
                {
                    int index = _cell.RowIndex * grid_size + _cell.ColumnIndex;
                    _pal.Entries[index] = Color.Yellow;
                }
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

            this.RefreshPreview();
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

        private void RefreshPreview()
        {
            ColorPalette _pal = this.pictureBox1.Image.Palette;

            for (int i = 0; i < this.FColors.Length; i++)
            {
                _pal.Entries[i] = this.FColors[i];
            }

            this.pictureBox1.Image.Palette = _pal;
            this.pictureBox1.Refresh();
        }

        private void chbIndicatePixels_CheckedChanged(object sender, EventArgs e)
        {
            ColorPalette _pal = this.pictureBox1.Image.Palette;
            if (this.chbIndicatePixels.Checked)
            {
                foreach (DataGridViewCell _cell in this.dataGridView1.SelectedCells)
                {
                    int index = _cell.RowIndex * grid_size + _cell.ColumnIndex;
                    _pal.Entries[index] = Color.Yellow;
                }
            }
            else
            {
                foreach (DataGridViewCell _cell in this.dataGridView1.SelectedCells)
                {
                    int index = _cell.RowIndex * grid_size + _cell.ColumnIndex;
                    _pal.Entries[index] = this.FColors[index];
                }
            }

            this.pictureBox1.Image.Palette = _pal;
            this.pictureBox1.Refresh();
        }

        const int palette_size = grid_size * grid_size;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Filter = "(*.stp;*.pal;*.act)|*.stp;*.pal;*.act";

            if(_ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = Path.GetExtension(_ofd.FileName);

                switch (ext.ToUpper())
                {
                    case ".STP": { this.LoadStpPalette(_ofd.FileName); break; }
                    case ".ACT": { this.LoadStpPalette(_ofd.FileName); break; }
                    case ".PAL": { this.LoadPalPalette(_ofd.FileName); break; }
                    default:
                        {
                            MessageBox.Show(String.Format(
                                "Invalid file format. File {0}. Only STP, PAL, ACT files are supported", 
                                    _ofd.FileName));
                            break;
                        }
                }
            }

            this.RefreshPreview();
        }

        // Pal - palette file (Windows format used in Fotoshop)
        private void LoadPalPalette(string fileName)
        {
            using (FileStream _fs = new FileStream(fileName, FileMode.Open))
            {
                using (BinaryReader _br = new BinaryReader(_fs))
                {
                    char[] riff = _br.ReadChars(4);
                    int fileLength = _br.ReadInt32();
                    char[] pal_data = _br.ReadChars(8);
                    int data_size = _br.ReadInt32();
                    int version = 0; // _br.ReadInt16();

                    if (new String(riff) != "RIFF")
                        MessageBox.Show(String.Format("Invalid file format. File {0}.", fileName));

                    MessageBox.Show(String.Format("{0} \nfile length - {1} \n{2} - {3} \nversion - {4}", 
                        new String(riff), fileLength, new String(pal_data), data_size, version));                   

                    this.FColors = new Color[palette_size];
                    for (int i = 0; i < grid_size; i++)
                    {
                        for (int j = 0; j < grid_size; j++)
                        {
                            int index = i * grid_size + j;

                            if (data_size > 0)
                            {                             
                                Color _ijc = Color.FromArgb(_fs.ReadByte(), _fs.ReadByte(), _fs.ReadByte());
                                _fs.ReadByte();
                                this.FColors[index] = _ijc;

                                this.dataGridView1.Rows[i].Cells[j].Style.BackColor = _ijc;
                            }
                            else
                            {
                                this.dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
                                this.FColors[index] = Color.Black;
                            }
                            this.dataGridView1.Rows[i].Cells[j].Value = index;

                            data_size -= 4;
                        }
                    }
                }
            }

            this.dataGridView1.Refresh();
        }

        // Stp - palette file (German STI-Edit). 
        private void LoadStpPalette(string fileName)
        {
            using (FileStream _fs = new FileStream(fileName, FileMode.Open))
            {
                if (_fs.Length < palette_size * 3)
                {
                    MessageBox.Show(String.Format("Invalid file format. File {0}.", fileName));
                    return;
                }

                this.FColors = new Color[palette_size];
                for (int i = 0; i < grid_size; i++)
                {
                    for (int j = 0; j < grid_size; j++)
                    {
                        int index = i * grid_size + j;

                        Color _ijc = Color.FromArgb(_fs.ReadByte(), _fs.ReadByte(), _fs.ReadByte());
                        this.FColors[index] = _ijc;

                        this.dataGridView1.Rows[i].Cells[j].Style.BackColor = _ijc;
                        this.dataGridView1.Rows[i].Cells[j].Value = index;
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog _sfd = new SaveFileDialog();
            _sfd.Filter = "(*.stp;*.pal;*act)|*.stp;*pal;*.act";
            _sfd.DefaultExt = "stp";

            if(_sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string _ext = Path.GetExtension(_sfd.FileName);

                switch (_ext.ToUpper())
                {

                    case ".STP": { SaveStpPalette(_sfd.FileName); break; }
                    case ".ACT": { SaveStpPalette(_sfd.FileName); break; }
                    case ".PAL": { SavePalPalette(_sfd.FileName); break; }
                    default:
                        {
                            MessageBox.Show(String.Format(
                                "Invalid file format. File {0}. Only STP, PAL, ACT files are supported",
                                    _sfd.FileName));
                            break;
                        }
                }
            }
        }

        private void SavePalPalette(string fileName)
        {
            FileStream _fs = null;
            try
            {
                _fs = File.OpenWrite(fileName);
            }
            catch (Exception ex)
            {
                StringBuilder _sb = new StringBuilder();
                _sb.AppendLine(String.Format("Open file error. File {0}", fileName));
                _sb.AppendLine(ex.Message);
                MessageBox.Show(_sb.ToString());
            }

            if (_fs != null)
            {
                using (_fs)
                {
                    int data_size = palette_size * 4;
                    int header_size = 20;
                    using (BinaryWriter _br = new BinaryWriter(_fs))
                    {
                        _br.Write(new char[] { 'R', 'I', 'F', 'F' });
                        _br.Write(data_size + header_size);
                        _br.Write(new char[] {'P', 'A', 'L', ' ', 'd', 'a', 't', 'a' });
                        _br.Write(data_size);

                        foreach (Color _c in this.FColors)
                        {
                            _br.Write(_c.R);
                            _br.Write(_c.G);
                            _br.Write(_c.B);
                            _br.Write((byte)255);
                        }
                    }
                }
            }
        }

        private void SaveStpPalette(string fileName)
        {
            FileStream _fs = null;
            try
            {
                _fs = File.OpenWrite(fileName);
            }
            catch (Exception ex)
            {
                StringBuilder _sb = new StringBuilder();
                _sb.AppendLine(String.Format("Open file error. File {0}", fileName));
                _sb.AppendLine(ex.Message);
                MessageBox.Show(_sb.ToString());
            }

            if (_fs != null)
            {
                using (_fs)
                {
                    foreach (Color _c in this.FColors)
                    {
                        _fs.WriteByte(_c.R);
                        _fs.WriteByte(_c.G);
                        _fs.WriteByte(_c.B);
                    }
                }
            }
        }
    }
}
