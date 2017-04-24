using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
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

namespace MapViewer
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

                this.Reload();
            }
        }

        public void Reload()
        {
            if (this.ViewModel == null)
                return;

            if (this.mgShape == null)
                this.mgShape = new Model3DGroup();

            this.mgShape.Children.Clear();

            this.FLayers = new List<GeometryModel3D>[this.ViewModel.DrewLayers.Length];
            for (int i = 0; i < this.FLayers.Length; i++)
                this.FLayers[i] = new List<GeometryModel3D>();

            this.FPh = new ProgressHolder();
            this.FPh.IsCancelable = true;
            try
            {
                ProgressWindow.Run(this.FPh);

                this.BuildMap();
            }
            finally
            {
                this.FPh.Progress = -1;
            }

            CreateLandSurface();

            this.SetTransparentAll(false);
        }

        public void BuildMap()
        {
            int _mapSize = this.ViewModel.Map.WORLD_SIZE;

            for (int i = 0; i < _mapSize; i++)
            {
                for (int j = 0; j < _mapSize; j++)
                {
                    for (int k = 0; k < this.ViewModel.DrewLayers.Length; k++)
                    {
                        if (k == 1 || k == 3 || !this.ViewModel.DrewLayers[k])
                            continue;

                        MapElement element = this.ViewModel.Map.Elementes[_mapSize * j + i];

                        foreach (LevelNode.TileIndex tileIndex in element.pLevelNodes[k].tileIndexes)
                        {
                            int _subIndex = tileIndex.usTypeSubIndex - 1;
                            JsdFile _jsdFile = this.ViewModel.Map.MapTileSet[tileIndex.ubType].Jsd;

                            if (_jsdFile == null)
                                continue;

                            JsdStruct _jsdStruct = _jsdFile.Structs.
                                SingleOrDefault(x => x.StructureNumber == _subIndex);

                            if (_jsdStruct != null)
                            {
                                for (int l = 0; l < _jsdStruct.Tiles.Length; l++)
                                {
                                    JsdTile _tile = _jsdStruct.Tiles[l].Clone();
                                    if (k > 3)
                                        _tile.Flags |= JsdTile.JsdTileFlags.TILE_ON_ROOF;
                                    _tile.XPosRelToBase += (sbyte)(i - _mapSize / 2);
                                    _tile.YPosRelToBase += (sbyte)(j - _mapSize / 2);
                                    GeometryModel3D _tileGeometry = this.AddTile(_tile);
                                    this.FLayers[k].Add(_tileGeometry);
                                }
                            }

                            if (this.FPh.Progress < 0)
                                return;
                        }
                    }
                }
                this.FPh.Progress = i * 100 / _mapSize;
            }
        }

        private void CreateLandSurface()
        {
            DiffuseMaterial _landMaterial = (DiffuseMaterial)this.FindResource("LandMaterial");
            DiffuseMaterial _landBackMaterial = (DiffuseMaterial)this.FindResource("LandBackMaterial");
            GeometryModel3D _landModel = new GeometryModel3D();
            _landModel.Material = _landMaterial;
            _landModel.BackMaterial = _landBackMaterial;

            double _landSize = 1024;
            MeshGeometry3D _landGeometry = new MeshGeometry3D();
            List<Point3D> _landPositions = new List<Point3D>(4);
            double _landLevel = -this.ViewModel.Center.Z;

            _landPositions.Add(new Point3D(-_landSize, _landLevel, -_landSize));
            _landPositions.Add(new Point3D(-_landSize, _landLevel, _landSize));
            _landPositions.Add(new Point3D(_landSize, _landLevel, _landSize));
            _landPositions.Add(new Point3D(_landSize, _landLevel, -_landSize));

            List<int> _landIndeces = new List<int>() { 0, 1, 2, 2, 3, 0 };

            _landGeometry.Positions = new Point3DCollection(_landPositions);
            _landGeometry.TriangleIndices = new Int32Collection(_landIndeces);
            _landModel.Geometry = _landGeometry;
            this.mgShape.Children.Add(_landModel);
        }

        private List<GeometryModel3D>[] FLayers;

        private MaterialGroup FTileMaterial;
        private Material FBackTileMaterial;
        private ProgressHolder FPh;

        private GeometryModel3D AddTile(JsdTile aTile)
        {
            int _tileSize = JsdTile.GetProfileXSize(aTile.IsHighDefenition);
            int _tileBottom = (aTile.Flags & JsdTile.JsdTileFlags.TILE_ON_ROOF) > 0 ?
                JsdTile.GetProfileZSize(aTile.IsHighDefenition) : 0;

            GeometryModel3D _tileGeometry = new GeometryModel3D();
            _tileGeometry.Geometry = new MeshGeometry3D();
            _tileGeometry.Material = this.FTileMaterial;
            _tileGeometry.BackMaterial = this.FBackTileMaterial;

            TileGeometry _tile = new TileGeometry(
                aTile,
                _tileGeometry,
                aTile.IsHighDefenition,
                this.ViewModel.Center);

            for (int i = 0; i < aTile.ProfileSize; i++)
            {
                int y = i % aTile.ProfileXSize;
                int x = i / aTile.ProfileXSize;
                for(int z = 0; z < aTile.ProfileZSize; z++)
                {
                    int _mask = 1 << z;
                    if ((aTile.Shape[i] & _mask) > 0)
                        _tile.AttachCube(x, y, z * 2, false, _tileSize, _tileBottom);
                }
            }

            //_tile.JoinCubeSides();
            _tile.UpdateTileGeometry();
            this.mgShape.Children.Add(_tileGeometry);
            return _tileGeometry;
        }


        public ShapeView3D()
        {
            InitializeComponent();
            this.FTileMaterial = (MaterialGroup)this.FindResource("TileMaterial");
            this.FBackTileMaterial = (DiffuseMaterial)this.FindResource("BackTileMaterial");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Reload();
        }

        public void AddLayer(int aLayerNumber)
        {
            if (this.FLayers == null)
                return;

            foreach(GeometryModel3D _geometry in this.FLayers[aLayerNumber])
            {
                if (!this.mgShape.Children.Contains(_geometry))
                    this.mgShape.Children.Add(_geometry);
            }
        }

        public void RemoveLayer(int aLayerNumber)
        {
            if (this.FLayers == null)
                return;

            foreach (GeometryModel3D _geometry in this.FLayers[aLayerNumber])
            {
                if (this.mgShape.Children.Contains(_geometry))
                    this.mgShape.Children.Remove(_geometry);
            }
        }

        private Point FMousePosition;

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            Point _newPosition = e.GetPosition((IInputElement)sender);

            if (e.RightButton == MouseButtonState.Pressed)
            {
                this.ViewModel.CameraPositionX += -(_newPosition.X - this.FMousePosition.X) * this.ViewModel.Step;
                this.ViewModel.CameraPositionY += (_newPosition.Y - this.FMousePosition.Y) * this.ViewModel.Step;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //this.ViewModel.LookDirection = new Vector3D(
                //    this.ViewModel.LookDirection.X + (_newPosition.X - this.FMousePosition.X),
                //    this.ViewModel.LookDirection.Y + (_newPosition.Y - this.FMousePosition.Y),
                //    this.ViewModel.LookDirection.Z);

                this.anrY.Angle += (_newPosition.X - this.FMousePosition.X) / 10;
                this.anrX.Angle += (_newPosition.Y - this.FMousePosition.Y) / 10;
            }

            this.FMousePosition = _newPosition;
        }

        private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double _delta = e.Delta / 50;
            Point3D _newPosition = new Point3D(
                this.ViewModel.CameraPosition.X,
                this.ViewModel.CameraPosition.Y,
                this.ViewModel.CameraPosition.Z + _delta * 10 * this.ViewModel.Step);
            this.ViewModel.CameraPosition = _newPosition;

        }

        private void Viewport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                Point3D _newPosition = new Point3D(
                    this.ViewModel.CameraPosition.X - 10 * this.ViewModel.Step,
                    this.ViewModel.CameraPosition.Y - 10 * this.ViewModel.Step,
                    this.ViewModel.CameraPosition.Z - 10 * this.ViewModel.Step);
                this.ViewModel.CameraPosition = _newPosition;
            }
            if (e.Key == Key.S)
            {
                Point3D _newPosition = new Point3D(
                    this.ViewModel.CameraPosition.X + 10 * this.ViewModel.Step,
                    this.ViewModel.CameraPosition.Y + 10 * this.ViewModel.Step,
                    this.ViewModel.CameraPosition.Z + 10 * this.ViewModel.Step);
                this.ViewModel.CameraPosition = _newPosition;
            }
            if(e.Key == Key.D)
            {
                this.ViewModel.CameraPositionX -= 10 * this.ViewModel.Step;
            }
            if(e.Key == Key.A)
            {
                this.ViewModel.CameraPositionX += 10 * this.ViewModel.Step;
            }
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

            this.SetTransparentAll(_isTransparent);
        }

        private void SetTransparentAll(bool _isTransparent)
        {
            DiffuseMaterial _material = (DiffuseMaterial)this.FTileMaterial.Children[0];
            this.SetTransparent(_material, _isTransparent);

            DiffuseMaterial _backMaterial = (DiffuseMaterial)this.FBackTileMaterial;
            this.SetTransparent(_backMaterial, _isTransparent);
        }
    }
}
