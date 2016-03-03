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

namespace MapViewer
{
    /// <summary>
    /// Interaction logic for GlobalMapView.xaml
    /// </summary>
    public partial class GlobalMapView : UserControl
    {
        public GlobalMapView()
        {
            InitializeComponent();

            this.cMap.Width = MapImageWidth * (MapSize + 1) / MapSize;
            this.cMap.Height = MapImageHeight * (MapSize + 1) / MapSize;

            this.iMap.Width = MapImageWidth;
            this.iMap.Height = MapImageHeight;

            this.FSelectionRect = new Path();
            this.FSelectionRect.StrokeThickness = 2;
            this.FSelectionRect.Stroke = Brushes.Yellow;
            this.cMap.Children.Add(this.FSelectionRect);
        }

        private int MapSize = 16;
        private int MapImageWidth = 338;
        private int MapImageHeight = 290;

        private Path FSelectionRect;
        private double FCellWidth;
        private double FCellHeigth;
        private List<TextBlock> FHorisontalTextBoxes = new List<TextBlock>(16);
        private List<TextBlock> FVerticalTextBoxes = new List<TextBlock>(16);
        private Brush FHighlightBrush = Brushes.OrangeRed;
        private Brush FNormalBrush = Brushes.Gray;
        private bool FIsGlobalMapLoaded = false;

        static GlobalMapView()
        {
            MapImageProperty = DependencyProperty.Register
                ("MapImage", typeof(BitmapSource), typeof(GlobalMapView), 
                new UIPropertyMetadata(null, MapImagePropertyChanged));

            SelectedSectorProperty = DependencyProperty.Register(
                "SelectedSector", typeof(SectorNumber), typeof(GlobalMapView),
                new UIPropertyMetadata(null, SelectedSectorChanged));

            LevelProperty = DependencyProperty.Register
                ("Level", typeof(int), typeof(GlobalMapView));
        }

        private static void MapImagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlobalMapView _mapView = (GlobalMapView)d;

            if (!_mapView.FIsGlobalMapLoaded)
            {

                _mapView.FCellWidth = _mapView.MapImageWidth / _mapView.MapSize;
                _mapView.FCellHeigth = _mapView.MapImageHeight / _mapView.MapSize;

                TextBlock _tb0 = new TextBlock();
                _mapView.cMap.Children.Add(_tb0);

                for (int i = 0; i < _mapView.MapSize; i++)
                {
                    TextBlock _tb1 = new TextBlock();
                    _tb1.Text = (i + 1).ToString();
                    _tb1.Foreground = _mapView.FNormalBrush;
                    Canvas.SetLeft(_tb1, _mapView.FCellWidth * (i + 1));
                    _mapView.FHorisontalTextBoxes.Add(_tb1);
                    _mapView.cMap.Children.Add(_tb1);
                }

                for (int i = 0; i < _mapView.MapSize; i++)
                {
                    TextBlock _tb2 = new TextBlock();
                    char _a = (char)('A' + i);
                    _tb2.Text = new String(_a, 1);
                    _tb2.Foreground = _mapView.FNormalBrush;
                    Canvas.SetTop(_tb2, _mapView.FCellHeigth * (i + 1));
                    _mapView.FVerticalTextBoxes.Add(_tb2);
                    _mapView.cMap.Children.Add(_tb2);
                }
            }

            _mapView.FIsGlobalMapLoaded = true;

            if (_mapView.MapImage != null)
            {
                _mapView.iMap.Source = _mapView.MapImage;

                _mapView.tbLevelNumber.Text = (_mapView.Level + 1).ToString();

                if (_mapView.Level == 0)
                {
                    double _mapSize = _mapView.MapSize;
                    double _scale = (_mapView.MapSize + 1d) / _mapView.MapSize;
                    _mapView.iMap.RenderTransform = new ScaleTransform(_scale, _scale);

                    Canvas.SetLeft(_mapView.iMap, 0);
                    Canvas.SetTop(_mapView.iMap, 0);
                }
                else
                {
                    Canvas.SetLeft(_mapView.iMap, _mapView.FCellWidth);
                    Canvas.SetTop(_mapView.iMap, _mapView.FCellHeigth);
                }
            }
        }

        private static void SelectedSectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //GlobalMapView _mapView = (GlobalMapView)d;
            //if(_mapView.cMap.Children.Contains(_mapView.FSelectionRect))
            //    _mapView.cMap.Children.Remove(_mapView.FSelectionRect);

