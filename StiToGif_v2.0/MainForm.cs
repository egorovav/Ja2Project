using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using StiLib;
using System.IO;

namespace StiToGif
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.ttDelay.SetToolTip(this.nudDelay, "Frame delay.");
            this.ttDelay.SetToolTip(this.nudForeshorteningCount, "Foreshortening count.");
            this.ttDelay.SetToolTip(this.nudForeshorteningIndex, "Foreshortening index. Use zero for all.");
            this.ttDelay.SetToolTip(this.chbTransparentBackground, "Background is transparent.");

            this.nudDelay.Value = 10;
            this.nudForeshorteningCount.Value = 8;
            this.nudForeshorteningIndex.Value = 0;
        }

        private void btnStiToGif_Click(object sender, EventArgs e)
        {
            this.ofd.Filter = "GIF файлы (*.sti)|*.sti";
            if (this.ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string _stiFileName in this.ofd.FileNames)
                {
                    try
                    {
                        StciData stciData = new StciData(_stiFileName, 0);
                        List<ExtendedBitmap> _bmps = new List<ExtendedBitmap>();
                        int _foreshorteningIndex = (int)this.nudForeshorteningIndex.Value;
                        if (stciData._Indexed != null)
                        {
                            ETRLEData data = IndexedConverter.LoadIndexedImageData(stciData);
                            _bmps = IndexedConverter.ConvertEtrleDataToBitmaps(data, _foreshorteningIndex);

                        }
                        else
                        {
                            StringBuilder _sb = new StringBuilder();
                            _sb.AppendLine(String.Format(
                                "Не индексированный STI-файл {0}, конвертация в GIF невозможна.", _stiFileName));
                            _sb.AppendLine();
                            _sb.AppendLine(String.Format(
                                "Not indexed STI-file {0}, convertation to STI is denied.", _stiFileName));

                            MessageBox.Show(_sb.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string _gifFileName = Path.ChangeExtension(_stiFileName, "gif");
                        if(_foreshorteningIndex > 0)
                        {
                            string _fileNameWithoutExtention = Path.GetFileNameWithoutExtension(_stiFileName);
                            string _path = Path.Combine(Path.GetDirectoryName(_stiFileName), _fileNameWithoutExtention);
                            _gifFileName = String.Format("{0}_F_{1}.gif", _path, _foreshorteningIndex);
                        }
                        ushort _delay = (ushort)this.nudDelay.Value;
                        bool _isTransparentBackground = this.chbTransparentBackground.Checked;
                        GIF.ConvertBitmapsToGif(_bmps, _gifFileName, _delay, _isTransparentBackground, false);
                    }
                    catch (Exception exc)
                    {
                        string _excMessage = String.Format("{0}\n{1}\n{2}", _stiFileName, exc.Message, exc.StackTrace);
                        ExceptionForm _excForm = new ExceptionForm(_excMessage);
                        _excForm.ShowDialog();
                        //MessageBox.Show(_excMessage);
                    }
                    
                }
            }
        }

        private void btnGifToSti_Click(object sender, EventArgs e)
        {
            this.ofd.Filter = "GIF файлы (*.gif)|*.gif";
            if(this.ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string _gifFileName in this.ofd.FileNames)
                {
                    try
                    {

                        bool _useLocalPalette = false;
                        int _foreshorteningCount = (int)this.nudForeshorteningCount.Value;
                        List<ExtendedBitmap> _bmps =
                            GIF.ConvertGifToBitmaps(_gifFileName, _foreshorteningCount, out _useLocalPalette);

                        if (_foreshorteningCount != 0 && _bmps.Count % _foreshorteningCount != 0)
                        {
                            StringBuilder _sb = new StringBuilder();
                            _sb.AppendLine(String.Format(
                                "Количество кадров - {0} в файле {1} не делится на количество ракурсов - {2}.",
                                   _bmps.Count, _gifFileName, _foreshorteningCount));
                            _sb.AppendLine();
                            _sb.AppendLine(String.Format(
                                "Frame number - {0} in file {1} is not devided by foreghortening number - {2}.",
                                    _bmps.Count, _gifFileName, _foreshorteningCount));

                            MessageBox.Show(_sb.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        if (_useLocalPalette)
                        {
                            StringBuilder _sb = new StringBuilder();
                            _sb.AppendLine(String.Format(
                                "GIF-файл {0} использует локальные палитры, конвертация в STI невозможна.", _gifFileName));
                            _sb.AppendLine();
                            _sb.AppendLine(String.Format(
                                "GIF-file {0} uses local palettes, convertation to STI is denied.", _gifFileName));

                            MessageBox.Show(_sb.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string _stiFileName = Path.ChangeExtension(_gifFileName, "sti");
                        IndexedConverter.ConvertBitmapsToEtrleData(_bmps, _stiFileName);
                    }
                    catch (Exception exc)
                    {
                        string _excMessage = String.Format("{0}\n{1}\n{2}", _gifFileName, exc.Message, exc.StackTrace);
                        ExceptionForm _excForm = new ExceptionForm(_excMessage);
                        _excForm.ShowDialog();
                        //MessageBox.Show(_excMessage);
                    }
                }
            }
        }
    }
}
