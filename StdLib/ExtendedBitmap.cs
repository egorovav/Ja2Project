using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace StiLib
{
    public class ExtendedBitmap
    {
        public ExtendedBitmap()
        {
        }
        public ExtendedBitmap(Bitmap bm, Int16 offsetX, Int16 offsetY)
        {
            this.Bm = bm;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }

        public ExtendedBitmap(Bitmap bm, RGB rgb)//, UInt16 height, UInt16 width)
        {
            this.Bm = bm;
            this.RgbData = rgb;
            //this.Height = height;
            //this.Width = width;
        }

        private ExtendedBitmap(ExtendedBitmap old)
        {
            this.Bm = (Bitmap)old.Bm.Clone();
            this.OffsetX = old.OffsetX;
            this.OffsetY = old.OffsetY;
            this.id = old.Id;
            this.ApplicationData = old.ApplicationData;

            this.RgbData = old.RgbData;
            //this.Height = old.Height;
            //this.Width = old.Width;
        }

        string id;
        public string Id
        {
            get
            {
                if (id == null)
                    id = Guid.NewGuid().ToString();
                return id;
            }
        }

        private UInt32 FTransparentColorIndex = 0;
        public UInt32 TransparentColorIndex
        {
            get { return this.FTransparentColorIndex; }
            set { this.FTransparentColorIndex = value; }
        }

        //private byte FBehaviorFlags = 0;
        //public byte BehaviorFlags
        //{
        //    get { return this.FBehaviorFlags; }
        //    set { this.FBehaviorFlags = value; }
        //}


        private Bitmap _bm;
        public Bitmap Bm
        {
            get 
            {
                return _bm;

                //return BMP.ConvertIndexedToArgb32(_bm);

                //Bitmap _argbFile = BMP.ConvertIndexedToArgb32(_bm);
                //return BMP.Convert32argbToIndexed(_argbFile, _bm.Palette);
            }
            set
            {
                _bm = value;
            }
        }

        public Int16 OffsetX;
        public Int16 OffsetY;
        public byte[] ApplicationData;
        // RgbData
        public RGB RgbData;
        //public UInt16 Height;
        //public UInt16 Width;
        //
        public int ForeshorteningLength
        {
            get
            {
                if (ApplicationData != null)
                {
                    foreach (int value in ApplicationData)
                        if (value != 0)
                            return value;
                }
                return 0;
            }
        }

        public virtual ExtendedBitmap Clone()
        {
            ExtendedBitmap newExBm = new ExtendedBitmap(this);
            newExBm.TransparentColorIndex = this.TransparentColorIndex;
            return newExBm;
        }

        // Trim background pixels
        public void Trim()
        {
            Color bc = this.Bm.Palette.Entries[0];
            int _top = -1;
            int _left = this.Bm.Width;

            for (int i = 0; i < this.Bm.Height; i++)
            {
                for (int j = 0; j < this.Bm.Width; j++)
                {
                    Color c = this.Bm.GetPixel(j, i);
                    if (c != bc)
                    {
                        if (_top < 0)
                            _top = i;

                        if (_left > j)
                            _left = j;

                        continue;
                    }
                }
            }

            int _bottom = this.Bm.Height;
            int _right = 0;

            for (int i = this.Bm.Height - 1; i > 0; i--)
            {
                for (int j = this.Bm.Width - 1; j > 0; j--)
                {
                    Color c = this.Bm.GetPixel(j, i);
                    if (c != bc)
                    {
                        if (_bottom == this.Bm.Height)
                            _bottom = i;

                        if (_right < j)
                            _right = j;
                    }
                }
            }

            int _width = _right - _left;
            int _height = _bottom - _top;

            this.OffsetX += (short)(_left - this.Bm.Width / 2);
            this.OffsetY += (short)(_top - this.Bm.Height / 2);

            this.Bm = this.Bm.Clone(new Rectangle(_left, _top, _width, _height), PixelFormat.Format8bppIndexed);
        }
    }
}
