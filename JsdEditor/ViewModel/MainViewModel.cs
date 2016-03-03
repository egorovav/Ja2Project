using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace JsdEditor
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            this.JsdFiles = new ObservableCollection<JsdFileViewModel>();

            this.FLoadSlfWorker = new BackgroundWorker();
            this.FLoadSlfWorker.WorkerReportsProgress = true;
            this.FLoadSlfWorker.DoWork += FLoadSlfWorker_DoWork;
            this.FLoadSlfWorker.ProgressChanged += FLoadWorkers_ProgressChanged;
            this.FLoadSlfWorker.RunWorkerCompleted += FLoadWorkers_RunWorkerCompleted;

            this.FLoadJsdWorker = new BackgroundWorker();
            this.FLoadJsdWorker.WorkerReportsProgress = true;
            this.FLoadJsdWorker.DoWork += FLoadJsdWorker_DoWork;
            this.FLoadJsdWorker.ProgressChanged += FLoadWorkers_ProgressChanged;
            this.FLoadJsdWorker.RunWorkerCompleted += FLoadWorkers_RunWorkerCompleted;

            this.FConvertBackGroundWorker = new BackgroundWorker();
            this.FConvertBackGroundWorker.WorkerReportsProgress = true;
            this.FConvertBackGroundWorker.DoWork += FConvertBackGroundWorker_DoWork;
            this.FConvertBackGroundWorker.ProgressChanged += FConvertBackGroundWorker_ProgressChanged;
        }

        private BackgroundWorker FLoadSlfWorker;
        private BackgroundWorker FLoadJsdWorker;
        private BackgroundWorker FConvertBackGroundWorker;

        public static string FolderNamePropertyName = "FolderName";
        private string FFolderName;
        public string FolderName
        {
            get { return this.FFolderName; }
            set
            {
                this.FFolderName = value;
                if (value != String.Empty)
                {
                    this.SlfFileName = String.Empty;
                    this.JsdFiles.Clear();
                    this.JsdFilesCount = 0;

                    string[] _files = Directory.GetFiles(value, "*.jsd", SearchOption.AllDirectories);
                    this.FLoadJsdWorker.RunWorkerAsync(_files);
                }

                NotifyPropertyChanged(FolderNamePropertyName);
                NotifyPropertyChanged(FolderNameIsNotEmptyPropertyName);
            }
        }

        public static string FolderNameIsNotEmptyPropertyName = "FolderNameIsNotEmpty";
        public bool FolderNameIsNotEmpty
        {
            get { return !String.IsNullOrEmpty(this.FolderName); }
        }

        public static string SlfFileNamePropertyName = "SlfFileName"; 
        private string FSlfFileName;
        public string SlfFileName
        {
            get { return this.FSlfFileName; }
            set
            {                
                this.FSlfFileName = value;
                if (value != String.Empty)
                {
                    this.FolderName = String.Empty;    
                    this.JsdFiles.Clear();
                    this.JsdFilesCount = 0;

                    this.FLoadSlfWorker.RunWorkerAsync(value);
                }

                NotifyPropertyChanged(SlfFileNamePropertyName);
                NotifyPropertyChanged(SlfFileNameIsNotEmptyPropertyName);
            }
        }

        public static string SlfFileNameIsNotEmptyPropertyName = "SlfFileNameIsNotEmpty";
        public bool SlfFileNameIsNotEmpty
        {
            get { return !String.IsNullOrEmpty(this.SlfFileName); }
        }

        public static string JsdFilesCountPropertyName = "JsdFilesCount";
        private int FCount;
        public int JsdFilesCount
        {
            get { return this.FCount; }
            set
            {
                this.FCount = value;
                NotifyPropertyChanged(JsdFilesCountPropertyName);
            }
        }

        public static string ProgressPropertyName = "Progress";
        private double FProgress;
        public double Progress
        {
            get { return this.FProgress; }
            set
            {
                this.FProgress = value;
                NotifyPropertyChanged(ProgressPropertyName);
            }
        }

        public static string IsBusyPropertyName = "IsBusy";
        private bool FIsBusy;
        public bool IsBusy
        {
            get { return this.FIsBusy; }
            set 
            { 
                this.FIsBusy = value;
                NotifyPropertyChanged(IsBusyPropertyName);
            }
        }

        public ObservableCollection<JsdFileViewModel> JsdFiles
        {
            get;
            set;
        }

        public static string SelectedJsdFilePropertyName = "SelectedJsdFile";
        private JsdFileViewModel FSelectedJsdFile;
        public JsdFileViewModel SelectedJsdFile
        {
            get { return this.FSelectedJsdFile; }
            set
            {
                this.FSelectedJsdFile = value;
                NotifyPropertyChanged(SelectedJsdFilePropertyName);
            }
        }

        private string[] FFilesNames;
        public string[] FilesNames
        {
            get { return this.FFilesNames; }
            set
            {
                this.FFilesNames = value;
                this.JsdFiles.Clear();
                this.JsdFilesCount = 0;
                this.SlfFileName = String.Empty;
                this.FolderName = String.Empty;
                this.FLoadJsdWorker.RunWorkerAsync(value);
            }
        }

        public static string StiFileNamePropertyName = "StiFileName";
        private string FStiFileName;
        public string StiFileName
        {
            get { return this.FStiFileName; }
            set 
            {
                this.FStiFileName = value;
                FileStream _fs = new FileStream(value, FileMode.Open);
                IStci _stci = StciLoader.LoadStci(_fs);
                _fs.Close();
                if (this.SelectedJsdFile != null && _stci != null && _stci.IsIndexed)
                    this.SelectedJsdFile.StciFile = (StciIndexed)_stci;

                NotifyPropertyChanged(StiFileNamePropertyName);
            }
        }

        public static string StiFolderNamePropertyName = "StiFolderName";
        private string FStiFolderName;
        public string StiFolderName
        {
            get { return this.FStiFolderName; }
            set 
            { 
                this.FStiFolderName = value;
                foreach (JsdFileViewModel _jsdFileViewModel in this.JsdFiles)
                {
                    string _stiFileName = Path.ChangeExtension(_jsdFileViewModel.FileName, ".sti");
                    string _stiFullFileName = Path.Combine(value, _stiFileName);
                    if (File.Exists(_stiFullFileName))
                    {
                        FileStream _fs = new FileStream(_stiFullFileName, FileMode.Open);
                        IStci _stci = StciLoader.LoadStci(_fs);
                        _fs.Close();
                        if (_stci.IsIndexed)
                        {
                            _jsdFileViewModel.StciFile = (StciIndexed)_stci;
                        }
                    }
                }
                NotifyPropertyChanged(StiFolderNamePropertyName);
            }
        }

        private void LoadJsdFiles(string[] _files)
        {
            foreach (string _file in _files)
            {
                JsdFileViewModel _jsdFileViewModel = LoadJsdFile(_file);

                this.JsdFilesCount++;
                this.FLoadJsdWorker.ReportProgress(100 * this.JsdFilesCount / _files.Length, _jsdFileViewModel);
            }
        }

        public JsdFileViewModel LoadJsdFile(string aFile)
        {
            FileStream _fs = new FileStream(aFile, FileMode.Open);
            JsdFile _jsdFile = JsdFile.Load(_fs);
            _fs.Close();

            string _fileName = String.Empty;
            if (this.FolderName != String.Empty)
                _fileName = aFile.Replace(this.FolderName, String.Empty);
            else
                _fileName = Path.GetFileName(aFile);

            JsdFileViewModel _jsdFileViewModel = new JsdFileViewModel(_jsdFile, _fileName);
            _jsdFileViewModel.FullFileName = aFile;

            string _stiFileName = Path.ChangeExtension(aFile, ".sti");
            if (File.Exists(_stiFileName))
            {
                _fs = new FileStream(_stiFileName, FileMode.Open);
                IStci _stci = StciLoader.LoadStci(_fs);
                _fs.Close();
                if (_stci.IsIndexed)
                {
                    _jsdFileViewModel.StciFile = (StciIndexed)_stci;
                }
            }
            return _jsdFileViewModel;
        }

        private void FLoadJsdWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.IsBusy = true;

            try
            {
                this.LoadJsdFiles((string[])e.Argument);
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private void FLoadSlfWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.IsBusy = true;
            try
            {
                string _slfFileName = (string)e.Argument;
                SlfFile _slfFile = new SlfFile(_slfFileName);
                _slfFile.LoadRecords();
                var _jsdRecords = _slfFile.Records.Where(x => x.FileNameExtention.ToLower() == ".jsd");
                int _recNum = _jsdRecords.Count();
                foreach (SlfFile.Record _record in _jsdRecords)
                {
                    JsdFileViewModel _jsdFileViewModel = null;

                    MemoryStream _ms = new MemoryStream(_record.Data);
                    JsdFile _jsdFile = JsdFile.Load(_ms);
                    _ms.Close();

                    _jsdFileViewModel = new JsdFileViewModel(_jsdFile, _record.FileName);

                    SlfFile.Record _stiRecord = _slfFile.Records
                        .Where(x => x.FileName.ToLower() == _record.FileName.ToLower().Replace(".jsd", ".sti"))
                        .SingleOrDefault();

                    if (_stiRecord != null)
                    {
                        _ms = new MemoryStream(_stiRecord.Data);
                        IStci _stci = StciLoader.LoadStci(_ms);
                        if (_stci.IsIndexed)
                            _jsdFileViewModel.StciFile = (StciIndexed)_stci;
                    }

                    this.JsdFilesCount++;
                    this.FLoadSlfWorker.ReportProgress(100 * this.JsdFilesCount / _recNum, _jsdFileViewModel);
                }
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private void FLoadWorkers_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Progress = e.ProgressPercentage;
            this.JsdFiles.Add((JsdFileViewModel)e.UserState);
        }

        private void FLoadWorkers_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.JsdFiles != null && this.JsdFiles.Count > 0)
                this.SelectedJsdFile = this.JsdFiles[0];
        }

        private void FConvertBackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.IsBusy = true;

            try
            {
                string[] _files = (string[])e.Argument;
                int _filesCount = 0;
                foreach (string _fileName in _files)
                {
                    JsdFile.ConvertJsdFileToHighDefinition(_fileName);
                    _filesCount++;
                    this.FConvertBackGroundWorker.ReportProgress(100 * _filesCount / _files.Length);
                }
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private void FConvertBackGroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Progress = e.ProgressPercentage;
        }

        public void Convert(string[] aFiles)
        {
            this.FConvertBackGroundWorker.RunWorkerAsync(aFiles);
        }
    }
}
