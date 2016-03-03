using Ja2Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Ja2DataTest.ViewModel
{
    public class SlfTestViewModel : TestViewModel
    {
        public SlfTestViewModel()
        {

        }

        public override string FolderName
        {
            get { return base.FolderName; }
            set
            {
                base.FolderName = value;
                if (!String.IsNullOrEmpty(value))
                {
                    this.FReloadAllCommand.CanExecuteProperty = true;
                    this.FExtractAllCommand.CanExecuteProperty = true;
                    this.CreateAllCommand.IsCanExecute = true;
                    this.CreateCommand.CanExecuteProperty = true;
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
                    this.FExtractCommand.IsCanExecute = true;
                    SlfFile _slf = new SlfFile(value);
                    this.ResultString = _slf.ToString();
                }
            }
        }

        private ReloadAllCommand FReloadAllCommand = new ReloadAllCommand();
        public ReloadAllCommand ReloadAllCommand
        {
            get { return this.FReloadAllCommand; }
        }

        private ExtractAllCommand FExtractAllCommand = new ExtractAllCommand();
        public ExtractAllCommand ExtractAllCommand
        {
            get { return this.FExtractAllCommand; }
        }

        private CreateAllCommand FCreateAllCommand = new CreateAllCommand();
        public CreateAllCommand CreateAllCommand
        {
            get { return this.FCreateAllCommand; }
        }

        private ExtractCommand FExtractCommand = new ExtractCommand();
        public ExtractCommand ExtractCommand
        {
            get { return this.FExtractCommand; }
        }

        private CreateCommand FCreateCommand = new CreateCommand();
        public CreateCommand CreateCommand
        {
            get { return this.FCreateCommand; }
        }
    }

    public class ReloadAllCommand : ICommand
    {
        private bool FCanExecute = false;
        public bool CanExecuteProperty
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
            DirectoryInfo _dir = new DirectoryInfo(_viewModel.FolderName);

            this.CanExecuteProperty = false;
            this.CanExecuteProperty = await Task.Run<bool>(() =>
            {
                try
                {
                    _viewModel.StatusString = String.Empty;
                    _viewModel.ResultString = String.Empty;
                    _viewModel.StatusString = String.Empty;
                    int _count = 0;
                    foreach (FileInfo _file in _dir.GetFiles("*.SLF"))
                    {
                        SlfFile _slf = new SlfFile(_file.FullName);
                        _viewModel.StatusString = String.Format("{0} processing ...", _file.Name);
                        _slf.LoadRecords();
                        _slf.Save();
                        _viewModel.ResultString = String.Format("{0} files processed.", ++_count);
                    }

                    _viewModel.StatusString = "Done";
                }
                catch(Exception exc)
                {
                    _viewModel.ErrorString = Common.GetErrorString(exc);
                }

                return true;
            });
        }
    }

    public class ExtractAllCommand : ICommand
    {
        private bool FCanExecute = false;
        public bool CanExecuteProperty
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
            DirectoryInfo _dir = new DirectoryInfo(_viewModel.FolderName);

            this.CanExecuteProperty = false;
            this.CanExecuteProperty = await Task.Run<bool>(() =>
            {
                try
                {
                    _viewModel.StatusString = String.Empty;
                    _viewModel.ResultString = String.Empty;
                    _viewModel.StatusString = String.Empty;

                    int _count = 0;
                    int _filesCount = 0;
                    foreach (FileInfo _file in _dir.GetFiles("*.SLF"))
                    {
                        _viewModel.StatusString = String.Format("{0} processing ...", _file.Name);

                        SlfFile _slf = new SlfFile(_file.FullName);
                        int _fileCount = _slf.Extract(false);
                        _filesCount += _fileCount;

                        _viewModel.ResultString = String.Format(
                            "{0} files processed. {1} files extract", ++_count, _filesCount);
                    }

                    _viewModel.StatusString = "Done";
                }
                catch (Exception exc)
                {
                    _viewModel.ErrorString = Common.GetErrorString(exc);
                }

                return true;
            });
        }
    }

    public class CreateAllCommand : ICommand
    {
        public CreateAllCommand()
        { 
        }

        public CreateAllCommand(string aSearchPattern)
        {
            this.FSearchPattern = aSearchPattern;
        }

        private string FSearchPattern = "*.*";

        private bool FIsCanExecute = false;
        public bool IsCanExecute
        {
            set
            {
                this.FIsCanExecute = value;
                if (this.CanExecuteChanged != null)
                    this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.FIsCanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            TestViewModel _viewModel = (TestViewModel)parameter;
            DirectoryInfo _dir = new DirectoryInfo(_viewModel.FolderName);

            this.IsCanExecute = false;
            this.IsCanExecute = await Task.Run<bool>(() =>
            {
                try
                {
                    _viewModel.StatusString = String.Empty;
                    _viewModel.ResultString = String.Empty;
                    _viewModel.StatusString = String.Empty;

                    int _count = 0;
                    StringBuilder _sb = new StringBuilder();
                    SlfFile _slf = SlfFile.Create(_dir, this.FSearchPattern, SearchOption.TopDirectoryOnly);
                    if (_slf != null)
                    {
                        string _fileName = String.Format("{0}.SLF", _dir.Name);
                        string _fullFileName = Path.Combine(_dir.FullName, _fileName);
                        _slf.Save(_fullFileName);
                    }
                    foreach (DirectoryInfo _subDir in _dir.GetDirectories())
                    {
                        _viewModel.StatusString = String.Format("{0} processing ...", _subDir.Name);

                        _slf = SlfFile.Create(_subDir, this.FSearchPattern, SearchOption.AllDirectories);
                        if (_slf != null)
                        {
                            string _fileName = String.Format("{0}.SLF", _subDir.Name);
                            string _fullFileName = Path.Combine(_subDir.Parent.FullName, _fileName);
                            _slf.Save(_fullFileName);
                        }

                        _viewModel.ResultString = String.Format(
                            "{0} directories processed.", ++_count);
                    }

                    _viewModel.StatusString = "Done";
                }
                catch (Exception exc)
                {
                    _viewModel.ErrorString = Common.GetErrorString(exc);
                }

                return true;
            });
        }
    }

    public class ExtractCommand : ICommand
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
            this.IsCanExecute = await Task.Run<bool>(() =>
            {
                try
                {
                    _viewModel.StatusString = String.Empty;
                    _viewModel.ResultString = String.Empty;
                    _viewModel.StatusString = String.Empty;

                    int _filesCount = 0;

                    SlfFile _slf = new SlfFile(_viewModel.FileName);
                    int _fileCount = _slf.Extract(false);
                    _filesCount += _fileCount;

                    _viewModel.ResultString = String.Format(
                        "{0} files extract", _filesCount);

                    _viewModel.StatusString = "Done";
                }
                catch (Exception exc)
                {
                    _viewModel.ErrorString = Common.GetErrorString(exc);
                }

                return true;
            });
        }
    }

    public class CreateCommand : ICommand
    {
        private bool FCanExecute = false;
        public bool CanExecuteProperty
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
            TestViewModel _viewModel = (TestViewModel)parameter;
            DirectoryInfo _dir = new DirectoryInfo(_viewModel.FolderName);

            this.CanExecuteProperty = false;
            this.CanExecuteProperty = await Task.Run<bool>(() =>
            {
                try
                {
                    _viewModel.StatusString = String.Empty;
                    _viewModel.ResultString = String.Empty;
                    _viewModel.StatusString = String.Empty;

                    SlfFile _slf = SlfFile.Create(_dir, SearchOption.AllDirectories);
                    if(_slf != null)
                    {
                        string _fileName = String.Format("{0}.SLF", _dir.Name);
                        string _fullFileName = Path.Combine(_dir.Parent.FullName, _fileName);
                        _slf.Save(_fullFileName);
                    }

                    _viewModel.StatusString = "Done";
                }
                catch (Exception exc)
                {
                    _viewModel.ErrorString = Common.GetErrorString(exc);
                }

                return true;
            });
        }
    }
}
