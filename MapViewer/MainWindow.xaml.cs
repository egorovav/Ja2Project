using CommonWpfControls;
using Ja2Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MapViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.ViewModel = new MainViewModel();

            InitializeComponent();
            this.ImageViewModel = new ImageViewModel();

            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.ViewModel.PropertyChanged += gmv.MainViewModel_PropertyChanged;

            try
            {
                System.Configuration.AppSettingsReader _asr = new System.Configuration.AppSettingsReader();
                this.ViewModel.DataFolder = (string)_asr.GetValue("Ja2Data", typeof(String));
            }
            catch(Exception _exc)
            {
                StringBuilder _sb = new StringBuilder();
                _sb.AppendLine(_exc.Message);
                _sb.AppendLine("Add Ja2Data key to MapViewer.exe.config file,");
                _sb.AppendLine("or use Data menu to select JA2 resources folder.");
                MessageBox.Show(_sb.ToString(), "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            this.gmv.ViewModel.DataFolder = this.ViewModel.DataFolder;
            this.gmv.ViewModel.PropertyChanged += GlobalMapViewModel_PropertyChanged;

            //System.Configuration.AppSettingsReader _asr = new System.Configuration.AppSettingsReader();
            //string _dataFolder = (string)_asr.GetValue("Ja2Data", typeof(String));
            this.FMapFolder = Path.Combine(this.ViewModel.DataFolder, "maps");
        }

        
        private MainViewModel ViewModel
        {
            get { return (MainViewModel)this.DataContext; }
            set { this.DataContext = value; }
        }

        private ImageViewModel ImageViewModel
        {
            get { return (ImageViewModel)this.iMap.DataContext; }
            set { this.iMap.DataContext = value; }
        }

        private string FMapFolder;
        private List<CheckBox> FLayersSwitch = new List<CheckBox>();

        void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MainViewModel.MapFileNamePropertyName)
            {
                bool[] _drewLevels = this.FLayersSwitch.Select(x => x.IsChecked == true).ToArray();
                DrawingMapInfo _info = new DrawingMapInfo(this.ViewModel.Map, _drewLevels);
                if (this.ViewModel.IsLoadImage)
                    this.DrawMap(_info);

                if(this.ViewModel.IsLoadStructure)
                    this.svMapStructure.ViewModel = new StructureViewModel3D(this.ViewModel.Map, _drewLevels);
            }

            if(e.PropertyName == MainViewModel.DataFolderPropertyName)
            {
                this.FMapFolder = Path.Combine(this.ViewModel.DataFolder, "maps");
            }
        }

        private void DrawMap(object aDrewInfo)
        {
            DrawingMapInfo _drewInfo = (DrawingMapInfo)aDrewInfo;

            this.mv.DrawMapByLayer(_drewInfo.Map, _drewInfo.IsDrewLayers);
        }

        private void GlobalMapViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GlobalMapsViewModel.SelectedSectorPropertyName)
            {
                GlobalMapsViewModel _mapsViewModel = (GlobalMapsViewModel)sender;
                if (_mapsViewModel.SelectedSector != null)
                {
                    string _mapFileName = String.Format("{0}.DAT", _mapsViewModel.SelectedSector);
                    _mapFileName = Path.Combine(this.FMapFolder, _mapFileName);

                    try
                    {
                        this.ViewModel.MapFileName = _mapFileName;
                    }
                    catch (Exception _exc)
                    {
                        MessageBox.Show(_exc.Message, "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Title = "Open Map File.";
            _ofd.Filter = "Map Files (*.dat) | *.dat";
            _ofd.InitialDirectory = this.FMapFolder;

            if (_ofd.ShowDialog() == true)
            {
                this.ViewModel.MapFileName = _ofd.FileName;
            }
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox _chb = (CheckBox)sender;
            int _layerNumber = (int)_chb.Tag;
            this.mv.AddVisual(_layerNumber);
            this.svMapStructure.AddLayer(_layerNumber);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox _chb = (CheckBox)sender;
            int _layerNumber = (int)_chb.Tag;
            this.mv.RemoveVisual(_layerNumber);
            this.svMapStructure.RemoveLayer(_layerNumber);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                CheckBox _cb = new CheckBox();
                _cb.Content = String.Format("Layer {0}", i + 1);
                _cb.Tag = i;
                _cb.IsChecked = (i != 3); // shadow
                _cb.Checked += CheckBox_Checked;
                _cb.Unchecked += CheckBox_Unchecked;
                this.FLayersSwitch.Add(_cb);
                this.spLayers.Children.Add(_cb);
            }
        }

        private void mv_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double _delta = (double)e.Delta / 1000f;
            if (_delta > 0 && this.ImageViewModel.ImageScale < 10 || _delta < 0 && this.ImageViewModel.ImageScale > 0.1)
            {
                Point _mp = e.GetPosition(this.mv);
                this.ImageViewModel.ImageScale += _delta;
                this.ImageViewModel.ImageX -= _mp.X * _delta;
                this.ImageViewModel.ImageY -= _mp.Y * _delta;
                double _sizeY = this.iMap.ActualHeight;
            }
        }

        private Point FLastMousePosition = new Point(0, 0);
        private void mv_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Cursor = Cursors.Hand;
                Point _currentPositon = e.GetPosition(this);
                if (this.FLastMousePosition.X != 0)
                {
                    this.ImageViewModel.ImageX += (_currentPositon.X - this.FLastMousePosition.X);
                    this.ImageViewModel.ImageY += (_currentPositon.Y - this.FLastMousePosition.Y);
                }
                this.FLastMousePosition = _currentPositon;
            }
            else
            {
                this.FLastMousePosition = new Point(0, 0);
                this.Cursor = Cursors.Arrow;
            }
        }

        private void SaveAsJpeg_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog _sfd = new SaveFileDialog();
            _sfd.Title = "Save Map Image As JPEG.";
            _sfd.Filter = "JPEG Files (*.jpeg;*.jpg) | *.jpeg;*.jpg";
            _sfd.FileName = this.gmv.ViewModel.SelectedSector.ToString();

            if (_sfd.ShowDialog() == true)
            {
                RenderTargetBitmap _bitmap = CreateVisualBitmap();

                JpegBitmapEncoder _encoder = new JpegBitmapEncoder();
                _encoder.QualityLevel = 100;
                _encoder.Frames.Add(BitmapFrame.Create(_bitmap));

                using (FileStream _fs = new FileStream(_sfd.FileName, FileMode.Create, FileAccess.Write))
                {
                    _encoder.Save(_fs);
                    _fs.Flush();
                    _fs.Close();
                }
            }
        }

        private void SaveAsBmp_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog _sfd = new SaveFileDialog();
            _sfd.Title = "Save Map Image As BMP.";
            _sfd.Filter = "BMP Files (*.bmp) | *.bmp";
            _sfd.FileName = this.gmv.ViewModel.SelectedSector.ToString();

            if (_sfd.ShowDialog() == true)
            {
                RenderTargetBitmap _bitmap = CreateVisualBitmap();

                BmpBitmapEncoder _encoder = new BmpBitmapEncoder();
                _encoder.Frames.Add(BitmapFrame.Create(_bitmap));

                using (FileStream _fs = new FileStream(_sfd.FileName, FileMode.Create, FileAccess.Write))
                {
                    _encoder.Save(_fs);
                    _fs.Flush();
                    _fs.Close();
                }
            }
        }

        private RenderTargetBitmap CreateVisualBitmap()
        {
            PresentationSource _ps = PresentationSource.FromVisual(this);
            Matrix _m = _ps.CompositionTarget.TransformToDevice;
            double _dpiX = _m.M11 * 96;
            double _dpiY = _m.M22 * 96;
            RenderTargetBitmap _bitmap = null;
            if (this.tc.SelectedIndex == 2)
            {
                _bitmap = new RenderTargetBitmap(
                    (int)this.svMapStructure.vp.ActualWidth,
                    (int)this.svMapStructure.vp.ActualHeight,
                    _dpiX, _dpiY, PixelFormats.Default);
                _bitmap.Render(this.svMapStructure.vp);
            }
            else
            {
                int _imageHeigth = this.ViewModel.Map.WORLD_SIZE * StructureImage.TileHeight;
                _bitmap = new RenderTargetBitmap(_imageHeigth * 2, _imageHeigth, _dpiX, _dpiY, PixelFormats.Default);
                _bitmap.Render(this.mv);
            }
            return _bitmap;
        }

        ProgressHolder FPh;

        private void ConvertToJpeg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Title = "Convert Map File To JPEG.";
            _ofd.Filter = "Map Files (*.dat) | *.dat";
            _ofd.Multiselect = true;

            if (_ofd.ShowDialog() == true)
            {
                this.FPh = new ProgressHolder();
                this.FPh.IsCancelable = true;
                ProgressWindow.Run(this.FPh);
                try
                {
                    for (int i = 0; i < _ofd.FileNames.Length && this.FPh.Progress > 0; i++)
                    {
                        Map _map = new Map(_ofd.FileNames[i]);
                        _map.Load();
                        this.ViewModel.LoadMapTileSet(_map);
                        MapImage _mapImage = new MapImage(_map);
                        bool[] _drewLevels = this.FLayersSwitch.Select(x => x.IsChecked == true).ToArray();
                        WriteableBitmap _bitmap = _mapImage.GetMapBitmap(_drewLevels);

                        JpegBitmapEncoder _encoder = new JpegBitmapEncoder();
                        _encoder.QualityLevel = 100;
                        BitmapFrame _bf = BitmapFrame.Create(_bitmap);
                        _encoder.Frames.Add(_bf);
                        string _destFileName = Path.ChangeExtension(_ofd.FileNames[i], ".jpeg");
                        using (FileStream _fs = new FileStream(_destFileName, FileMode.Create, FileAccess.Write))
                        {
                            _encoder.Save(_fs);
                            _fs.Flush();
                            _fs.Close();
                        }

                        this.FPh.Progress = 100 * i / _ofd.FileNames.Length;
                    }
                }
                finally
                {
                    this.FPh.Progress = -1;
                }
            }
        }

        private void ConvertToBmp_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Title = "Convert Map File To BMP.";
            _ofd.Filter = "Map Files (*.dat) | *.dat";
            _ofd.Multiselect = true;

            if (_ofd.ShowDialog() == true)
            {
                this.FPh = new ProgressHolder();
                this.FPh.IsCancelable = true;
                ProgressWindow.Run(this.FPh);
                try
                {
					for (int i = 0; i < _ofd.FileNames.Length && this.FPh.Progress > 0; i++)
                    {
                        Map _map = new Map(_ofd.FileNames[i]);
                        _map.Load();
                        this.ViewModel.LoadMapTileSet(_map);
                        MapImage _mapImage = new MapImage(_map);
                        bool[] _drewLevels = this.FLayersSwitch.Select(x => x.IsChecked == true).ToArray();
                        WriteableBitmap _bitmap = _mapImage.GetMapBitmap(_drewLevels);

                        BmpBitmapEncoder _encoder = new BmpBitmapEncoder();
                        BitmapFrame _bf = BitmapFrame.Create(_bitmap);
                        _encoder.Frames.Add(_bf);
                        string _destFileName = Path.ChangeExtension(_ofd.FileNames[i], ".bmp");
                        using (FileStream _fs = new FileStream(_destFileName, FileMode.Create, FileAccess.Write))
                        {
                            _encoder.Save(_fs);
                            _fs.Flush();
                            _fs.Close();
                        }

                        this.FPh.Progress = 100 * i / _ofd.FileNames.Length;
                    }
                }
                finally
                {
                    this.FPh.Progress = -1;
                }
            }
        }

        private void ChangeData_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog _fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (_fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    this.ViewModel.DataFolder = _fbd.SelectedPath;
                }
                catch (Exception _exc)
                {
                    MessageBox.Show(_exc.Message, "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
