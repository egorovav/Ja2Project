using System;
using System.Drawing;
using System.Windows.Forms;
using StiLib;

namespace dotNetStiEditor
{
	public class Sprite
	{
		public string Name;
		public Sprite(Canvas canvas, bool useOffset)
		{
			this.oCanvas=canvas;
            this.oFrame = 0;
            int positionX = 0;
            int positionY = 0;
						int maxHeigth = 0;
						int minHeigth = Int32.MaxValue;
						int minWidth = Int32.MaxValue;
            foreach (ExtendedBitmap exBm in canvas.PictureFile)
            {
							if (exBm.OffsetX < 0)
								positionX = Math.Max(positionX, Math.Abs(exBm.OffsetX));
							if (exBm.OffsetY < 0)
								positionY = Math.Max(positionY, Math.Abs(exBm.OffsetY));
							maxHeigth = Math.Max(maxHeigth, exBm.Bm.Height);
							minHeigth = Math.Min(minHeigth, exBm.Bm.Height);
							minWidth = Math.Min(minWidth, exBm.Bm.Width);
            }
						positionX = (int)(positionX * 2.3);
						positionY = (int)(positionY * 2.3);
						this.pPosition = this.TilePosition = new Point(positionX, positionY);
			 
						this.useOffset = useOffset;
						this.MaxHeigth = maxHeigth;
						this.MinHeigth = minHeigth;
						this.MinWidth = minWidth;
		}
		public Canvas oCanvas;
		private Point pPosition=new Point(0,0);
		private double pScale=1.0;
		private int pFrame=0;
		public bool oAnimated=false;
		public int oZorder=0;
		public int MaxHeigth;
		public int MinHeigth;
		public int MinWidth;
        public Size oSize
        {
            get 
            {
                Size picSize = oImage.Bm.Size;
                return new Size((int)(picSize.Width * pScale), (int)(picSize.Height * pScale)); 
            }
        }

		public Point TilePosition;

        public ExtendedBitmap oImage
        {
            get { return oCanvas.PictureFile[pFrame]; }
        }
        public Rectangle oDestRect
        {
            get { return new Rectangle(new Point(oPosition.X, oPosition.Y), oSize); }
        }
        Rectangle pDestBgrRect = new Rectangle();
        public Rectangle oDestBgrRect
        {
            get 
            {
                Rectangle union = Rectangle.Union(oDestRect, pDestBgrRect);
                pDestBgrRect = oDestRect;
                return union;
            }
        }

		public long oTimeStamp=0;
		public int oFPS=30;

		public int oFrame
		{
			get
			{
				return pFrame;
			}
			set
			{
                pFrame = value % oCanvas.NumberOfFrames;
			}
		}

		public double oScale
		{
			get
			{
				return pScale;
			}
			set
			{
				pScale=value;
				//oSize = new Size(Convert.ToInt32(oCanvas.SpriteSize.Width * pScale), Convert.ToInt32(oCanvas.SpriteSize.Height * pScale));
				//oDestRect = new Rectangle(pPosition.X, pPosition.Y, oSize.Width, oSize.Height);
			}
		}
		bool useOffset = true;
		public Point oPosition
		{
			get
			{
				Point point = new Point(pPosition.X, pPosition.Y);
				if (useOffset)
					point.Offset((int)(oImage.OffsetX * oScale), (int)(oImage.OffsetY * oScale));
				else
					// числа -20, -10 подобраны 
					point.Offset((int)(-20 * (oScale - 1)), (int)(-10 * (oScale - 1)));
				return point;
			}
			set
			{
				pPosition = new Point(pPosition.X + value.X, pPosition.Y + value.Y);
			}
		}
		public bool MoveAnimation()
		{
			long elapsed=DateTime.Now.Ticks-oTimeStamp;
			if(oTimeStamp==0)
			{
				oTimeStamp=elapsed;
				return false;
			}
			int frameDuration=Convert.ToInt32(1000.0/oFPS);
			int frame=Convert.ToInt32(elapsed/10000/frameDuration);
			if (frame>0)
			{
				oFrame=oFrame+frame;
				oTimeStamp=DateTime.Now.Ticks;
				return true;
			}
			return false;
		}
	}
}
