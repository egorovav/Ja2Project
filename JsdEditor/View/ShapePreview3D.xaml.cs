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

namespace JsdEditor
{
    /// <summary>
    /// Interaction logic for ShapePreview3D.xaml
    /// </summary>
    public partial class ShapePreview3D : UserControl
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
            this.FTiles = new List<TileGeometry>();

            if (this.ViewModel.Structure == null)
                return;

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
                    _ph.Progress = i * 100 / _tilesCount;
                }
            }
            finally
            {
                _ph.Progress = -1;
            }

            CreateLandSurface();
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

        private void CreateLandSurface()
        {
            DiffuseMaterial _landMaterial = (DiffuseMaterial)this.FindResource("LandMaterial");
            DiffuseMaterial _landBackMaterial = (DiffuseMaterial)this.FindResource("LandBackMaterial");
            GeometryModel3D _landModel = new GeometryModel3D();
            _landModel.Material = _landMaterial;
            _landModel.BackMaterial = _landBackMaterial;

            double _landSize = JsdTile.GetProfileXSize(this.ViewModel.Structure.IsHighDefenition) * 5;
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

        private List<TileGeometry> FTiles;

        private Material FTileMaterial;
        private Material FBackTileMaterial;

        private void AddTile(TileViewModel aTile)
        {
            GeometryModel3D _tileGeometry = new GeometryModel3D();
            _tileGeometry.Geometry = new MeshGeometry3D();
            _tileGeometry.Material = this.FTileMaterial;
            _tileGeometry.BackMaterial = this.FBackTileMaterial;

            TileGeometry _tile = new TileGeometry(
                aTile,
                _tileGeometry,
                null,
                this.ViewModel.Structure.IsHighDefenition,
                this.ViewModel.Center);

            this.FTiles.Add(_tile);

            int _tileSize = JsdTile.GetProfileXSize(aTile.IsHighDefenition);
            int _tileBottom = aTile.TileIsOnRoof ? JsdTile.GetProfileZSize(aTile.IsHighDefenition) : 0;

            foreach (LayerCellViewModel _cell in aTile.ShapeViewModel.Cells)
            {
                if (_cell.LayerCellValue)
                    _tile.AttachCube(_cell.X, _cell.Y, _cell.Z * 2, false, _tileSize, _tileBottom, false);
            }

            //_tile.JoinCubeSides();

            _tile.UpdateTileGeometry();

            this.mgShape.Children.Add(_tileGeometry);
        }


        public ShapePreview3D()
        {
            InitializeComponent();
            this.FTileMaterial = (MaterialGroup)this.FindResource("TileMaterial");
            this.FBackTileMaterial = (DiffuseMaterial)this.FindResource("BackTileMaterial");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Reload();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Reload();
        }
    }
}
