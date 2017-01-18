using Ja2Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ja2DataImage
{
	public class Converter
	{
		public static GifBitmapCoder ConvertStciIndexedToGifCoder(StciIndexed aStci, UInt16 aDelay, bool aUseTransparent)
		{
			var _stciPalette = aStci.Palette;
			var _colors = new List<Color>(StciIndexed.NUMBER_OF_COLORS);
			for (int i = 0; i < StciIndexed.NUMBER_OF_COLORS; i++)
				_colors.Add(Color.FromRgb(_stciPalette[i * 3], _stciPalette[i * 3 + 1], _stciPalette[i * 3 + 2]));
			var _palette = new BitmapPalette(_colors);

			var _gifCoder = new GifBitmapCoder(_palette);
			_gifCoder.Extentions.Add(new GifCommentExtension("Egorov A. V. for ja2.su"));
			int j = 0;
			foreach (var _subImage in aStci.Images)
			{
				var _header = _subImage.Header;

				var _imageSource = BitmapSource.Create(
					_header.Width,
					_header.Height,
					96, 96,
					PixelFormats.Indexed8,
					_palette,
					_subImage.ImageData,
					_header.Width);

				var _frame = BitmapFrame.Create(_imageSource);
				var _bf = new GifBitmapFrame(_frame, _header.OffsetX, _header.OffsetY);
				_bf.TransparentColorIndex = (byte)aStci.Header.TransparentColorIndex;

				if (_subImage.AuxData != null)
				{
					var _auxData = new byte[AuxObjectData.SIZE];
					_subImage.AuxData.Save(new MemoryStream(_auxData));
					_bf.Extensions.Add(new GifApplicationExtension("STI_EDIT1.0", _auxData));
				}

				_bf.DisposalMethod = GifFrameDisposalMethod.RestoreToBackgroundColor;
				_bf.UseGlobalPalette = true;
				_bf.UseTransparency = aUseTransparent;
				_bf.Delay = aDelay;
				if (aUseTransparent)
					_bf.TransparentColorIndex = (byte)aStci.Header.TransparentColorIndex;
				_gifCoder.AddFrame(_bf);
			}

			return _gifCoder;
		}

		public static StciIndexed ConvertGifFramesToStciIndexed(
			List<GifBitmapFrame> aBitmaps, bool aIsTransparent, bool aIsTrim, int aForeshotingAmount)
		{
			if (aBitmaps.Count == 0)
				return null;

			var _subHeader = new StciIndexedHeader((ushort)aBitmaps.Count);
			var _palette = new byte[StciIndexed.NUMBER_OF_COLORS * 3];
			for (int i = 0; i < StciIndexed.NUMBER_OF_COLORS; i++)
			{
				var _color = aBitmaps[0].Frame.Palette.Colors[i];
				_palette[i * 3] = _color.R;
				_palette[i * 3 + 1] = _color.G;
				_palette[i * 3 + 2] = _color.B;
			}
			var _appDataSize = 0;
			if (aBitmaps[0].Extensions.FirstOrDefault(
					x => x.ExtensionType == ExtensionType.ApplicationExtension &&
						((GifApplicationExtension)x).ApplicationId == "STI_EDIT1.0") != null)
				_appDataSize = aBitmaps.Count * 16;

			var _header = new StciHeader(0, aBitmaps[0].TransparentColorIndex, (uint)_appDataSize, _subHeader);

			if (aIsTransparent)
				_header.Flags |= StciFlags.STCI_TRANSPARENT;

			var _subImages = new StciSubImage[aBitmaps.Count];
			BitmapFrame _prevFrame = null;
			for (int i = 0; i < aBitmaps.Count; i++)
			{
				if (aIsTrim)
					aBitmaps[i].Trim();

				var _bf = aBitmaps[i].Frame;

				var _subImageHeader = new StciSubImageHeader();
				if (aBitmaps[i].DisposalMethod == GifFrameDisposalMethod.NotDispose)
				{
					if (_prevFrame != null)
					{
						var _wb = new WriteableBitmap(_prevFrame);
						byte[] _buffer = new byte[_bf.PixelWidth * _bf.PixelHeight];
						_bf.CopyPixels(_buffer, _bf.PixelWidth, 0);
						var _rect = new Int32Rect(aBitmaps[i].OffsetX, aBitmaps[i].OffsetY, _bf.PixelWidth, _bf.PixelHeight);
						_wb.WritePixels(_rect, _buffer, _bf.PixelWidth, 0);
						_bf = BitmapFrame.Create(_wb);
					}

					_prevFrame = _bf;
				}
				else
				{
					_subImageHeader.OffsetX = aBitmaps[i].OffsetX;
					_subImageHeader.OffsetY = aBitmaps[i].OffsetY;
				}
				_subImageHeader.Width = (ushort)_bf.PixelWidth;
				_subImageHeader.Height = (ushort)_bf.PixelHeight;

				var _subImage = new StciSubImage(_subImageHeader);
				_subImage.ImageData = new byte[_subImage.Header.Width * _subImage.Header.Height];
				_bf.CopyPixels(_subImage.ImageData, _subImage.Header.Width, 0);


				if (aForeshotingAmount != 0)
				{
					_subImage.AuxData = new AuxObjectData();
					if (i % aForeshotingAmount == 0)
					{
						_subImage.AuxData.Flags = AuxObjectFlags.AUX_ANIMATED_TILE;
						_subImage.AuxData.NumberOfFrames = (byte)(aBitmaps.Count / aForeshotingAmount);
					}
				}
				else
				{
					var _appDataExt = aBitmaps[i].Extensions.FirstOrDefault(x =>
						x.ExtensionType == ExtensionType.ApplicationExtension &&
							((GifApplicationExtension)x).ApplicationId == "STI_EDIT1.0");
					if (_appDataExt != null)
					{
						_subImage.AuxData = new AuxObjectData();
						_subImage.AuxData.Load(new MemoryStream(_appDataExt.Data));
					}
				}

				_subImages[i] = _subImage;
			}

			var _stci = new StciIndexed(_header, _palette, _subImages);
			return _stci;
		}
	}
}
