using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JsdEditor
{
    /// <summary>
    /// Interaction logic for NewJsdFileView.xaml
    /// </summary>
    public partial class NewJsdFileView : Window
    {
        public NewJsdFileView()
        {
            InitializeComponent();
        }

        public bool IsOk
        {
            get;
            protected set;
        }

        public JsdFileViewModel NewJsdFileViewModel
        {
            get;
            protected set;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            JsdFileViewModel _jsdFileViewModel = this.CreateJsdFile();

            if (_jsdFileViewModel != null)
            {
                this.NewJsdFileViewModel = _jsdFileViewModel;
                this.IsOk = true;
                this.Close();
            }
        }

        private JsdFileViewModel CreateJsdFile()
        {
            bool _isOk = true;
            StringBuilder _sbErrorMessage = new StringBuilder("Error:\n");
            if (this.tbFileName.Text == String.Empty)
            {
                _sbErrorMessage.AppendLine("File name is empty.");
                _isOk = false;
            }

            int _structsNum = 0;
            if (!Int32.TryParse(this.tbStructsNum.Text, out _structsNum))
            {
                _sbErrorMessage.AppendLine(String.Format("\"{0}\" is not a number.", this.tbStructsNum.Text));
                _isOk = false;
            }

            int _auxDataNum = 0;
            if (!Int32.TryParse(this.tbAuxDataNum.Text, out _auxDataNum))
            {
                _sbErrorMessage.AppendLine(String.Format("\"{0}\" is not a number.", this.tbAuxDataNum.Text));
                _isOk = false;
            }

            int _tileLocImagesNum = 0;
            if (!Int32.TryParse(this.tbTileLocsImagesNum.Text, out _tileLocImagesNum))
            {
                _sbErrorMessage.AppendLine(String.Format("\"{0}\" is not a number.", this.tbTileLocsImagesNum.Text));
                _isOk = false;
            }

            bool _isHighDefenition = this.cbIsHD.IsChecked == true;

            JsdFileViewModel _jsdFileViewModel = null;
            if (_isOk)
            {
                Ja2Data.JsdFile _jsdFile = new Ja2Data.JsdFile(
                    _structsNum, _auxDataNum, _tileLocImagesNum, _isHighDefenition);
                _jsdFileViewModel = new JsdFileViewModel(_jsdFile, this.tbFileName.Text);
            }
            else
            {
                this.tbError.Text = _sbErrorMessage.ToString();
                this.ttErrorMessage.IsOpen = true;
            }

            return _jsdFileViewModel;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.IsOk = false;
            this.Close();
        }
    }
}

