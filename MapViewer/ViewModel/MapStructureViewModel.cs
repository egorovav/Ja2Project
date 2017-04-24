using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace MapViewer
{
    public class StructureViewModel3D : BaseViewModel
    {
        public StructureViewModel3D(Map aMap, bool[] aDrewLayers)
        {
            this.FMap = aMap;
            this.FDrewLayers = aDrewLayers;

            int _distance = 1000;
            this.FCameraPosition = new Point3D(_distance, _distance, _distance);

            this.FLookDirection = new Vector3D(-this.FCameraPosition.X, -this.FCameraPosition.Y, -this.FCameraPosition.Z);
        }

        private Map FMap;
        public Map Map
        {
            get { return this.FMap; }
        }

        private bool[] FDrewLayers;
        public bool[] DrewLayers
        {
            get { return this.FDrewLayers; }
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
                NotifyPropertyChanged(PointOfViewPropertyName);
            }
        }

        public static string LookDirectionPropertyName = "LookDirection";
        private Vector3D FLookDirection;
        public Vector3D LookDirection
        {
            get { return this.FLookDirection; }
            set
            {
                this.FLookDirection = value;
                NotifyPropertyChanged(LookDirectionPropertyName);
                NotifyPropertyChanged(PointOfViewPropertyName);
            }
        }

        public static string PointOfViewPropertyName = "PointOfView";
        public Point3D PointOfView
        {
            get { return this.CameraPosition + this.LookDirection; }
        }

        public Point3D Center
        {
            get { return new Point3D(0, 0, 0); }
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

        public double Step
        {
            get { return Math.Max(0.1, this.CameraPositionX / 1000); }
        }
    }
}
