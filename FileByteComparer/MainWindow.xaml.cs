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

namespace FileByteComparer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private byte[] FFile1;
        private byte[] FFile2;

        private void btnOpenFile1_Click(object sender, RoutedEventArgs e)
        {
            this.tbxFileName1.Text = ReadFile(ref this.FFile1);
        }

        private void btnOpenFile2_Click(object sender, RoutedEventArgs e)
        {
            this.tbxFileName2.Text = ReadFile(ref this.FFile2);
        }

        private string ReadFile(ref byte[] aFile)
        {
            Microsoft.Win32.OpenFileDialog _ofd = new Microsoft.Win32.OpenFileDialog();
            _ofd.CheckFileExists = false;
            if (_ofd.ShowDialog() == true)
            {
                using (FileStream _fs = new FileStream(_ofd.FileName, FileMode.Open))
                {
                    aFile = new byte[_fs.Length];
                    _fs.Read(aFile, 0, aFile.Length);
                }

            }

            return _ofd.FileName;
        }

        private string CompareFiles()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendFormat("File 1 Length - {0}\n", this.FFile1.Length);
            _sb.AppendFormat("File 2 Length - {0}\n", this.FFile2.Length);

            for(int i = 0; i < this.FFile1.Length; i++)
            {
                if (this.FFile1[i] != this.FFile2[i])
                    _sb.AppendFormat("Byte {0}: File 1 - {1}, File 2 - {2}\n", i, this.FFile1[i], this.FFile2[i]);
            }

            return _sb.ToString();
        }

        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            this.tbResult.Text = this.CompareFiles();
        }  
    }
}
