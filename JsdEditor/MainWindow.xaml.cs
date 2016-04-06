using Ja2Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
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

namespace JsdEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel
        {
            get { return this.DataContext as MainViewModel; }
            set { this.DataContext = value; }
        }

        public MainWindow()
        {
            this.ViewModel = new MainViewModel();
            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            InitializeComponent();

            string _initialFileName = ((App)Application.Current).FileName;
            if (!String.IsNullOrEmpty(_initialFileName))
                this.ViewModel.FilesNames = new string[] { _initialFileName };

            try
            {
                System.Configuration.AppSettingsReader _asr = new System.Configuration.AppSettingsReader();
                this.ViewModel.FolderName = (string)_asr.GetValue("FolderName", typeof(String));
            }
            catch (Exception _exc)
            {
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == MainViewModel.SelectedJsdFilePropertyName)
            {
                if (this.ViewModel.SelectedJsdFile != null)
                {
                    this.jfv.ViewModel = this.ViewModel.SelectedJsdFile;
                    this.jfv.ViewModel.PropertyChanged += SelectedJsdFile_PropertyChanged;

                    StructureViewModel _selectedStructure = null;
                    if (this.jfv.ViewModel.Structs.Count > 0)
                    {
                        if (this.jfv.ViewModel.SelectedStruct != this.jfv.ViewModel.Structs[0])
                        {
                            _selectedStructure = this.jfv.ViewModel.Structs[0];
                            if (this.jfv.ViewModel.Images != null && this.jfv.ViewModel.Images.Length > 0)
                                _selectedStructure.Image = (StructureImage)this.jfv.lbImages.Items[0];
                            this.jfv.ViewModel.SelectedStruct = _selectedStructure;
                        }
                        else
                            this.SetJsdViewModel(this.jfv.ViewModel);
                    }
                    else
                    {
                        _selectedStructure = new StructureViewModel(null);
                        if (this.jfv.ViewModel.Images != null && this.jfv.ViewModel.Images.Length > 0)
                            _selectedStructure.Image = (StructureImage)this.jfv.lbImages.Items[0];
                        this.jfv.ViewModel.SelectedStruct = _selectedStructure;
                    }

                    if (this.jfv.ViewModel.AuxData.Count > 0)
                        this.jfv.ViewModel.SelectedAuxData = this.jfv.ViewModel.AuxData[0];

                    this.dgJsdFiles.ScrollIntoView(this.ViewModel.SelectedJsdFile);
                }
            }
        }

        void SelectedJsdFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == JsdFileViewModel.SelectedStructPropertyName)
            {
                JsdFileViewModel _jsdViewModel = (JsdFileViewModel)sender;
                this.SetJsdViewModel(_jsdViewModel);
            }

            if (e.PropertyName == JsdFileViewModel.SelectedAuxDataPropertyName)
            {
                JsdFileViewModel _jsdViewModel = (JsdFileViewModel)sender;
                this.SetJsdViewModel(_jsdViewModel);
            }
        }

        private void SetJsdViewModel(JsdFileViewModel aJsdViewModel)
        {
            StructureViewModel _selectedStructure = aJsdViewModel.SelectedStruct;
            if (_selectedStructure != null)
            {
                if (aJsdViewModel.Images != null && this.jfv.lbImages.Items.Count > 0 &&
                    aJsdViewModel.ImageSelectedIndex > 0 && 
                    aJsdViewModel.ImageSelectedIndex < this.jfv.lbImages.Items.Count)
                    _selectedStructure.Image =
                        (StructureImage)this.jfv.lbImages.Items[aJsdViewModel.ImageSelectedIndex];

                if (aJsdViewModel.SelectedAuxData != null)
                    _selectedStructure.TileLocData = aJsdViewModel.SelectedAuxData.TileLocData;
            }
            this.sv.ViewModel = _selectedStructure;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            bool _thereIsChanges = CheckChanges();

            if (_thereIsChanges)
                return;

            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Title = "Open JSD File.";
            _ofd.Filter = "JSD Files (*.jsd) | *.jsd";
            _ofd.Multiselect = true;
         
            if (_ofd.ShowDialog() == true)
            {
                this.ViewModel.FilesNames = _ofd.FileNames;
            }
        }

        private bool CheckChanges()
        {
            foreach (JsdFileViewModel _file in this.ViewModel.JsdFiles)
            {
                if (_file.IsFileChanged)
                {
                    string _message = String.Format("File {0} is changed. Save it?", _file.FileName);
                    MessageBoxResult _result = MessageBox.Show(
                        _message, "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    if(_result == MessageBoxResult.Yes && _file.FullFileName != null)
                    {
                        using (FileStream _fs = new FileStream(_file.FullFileName, FileMode.Create))
                            _file.JsdFile.Save(_fs);

                        _file.IsFileChanged = false;
                    }

                    if (_result == MessageBoxResult.Cancel)
                        return true;
                }
            }
            return false;
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewJsdFileView _newJsdWindow = new NewJsdFileView();
            _newJsdWindow.Closed += NewJsdWindow_Closed;
            _newJsdWindow.ShowDialog();
        }

        private void NewJsdWindow_Closed(object sender, EventArgs e)
        {
            NewJsdFileView _newJsdWindow = (NewJsdFileView)sender;
            if (_newJsdWindow.IsOk)
            {
                this.ViewModel.JsdFiles.Add(_newJsdWindow.NewJsdFileViewModel);
                this.ViewModel.SelectedJsdFile = _newJsdWindow.NewJsdFileViewModel;
            }
        }

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog _ofd = new SaveFileDialog();
            //OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Title = "Save JSD File.";
            _ofd.Filter = "JSD Files (*.jsd) | *.jsd";
            _ofd.AddExtension = true;
            _ofd.CheckFileExists = false;
            _ofd.FileName = this.ViewModel.SelectedJsdFile.FileName;
            if (_ofd.ShowDialog() == true)
            {
                if (File.Exists(_ofd.FileName))
                {
                    MessageBoxResult _result = MessageBox.Show(
                        String.Format("File {0} will be overrwritten.", _ofd.FileName), 
                        "Saving JSD file.", MessageBoxButton.OKCancel);
                    if (_result != MessageBoxResult.OK)
                        return;
                }

                using(FileStream _fs = new FileStream(_ofd.FileName, FileMode.Create))
                    this.ViewModel.SelectedJsdFile.JsdFile.Save(_fs);

                this.ViewModel.SelectedJsdFile.IsFileChanged = false;

                JsdFileViewModel _jsdVm = this.ViewModel.LoadJsdFile(_ofd.FileName);
                this.ViewModel.JsdFiles.Add(_jsdVm);
                this.ViewModel.SelectedJsdFile = _jsdVm;
            }
        }

        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (this.ViewModel.SelectedJsdFile != null);
        }

        private void OpenFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool _thereIsChanges = CheckChanges();

            if (_thereIsChanges)
                return;

            System.Windows.Forms.FolderBrowserDialog _fbd = new System.Windows.Forms.FolderBrowserDialog();
 
            if (_fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ViewModel.FolderName = _fbd.SelectedPath;
            }
        }

        private void OpenSlfMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool _thereIsChanges = CheckChanges();

            if (_thereIsChanges)
                return;

            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Title = "Open SLF File.";
            _ofd.Filter = "SLF Files (*.slf) | *.slf";

            if (_ofd.ShowDialog() == true)
            {
                this.ViewModel.SlfFileName = _ofd.FileName;
                
            }
        }

        private void OpenStiMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Title = "Open STI File.";
            _ofd.Filter = "STI Files (*.sti) | *.sti";
            if (_ofd.ShowDialog() == true)
            {
                this.ViewModel.StiFileName = _ofd.FileName;
            }
        }

        private void OpenStiFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog _fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (_fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ViewModel.StiFolderName = _fbd.SelectedPath;
            }
        }

        private void ConvertFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Title = "Convert JSD Files to HD Format.";
            _ofd.Filter = "JSD Files (*.jsd) | *.jsd";
            _ofd.Multiselect = true;
            if (_ofd.ShowDialog() == true)
            {
                MessageBoxResult _result = MessageBox.Show(
                    "Files will be overrwritten.", "Convertation JSD to JSD-HD", MessageBoxButton.OKCancel);
                if (_result != MessageBoxResult.OK)
                    return;

                this.ViewModel.IsBusy = true;

                try
                {
                    this.ViewModel.Convert(_ofd.FileNames);
                }
                finally
                {
                    this.ViewModel.IsBusy = false;
                }
            }
        }

        private void ConvertFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog _fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (_fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] _files = Directory.GetFiles(_fbd.SelectedPath, "*.jsd", SearchOption.AllDirectories);
                this.ViewModel.Convert(_files);
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (File.Exists(this.ViewModel.SelectedJsdFile.FullFileName))
            {
                MessageBoxResult _result = MessageBox.Show(
                    String.Format("File {0} will be overrwritten.", this.ViewModel.SelectedJsdFile.FullFileName),
                    "Saving JSD file.", MessageBoxButton.OKCancel);
                if (_result != MessageBoxResult.OK)
                    return;
            }

            using (FileStream _fs = new FileStream(this.ViewModel.SelectedJsdFile.FullFileName, FileMode.Create))
                this.ViewModel.SelectedJsdFile.JsdFile.Save(_fs);

            this.ViewModel.SelectedJsdFile.IsFileChanged = false;
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.ViewModel.SelectedJsdFile == null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = !String.IsNullOrEmpty(this.ViewModel.SelectedJsdFile.FullFileName);
            }           
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            bool _thereIsChanges = CheckChanges();

            if (_thereIsChanges)
                e.Cancel = true;

            string asName = "FolderName";

            System.Configuration.Configuration config =
              ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings.AllKeys.Contains(asName))
                config.AppSettings.Settings[asName].Value = this.ViewModel.FolderName;                
            else
                config.AppSettings.Settings.Add(asName, this.ViewModel.FolderName);

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
