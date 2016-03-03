using Ja2Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapViewer
{
    public class StructureImage
    {
        public const int TileWidth = 20;
        public const int TileHeight = 10;

        public StructureImage(StciSubImage aStciImage, List<Color> aPalette)
        {
            PixelFormat _pf = PixelFormats.Indexed8;

            this.Height = aStciImage.Header.Height;
            this.Width = aStciImage.Header.Width;
            this.OffsetX = aStciImage.Header.OffsetX;
            this.OffsetY = aStciImage.Header.OffsetY;
            this.Stride = aStciImage.Header.Width * _pf.BitsPerPixel / 8;
            aPalette[0] = Color.FromArgb(0x00, 0x00, 0x00, 0x00);
            BitmapPalette _pb = new BitmapPalette(aPalette);


            this.Bitmap = BitmapSource.Create(
                this.Width,
                this.Height,
                96,
                96,
                _pf,
                _pb,
                aStciImage.ImageData,
                this.Stride);
        }

        public BitmapSource Bitmap
        {
            get;
            protected set;
        }

        public int Height
        {
            get;
            protected set;
        }

        public int Width
        {
            get;
            protected set;
        }

        public int OffsetX
        {
            get;
            protected set;
        }

        public int OffsetY
        {
            get;
            protected set;
        }

        public int Stride
        {
            get;
            protected set;
        }

        private PointCollection FBaseTilePoints;
        public PointCollection BaseTilePoints
        {
            get
            {
                if (this.FBaseTilePoints == null)
                {
                    this.FBaseTilePoints = GetBasePoints(this.OffsetX, this.OffsetY);
                }
                return this.FBaseTilePoints;
            }
        }

        public static PointCollection GetBasePoints(double aOffsetX, double aOffsetY)
        {
            PointCollection _baseTilePoints = new PointCollection(4);

            double _offsetX = -aOffsetX;
            double _offsetY = -aOffsetY;
            //0,10,20,0,40,10,20,20
            _baseTilePoints.Add(new System.Windows.Point(0 + _offsetX, TileHeight + _offsetY));
            _baseTilePoints.Add(new System.Windows.Point(TileWidth + _offsetX, 0 + _offsetY));
            _baseTilePoints.Add(new System.Windows.Point(TileWidth * 2 + _offsetX, TileHeight + _offsetY));
            _baseTilePoints.Add(new System.Windows.Point(TileWidth + _offsetX, TileHeight * 2 + _offsetY));

            return _baseTilePoints;
        }
    }
}
