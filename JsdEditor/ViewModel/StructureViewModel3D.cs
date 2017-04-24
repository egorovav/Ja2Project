using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace JsdEditor
{
    public class StructureViewModel3D : BaseViewModel
    {
        public StructureViewModel3D(StructureViewModel aStruct)
        {
            this.FStructure = aStruct;

            int _distance = 30;
            if (aStruct != null && aStruct.IsHighDefenition)
                this.FCameraPosition = new Point3D(_distance * 2, _distance * 2, _distance * 2);
            else
                this.FCameraPosition = new Point3D(_distance, _distance, _distance);

            this.FLookDirection = new Vector3D(-this.FCameraPosition.X, -this.FCameraPosition.Y, -this.FCameraPosition.Z);
        }

        private StructureViewModel FStructure;
        public StructureViewModel Structure
        {
            get { return this.FStructure; }
        }

        public static string CameraPositionPropertyName = "CameraPosition";
        private Point3D FCameraPosition;
        public Point3D CameraPosition
        {
            get { return this.FCameraPosition; }
            set
            {
                this.FCameraPosition = value;
                NotifyPropertyChanged(CameraPositionPropertyName);
                NotifyPropertyChanged(CameraPositionXPropertyName);
                NotifyPropertyChanged(CameraPositionYPropertyName);
                NotifyPropertyChanged(CameraPositionZPropertyName);
            }
        }

        public static string LookDirectionPropertyName = "LookDirection";
        private Vector3D FLookDirection;
        public Vector3D LookDirection
        {
            get {  return this.FLookDirection; }
            set
            {
                this.FLookDirection = value;
                NotifyPropertyChanged(LookDirectionPropertyName);
            }
        }

        public Point3D Center
        {
            get
            {
                if (this.Structure.Tiles.Count == 0)
                    return new Point3D(0, 0, 0);

                double _tileWidth = JsdTile.GetProfileXSize(this.Structure.IsHighDefenition);
                double _tileHeigth = JsdTile.GetProfileZSize(this.Structure.IsHighDefenition) * 2;

                double _maxX = this.Structure.Tiles
                    .Select(x => x.XPosRelToBase * _tileWidth + x.ShapeViewModel.MaxX + 1)
                    .Max();
                double _maxY = this.Structure.Tiles
                    .Select(x => x.YPosRelToBase * _tileWidth + x.ShapeViewModel.MaxY + 1)
                    .Max();
                double _maxZ = this.Structure.Tiles
                    .Select(x => (x.TileIsOnRoof ? _tileHeigth : 0) + (x.ShapeViewModel.MaxZ + 1) * 2)
                    .Max();

                double _minX = this.Structure.Tiles
                    .Select(x => x.XPosRelToBase * _tileWidth + x.ShapeViewModel.MinX)
                    .Min();
                double _minY = this.Structure.Tiles
                    .Select(x => x.YPosRelToBase * _tileWidth + x.ShapeViewModel.MinY)
                    .Min();
                double _minZ = this.Structure.Tiles
                    .Select(x => (x.TileIsOnRoof ? _tileHeigth : 0) + x.ShapeViewModel.MinZ * 2)
                    .Min();

                Point3D _center = new Point3D(
                    (_maxX + _minX) / 2, 
                    (_maxY + _minY) / 2, 
                    (_maxZ + _minZ) / 2);
                return _center;
            }
        }

        public Point3D TileCenter
        {
            get
            {
                if (this.Structure.Tiles.Count == 0)
                    return new Point3D(0, 0, 0);

                double _tileWidth = JsdTile.GetProfileXSize(this.Structure.IsHighDefenition);
                double _tileHeigth = JsdTile.GetProfileZSize(this.Structure.IsHighDefenition) * 2;

                double _maxX = this.Structure.Tiles.Max(x => x.XPosRelToBase + 1);
                double _maxY = this.Structure.Tiles.Max(x => x.YPosRelToBase + 1);
                double _maxZ = this.Structure.Tiles.Max(x => x.TileIsOnRoof ? 2 : 1);

                double _minX = this.Structure.Tiles.Min(x => x.XPosRelToBase);
                double _minY = this.Structure.Tiles.Min(x => x.YPosRelToBase);
                double _minZ = this.Structure.Tiles.Min(x => x.TileIsOnRoof ? 1 : 0);

                Point3D _center = new Point3D(
                    (_maxX + _minX) * _tileWidth / 2,
                    (_maxY + _minY) * _tileWidth / 2,
                    (_maxZ + _minZ) * _tileHeigth / 2);
                return _center;
            }
        }

        public static string CameraPositionXPropertyName = "CameraPositionX";
        public double CameraPositionX
        {
            get { return this.CameraPosition.X; }
            set
            {
                double _delta = value - this.CameraPosition.X;
                this.CameraPosition = new Point3D(
                    this.CameraPosition.X + _delta, this.CameraPosition.Y, this.CameraPosition.Z - _delta);
                NotifyPropertyChanged(CameraPositionXPropertyName);
            }
        }

        public static string CameraPositionYPropertyName = "CameraPositionY";
        public double CameraPositionY
        {
            get { return this.CameraPosition.Y; }
            set
            {
                this.CameraPosition = new Point3D(this.CameraPosition.X, value, this.CameraPosition.Z);
                NotifyPropertyChanged(CameraPositionYPropertyName);
            }
        }

        public static string CameraPositionZPropertyName = "CameraPositionZ";
        public double CameraPositionZ
        {
            get { return this.CameraPosition.X; }
            set
            {
                double _delta = value - this.CameraPosition.X;
                this.CameraPosition = new Point3D(
                    this.CameraPosition.X + _delta, this.CameraPosition.Y + _delta, this.CameraPosition.Z + _delta);
                NotifyPropertyChanged(CameraPositionZPropertyName);
            }
        }

        public static string ShowSelectedTileOnlyPropertyName = "ShowSelectedTileOnly";
        private bool FShowSelectedTileOnly;
        public bool ShowSelectedTileOnly
        {
            get { return this.FShowSelectedTileOnly; }
            set
            {
                this.FShowSelectedTileOnly = value;
                NotifyPropertyChanged(ShowSelectedTileOnlyPropertyName);
            }
        }

        public static string FilledPositionCountPropertyName = "FilledPositionCount";
        private int FFilledPositionCount;
        public int FilledPositionCount
        {
            get { return this.FFilledPositionCount; }
            set
            {
                this.FFilledPositionCount = value;
                NotifyPropertyChanged(FilledPositionCountPropertyName);
            }
        }
    }
}
