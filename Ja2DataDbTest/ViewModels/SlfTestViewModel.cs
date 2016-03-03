//using Ja2Data;
using Ja2DataDb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;


namespace Ja2DataTest.ViewModel
{
    public class SlfTestViewModel : TestViewModel
    {
        public SlfTestViewModel()
        {
            this.FDataAccess = new Ja2DataUploader(1);
        }


        private Ja2DataUploader FDataAccess;
        public Ja2DataUploader DataAccess
        {
            get { return this.FDataAccess; }
        }

        public override string FolderName
        {
            get { return base.FolderName; }
            set
            {
                base.FolderName = value;
                if (!String.IsNullOrEmpty(value))
                {
                    this.FUploadAllCommand.IsCanExecute = true;
                    this.FDownloadAllCommand.IsCanExecute = true;
                }
            }
        }

        public override string FileName
        {
            get { return base.FileName; }
            set
            {
                base.FileName = value;
                if (!String.IsNullOrEmpty(value))
                {
                    this.FDownloadCommand.IsCanExecute = true;
                    if (File.Exists(value))
                    {
                        this.FUploadCommand.IsCanExecute = true;
                        //Ja2Data.SlfFile _slf = new Ja2Data.SlfFile(value);
                        //this.ResultString = _slf.ToString();
                    }
                }
            }
        }

        public static string DataInfoIdPropertyName = "DataInfoId";
        private int FDataInfoId = 1;
        public int DataInfoId
        {
            get { return this.FDataInfoId; }
            set
            {
                this.FDataInfoId = value;
                NotifyPropertyChanged(DataInfoIdPropertyName);
            }
        }

        public static string SelectedSlfProrertyName = "SelectesSlf";
        private Ja2DataDb.SlfFile FSelectedSlf;
        public Ja2DataDb.SlfFile SelectedSlf
        {
            get { return this.FSelectedSlf; }
            set
            {
                this.SelectedSlf = value;
                NotifyPropertyChanged(SelectedSlfProrertyName);
            }
        }

        public static string SlfItemSourcePropertyName = "SlfItemSource";
        private BindingList<SlfInfo> FSlfItemSource = new BindingList<SlfInfo>();
        public BindingList<SlfInfo> SlfItemSource
        {
            get { return this.FSlfItemSource; }
            set
            {
                this.FSlfItemSource = value;
                NotifyPropertyChanged(SlfItemSourcePropertyName);
            }
        }

        private UploadAllCommand FUploadAllCommand = new UploadAllCommand();
        public UploadAllCommand UploadAllCommand
        {
            get { return this.FUploadAllCommand; }
        }

        private UploadCommand FUploadCommand = new UploadCommand();
        public UploadCommand UploadCommand
        {
            get { return this.FUploadCommand; }
        }

        private DownloadCommand FDownloadCommand = new DownloadCommand();
        public DownloadCommand DownloadCommand
        {
            get { return this.FDownloadCommand; }
        }

        private DownloadAllCommand FDownloadAllCommand = new DownloadAllCommand();
        public DownloadAllCommand DownloadAllCommand
        {
            get { return this.FDownloadAllCommand; }
        }
    }

    public class UploadAllCommand : ICommand
    {
        private bool FCanExecute = false;
        public bool IsCanExecute
        {
            get { return this.FCanExecute; }
            set
            {
                this.FCanExecute = value;
                if (this.CanExecuteChanged != null)
                    this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.FCanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            SlfTestViewModel _viewModel = (SlfTestViewModel)parameter;

            _viewModel.StatusString = String.Empty;
            _viewModel.ResultString = String.Empty;
            _viewModel.StatusString = String.Empty;
            int _rowCount = 0;
            int _filesCount = 0;
            this.IsCanExecute = false;
            DateTime _start = DateTime.Now;

            try
            {
                DirectoryInfo _dir = new DirectoryInfo(_viewModel.FolderName);
                FileInfo[] _files = _dir.GetFiles("*.SLF");
                foreach (FileInfo _file in _files)
                {
                    _viewModel.StatusString = String.Format("{0} processing...", _file.Name);
                    SlfInfo _slf = await Task<SlfInfo>.Run(() =>
                    {
                        return _viewModel.DataAccess.UploadSlfFile(_file.FullName, _viewModel.DataInfoId);
                    });

                    _rowCount += _slf.RowsInserted;

                    _viewModel.SlfItemSource.Add(_slf);
                    _viewModel.ResultString = String.Format("{0} rows inserted.\n", _rowCount);
                    _viewModel.ResultString += String.Format("{0} files processed.", ++_filesCount);
                }
            }
            catch (Exception exc)
            {
                _viewModel.ErrorString = Ja2Data.Common.GetErrorString(exc);
            }

            _viewModel.StatusString = String.Format("Done. Duration {0}", DateTime.Now - _start);
            this.IsCanExecute = true;
        }
    }

