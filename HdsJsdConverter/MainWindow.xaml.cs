using CommonWpfControls;
using Ja2Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HdsJsdConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine("Application is intended for convertation JSD (JA2 Structure Data) files to JSD-HD format.");
            _sb.AppendLine("Each of 100 JSD-tile cubes is divided into 8 parts.");
            _sb.AppendLine("So JSD-HD tiles contains 10x10x8=800 cubes.");
            this.tb.Text = _sb.ToString();

        }

        string[] FFileNames;

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult _result = MessageBox.Show(
    "Files will be overrwritten.", "Convertation JSD to JSD-HD", MessageBoxButton.OKCancel);
            if (_result != MessageBoxResult.OK)
                return;

            ProgressHolder _ph = new ProgressHolder();
            _ph.IsCancelable = true;
            ProgressWindow.Run(_ph);

            int _filesCount = 0;
            try
            {
                foreach (string _fileName in this.FFileNames)
                {
                    JsdFile.ConvertJsdFileToHighDefinition(_fileName);
                    _filesCount++;

                    _ph.Progress = 100 * _filesCount / this.FFileNames.Length;
                }
            }
            finally
            {
                _ph.Progress = -1;
            }
        }
        

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Title = "Select JSD Files.";
            _ofd.Filter = "JSD Files (*.jsd) | *.jsd";
            _ofd.Multiselect = true;
            if (_ofd.ShowDialog() == true)
            {
                this.FFileNames = _ofd.FileNames;
                StringBuilder _sb = new StringBuilder();
                foreach(string _fileName in this.FFileNames)
                {
                    _sb.AppendLine(_fileName);
                }
                this.tb.Text = String.Format("{0} files selected.\n{1}", this.FFileNames.Length, _sb.ToString());
            }
        }

        private void OpenDirectory_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog _fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (_fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.FFileNames = Directory.GetFiles(_fbd.SelectedPath, "*.jsd", SearchOption.AllDirectories);
                this.tb.Text = String.Format("{0} files selected.\n{1}", this.FFileNames.Length, _fbd.SelectedPath);
            }
        }
    }
}
