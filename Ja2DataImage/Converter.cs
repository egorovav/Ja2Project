using ExtendedGifEncoder;
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
		private const string ApplicationId = "STI_EDIT1.0";

		public static GifBitmapCoder ConvertStciIndexedToGifCoder(StciIndexed aStci, UInt16 aDelay, bool aUseTransparent)
		{
			var _stciPalette = aStci.Palette;
			var _colors = new List<Color>(StciIndexed.NUMBER_OF_COLORS);
			for (int i = 0; i < StciIndexed.NUMBER_OF_COLORS; i++)
				_colors.Add(Color.FromRgb(_stciPalette[i * 3], _stciPalette[i * 3 + 1], _stciPalette[i * 3 + 2]));
			var _palette = new BitmapPalette(_colors);

			var _gifCoder = new GifBitmapCoder(_palette);
			_gifCoder.Extensions.Add(new GifCommentExtension("Egorov A. V. for ja2.su"));

			Int16 _minOffsetX = Int16.MaxValue;
			Int16 _minOffsetY = Int16.MaxValue;

			foreach (var _subImage in aStci.Images)
			{
				var _header = _subImage.Header;

				_minOffsetX = Math.Min(_minOffsetX, _header.OffsetX);
				_minOffsetY = Math.Min(_minOffsetY, _header.OffsetY);
			}

			if (_minOffsetX < 0 || _minOffsetY < 0)
			{
				var _shiftData = new List<byte>(4);
				_shiftData.AddRange(BitConverter.GetBytes(_minOffsetX));
				_shiftData.AddRange(BitConverter.GetBytes(_minOffsetY));
				var _shiftExtension = new GifApplicationExtension(ApplicationId, _shiftData.ToArray());
				_gifCoder.Extensions.Add(_shiftExtension);
			}

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
				var _offsetX = _header.OffsetX;
				var _offsetY = _header.OffsetY;
				if(_minOffsetX < 0 || _minOffsetY < 0)
				{
					// GIF format suports only positive offsets
					_offsetX = (short)(_offsetX - _minOffsetX);
					_offsetY = (short)(_offsetY - _minOffsetY);
				}
				var _bf = new GifBitmapFrame(_frame, (ushort)_offsetX, (ushort)_offsetY);
				_bf.TransparentColorIndex = (byte)aStci.Header.TransparentColorIndex;

				if (_subImage.AuxData != null)
				{
					var _auxData = new byte[AuxObjectData.SIZE];
					_subImage.AuxData.Save(new MemoryStream(_auxData));
					_bf.Extensions.Add(new GifApplicationExtension(ApplicationId, _auxData));
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
			GifBitmapCoder aCoder, bool aIsTransparent, bool aIsTrim, int aForeshotingAmount)
		{
			List<GifBitmapFrame> _bitmaps = aCoder.Frames;
			if (_bitmaps.Count == 0)
				return null;

			var _subHeader = new StciIndexedHeader((ushort)_bitmaps.Count);
			var _palette = new byte[StciIndexed.NUMBER_OF_COLORS * 3];
			for (int i = 0; i < StciIndexed.NUMBER_OF_COLORS; i++)
			{
				var _color = _bitmaps[0].Frame.Palette.Colors[i];
				_palette[i * 3] = _color.R;
				_palette[i * 3 + 1] = _color.G;
				_palette[i * 3 + 2] = _color.B;
			}
			var _appDataSize = 0;
			if (_bitmaps[0].Extensions.FirstOrDefault(
					x => x.ExtensionType == ExtensionType.ApplicationExtension &&
						((GifApplicationExtension)x).ApplicationId == ApplicationId) != null)
				_appDataSize = _bitmaps.Count * 16;

			var _header = new StciHeader(0, _bitmaps[0].TransparentColorIndex, (uint)_appDataSize, _subHeader);

			if (aIsTransparent)
				_header.Flags |= StciFlags.STCI_TRANSPARENT;

			var _subImages = new StciSubImage[_bitmaps.Count];
			BitmapFrame _prevFrame = null;

			var _shiftEx = aCoder.Extensions.FirstOrDefault(x => x.ExtensionType == ExtensionType.ApplicationExtension &&
				((GifApplicationExtension)x).ApplicationId == ApplicationId);

			int _shiftX = 0;
			int _shiftY = 0;
			if(_shiftEx != null)
			{
				_shiftX = BitConverter.ToInt16(_shiftEx.Data, 0);
				_shiftY = BitConverter.ToInt16(_shiftEx.Data, 2);
			}

			for (int i = 0; i < _bitmaps.Count; i++)
			{
				if (aIsTrim)
					_bitmaps[i].Trim();

				var _bf = _bitmaps[i].Frame;

				var _subImageHeader = new StciSubImageHeader();
				if (_bitmaps[i].DisposalMethod == GifFrameDisposalMethod.NotDispose)
				{
					if (_prevFrame != null)
					{
						var _wb = new WriteableBitmap(_prevFrame);
						byte[] _buffer = new byte[_bf.PixelWidth * _bf.PixelHeight];
						_bf.CopyPixels(_buffer, _bf.PixelWidth, 0);
						var _rect = new Int32Rect(_bitmaps[i].OffsetX, _bitmaps[i].OffsetY, _bf.PixelWidth, _bf.PixelHeight);
						_wb.WritePixels(_rect, _buffer, _bf.PixelWidth, 0);
						_bf = BitmapFrame.Create(_wb);
					}

					_prevFrame = _bf;
				}
				else
				{
					_subImageHeader.OffsetX = (short)(_bitmaps[i].OffsetX + _shiftX);
					_subImageHeader.OffsetY = (short)(_bitmaps[i].OffsetY + _shiftY);
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
						_subImage.AuxData.NumberOfFrames = (byte)(_bitmaps.Count / aForeshotingAmount);
					}
				}
				else
				{
					var _appDataExt = _bitmaps[i].Extensions.FirstOrDefault(x =>
						x.ExtensionType == ExtensionType.ApplicationExtension &&
							((GifApplicationExtension)x).ApplicationId == ApplicationId);
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
