using Ja2Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ja2DataTest.ViewModel
{
    public class PcxTestViewModel : TestViewModel
    {
        public override string FileName
        {
            get { return base.FileName; }
            set
            {
                base.FileName = value;
                this.FLoadPcxCommand.IsCanExecute = !String.IsNullOrEmpty(base.FileName);
            }
        }

        public static string PcxImagePropertyName = "PcxImage";
        private ImageSource FPcxImage;
        public ImageSource PcxImage
        {
            get { return this.FPcxImage; }
            set
            {
                this.FPcxImage = value;
                NotifyPropertyChanged(PcxImagePropertyName);
            }
        }

        private LoadPcxCommand FLoadPcxCommand = new LoadPcxCommand();
        public LoadPcxCommand LoadPcxCommand
        {
            get { return this.FLoadPcxCommand; }
        }
    }

    public class LoadPcxCommand : ICommand
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

        public void Execute(object parameter)
        {
            PcxTestViewModel _viewModel = (PcxTestViewModel)parameter;

            try
            {
                _viewModel.StatusString = String.Empty;
                _viewModel.ResultString = String.Empty;

                PcxObject _pcx = PcxObject.LoadPcx(_viewModel.FileName);
                _viewModel.ResultString = _pcx.ToString();

                List<Color> _palette = _pcx.ColorPalette
                    .Select(x => new Color() { A = 255, R = x.Red, G = x.Green, B = x.Blue })
                    .ToList();

                PixelFormat _pf = PixelFormats.Indexed8;

                _viewModel.PcxImage = BitmapSource.Create(
                    _pcx.Width,
                    _pcx.Height,
                    96,
                    96,
                    _pf,
                    new BitmapPalette(_palette),
                    _pcx.ImageData,
                    _pcx.Width * _pf.BitsPerPixel / 8);

                _viewModel.StatusString = "Done";
            }
            catch (Exception exc)
            {
                _viewModel.ErrorString = Common.GetErrorString(exc);
            }
        }
    }
}
