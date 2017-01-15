using Ja2Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ja2DataImage
{
	public class ExtendedBitmap
	{
		public ExtendedBitmap()
		{
		}
		public ExtendedBitmap(BitmapFrame bm, Int16 offsetX, Int16 offsetY)
		{
			this.Bm = bm;
			this.OffsetX = offsetX;
			this.OffsetY = offsetY;
		}

		public ExtendedBitmap(BitmapFrame bm, StciRgb rgb)
		{
			this.Bm = bm;
			this.RgbData = rgb;
		}

		private ExtendedBitmap(ExtendedBitmap old)
		{
			this.Bm = (BitmapFrame)old.Bm.Clone();
			this.OffsetX = old.OffsetX;
			this.OffsetY = old.OffsetY;
			this.id = old.Id;
			this.ApplicationData = old.ApplicationData;

			this.RgbData = old.RgbData;
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

		private BitmapFrame _bm;
		public BitmapFrame Bm
		{
			get
			{
				return _bm;
			}
			set
			{
				_bm = value;
			}
		}

		public Int16 OffsetX;
		public Int16 OffsetY;
		public AuxObjectData ApplicationData;
		// RgbData
		public StciRgb RgbData;

		public bool IsForeshorteningStarter
		{
			get { return (ApplicationData.Flags |= AuxObjectFlags.AUX_ANIMATED_TILE) != 0; }
		}

		public int ForeshorteningLength
		{
			get { return ApplicationData.NumberOfFrames; }
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
			Color bc = this.Bm.Palette.Colors[0];
			int _top = -1;
			int _left = this.Bm.PixelWidth;
			byte[] imageData = new byte[this.Bm.PixelWidth * this.Bm.PixelHeight];
			//this.Bm.CopyPixels(imageData, this.Bm.PixelWidth + (4 - this.Bm.PixelWidth % 4) % 4, 0);
			this.Bm.CopyPixels(imageData, this.Bm.PixelWidth, 0);

			for (int i = 0; i < this.Bm.PixelHeight; i++)
			{
				for (int j = 0; j < this.Bm.PixelWidth; j++)
				{
					byte colorIndex = imageData[i * this.Bm.PixelWidth + j];
					Color c = this.Bm.Palette.Colors[colorIndex];
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

			int _bottom = this.Bm.PixelHeight;
			int _right = 0;

			for (int i = this.Bm.PixelHeight - 1; i > 0; i--)
			{
				for (int j = this.Bm.PixelWidth - 1; j > 0; j--)
				{
					byte colorIndex = imageData[i * this.Bm.PixelWidth + j];
					Color c = this.Bm.Palette.Colors[colorIndex];
					if (c != bc)
					{
						if (_bottom == this.Bm.PixelHeight)
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

			byte[] trimedData = new byte[_width * _height];
			this.Bm.CopyPixels(new Int32Rect(_left, _top, _width, _height), trimedData, _width, 0);
			var source = BitmapFrame.Create(
				_width, _height, 96, 96, PixelFormats.Indexed8, this.Bm.Palette, trimedData, _width);
			this.Bm = BitmapFrame.Create(source);
		}

		public static List<ExtendedBitmap> ConvertStciIndexedToBitmaps(StciIndexed aStci)
		{
			var _result = new List<ExtendedBitmap>(aStci.Images.Length);

			foreach(var _subImage in aStci.Images)
			{
				var _stciPalette = aStci.Palette;
				var _header = _subImage.Header;

				var _colors = new List<Color>(StciIndexed.NUMBER_OF_COLORS);
				for (int i = 0; i < StciIndexed.NUMBER_OF_COLORS; i++)
					_colors.Add(Color.FromRgb(_stciPalette[i * 3], _stciPalette[i * 3 + 1], _stciPalette[i * 3 + 2]));
				var _palette = new BitmapPalette(_colors);

				var _imageSource = BitmapSource.Create(
					_header.Width, 
					_header.Height, 
					96, 96, 
					PixelFormats.Indexed8, 
					_palette, 
					_subImage.ImageData, 
					_header.Width);

				var _frame = BitmapFrame.Create(_imageSource);
				var _bm = new ExtendedBitmap(_frame, _header.OffsetX, _header.OffsetY);
				_bm.TransparentColorIndex = (uint)aStci.Header.TransparentColorIndex;
				_bm.ApplicationData = _subImage.AuxData;
				_result.Add(_bm);
			}

			return _result;
		}

		public static StciIndexed ConvertBitmapsToStciIndexed(List<ExtendedBitmap> aBitmaps, bool aIsTransparent, bool aIsTrim)
		{
			if (aBitmaps.Count == 0)
				return null;

			var _subHeader = new StciIndexedHeader((ushort)aBitmaps.Count);
			var _palette = new byte[StciIndexed.NUMBER_OF_COLORS * 3];
			for(int i = 0; i < StciIndexed.NUMBER_OF_COLORS; i++)
			{
				var _color = aBitmaps[0].Bm.Palette.Colors[i];
				_palette[i * 3] = _color.R;
				_palette[i * 3 + 1] = _color.G;
				_palette[i * 3 + 2] = _color.B;
			}
			var _appDataSize = 0;
			if (aBitmaps[0].ApplicationData != null)
				_appDataSize = aBitmaps.Count * 16;

			var _header = new StciHeader(0, aBitmaps[0].TransparentColorIndex, (uint)_appDataSize, _subHeader);

			if (aIsTransparent)
				_header.Flags |= StciFlags.STCI_TRANSPARENT;

			var _subImages = new StciSubImage[aBitmaps.Count];
			for(int i = 0; i < aBitmaps.Count; i++)
			{
				if (aIsTrim)
					aBitmaps[i].Trim();

				var _subImageHeader = new StciSubImageHeader();
				_subImageHeader.OffsetX = aBitmaps[i].OffsetX;
				_subImageHeader.OffsetY = aBitmaps[i].OffsetY;
				_subImageHeader.Width = (ushort)aBitmaps[i].Bm.PixelWidth;
				_subImageHeader.Height = (ushort)aBitmaps[i].Bm.PixelHeight;

				var _subImage = new StciSubImage(_subImageHeader);
				_subImage.ImageData = new byte[_subImage.Header.Width * _subImage.Header.Height];
				aBitmaps[i].Bm.CopyPixels(_subImage.ImageData, _subImage.Header.Width, 0);
				_subImage.AuxData = aBitmaps[i].ApplicationData;
				_subImages[i] = _subImage;
			}

			var _stci = new StciIndexed(_header, _palette, _subImages);
			return _stci;
		}
	}
}
