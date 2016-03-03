using Ja2Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ja2DataTest.ViewModel
{
    public class JsdTestViewModel : TestViewModel
    {

        public JsdTestViewModel()
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
                    this.ReloadJsdCommand.IsCanExecute = true;
                    this.RotateJsdCommand.IsCanExecute = true;
                }
            }
        }

        public override string FolderName
        {
            get { return base.FolderName; }
            set
            {
                base.FolderName = value;
                if (!String.IsNullOrEmpty(value))
                {
                    this.ReloadAllJsdCommand.IsCanExecute = true;
                    this.ConvertAllJsdCommand.IsCanExecute = true;
                    this.CreateAllSlfCommand.IsCanExecute = true;
                }
            }
        }

        private ReloadJsdCommand FReloadJsdCommand = new ReloadJsdCommand();
        public ReloadJsdCommand ReloadJsdCommand
        {
            get { return this.FReloadJsdCommand; }
        }

        private ReloadAllJsdCommand FReloadAllJsdCommand = new ReloadAllJsdCommand();
        public ReloadAllJsdCommand ReloadAllJsdCommand
        {
            get { return this.FReloadAllJsdCommand; }
        }

        private ConvertAllJsdCommand FConvertAllJsdCommand = new ConvertAllJsdCommand();
        public ConvertAllJsdCommand ConvertAllJsdCommand
        {
            get { return this.FConvertAllJsdCommand; }
        }

        private CreateAllCommand FCreateAllSlfCommand = new CreateAllCommand("*.JSD");
        public CreateAllCommand CreateAllSlfCommand
        {
            get { return this.FCreateAllSlfCommand; }
        }

        private RotateJsdCommand FRotateJsdCommand = new RotateJsdCommand();
        public RotateJsdCommand RotateJsdCommand
        {
            get { return this.FRotateJsdCommand; }
        }
    }

    public class ReloadJsdCommand : ICommand
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

                    _viewModel.ResultString = JsdFile.ReloadJsdFile(_viewModel.FileName);

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

    public class RotateJsdCommand : ICommand
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

                    using (FileStream _fs = new FileStream(_viewModel.FileName, FileMode.Open))
                    {
                        JsdFile _jsd = JsdFile.Load(_fs);
                        _viewModel.ResultString += _jsd.ToString();

                        foreach (JsdStruct _struct in _jsd.Structs)
                            _struct.Rotate(true);

                        _viewModel.ResultString += "===================================================";
                        _viewModel.ResultString += _jsd.ToString();
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

    public class ReloadAllJsdCommand : ICommand
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

                    FileInfo[] _files = _dir.GetFiles("*.JSD", SearchOption.AllDirectories);

                    string _currentDir = String.Empty;
                    int _i = 0;
                    foreach (FileInfo _file in _files)
                    {
                        JsdFile.ReloadJsdFile(_file.FullName);

                        if (_currentDir != _file.DirectoryName)
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

    public class ConvertAllJsdCommand : ICommand
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

                    FileInfo[] _files = _dir.GetFiles("*.JSD", SearchOption.AllDirectories);

                    string _currentDir = String.Empty;
                    int _i = 0;
                    foreach (FileInfo _file in _files)
                    {
                        JsdFile.ConvertJsdFileToHighDefinition(_file.FullName);

                        if (_currentDir != _file.DirectoryName)
                        {
                            _currentDir = _file.DirectoryName;
                            _viewModel.StatusString = String.Format("{0} processing ...", _currentDir);
                            _viewModel.ResultString = String.Format("{0} folders processed ...", ++_i);
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
