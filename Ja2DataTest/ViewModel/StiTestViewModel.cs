using Ja2Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ja2DataTest.ViewModel
{
    public class StiTestViewModel : TestViewModel
    {

        public StiTestViewModel()
        {
            
        }

        public override string FileName
        {
            get { return base.FileName; }
            set
            {
                base.FileName = value;
                if (!String.IsNullOrEmpty(value))
                {
                    this.ReloadStciCommand.IsCanExecute = true;
                    this.ExtractT26Command.IsCanExecute = true;
                }
            }
        }

        public override string FolderName
        {
            get {  return base.FolderName; }
            set
            {
                base.FolderName = value;
                if (!String.IsNullOrEmpty(value))
                {
                    this.ReloadAllStciCommand.IsCanExecute = true;
                }
            }
        }

        private ReloadStciCommand FReloadStciCommand = new ReloadStciCommand();
        public ReloadStciCommand ReloadStciCommand
        {
            get { return this.FReloadStciCommand; }
        }

        private ReloadAllStciCommand FReloadAllStciCommand = new ReloadAllStciCommand();
        public ReloadAllStciCommand ReloadAllStciCommand
        {
            get { return this.FReloadAllStciCommand; }
        }

        private ExtractT26Command FExtractT26Command = new ExtractT26Command();
        public ExtractT26Command ExtractT26Command
        {
            get { return this.FExtractT26Command; }
        }
    }

    public class ReloadStciCommand : ICommand
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
            TestViewModel _viewModel = (TestViewModel)parameter;

            this.IsCanExecute = false;
            this.IsCanExecute = await Task.Run<bool>(() =>
            {
                try
                {
                    _viewModel.StatusString = String.Empty;
                    _viewModel.ResultString = String.Empty;
                    _viewModel.StatusString = String.Empty;

                    _viewModel.ResultString = StciLoader.ReloadStci(_viewModel.FileName);

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

    public class ReloadAllStciCommand : ICommand
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

                    FileInfo[] _files = _dir.GetFiles("*.STI", SearchOption.AllDirectories);

                    string _currentDir = String.Empty;
                    int _i = 0;
                    foreach(FileInfo _file in _files)
                    {
                        StciLoader.ReloadStci(_file.FullName);

                        if(_currentDir !=  _file.DirectoryName)
                        {
                            _currentDir = _file.DirectoryName;
                            _viewModel.StatusString = String.Format("{0} processing ...", _currentDir);
                            _viewModel.ResultString = String.Format("{0} folders processed", ++_i);
                        }
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

    public class ExtractT26Command : ICommand
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
            TestViewModel _viewModel = (TestViewModel)parameter;

            this.IsCanExecute = false;
            this.IsCanExecute = await Task.Run<bool>(() =>
            {
                try
                {
                    _viewModel.StatusString = String.Empty;
                    _viewModel.ResultString = String.Empty;
                    _viewModel.StatusString = String.Empty;

                    int i = 0;
                    string _folderName = Path.Combine(
                        Path.GetDirectoryName(_viewModel.FileName),
                        Path.GetFileNameWithoutExtension(_viewModel.FileName));

                    Directory.CreateDirectory(_folderName);

                    using(FileStream _input = new FileStream(_viewModel.FileName, FileMode.Open))
                    {
                        using (BinaryReader _br = new BinaryReader(_input))
                        {
                            while (_input.Position < _input.Length)
                            {
                                IStci _stci = StciLoader.LoadStci(_input);
                                i++;
                                string _fileName = Path.Combine(_folderName, i.ToString() + ".STI");
                                using (FileStream _output = new FileStream(_fileName, FileMode.Create))
                                {
                                    _stci.Save(_output);
                                }
                            }
                        }
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
