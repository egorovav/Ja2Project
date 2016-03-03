using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using StiLib;
//using SpriteCanvas;
//using SpriteWorld;
using System.Drawing.Imaging;
using System.Resources;
using System.IO;
using Resources = LocalizerNameSpace.Localizer;
using System.Runtime.InteropServices;

namespace dotNetStiEditor
{
	public partial class Player : UserControl
	{
		public Player(WorkSpace workSpace)
		{
			Initialize();

			this.workSpace = workSpace;
			string backgroundFolder = Path.Combine(Application.StartupPath, "Background");
			if (Directory.Exists(backgroundFolder))
			{
				DirectoryInfo dir = Directory.CreateDirectory(backgroundFolder);
				FileInfo[] files = dir.GetFiles();
				if (files.Length > 0)
				{
					setBackGround(files[0].FullName);
					foreach (FileInfo file in files)
					{
						FileInfo localFile = file;
						this.фонToolStripMenuItem.DropDownItems.Add(file.Name, null,
						delegate(object sender, EventArgs e)
						{
							setBackGround(localFile.FullName);
						});
					}
				}
				//{
				//  setBackGround(dotNetStiEditor.Properties.Resources.Background);
				//}
			}
			else
			{
				setBackGround(dotNetStiEditor.Properties.Resources.Background);

                MessageBox.Show(LocalizerNameSpace.Localizer.GetString("NoFolder"), 
                    LocalizerNameSpace.Localizer.GetString("Attention"),
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		void Initialize()
		{
			InitializeComponent();

			#region SET LOCAL STRINGS
			this.label4.Text = Resources.GetString("Displacement");
			this.label3.Text = Resources.GetString("Displacement");
			this.загрузитьToolStripMenuItem.Text = Resources.GetString("Load");
			this.label5.Text = Resources.GetString("Number");
			this.label1.Text = Resources.GetString("Number");
			this.сохранитьСмещенияButton3.Text = Resources.GetString("SaveDisplacement");
			this.label6.Text = Resources.GetString("Scale");
			this.label2.Text = Resources.GetString("Scale");
			this.button1.Text = Resources.GetString("Start");
			this.button2.Text = Resources.GetString("Stop");
			this.фонToolStripMenuItem.Text = Resources.GetString("Background");
            this.chbForAllFrames.Text = Resources.GetString("ForAllFrames                                                                                                               ");
			#endregion
		}

		public void AddMovie(List<ExtendedBitmap> movie, Point point)
		{
			if (this.прозрачностьToolStripMenuItem.Checked || this.прозрачностьТеньToolStripMenuItem.Checked)
				movie = movie.ConvertAll<ExtendedBitmap>(
					delegate(ExtendedBitmap exBm)
					{
						exBm = exBm.Clone();
						Bitmap bm = exBm.Bm;
						ColorPalette tempPalette = bm.Palette;
						if (tempPalette.Entries.Length >= 255)
						{
							if (this.прозрачностьToolStripMenuItem.Checked)
								tempPalette.Entries[0] = Color.Transparent;
							if (this.прозрачностьТеньToolStripMenuItem.Checked)
								tempPalette.Entries[254] = Color.Transparent;
							bm.Palette = tempPalette;
						}
						return exBm;
					});

            if (this.leftWorld != null)
            {
                Canvas newCanvas = new Canvas(movie);
                this.leftWorld.AddSprite(newCanvas, point, 0, 15, false, true);
                //this.leftWorld.Library.Item(2).oPosition = this.leftWorld.Library.Item(2).TilePosition;
                this.leftWorld.RequestRendering(2);
            }
		}

		public void SetMovie(List<ExtendedBitmap> leftMovie, List<ExtendedBitmap> rigthMovie)
		{
			this.pictureBox1.Invalidate();
			this.leftCanvas = this.rightCanvas = null;
			this.leftWorld = this.rightWorld = null;
			if (leftMovie != null && leftMovie.Count > 0)
			{
				createCanvas(leftMovie, new Point(0, 0), out this.leftCanvas, out this.leftWorld);
				this.numericUpDown1.Value = 1;
				this.leftXNumericUpDown2.Value = leftCanvas.PictureFile[0].OffsetX;
				this.leftYNumericUpDown3.Value = leftCanvas.PictureFile[0].OffsetY;
				this.numericUpDown1.Enabled = true;
				this.leftXNumericUpDown2.Enabled = true;
				this.leftYNumericUpDown3.Enabled = true;
				this.hScrollBar1.Enabled = true;

				this.сохранитьСмещенияButton3.Enabled = true;
				this.FPSScroll.Enabled = true;
				this.button1.Enabled = true;
				this.button2.Enabled = true;
			}
			else
			{
				this.numericUpDown1.Enabled = false;
				this.leftXNumericUpDown2.Enabled = false;
				this.leftYNumericUpDown3.Enabled = false;
				this.hScrollBar1.Enabled = false;
			}
			if (rigthMovie != null && rigthMovie.Count > 0)
			{
				createCanvas(rigthMovie, new Point(400, 0), out this.rightCanvas, out this.rightWorld);
				this.numericUpDown4.Value = 1;
				this.rightXNumericUpDown6.Value = rightCanvas.PictureFile[0].OffsetX;
				this.rightYNumericUpDown5.Value = rightCanvas.PictureFile[0].OffsetY;
				this.numericUpDown4.Enabled = true;
				this.hScrollBar2.Enabled = true;

				this.сохранитьСмещенияButton3.Enabled = true;
				this.FPSScroll.Enabled = true;
				this.button1.Enabled = true;
				this.button2.Enabled = true;
			}
			else
			{
				this.numericUpDown4.Enabled = false;
				this.hScrollBar2.Enabled = false;
			}
		}

		void createCanvas
				(List<ExtendedBitmap> movie, Point point, out Canvas canvas, out World world)
		{
			if (this.прозрачностьToolStripMenuItem.Checked || this.прозрачностьТеньToolStripMenuItem.Checked)
				movie = movie.ConvertAll<ExtendedBitmap>(
					delegate(ExtendedBitmap exBm)
					{
						exBm = exBm.Clone();
						Bitmap bm = exBm.Bm;
						ColorPalette tempPalette = bm.Palette;
						if (tempPalette.Entries.Length >= 255)
						{
							if (this.прозрачностьToolStripMenuItem.Checked)
								tempPalette.Entries[0] = Color.Transparent;
							if (this.прозрачностьТеньToolStripMenuItem.Checked)
								tempPalette.Entries[254] = Color.Transparent;
							bm.Palette = tempPalette;
						}
						return exBm;
					});

			List<ExtendedBitmap> tileMovie = new List<ExtendedBitmap>();
			//foreach (ExtendedBitmap exBm in movie)
			if (this.отрисовыватьТайлToolStripMenuItem.Checked)
				tileMovie.Add(new ExtendedBitmap(dotNetStiEditor.Properties.Resources.Tile, 0, 0));
			else
				tileMovie.Add(new ExtendedBitmap(new Bitmap(1, 1), 0, 0));
			canvas = new Canvas(movie);
			TileCanvas = new Canvas(tileMovie);
			world = new World();
			this.graphics = this.pictureBox1.CreateGraphics();
			world.CreateViewport(pictureBox1, this.graphics, point,
					new Rectangle(point, new Size((pictureBox1.Width - 30) / 2, pictureBox1.Height)),
					pictureBox1.Image, this.прозрачностьToolStripMenuItem.Checked);

			world.AddSprite(TileCanvas, new Point(), 0, 15, false, false);
			world.AddSprite(canvas, point, 0, 15, false, true);
			Sprite sprite = world.Library.Item(1);
			ExtendedBitmap firstExBm = canvas.PictureFile[0];
			Point tilePosition = sprite.TilePosition;
			if (firstExBm.ApplicationData != null)
				tilePosition.Offset(-20, -10);

			world.Library.Item(0).oPosition = tilePosition;

			this.FPSScroll.Value = 15;
			textBox2.Text = "15";

			FPStimer.Start();
			timer.Start();
			AnimationTimer.Start();
			world.RequestRendering(0);
			world.RequestRendering(1);
		}

		private WorkSpace workSpace;
		private Canvas leftCanvas;
		private Canvas rightCanvas;
		private World leftWorld;
		private World rightWorld;
		private Canvas TileCanvas;
		private Graphics graphics;

		private void timer_Tick(object sender, System.EventArgs e)
		{
			if (leftWorld != null)
				leftWorld.RenderingLoop();
			if (rightWorld != null)
				rightWorld.RenderingLoop();
		}
		private void FPSScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (leftWorld != null)
			{
				leftWorld.Library.Item(1).oFPS = e.NewValue;
				textBox2.Text = Convert.ToString(leftWorld.Library.Item(1).oFPS);

				if (leftWorld.Library.Count > 2)
				{
					leftWorld.Library.Item(2).oFPS = e.NewValue;
					textBox2.Text = Convert.ToString(leftWorld.Library.Item(2).oFPS);
				}
			}
			if (rightWorld != null)
				rightWorld.Library.Item(1).oFPS = e.NewValue;
		}
		private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (leftWorld != null)
				leftWorld.RePaint(sender, e.Graphics, e.ClipRectangle);
			if (rightWorld != null)
				rightWorld.RePaint(sender, e.Graphics, e.ClipRectangle);
		}
		private void AnimationTimer_Tick(object sender, System.EventArgs e)
		{
			if (leftWorld != null)
				leftWorld.UpdateAnimated();
			if (rightWorld != null)
				rightWorld.UpdateAnimated();
		}

		private void стартButton1_Click(object sender, EventArgs e)
		{
			if (leftWorld != null)
			{
				leftWorld.StartAnimation(1);
				leftWorld.StartAnimation(0);
				if (this.leftWorld.Library.Count > 2)
				{
					this.leftWorld.StartAnimation(2);
				}
			}
			if (rightWorld != null)
			{
				rightWorld.StartAnimation(1);
				rightWorld.StartAnimation(0);
			}
			this.numericUpDown1.Enabled = false;
			this.leftXNumericUpDown2.Enabled = false;
			this.leftYNumericUpDown3.Enabled = false;
			this.numericUpDown4.Enabled = false;
		}
		private void стопButton2_Click(object sender, EventArgs e)
		{
			if (leftWorld != null)
			{
				leftWorld.StopAnimation(1);
				leftWorld.StopAnimation(0);
				this.numericUpDown1.Value = leftWorld.Library.Item(1).oFrame + 1;
				this.numericUpDown1.Enabled = true;
				this.leftXNumericUpDown2.Enabled = true;
				this.leftYNumericUpDown3.Enabled = true;

				if (leftWorld.Library.Count > 2)
				{
					leftWorld.StopAnimation(2);
					this.numericUpDown4.Value = leftWorld.Library.Item(2).oFrame + 1;
				}
			}
			if (rightWorld != null)
			{
				rightWorld.StopAnimation(1);
				rightWorld.StopAnimation(0);
				this.numericUpDown4.Value = rightWorld.Library.Item(1).oFrame + 1;
				this.numericUpDown4.Enabled = true;
			}
		}
		Dictionary<string, Bitmap> backGroundCache = new Dictionary<string, Bitmap>();
		private void загрузитьФонToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//this.openFileDialog1.Filter = "Image Files (*.sti, *.gif, *.tif)|*.sti; *.gif; *.tif";
			if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string bgrName = this.openFileDialog1.FileName;
				setBackGround(bgrName);
			}

		}
		private void поменятьФонToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string itemName = ((ToolStripMenuItem)sender).Text;
			string bgrName = Application.StartupPath + itemName;
			setBackGround(bgrName);
		}
		void setBackGround(string bgrName)
		{
			if (File.Exists(bgrName))
			{
				Bitmap bgrImage = null;
				if (!backGroundCache.TryGetValue(bgrName, out bgrImage))
				{
					try
					{
						bgrImage = new Bitmap(bgrName);
					}
					catch (ArgumentException exc)
					{
						MessageBox.Show(
								String.Format("{0}\n Файл {1}\n {2}", exc.Message, bgrName,
                                LocalizerNameSpace.Localizer.GetString("NotAPicture")),
                                LocalizerNameSpace.Localizer.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					backGroundCache.Add(bgrName, bgrImage);
				}
				setBackGround(bgrImage);
			}
			else
				MessageBox.Show(
                String.Format("Файл {0}\n {1}", bgrName, LocalizerNameSpace.Localizer.GetString("NotExists")),
                LocalizerNameSpace.Localizer.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void setBackGround(Image bgrImage)
		{
			if (bgrImage != null)
			{
				this.pictureBox1.Image = bgrImage;
				if (this.leftWorld != null)
				{
					((WorldView)this.leftWorld.Viewport[0]).SetBackground(bgrImage);
					this.leftWorld.RequestRendering(1);
					this.leftWorld.RequestRendering(0);
					if (this.leftWorld.Library.Count > 2)
					{
						this.leftWorld.RequestRendering(2);
					}
				}
				if (this.rightWorld != null)
				{
					((WorldView)this.rightWorld.Viewport[0]).SetBackground(bgrImage);
					this.rightWorld.RequestRendering(1);
					this.rightWorld.RequestRendering(0);
				}
			}
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			NumericUpDown num = (NumericUpDown)sender;
			//Bitmap imageForSave = CreateBitmapFromGraphics(this.graphics, 100, 100, 200, 200);
			//imageForSave.Save("bm_" + num.Value.ToString() + ".gif", ImageFormat.Gif);
			Canvas canvas = this.leftCanvas;
			if (canvas != null)
			{
				changeFrame(num, this.leftWorld, canvas.NumberOfFrames);
				this.leftXNumericUpDown2.Value = canvas.PictureFile[(int)num.Value - 1].OffsetX;
				this.leftYNumericUpDown3.Value = canvas.PictureFile[(int)num.Value - 1].OffsetY;
			}
		}
		private void numericUpDown4_ValueChanged(object sender, EventArgs e)
		{
			NumericUpDown num = (NumericUpDown)sender;
			Canvas canvas = this.rightCanvas;
			if (canvas != null)
			{
				changeFrame((NumericUpDown)sender, this.rightWorld, canvas.NumberOfFrames);
				this.rightXNumericUpDown6.Value = canvas.PictureFile[(int)num.Value - 1].OffsetX;
				this.rightYNumericUpDown5.Value = canvas.PictureFile[(int)num.Value - 1].OffsetY;
			}
		}
		void changeFrame(NumericUpDown num, World world, int numberOfFrames)
		{
			int value = (int)num.Value;
			if (value > numberOfFrames)
			{
				value = value % numberOfFrames;
				num.Value = value;
			}
			if (value == 0)
			{
				value = numberOfFrames;
				num.Value = value;
			}
			world.Library.Item(1).oFrame = value - 1;
			world.RequestRendering(1);

			if (world.Library.Count > 2)
			{
				world.Library.Item(2).oFrame = value - 1;
				world.RequestRendering(2);
			}
		}
		private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
		{
			if (this.leftWorld != null)
			{
				this.leftWorld.ResizeSprite(0, Convert.ToDouble(e.NewValue) / 100.0);
				this.leftWorld.ResizeSprite(1, Convert.ToDouble(e.NewValue) / 100.0);
				if (this.leftWorld.Library.Count > 2)
				{
					this.leftWorld.ResizeSprite(2, Convert.ToDouble(e.NewValue) / 100.0);
				}
			}
			this.hScrollBar1.Focus();
		}
		private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
		{
			if (this.rightWorld != null)
			{
				this.rightWorld.ResizeSprite(0, Convert.ToDouble(e.NewValue) / 100.0);
				this.rightWorld.ResizeSprite(1, Convert.ToDouble(e.NewValue) / 100.0);
			}
			this.hScrollBar2.Focus();
		}

		private void hScrollBar1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				this.hScrollBar1.Value = 100;
				if (this.leftWorld != null)
				{
					this.leftWorld.ResizeSprite(0, 1);
					this.leftWorld.ResizeSprite(1, 1);
					if (this.leftWorld.Library.Count > 2)
					{
						this.leftWorld.ResizeSprite(2, 1);
					}
				}
			}
		}
		private void hScrollBar2_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				this.hScrollBar2.Value = 100;
				if (this.rightWorld != null)
				{
					this.rightWorld.ResizeSprite(0, 1);
					this.rightWorld.ResizeSprite(0, 1);
				}
			}
		}
		private void menuStrip1_Paint(object sender, PaintEventArgs e)
		{
			if (this.leftWorld != null)
			{
				this.leftWorld.RequestRendering(1);
				this.leftWorld.RequestRendering(0);
				if (this.leftWorld.Library.Count > 2)
				{
					this.leftWorld.RequestRendering(2);
				}
			}
			if (this.rightWorld != null)
			{
				this.rightWorld.RequestRendering(1);
				this.rightWorld.RequestRendering(0);
			}
		}

		private void сохранитьСмещенияButton3_Click(object sender, EventArgs e)
		{
            if (MessageBox.Show(LocalizerNameSpace.Localizer.GetString("SaveDisplacementForWorking"),
                    LocalizerNameSpace.Localizer.GetString("Attention"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				if (leftCanvas != null)
				{
                    if (chbForAllFrames.Checked)
                    {
                        for (int i = 0; i < leftCanvas.NumberOfFrames; i++)
                        {
                            ExtendedBitmap playerExBm = leftCanvas.PictureFile[i];
                            foreach (ExtendedBitmap workSpaceExBm in workSpace.workSpace)
                            {
                                if (workSpaceExBm.Id == playerExBm.Id)
                                {
                                    workSpaceExBm.OffsetX = (short)leftXNumericUpDown2.Value;
                                    workSpaceExBm.OffsetY = (short)leftYNumericUpDown3.Value;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < leftCanvas.NumberOfFrames; i++)
                        {
                            ExtendedBitmap playerExBm = leftCanvas.PictureFile[i];
                            foreach (ExtendedBitmap workSpaceExBm in workSpace.workSpace)
                            {
                                if (workSpaceExBm.Id == playerExBm.Id)
                                {
                                    workSpaceExBm.OffsetX = playerExBm.OffsetX;
                                    workSpaceExBm.OffsetY = playerExBm.OffsetY;
                                }
                            }
                        }
                    }
				}
			}
		}

        private void leftXNumericUpDown2_ValueChanged(object sender, EventArgs e)
        {

            this.leftCanvas.PictureFile[this.leftWorld.Library.Item(1).oFrame].OffsetX =
                (short)((NumericUpDown)sender).Value;

            if (this.leftWorld.Library.Count > 2)
                this.leftCanvas.PictureFile[this.leftWorld.Library.Item(2).oFrame].OffsetX =
                    (short)((NumericUpDown)sender).Value;


            this.leftWorld.RequestRendering(1);
            this.leftWorld.RequestRendering(0);
            if (this.leftWorld.Library.Count > 2)
            {
                this.leftWorld.RequestRendering(2);
            }
        }

        private void leftYNumericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            this.leftCanvas.PictureFile[this.leftWorld.Library.Item(1).oFrame].OffsetY =
                (short)((NumericUpDown)sender).Value;

            if (this.leftWorld.Library.Count > 2)
                this.leftCanvas.PictureFile[this.leftWorld.Library.Item(2).oFrame].OffsetY =
                    (short)((NumericUpDown)sender).Value;

            this.leftWorld.RequestRendering(1);
            this.leftWorld.RequestRendering(0);
            if (this.leftWorld.Library.Count > 2)
            {
                this.leftWorld.RequestRendering(2);
            }
        }

		private void rightXNumericUpDown6_ValueChanged(object sender, EventArgs e)
		{
			this.rightCanvas.PictureFile[this.rightWorld.Library.Item(1).oFrame].OffsetX =
				(short)((NumericUpDown)sender).Value;
			this.rightWorld.RequestRendering(1);
			this.rightWorld.RequestRendering(0);
		}

		private void rightYNumericUpDown5_ValueChanged(object sender, EventArgs e)
		{
			this.rightCanvas.PictureFile[this.rightWorld.Library.Item(1).oFrame].OffsetY =
				(short)((NumericUpDown)sender).Value;

			this.rightWorld.RequestRendering(1);
			this.rightWorld.RequestRendering(0);
		}

		//public bool pMouseDrag = false;
		//private void pictureBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		//{
		//  //when we click mouse we will start dragging sprites underneath
		//  //when we click again we drop 'em
		//  if (pMouseDrag)
		//  {
		//    pMouseDrag = false;
		//    return;
		//  }
		//  leftWorld.StartMouseDrag(e.X, e.Y);
		//  pMouseDrag = true;
		//}

		//private void pictureBox1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		//{
		//  //if we selected some sprites move 'em around
		//  if (pMouseDrag)
		//  {
		//    leftWorld.MoveSelected(e.X, e.Y);
		//  }

		//}

		Bitmap CreateBitmapFromGraphics(Graphics graphicsSource, int x, int y, int width, int height)
		{
			//Получаем HDC (контекст устройства)
			IntPtr hDCSource = graphicsSource.GetHdc();
			//Создаем HDC, совместимый с только-что полученным
			IntPtr hDCDestination = Win32.CreateCompatibleDC(hDCSource);
			//Создаем Win32 bitmap, совместимый с HDC (полученном на первом шаге)
			IntPtr hBitmap = Win32.CreateCompatibleBitmap(hDCSource, width, height);
			//Выбираем bitmap в DC (делаем его текущим), чтобы затем рисовать на нем
			IntPtr hPreviousBitmap = Win32.SelectObject(hDCDestination, hBitmap);
			//Выполняем копирование из полученного DC во вновь созданный совместимый DC,
			//получая таким образом в совместимом bitmap?e нужную картинку
			Win32.BitBlt(hDCDestination, 0, 0, width, height, hDCSource, x, y,
			Win32.SRCCOPY);
			//Создаем объект System.Drawing.Bitmap
			Bitmap bitmap = Bitmap.FromHbitmap(hBitmap);
			//Удаляем уже ненужные объекты
			Win32.DeleteObject(hDCDestination);
			Win32.DeleteObject(hBitmap);
			//Освобождаем DC
			graphicsSource.ReleaseHdc(hDCSource);
			return bitmap;
		}
	}

	public class Win32
	{
		[DllImport("User32.dll")]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("User32.dll")]
		public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("Gdi32.dll")]
		public static extern int BitBlt(
			IntPtr hdcDest,
			int nXDest,
			int nYDest,
			int nWidth,
			int nHeight,
			IntPtr hdcSrc,
			int nXSrc,
			int nYSrc,
			int dwRop
			);

		[DllImport("Gdi32.dll")]
		public static extern int StretchBlt(
			IntPtr hdcDest,
			int nXOriginDest,
			int nYOriginDest,
			int nWidthDest,
			int nHeightDest,
			IntPtr hdcSrc,
			int nXOriginSrc,
			int nYOriginSrc,
			int nWidthSrc,
			int nHeightSrc,
			int dwRop
			);

		public const int SRCCOPY = 0x00CC0020;

		[DllImport("Gdi32.dll")]
		public static extern int SetROP2(IntPtr hDC, int fnDrawMode);

		public const int R2_NOT = 6;
		public const int R2_NOP = 11;

		[DllImport("Gdi32.dll")]
		static extern int MoveToEx(IntPtr hDC, int x, int y, IntPtr lpPoint);

		public static int MoveTo(IntPtr hDC, int x, int y)
		{
			return MoveToEx(hDC, x, y, IntPtr.Zero);
		}

		[DllImport("Gdi32.dll")]
		public static extern int LineTo(IntPtr hDC, int nXEnd, int nYEnd);

		[DllImport("Gdi32.dll")]
		static extern int GetPixel(
			IntPtr hdc,
			int nXPos,
			int nYPos
			);

		public static Color GetPixelColor(IntPtr hDC, int x, int y)
		{
			long colorRef = GetPixel(hDC, x, y);
			return Color.FromArgb((byte)colorRef, (byte)(colorRef >> 8), (byte)(colorRef >> 16));
		}

		[DllImport("Gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hGdiObj);

		[DllImport("Gdi32.dll")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("Gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

		[DllImport("Gdi32.dll")]
		public static extern int DeleteObject(IntPtr hObject);
	}
}
