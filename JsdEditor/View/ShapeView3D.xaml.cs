using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JsdEditor
{
    /// <summary>
    /// Interaction logic for ShapeView3D.xaml
    /// </summary>
    public partial class ShapeView3D : UserControl
    {
        public StructureViewModel3D ViewModel
        {
            get { return this.DataContext as StructureViewModel3D; }
            set
            {
                this.DataContext = value;
                this.ViewModel.Structure.PropertyChanged += Structure_PropertyChanged;
                this.ViewModel.Structure.Tiles.CollectionChanged += Tiles_CollectionChanged;
                this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;

                this.Reload();
            }
        }

        public void Reload()
        {

            if (this.mgShape == null)
                this.mgShape = new Model3DGroup();

            this.mgShape.Children.Clear();

            this.FTiles = new List<TileGeometry>();
            this.FSelectedTile = null;
            this.ViewModel.FilledPositionCount = 0;

            int _tilesCount = this.ViewModel.Structure.Tiles.Count;
            ProgressHolder _ph = new ProgressHolder();
            if (_tilesCount > 1)
            {
                Thread _thr = new Thread(ProgressWindowShow);
                _thr.SetApartmentState(ApartmentState.STA);
                _thr.IsBackground = true;
                _thr.Start(_ph);
            }

            try
            {
                for (int i = 0; i < _tilesCount; i++)
                {
                    this.AddTile(this.ViewModel.Structure.Tiles[i]);
                    this.ViewModel.Structure.Tiles[i].AttachHandler(this, Tile_PropertyChanged);
                    _ph.Progress = i * 100 / _tilesCount;
                }
            }
            finally
            {
                _ph.Progress = -1;
            }

            CreateLandSurface();
            SetTransparentAll(false);
        }

        private void ProgressWindowShow(object aProgressHolder)
        {
            ProgressHolder _ph = (ProgressHolder)aProgressHolder;
            ProgressWindow _pw = new ProgressWindow(_ph);
            _pw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _pw.Topmost = true;
            _pw.Show();
            System.Windows.Threading.Dispatcher.Run();
        }

        private TileGeometry FSelectedTile;
        private List<TileGeometry> FTiles;

        private MaterialGroup FTileMaterial;
        private MaterialGroup FAltTileMaterial;
        private Material FBackTileMaterial;
        private MaterialGroup FSelectedTileMaterial;
        private MaterialGroup FAltSelectedTileMaterial;
        private Material FSelectedBackTileMaterial;

        private void AddTile(TileViewModel aTile)
        {
            GeometryModel3D _tileGeometry = new GeometryModel3D();
            _tileGeometry.Geometry = new MeshGeometry3D();

            GeometryModel3D _altTileGeometry = new GeometryModel3D();
            _altTileGeometry.Geometry = new MeshGeometry3D();

            if (aTile.IsSelected)
            {
                _tileGeometry.Material = this.FSelectedTileMaterial;
                _tileGeometry.BackMaterial = this.FSelectedBackTileMaterial;
                _altTileGeometry.Material = this.FAltSelectedTileMaterial;
                _altTileGeometry.BackMaterial = this.FAltSelectedTileMaterial;
            }
            else
            {
                _tileGeometry.Material = this.FTileMaterial;
                _tileGeometry.BackMaterial = this.FBackTileMaterial;
                _altTileGeometry.Material = this.FAltTileMaterial;
                _altTileGeometry.BackMaterial = this.FAltTileMaterial;
            }

            TileGeometry _tile = new TileGeometry(
                aTile,
                _tileGeometry,
                _altTileGeometry,
                this.ViewModel.Structure.IsHighDefenition,
                this.ViewModel.Center);

            this.FTiles.Add(_tile);

            int _tileSize = JsdTile.GetProfileXSize(aTile.IsHighDefenition);
            int _tileBottom = aTile.TileIsOnRoof ? JsdTile.GetProfileZSize(aTile.IsHighDefenition) : 0;

            foreach (LayerCellViewModel _cell in aTile.ShapeViewModel.Cells)
            {
                if (_cell.LayerCellValue)
                {
                    _tile.AddCube(_cell.X, _cell.Y, _cell.Z, _tileSize, _tileBottom);
                    this.ViewModel.FilledPositionCount++;
                }
                _cell.AttachHandler(this, Cell_PropertyChanged);
            }

            _tile.UpdateTileGeometry();

            this.mgShape.Children.Add(_tileGeometry);
            this.mgShape.Children.Add(_altTileGeometry);
        }

        void Cell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LayerCellViewModel _cell = (LayerCellViewModel)sender;
            this.ProcessCell(_cell, this.ViewModel.Structure.SelectedTile);
        }

        private void ProcessCell(LayerCellViewModel aCell, TileViewModel aTile)
        {
            TileGeometry _tile = this.FTiles.Where(x => x.Tile == aTile).SingleOrDefault();
            int _tileSize = JsdTile.GetProfileXSize(aTile.IsHighDefenition);
            int _tileBottom = aTile.TileIsOnRoof ? JsdTile.GetProfileZSize(aTile.IsHighDefenition) : 0;

            if (aCell.LayerCellValue)
            {
                _tile.AddCube(aCell.X, aCell.Y, aCell.Z, _tileSize, _tileBottom);
                this.ViewModel.FilledPositionCount++;
            }
            else
            {
                _tile.RemoveCube(aCell.X, aCell.Y, aCell.Z, _tileSize, _tileBottom);
                this.ViewModel.FilledPositionCount--;
            }

            _tile.UpdateTileGeometry();
        }

        private void CreateLandSurface()
        {
            DiffuseMaterial _landMaterial = (DiffuseMaterial)this.FindResource("LandMaterial");
            DiffuseMaterial _landBackMaterial = (DiffuseMaterial)this.FindResource("LandBackMaterial");
            GeometryModel3D _landModel = new GeometryModel3D();
            _landModel.Material = _landMaterial;
            _landModel.BackMaterial = _landBackMaterial;

            int _landSize = 5;
            Point3D _tileCenter = this.ViewModel.TileCenter;
            Point3D _center = this.ViewModel.Center;
            double _shiftX = 2 * _tileCenter.X - _center.X;
            double _shiftY = 2 * _tileCenter.Y - _center.Y;
            double _landLevel = -_center.Z;

            double _tileSize = JsdTile.GetProfileXSize(this.ViewModel.Structure.IsHighDefenition);
            MeshGeometry3D _landGeometry = new MeshGeometry3D();
            List<Point3D> _landPositions = new List<Point3D>(_landSize * _landSize * 4 * 4);
            List<int> _landIndeces = new List<int>(_landSize * _landSize * 4 * 6);

            int _tileCount = 0;
            int _xStartIndex = (int)(-_landSize - _shiftX / _tileSize);
            int _xEndIndex = _xStartIndex + 2 * _landSize;
            int _yStartIndex = (int)(-_landSize - _shiftY / _tileSize);
            int _yEndIndex = _yStartIndex + 2 * _landSize;

            for (int i = _xStartIndex; i < _xEndIndex; i++)
                for (int j = _yStartIndex; j < _yEndIndex; j++)
                {
                    if ((i + j) % 2 == 0)
                        continue;

                    _landPositions.Add(new Point3D(
                        _tileSize * (i - 1) + _shiftX, _landLevel, _tileSize * (j - 1) + _shiftY));
                    _landPositions.Add(new Point3D(
                        _tileSize * i + _shiftX, _landLevel, _tileSize * (j - 1) + _shiftY));
                    _landPositions.Add(new Point3D(
                        _tileSize * i + _shiftX, _landLevel, _tileSize * j + _shiftY));
                    _landPositions.Add(new Point3D(
                        _tileSize * (i - 1) + _shiftX, _landLevel, _tileSize * j + _shiftY));

                    int _startIndex = _tileCount * 4;
                    _landIndeces.AddRange(new List<int>() { 
                        _startIndex + 0, _startIndex + 3, _startIndex + 2, 
                        _startIndex + 2, _startIndex + 1, _startIndex + 0 });
                    _tileCount++;
                }

            _landGeometry.Positions = new Point3DCollection(_landPositions);
            _landGeometry.TriangleIndices = new Int32Collection(_landIndeces);
            _landModel.Geometry = _landGeometry;
            this.mgShape.Children.Add(_landModel);
        }

        void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == StructureViewModel3D.ShowSelectedTileOnlyPropertyName)
            {
                if (this.ViewModel.ShowSelectedTileOnly)
                {
                    ShowSelectedTile();
                }
                else
                {
                    this.Reload();
                }
            }
        }

        void Structure_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == StructureViewModel.SelectedTilePropertyName)
            {
                this.ChangeSelection();
            }
        }

        private void ChangeSelection()
        {
            if (this.ViewModel.ShowSelectedTileOnly)
            {
                ShowSelectedTile();
            }
            else
            {
                if (this.FTiles == null)
                    return;

                TileGeometry _tile = this.FTiles
                    .Where(x => x.IsSelected)
                    .SingleOrDefault();

                if (this.FSelectedTile != null)
                {
                    this.FSelectedTile.Geometry.Material = this.FTileMaterial;
                    this.FSelectedTile.Geometry.BackMaterial = this.FBackTileMaterial;
                    this.FSelectedTile.AltGeometry.Material = this.FAltTileMaterial;
                    this.FSelectedTile.AltGeometry.BackMaterial = this.FAltTileMaterial;

                }

                if (_tile != null)
                {
                    _tile.Geometry.Material = this.FSelectedTileMaterial;
                    _tile.Geometry.BackMaterial = this.FSelectedBackTileMaterial;
                    _tile.AltGeometry.Material = this.FAltSelectedTileMaterial;
                    _tile.AltGeometry.BackMaterial = this.FAltSelectedTileMaterial;

                    this.FSelectedTile = _tile;
                }
            }
        }

        private void ShowSelectedTile()
        {
            this.mgShape.Children.Clear();
            this.FTiles = new List<TileGeometry>();

            this.ViewModel.Structure.SelectedTile.IsSelected = true;
            this.AddTile(this.ViewModel.Structure.SelectedTile);
        }

        void Tile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Reload();
        }

        void Tiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Reload();
        }

        public ShapeView3D()
        {         
            InitializeComponent();
            this.FTileMaterial = (MaterialGroup)this.FindResource("TileMaterial");
            this.FBackTileMaterial = (DiffuseMaterial)this.FindResource("BackTileMaterial");
            this.FSelectedTileMaterial = (MaterialGroup)this.FindResource("SelectedTileMaterial");
            this.FSelectedBackTileMaterial = (DiffuseMaterial)this.FindResource("SelectedBackTileMaterial");
            this.FAltTileMaterial = (MaterialGroup)this.FindResource("AltTileMaterial");
            this.FAltSelectedTileMaterial = (MaterialGroup)this.FindResource("AltSelectedTileMaterial");
        }

        private void SetTransparent(DiffuseMaterial aMaterial, bool aIsTransparent)
        {
            SolidColorBrush _brush = (SolidColorBrush)aMaterial.Brush;
            Color _color = new Color();
            if (aIsTransparent) _color.A = 0xDD; else _color.A = 0xFF;
            _color.R = _brush.Color.R;
            _color.G = _brush.Color.G;
            _color.B = _brush.Color.B;
            aMaterial.Brush = new SolidColorBrush(_color);
        }

        private void TransparencyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            CheckBox _transparentCheckBox = (CheckBox)sender;
            bool _isTransparent = _transparentCheckBox.IsChecked == true;

            SetTransparentAll(_isTransparent);
        }

        private void SetTransparentAll(bool _isTransparent)
        {
            DiffuseMaterial _material = (DiffuseMaterial)this.FTileMaterial.Children[0];
            this.SetTransparent(_material, _isTransparent);

            DiffuseMaterial _altMaterial = (DiffuseMaterial)this.FAltTileMaterial.Children[0];
            this.SetTransparent(_altMaterial, _isTransparent);

            DiffuseMaterial _selectedMaterial = (DiffuseMaterial)this.FSelectedTileMaterial.Children[0];
            this.SetTransparent(_selectedMaterial, _isTransparent);

            DiffuseMaterial _selectedAltMaterial = (DiffuseMaterial)this.FAltSelectedTileMaterial.Children[0];
            this.SetTransparent(_selectedAltMaterial, _isTransparent);
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double _delta = e.Delta / 50;
            Point3D _newPosition = new Point3D(
                this.ViewModel.CameraPosition.X + _delta,
                this.ViewModel.CameraPosition.Y + _delta,
                this.ViewModel.CameraPosition.Z + _delta);
            this.ViewModel.CameraPosition = _newPosition;

        }
    }
}
