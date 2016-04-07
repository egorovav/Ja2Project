using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    /// Interaction logic for StructureView.xaml
    /// </summary>
    public partial class StructureView : UserControl
    {
        public StructureViewModel ViewModel
        {
            get { return this.DataContext as StructureViewModel; }
            set 
            { 
                this.DataContext = value;

                if(this.FImageTiles != null)
                {
                    foreach(Polygon _polygon in this.FImageTiles)
                        this.cStructureImage.Children.Remove(_polygon);
                }

                if (this.ViewModel != null)
                {
                    if (this.ViewModel.Tiles != null)
                    {

                        this.ViewModel.PropertyChanged += StructureViewModel_PropertyChanged;
                        this.ViewModel.Tiles.CollectionChanged += Tiles_CollectionChanged;

                        if (this.ViewModel.TileLocData != null)
                        {
                            this.ViewModel.TileLocData.CollectionChanged += TileLocData_CollectionChanged;
                            foreach (RelTileLoc _tileLoc in this.ViewModel.TileLocData)
                                _tileLoc.PropertyChanged += TileLoc_PropertyChanged;
                        }

                        this.svShape3D.ViewModel = new StructureViewModel3D(this.ViewModel);

                        if (this.ViewModel.Tiles.Count > 0)
                            this.ViewModel.SelectedTile = this.ViewModel.Tiles
                                .Where(x => x.XPosRelToBase == 0 && x.YPosRelToBase == 0 && !x.TileIsOnRoof)
                                .FirstOrDefault();
                        //.SingleOrDefault();

                        foreach (TileViewModel _tile in this.ViewModel.Tiles)
                            _tile.AttachHandler(this, TileViewModel_PropertyChanged);

                        if (this.ViewModel.SelectedTile != null)
                            this.svShape.ViewModel = this.ViewModel.SelectedTile.ShapeViewModel;
                        else
                            this.svShape.ViewModel = new ShapeViewModel(this.ViewModel.IsHighDefenition);
                    }

                    if (this.ViewModel.Tiles != null && this.ViewModel.Tiles.Count > 0)
                        SetTilesImage();
                    else if (this.ViewModel.Image != null && this.ViewModel.TileLocData != null)
                        SetTilesLocImage();

                }
            }
        }

        void TileLoc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetTilesLocImage();
        }

        void TileLocData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetTilesLocImage();
        }

        void Tiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach(object _obj in e.NewItems)
                {
                    TileViewModel _tile = (TileViewModel)_obj;
                    _tile.AttachHandler(this, TileViewModel_PropertyChanged);
                }
            }

            this.SetTilesImage();
        }

        void StructureViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == StructureViewModel.SelectedTilePropertyName)
            {
                TileViewModel _tile = (TileViewModel)this.dgTiles.SelectedItem;
                if (this.ViewModel.SelectedTile != _tile)
                {
                    this.dgTiles.SelectedItem = this.ViewModel.SelectedTile;
                    this.dgTiles.ScrollIntoView(this.ViewModel.SelectedTile);
                }

                this.SetTilesImage();

                if (this.ViewModel.SelectedTile != null)
                {
                    this.svShape.ViewModel = this.ViewModel.SelectedTile.ShapeViewModel;                   
                }
            }

            if(e.PropertyName == StructureViewModel.SelectedTileLocDataPropertyName)
            {
                RelTileLoc _tileLoc = (RelTileLoc)this.dgTileLocData.SelectedItem;
                if(this.ViewModel.SelectedTileLocData != _tileLoc)
                {
                    this.dgTileLocData.SelectedItem = this.ViewModel.SelectedTileLocData;
                    this.dgTileLocData.ScrollIntoView(this.ViewModel.SelectedTileLocData);
                }
                this.SetTilesLocImage();
            }
        }

        void TileViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == TileViewModel.XPosRelToBasePropertyName || 
                e.PropertyName == TileViewModel.YPosRelToBasePropertyName)
            {
                SetTilesImage();          
            }
        }

        private void SetTilesLocImage()
        {
            if (this.FImageTiles != null)
            {
                foreach (Polygon _polygon in this.FImageTiles)
                    this.cStructureImage.Children.Remove(_polygon);
            }

            this.FImageTiles = new List<Polygon>(this.ViewModel.Tiles.Count);
            double _shiftX = this.ViewModel.TileLocData.Min(x => x.X);
            double _shiftY = this.ViewModel.TileLocData.Max(x => x.Y);
            double _shift = _shiftX * StructureImage.TileWidth / 2 - _shiftY * StructureImage.TileHeight / 2;
            foreach (RelTileLoc _tileLoc in this.ViewModel.TileLocData)
            {
                if (_tileLoc != this.ViewModel.SelectedTileLocData)
                    AddTileLocImage(_tileLoc, _shift);
            }

            AddTileLocImage(this.ViewModel.SelectedTileLocData, _shift);
        }

        private void AddTileLocImage(RelTileLoc aTileLoc, double aShiftX)
        {
            if (aTileLoc == null || this.ViewModel.Image == null)
                return;

            PointCollection _baseTilePoints =
                StructureImage.GetBasePoints(aShiftX, StructureImage.TileHeight - this.ViewModel.Image.Height);
            int _xShift = aTileLoc.X - aTileLoc.Y;
            int _yShift = aTileLoc.X + aTileLoc.Y;

            Point _p0 = new Point(_baseTilePoints[0].X + _xShift * StructureImage.TileWidth / 2,
                                  _baseTilePoints[0].Y + _yShift * StructureImage.TileHeight / 2);
            Point _p1 = new Point(_baseTilePoints[1].X + _xShift * StructureImage.TileWidth / 2,
                                  _baseTilePoints[1].Y + _yShift * StructureImage.TileHeight / 2);
            Point _p2 = new Point(_baseTilePoints[2].X + _xShift * StructureImage.TileWidth / 2,
                                  _baseTilePoints[2].Y + _yShift * StructureImage.TileHeight / 2);
            Point _p3 = new Point(_baseTilePoints[3].X + _xShift * StructureImage.TileWidth / 2,
                                  _baseTilePoints[3].Y + _yShift * StructureImage.TileHeight / 2);

            Polygon _polygon = new Polygon();

            _polygon.Tag = aTileLoc;
            _polygon.MouseDown += TileLoc_MouseDown;

            _polygon.Stroke = 
                aTileLoc == this.ViewModel.SelectedTileLocData ? this.FImageSelectedTileBrush : this.FImageTileBrush;
            _polygon.StrokeThickness = 2;
            _polygon.Fill = Brushes.Transparent;
            _polygon.Points = new PointCollection(4);
            _polygon.Points.Add(_p0);
            _polygon.Points.Add(_p1);
            _polygon.Points.Add(_p2);
            _polygon.Points.Add(_p3);
            this.FImageTiles.Add(_polygon);
            this.cStructureImage.Children.Add(_polygon);
        }

        private void SetTilesImage()
        {
            if (this.FImageTiles != null)
            {
                foreach (Polygon _polygon in this.FImageTiles)
                    this.cStructureImage.Children.Remove(_polygon);
            }

            this.FImageTiles = new List<Polygon>(this.ViewModel.Tiles.Count);
            foreach (TileViewModel _tile in this.ViewModel.Tiles)
            {
                if(_tile != this.ViewModel.SelectedTile)
                    AddTileImage(_tile);
            }

            AddTileImage(this.ViewModel.SelectedTile);
        }

        private void AddTileImage(TileViewModel aTileViewModel)
        {
            if (this.ViewModel.Image == null || aTileViewModel == null)
                return;

            PointCollection _baseTilePoints = StructureImage.GetBasePoints(
                (this.ViewModel.MinXPosRelToBase - 1) * StructureImage.TileWidth,
                (this.ViewModel.MinYPosRelToBase - 1) * StructureImage.TileHeight);

            if(this.ViewModel.Image != null)
                _baseTilePoints = this.ViewModel.Image.BaseTilePoints;
            int _xShift = aTileViewModel.XPosRelToBase - aTileViewModel.YPosRelToBase;
            int _yShift = aTileViewModel.XPosRelToBase + aTileViewModel.YPosRelToBase;

            Point _p0 = new Point(_baseTilePoints[0].X + _xShift * StructureImage.TileWidth / 2,
                                  _baseTilePoints[0].Y + _yShift * StructureImage.TileHeight / 2);
            Point _p1 = new Point(_baseTilePoints[1].X + _xShift * StructureImage.TileWidth / 2,
                                  _baseTilePoints[1].Y + _yShift * StructureImage.TileHeight / 2);
            Point _p2 = new Point(_baseTilePoints[2].X + _xShift * StructureImage.TileWidth / 2,
                                  _baseTilePoints[2].Y + _yShift * StructureImage.TileHeight / 2);
            Point _p3 = new Point(_baseTilePoints[3].X + _xShift * StructureImage.TileWidth / 2,
                                  _baseTilePoints[3].Y + _yShift * StructureImage.TileHeight / 2);

            Polygon _polygon = new Polygon();

            _polygon.Tag = aTileViewModel;
            _polygon.MouseDown += TileImage_MouseDown;

            _polygon.Stroke = aTileViewModel == this.ViewModel.SelectedTile ? this.FImageSelectedTileBrush : this.FImageTileBrush;
            _polygon.StrokeThickness = 2;
            _polygon.Fill = Brushes.Transparent;
            _polygon.Points = new PointCollection(4);
            _polygon.Points.Add(_p0);
            _polygon.Points.Add(_p1);
            _polygon.Points.Add(_p2);
            _polygon.Points.Add(_p3);
            this.FImageTiles.Add(_polygon);
            this.cStructureImage.Children.Add(_polygon);
        }

        void TileImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Polygon _polygon = (Polygon)sender;
            this.ViewModel.SelectedTile = (TileViewModel)_polygon.Tag;
        }

        private void TileLoc_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Polygon _polygon = (Polygon)sender;
            this.ViewModel.SelectedTileLocData = (RelTileLoc)_polygon.Tag;
        }

        public StructureView()
        {
            InitializeComponent();
        }

        Brush FImageTileBrush = Brushes.Gray;
        Brush FImageSelectedTileBrush = Brushes.Yellow;

        private List<Polygon> FImageTiles;

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells != null && e.AddedCells.Count > 0)
            {
                TileViewModel _tile = (TileViewModel)e.AddedCells[0].Item;
                if(this.ViewModel.SelectedTile != _tile)
                    this.ViewModel.SelectedTile = (TileViewModel)e.AddedCells[0].Item;
            }
        }

        private Point FLastMousePosition = new Point(0, 0);
        private void cStructureImage_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Point _currentPositon = e.GetPosition(this);
                if (this.FLastMousePosition.X != 0)
                {
                    this.ViewModel.ImageX += (_currentPositon.X - this.FLastMousePosition.X);
                    this.ViewModel.ImageY += (_currentPositon.Y - this.FLastMousePosition.Y); 
                }
                this.FLastMousePosition = _currentPositon;
            }
            else
            {
                this.FLastMousePosition = new Point(0, 0);
            }
        }
    }
}
