using Ja2Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MapViewer
{
    public class TileGeometry
    {
        public const int CornersCount = 4;
        public const int SidesCount = 6;

        public TileGeometry(JsdTile aTile, 
            GeometryModel3D aTileGeometry,  
            bool aIsHighDefenition, 
            Point3D aCenter)
        {
            this.FTile = aTile;
            this.FGeometry = aTileGeometry;

            this.FIsHighDefenition = aIsHighDefenition;
            this.FSides = new List<CubeSide>();

            this.FCenter = aCenter;
        }

        private Point3D FCenter;

        private GeometryModel3D FGeometry;
        public GeometryModel3D Geometry
        {
            get { return this.FGeometry; }
        }

        private JsdTile FTile;
        public JsdTile Tile
        {
            get { return this.FTile; }
        }

        private List<CubeSide> FSides;
        private bool FIsHighDefenition;

        private int PointsCount
        {
            get { return this.FSides.Count * CornersCount; }
        }

        //public void AddCube(int aX, int aY, int aZ)
        //{
        //    this.AttachCube(aX, aY, aZ * 2, false);
        //} 

        //public void RemoveCube(int aX, int aY, int aZ)
        //{
        //    this.AttachCube(aX, aY, aZ * 2, true);
        //}

        private void AddCubeSide(CubeSide aSide)
        {
            int _index = this.FSides.FindIndex(new Predicate<CubeSide>(x => x.Equals(aSide))); 
            if(_index < 0)
            {
                this.FSides.Add(aSide);
            }
            else
            {
                this.FSides.RemoveAt(_index);
            }            
        }

        //public void JoinCubeSides()
        //{
        //    for(int i = 0; i < this.FSides.Count; i++)
        //    {
        //        if (this.FSides[i] == null)
        //            continue;

        //        for(int j = 0; j < this.FSides.Count; j++)
        //        {
        //            if (i == j || this.FSides[j] == null)
        //                continue;

        //            CubeSide _joinedSide = this.FSides[i].Join(this.FSides[j]);
        //            if (_joinedSide != null)
        //            {
        //                this.FSides[j] = _joinedSide;
        //                this.FSides[i] = null;
        //                break;
        //            }
        //        }
        //    }

        //    this.FSides = this.FSides.Where(x => x != null).ToList();
        //}

        public void AttachCube(int aX, int aY, int aZ, bool aIsReverse, int aTileSize, int aTileBottom)
        {
            double _x = aX + this.FTile.XPosRelToBase * aTileSize - this.FCenter.X;
            double _y = aZ + aTileBottom * 2 - this.FCenter.Z;
            double _z = aY + this.FTile.YPosRelToBase * aTileSize - this.FCenter.Y;

            Point3D _xyz = new Point3D(_x, _y, _z);
            Point3D _x1yz = new Point3D(_x + 1, _y, _z);
            Point3D _x1y2z = new Point3D(_x + 1, _y + 2, _z);
            Point3D _xy2z = new Point3D(_x, _y + 2, _z);
            Point3D _xy2z1 = new Point3D(_x, _y + 2, _z + 1);
            Point3D _xyz1 = new Point3D(_x, _y, _z + 1);
            Point3D _x1yz1 = new Point3D(_x + 1, _y, _z + 1);
            Point3D _x1y2z1 = new Point3D(_x + 1, _y + 2, _z + 1);

            CubeSide _frontSide = new CubeSide(_xyz, _x1yz, _x1y2z, _xy2z, this.PointsCount, aIsReverse);
            this.AddCubeSide(_frontSide);

            CubeSide _leftSide = new CubeSide(_xyz, _xy2z, _xy2z1, _xyz1, this.PointsCount, aIsReverse);
            this.AddCubeSide(_leftSide);

            CubeSide _rightSide = new CubeSide(_x1yz, _x1yz1, _x1y2z1, _x1y2z, this.PointsCount, aIsReverse);
            this.AddCubeSide(_rightSide);

            CubeSide _backSide = new CubeSide(_xyz1, _xy2z1, _x1y2z1, _x1yz1, this.PointsCount, aIsReverse);
            this.AddCubeSide(_backSide);

            CubeSide _bottomSide = new CubeSide(_xyz, _xyz1, _x1yz1, _x1yz, this.PointsCount, aIsReverse);
            this.AddCubeSide(_bottomSide);

            CubeSide _topSide = new CubeSide(_xy2z, _x1y2z, _x1y2z1, _xy2z1, this.PointsCount, aIsReverse);
            this.AddCubeSide(_topSide);
        }

        public Point3DCollection Positions
        {
            get
            {
                Point3D[] _points = new Point3D[this.PointsCount];

                for (int i = 0; i < this.FSides.Count; i++)
                    Array.Copy(this.FSides[i].Points, 0, _points, i * CornersCount, CornersCount);

                return new Point3DCollection(_points);
            }
        }

        private Int32Collection TriangleIndices
        {
            get
            {
                int[] _indeces = new int[this.FSides.Count * SidesCount];

                for (int i = 0; i < this.FSides.Count; i++)
                    Array.Copy(this.FSides[i].GetTriangleIndices(i * CornersCount), 0, _indeces, i * SidesCount, SidesCount);

                return new Int32Collection(_indeces);
            }
        }

        public void UpdateTileGeometry()
        {
            ((MeshGeometry3D)this.FGeometry.Geometry).Positions = this.Positions;
            ((MeshGeometry3D)this.FGeometry.Geometry).TriangleIndices = this.TriangleIndices;
        }
    }

    public class CubeSide
    {
        // Вершины упорядочены против часовой стрелки.
        public CubeSide(Point3D aP1, Point3D aP2, Point3D aP3, Point3D aP4, int aStartIndex, bool aIsReversed)
        {
            this.FPoints = new Point3D[] { aP1, aP2, aP3, aP4 };

            this.FStartIndex = aStartIndex;
            this.FIsReversed = aIsReversed;

            //Vector3D _v1 = new Vector3D(aP1.X - aP2.X, aP1.Y - aP2.Y, aP1.Z - aP2.Z);
            //Vector3D _v2 = new Vector3D(aP3.X - aP2.X, aP3.Y - aP2.Y, aP3.Z - aP2.Z);
            //this.FNormal = Vector3D.CrossProduct(_v1, _v2);
            //this.FNormal.Normalize();
        }

        //private Vector3D FNormal;

        private Point3D[] FPoints;
        public Point3D[] Points
        {
            get { return this.FPoints; }
        }

        private int FStartIndex;

        private bool FIsReversed;
        public bool IsReversed
        {
            get { return this.FIsReversed; }
        }

        public int[] GetTriangleIndices(int aStartIndex)
        {
            if (this.FIsReversed)
            {
                return new int[] { 
                                       aStartIndex + 0, aStartIndex + 1, aStartIndex + 3,
                                       aStartIndex + 1, aStartIndex + 2, aStartIndex + 3 
                        };

            }
            else
            {
                return new int[] { 
                                        aStartIndex + 0, aStartIndex + 3, aStartIndex + 1,
                                        aStartIndex + 1, aStartIndex + 3, aStartIndex + 2
                        };
            }
        }

        //public CubeSide Join(CubeSide aJoinedSide)
        //{
        //    if (this.FNormal == aJoinedSide.FNormal && this.IsReversed == aJoinedSide.IsReversed)
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            for (int j = 0; j < 4; j++)
        //            {
        //                if (this.FPoints[i] == aJoinedSide.FPoints[j] &&
        //                   this.FPoints[(i + 1) % 4] == aJoinedSide.FPoints[(j + 3) % 4])
        //                {
        //                    CubeSide _side = new CubeSide(
        //                        this.FPoints[(i + 2) % 4],
        //                        this.FPoints[(i + 3) % 4],
        //                        aJoinedSide.FPoints[(j + 1) % 4],
        //                        aJoinedSide.FPoints[(j + 2) % 4], 
        //                        0, this.IsReversed);

        //                    return _side;
        //                }
        //            }
        //        }
        //    }

        //    return null;
        //}

        public override bool Equals(object obj)
        {
            CubeSide _cubeSide = (CubeSide)obj; 
            foreach(Point3D _point in _cubeSide.FPoints)
            {
                if (!this.FPoints.Contains(_point))
                    return false;
            }

            return true;
        }

        private bool FHashCodeCompute = false; 
        private int FHashCode;
        public override int GetHashCode()
        {
            if(!this.FHashCodeCompute)
            {
                this.FHashCode = this.FPoints[0].GetHashCode() ^ 
                                    this.FPoints[1].GetHashCode() ^ 
                                    this.FPoints[2].GetHashCode() ^ 
                                    this.FPoints[3].GetHashCode();
            }

            return this.FHashCode; 
        }
    }
}
