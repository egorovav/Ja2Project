using System;
using System.Drawing;
using System.Windows.Forms;
using StiLib;

namespace SpriteWorld
{
	public class Sprite
	{
		public string Name;
		public Sprite(SpriteCanvas.Canvas canvas)
		{
			this.oCanvas=canvas;
            this.oFrame = 0;
            int positionX = 0;
            int positionY = 0;
            foreach (ExtendedBitmap exBm in canvas.PictureFile)
            {
                if (exBm.OffsetX < 0)
                    positionX = Math.Max(positionX, Math.Abs(exBm.OffsetX));
                if (exBm.OffsetY < 0)
                    positionY = Math.Max(positionY, Math.Abs(exBm.OffsetY));
            }
            this.pPosition = new Point(positionX, positionY);
		}
		public SpriteCanvas.Canvas oCanvas;
		private Point pPosition=new Point(0,0);
		private double pScale=1.0;
		private int pFrame=0;
		public bool oAnimated=false;
		public int oZorder=0;
        public Size oSize
        {
            get 
            {
                Size picSize = oImage.Bm.Size;
                return new Size((int)(picSize.Width * pScale), (int)(picSize.Height * pScale)); 
            }
        }
        ExtendedBitmap oImage
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
			}
		}

		public Point oPosition
		{
			get
			{
                return new Point(
                    pPosition.X + (int)(oImage.OffsetX * pScale), 
                    pPosition.Y + (int)(oImage.OffsetY * pScale));
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
