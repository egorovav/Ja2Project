using System;
using System.Collections.Generic;
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

namespace CommonWpfControls
{
    /// <summary>
    /// Interaction logic for TestFileControl.xaml
    /// </summary>
    public partial class TestFileControl : UserControl
    {
        public TestFileControl()
        {
            InitializeComponent();
        }

        private LinearGradientBrush GetBrush()
        {
            LinearGradientBrush _brush = new LinearGradientBrush();
            GradientStopCollection _grStops = new GradientStopCollection();
            Random _rnd = new Random(DateTime.Now.Millisecond);

            Color _lightColor = new Color();
            _lightColor.R = (byte)_rnd.Next(0, 255); 
            _lightColor.G = (byte)_rnd.Next(0, 255);
            _lightColor.B = (byte)_rnd.Next(0, 255);

            Color _mediumColor = new Color();
            _mediumColor.R = (byte)(_lightColor.R / 2);
            _mediumColor.G = (byte)(_lightColor.G / 2);
            _mediumColor.B = (byte)(_lightColor.G / 2);

            Color _darkColor = new Color();
            _darkColor.R = (byte)(_lightColor.R / 3);
            _darkColor.G = (byte)(_lightColor.G / 3);
            _darkColor.B = (byte)(_lightColor.G / 3);

            _grStops.Add(new GradientStop(_mediumColor, 0));
            _grStops.Add(new GradientStop(_lightColor, _rnd.Next(10)/10f));
            _grStops.Add(new GradientStop(_darkColor, 1));

            _brush.GradientStops = _grStops;

            return _brush;

        }

        static TestFileControl()
        {
            FileNameProperty = DependencyProperty.Register(
                "FileName", 
                typeof(String), 
                typeof(TestFileControl), 
                new UIPropertyMetadata(String.Empty, FileNameChanged));

            FolderNameProperty = DependencyProperty.Register(
                "FolderName",
                typeof(String),
                typeof(TestFileControl),
                new UIPropertyMetadata(String.Empty, FolderNameChanged));

            ResultStringProperty = DependencyProperty.Register(
                "ResultString",
                typeof(String),
                typeof(TestFileControl),
                new UIPropertyMetadata(String.Empty, ResultStringChanged));

            StatusStringProperty = DependencyProperty.Register(
                "StatusString",
                typeof(String),
                typeof(TestFileControl),
                new UIPropertyMetadata(String.Empty, StatusStringChanged));

            ErrorStringProperty = DependencyProperty.Register(
                "ErrorString",
                typeof(String),
                typeof(TestFileControl),
                new UIPropertyMetadata(String.Empty, ErrorStringChanged));
        }

        public static readonly DependencyProperty FileNameProperty;

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        private static void FileNameChanged(DependencyObject aDepObj, DependencyPropertyChangedEventArgs aArgs)
        {
            TestFileControl _tfc = (TestFileControl)aDepObj;
            if (aArgs.NewValue != null)
                _tfc.tbxFileName.Text = aArgs.NewValue.ToString();
            else
                _tfc.tbxFileName.Text = String.Empty;
        }

        public static readonly DependencyProperty FolderNameProperty;

        public string FolderName
        {
            get { return (string)GetValue(FolderNameProperty); }
            set { SetValue(FolderNameProperty, value); }
        }

        private static void FolderNameChanged(DependencyObject aDepObj, DependencyPropertyChangedEventArgs aArgs)
        {
            TestFileControl _tfc = (TestFileControl)aDepObj;
            if (aArgs.NewValue != null)
                _tfc.tbxFolderName.Text = aArgs.NewValue.ToString();
            else
                _tfc.tbxFolderName.Text = String.Empty;
        }

        public static readonly DependencyProperty ResultStringProperty;

        public string ResultString
        {
            get { return (string)GetValue(ResultStringProperty); }
            set { SetValue(ResultStringProperty, value); }
        }

        private static void ResultStringChanged(DependencyObject aDepObj, DependencyPropertyChangedEventArgs aArgs)
        {
            TestFileControl _tfc = (TestFileControl)aDepObj;
            if (aArgs.NewValue != null)
                _tfc.tbTestResult.Text = aArgs.NewValue.ToString();
            else
                _tfc.tbTestResult.Text = String.Empty;
        }

        public static readonly DependencyProperty StatusStringProperty;

        public string StatusString
        {
            get { return (string)GetValue(StatusStringProperty); }
            set { SetValue(StatusStringProperty, value); }
        }

        private static void StatusStringChanged(DependencyObject aDepObj, DependencyPropertyChangedEventArgs aArgs)
        {
            TestFileControl _tfc = (TestFileControl)aDepObj;
            if (aArgs.NewValue != null)
                _tfc.tbTestStatus.Text = aArgs.NewValue.ToString();
            else
                _tfc.tbTestStatus.Text = String.Empty;
        }

        public static readonly DependencyProperty ErrorStringProperty;

        public string ErrorString
        {
            get { return (string)GetValue(ErrorStringProperty); }
            set { SetValue(ErrorStringProperty, value); }
        }

        private static void ErrorStringChanged(DependencyObject aDepObj, DependencyPropertyChangedEventArgs aArgs)
        {
            TestFileControl _tfc = (TestFileControl)aDepObj;
            if (aArgs.NewValue != null)
                _tfc.tbTestErrors.Text = aArgs.NewValue.ToString();
            else
                _tfc.tbTestErrors.Text = String.Empty;
        }

        public string InitialDirName
        {
            get;
            set;
        }

        public string FileFilter
        {
            get;
            set;
        }

        private void OpenCmdExecute(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog _fbd = new System.Windows.Forms.FolderBrowserDialog();
            _fbd.SelectedPath = this.InitialDirName;
            if (_fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.FolderName = _fbd.SelectedPath;
            }
        }

        private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog _ofd = new Microsoft.Win32.OpenFileDialog();
            _ofd.InitialDirectory = this.InitialDirName;
            _ofd.Filter = this.FileFilter;
            _ofd.CheckFileExists = false;
            if (_ofd.ShowDialog() == true)
            {
                this.FileName = _ofd.FileName;
            }
        }        
    }
}