            //if (_mapView.SelectedSector != null && _mapView.SelectedSector.Level == _mapView.Level)
            //{
            //    Rect _rect = new Rect(
            //        (_mapView.SelectedSector.X + 1) * _mapView.FCellWidth,
            //        (_mapView.SelectedSector.Y + 1) * _mapView.FCellHeigth,
            //        _mapView.FCellWidth, _mapView.FCellHeigth);
            //    _mapView.FSelectionRect.Data = new RectangleGeometry(_rect);
            //    _mapView.cMap.Children.Add(_mapView.FSelectionRect);
            //}
        }

        public static readonly DependencyProperty LevelProperty;
        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        public static readonly DependencyProperty MapImageProperty;
        public BitmapSource MapImage
        {
            get { return (BitmapSource)GetValue(MapImageProperty); }
            set { SetValue(MapImageProperty, value); }
        }

        public static readonly DependencyProperty SelectedSectorProperty;
        public SectorNumber SelectedSector
        {
            get { return (SectorNumber)GetValue(SelectedSectorProperty); }
            set { SetValue(SelectedSectorProperty, value); }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //if (this.FIsGlobalMapLoaded || this.MapImage == null)
            //    return;

            //this.FCellWidth = MapImageWidth / MapSize;
            //this.FCellHeigth = MapImageHeight / MapSize;

            //TextBlock _tb0 = new TextBlock();
            //this.cMap.Children.Add(_tb0);

            //for (int i = 0; i < MapSize; i++)
            //{
            //    TextBlock _tb1 = new TextBlock();
            //    _tb1.Text = (i + 1).ToString();
            //    _tb1.Foreground = this.FNormalBrush;
            //    Canvas.SetLeft(_tb1, this.FCellWidth * (i + 1));
            //    this.FHorisontalTextBoxes.Add(_tb1);
            //    this.cMap.Children.Add(_tb1);
            //}

            //for (int i = 0; i < MapSize; i++)
            //{
            //    TextBlock _tb2 = new TextBlock();
            //    char _a = (char)('A' + i);
            //    _tb2.Text = new String(_a, 1);
            //    _tb2.Foreground = this.FNormalBrush;
            //    Canvas.SetTop(_tb2, this.FCellHeigth * (i + 1));
            //    this.FVerticalTextBoxes.Add(_tb2);
            //    this.cMap.Children.Add(_tb2);
            //}

            //this.iMap.Source = this.MapImage;

            //this.tbLevelNumber.Text = (this.Level + 1).ToString();

            //if (Level == 0)
            //{
            //    double _mapSize = MapSize;
            //    double _scale = (MapSize + 1d) / MapSize;
            //    iMap.RenderTransform = new ScaleTransform(_scale, _scale);

            //    Canvas.SetLeft(this.iMap, 0);
            //    Canvas.SetTop(this.iMap, 0);
            //}
            //else
            //{
            //    Canvas.SetLeft(this.iMap, this.FCellWidth);
            //    Canvas.SetTop(this.iMap, this.FCellHeigth);
            //}

            //this.FIsGlobalMapLoaded = true;
        }

        private SectorNumber GetSectorNumberFormPoint(Point _point)
        {
            if (Level == 0)
            {
                double _mapSize = MapSize;
                double _scale = (MapSize + 1d) / MapSize;
                Point _p = new Point(_point.X - this.FCellWidth, _point.Y - this.FCellHeigth);

                return new SectorNumber(this.Level,
                (int)Math.Truncate(_p.X * _scale / this.FCellWidth),
                (int)Math.Truncate(_p.Y * _scale / this.FCellHeigth));
            }

            return new SectorNumber(this.Level,
                (int)Math.Truncate(_point.X / this.FCellWidth),
                (int)Math.Truncate(_point.Y / this.FCellHeigth));

        }

        private SectorNumber FCurrentSector;
        private void iMap_MouseMove(object sender, MouseEventArgs e)
        {
            Point _mousePosition = e.GetPosition((Image)sender);
            SectorNumber _sector = this.GetSectorNumberFormPoint(_mousePosition);
            if (this.FCurrentSector == null || _sector.X != this.FCurrentSector.X || _sector.Y != this.FCurrentSector.Y)
            {
                CancelHighlighting();

                if (_sector.X >= 0 && _sector.Y >= 0 && _sector.X < MapSize && _sector.Y < MapSize)
                {
                    this.FHorisontalTextBoxes[_sector.X].Foreground = this.FHighlightBrush;
                    this.FVerticalTextBoxes[_sector.Y].Foreground = this.FHighlightBrush;
                    this.FHorisontalTextBoxes[_sector.X].FontWeight = FontWeights.ExtraBold;
                    this.FVerticalTextBoxes[_sector.Y].FontWeight = FontWeights.ExtraBold;
                    this.FCurrentSector = _sector;

                    if (this.cMap.Children.Contains(this.FSelectionRect))
                        this.cMap.Children.Remove(this.FSelectionRect);

                    Rect _rect = new Rect(
                        (this.FCurrentSector.X + 1) * this.FCellWidth,
                        (this.FCurrentSector.Y + 1) * this.FCellHeigth,
                        this.FCellWidth, this.FCellHeigth);
                    this.FSelectionRect.Data = new RectangleGeometry(_rect);
                    this.cMap.Children.Add(this.FSelectionRect);
                }
            }
        }

        private void cMap_MouseLeave(object sender, MouseEventArgs e)
        {
            CancelHighlighting();

            if (this.cMap.Children.Contains(this.FSelectionRect))
                this.cMap.Children.Remove(this.FSelectionRect);
        }

        private void CancelHighlighting()
        {
            if (this.FCurrentSector != null)
            {
                this.FHorisontalTextBoxes[this.FCurrentSector.X].Foreground = this.FNormalBrush;
                this.FVerticalTextBoxes[this.FCurrentSector.Y].Foreground = this.FNormalBrush;
                this.FHorisontalTextBoxes[this.FCurrentSector.X].FontWeight = FontWeights.Normal;
                this.FVerticalTextBoxes[this.FCurrentSector.Y].FontWeight = FontWeights.Normal;
            }
        }

        private void iMap_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point _mousePosition = e.GetPosition((Image)sender);
            this.SelectedSector = GetSectorNumberFormPoint(_mousePosition);
        }
    }
}