    public class UploadCommand : ICommand
    {
        private bool FCanExecute = false;
        public bool IsCanExecute
        {
            get { return this.FCanExecute; }
            set
            {
                this.FCanExecute = value;
                if (this.CanExecuteChanged != null)
                    this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.FCanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            SlfTestViewModel _viewModel = (SlfTestViewModel)parameter;

            _viewModel.StatusString = String.Empty;
            _viewModel.ResultString = String.Empty;
            _viewModel.StatusString = String.Empty;
            int _rowCount = 0;
            this.IsCanExecute = false;
            DateTime _start = DateTime.Now;

            try
            {
                FileInfo _file = new FileInfo(_viewModel.FileName);

                _viewModel.StatusString = String.Format("{0} processing...", _file.Name);
                SlfInfo _slf = await Task<SlfInfo>.Run(() =>
                {
                    return _viewModel.DataAccess.UploadSlfFile(_file.FullName, _viewModel.DataInfoId);
                });

                _rowCount += _slf.RowsInserted;

                _viewModel.SlfItemSource.Add(_slf);
                _viewModel.ResultString = String.Format("{0} rows inserted.\n", _rowCount);
            }
            catch (Exception exc)
            {
                _viewModel.ErrorString = Ja2Data.Common.GetErrorString(exc);
            }

            _viewModel.StatusString = String.Format("Done. Duration {0}", DateTime.Now - _start);
            this.IsCanExecute = true;
        }
    }

    public class DownloadCommand : ICommand
    {
        private bool FCanExecute = false;
        public bool IsCanExecute
        {
            set
            {
                this.FCanExecute = value;
                if (this.CanExecuteChanged != null)
                    this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.FCanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            SlfTestViewModel _viewModel = (SlfTestViewModel)parameter;

            this.IsCanExecute = false;
            DateTime _start = DateTime.Now;

            _viewModel.StatusString = String.Empty;
            _viewModel.ResultString = String.Empty;
            _viewModel.StatusString = String.Empty;
            try
            {
                SlfInfo _slf = await Task<SlfInfo>.Run(() =>
                {

                    _viewModel.StatusString = "Processed...";
                    DateTime _date = new DateTime(2000, 1, 1);
                    return _viewModel.DataAccess.DownloadSlfFile(_viewModel.FileName, _date, _viewModel.DataInfoId);
                });


                _viewModel.SlfItemSource.Add(_slf);
                _viewModel.StatusString = String.Format("Done. Duration {0}", DateTime.Now - _start);
            }
            catch (Exception exc)
            {
                _viewModel.ErrorString = Ja2Data.Common.GetErrorString(exc);
            }
        }
    }

    public class DownloadAllCommand : ICommand
    {
        private bool FCanExecute = false;
        public bool IsCanExecute
        {
            set
            {
                this.FCanExecute = value;
                if (this.CanExecuteChanged != null)
                    this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.FCanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            SlfTestViewModel _viewModel = (SlfTestViewModel)parameter;

            this.IsCanExecute = false;
            DateTime _start = DateTime.Now;

            _viewModel.StatusString = String.Empty;
            _viewModel.ResultString = String.Empty;
            _viewModel.StatusString = String.Empty;
            try
            {
                List<string> _fileNames = _viewModel.DataAccess.GetSlfFilesNames(_viewModel.DataInfoId);

                foreach (string _fileName in _fileNames)
                {
                    _viewModel.StatusString = String.Format("Processed {0}...", _fileName);
                    string _fullFileName = Path.Combine(_viewModel.FolderName, _fileName);
                    SlfInfo _slf = await Task<SlfInfo>.Run(() =>
                    {
                        DateTime _date = new DateTime(2000, 1, 1);
                        return _viewModel.DataAccess.DownloadSlfFile(_fullFileName, _date, _viewModel.DataInfoId);
                    });

                    _viewModel.SlfItemSource.Add(_slf);
                }

                _viewModel.StatusString = String.Format("Done. Duration {0}", DateTime.Now - _start);
            }
            catch (Exception exc)
            {
                _viewModel.ErrorString = Ja2Data.Common.GetErrorString(exc);
            }
        }
    }
}