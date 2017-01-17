using Ja2Data;
using Ja2DataImage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Ja2DataImage
{
	public class GIF
	{
		public static void ConvertBitmapsToGif
			(List<ExtendedBitmap> exBms, string fileName, UInt16 timeOut, bool transparent, bool useLocalPalette)
		{
			UInt16 screenWidth = 0;
			UInt16 screenHeight = 0;
			Int16 shiftX = 0;
			Int16 shiftY = 0;
			bool headerWrited = false;

			foreach (ExtendedBitmap exBm in exBms)
			{
				BitmapFrame bm = exBm.Frame;
				UInt16 curWidth = (UInt16)bm.PixelWidth;
				UInt16 curHeight = (UInt16)bm.PixelHeight;

				if (exBm.OffsetX > 0)
					curWidth += (UInt16)exBm.OffsetX;
				else
				{
					Int16 absShift = (Int16)Math.Abs(exBm.OffsetX);
					if (absShift > shiftX) shiftX = absShift;
				}
				if (exBm.OffsetY > 0)
					curHeight += (UInt16)exBm.OffsetY;
				else
				{
					Int16 absShift = (Int16)Math.Abs(exBm.OffsetY);
					if (absShift > shiftY) shiftY = absShift;
				}
				if (curWidth > screenWidth) screenWidth = curWidth;
				if (curHeight > screenHeight) screenHeight = curHeight;
			}
			screenHeight += (UInt16)shiftY;
			screenWidth += (UInt16)shiftX;

			BinaryWriter bw = new BinaryWriter(new FileStream(fileName, FileMode.Create));
			byte zero = (byte)0;
			byte _separator = (byte)0x2C;	// ','
			byte _terminator = (byte)0x3B;	// ';'
			byte[] _palette = null;
			foreach (ExtendedBitmap exBm in exBms)
			{
				// Convert BitmapFrame to Bitmap
				//Bitmap bm = null;

				GifBitmapEncoder _encoder = new GifBitmapEncoder();
				_encoder.Frames.Add(exBm.Frame);
				MemoryStream ms = new MemoryStream();

				// Use Bitmap instead BmpBitmapEncoder because Bitmap use compressing

				_encoder.Save(ms);

				ms.Position = 0;

				BinaryReader br = new BinaryReader(ms);

				// file header
				// br.ReadBytes(6);						// GIF89a
				// br.ReadUInt16();						// screen size compute from frames size
				// br.ReadUInt16();
				// br.ReadByte();						// header flags, encoder supply 0x70 for local palette usage
				// br.ReadByte();						// virtual background color (usualy equals 0)
				// br.ReadByte();						// pixel shape (usualy equals 0)
				// br.ReadByte();						// separaptor ','
				// image header
				// br.ReadInt16();						// offset X, take it from ExtendedBitmap
				// br.ReadInt16();						// offset Y, take it from ExtendedBitmap
				// br.ReadUInt16();						// width, take it from ExtendedBitmap
				// br.ReadUInt16();						// height, take it from ExtendedBitmap
				// br.ReadByte();						// flags, encoder supply 0x87 for local palette usage

				ms.Seek(23, SeekOrigin.Begin);

				byte[] _formatId = Encoding.ASCII.GetBytes("GIF89a");

				byte _imageFlags = (byte)0x87;
				if (!useLocalPalette)
					_imageFlags = 0;

				byte _headerFlags = (byte)0x70;
				if (!useLocalPalette)
					_headerFlags = (byte)0xF7;

				int _paletteDepth = _headerFlags % 8 + 1;
				int _paletteLength = (int)Math.Pow(2, _paletteDepth) * 3;
				if (_palette == null || useLocalPalette)
					_palette = br.ReadBytes(_paletteLength);	// local palette, will used like global if it's necessary
				else
					ms.Seek(_paletteLength, SeekOrigin.Current);

				int _imageDataLength = (int)(ms.Length - ms.Position);
				// last byte is a file terminator. don't put it into image data!
				_imageDataLength--;

				byte[] _imageData = br.ReadBytes(_imageDataLength);

				br.Close();

				// write header
				if (!headerWrited)
				{
					bw.Write(_formatId);
					bw.Write(screenWidth);		// screen size
					bw.Write(screenHeight);
					bw.Write(_headerFlags);
					bw.Write(zero);				// virtual background color
					bw.Write(zero);				// pixel shape

					// global palette
					if (!useLocalPalette)
						bw.Write(_palette);

					// file extentions
					string comment = "Egorov A. V. aka pipetz for www.ja2.su";
					bw.Write(new byte[] 
						{ 
							0x21,					// '!' - идентификатор начала расширения 
							0xFE,					// Тип расширения. FE - комментарий. 
							(byte)comment.Length,	// Размер блока расширения.
						});
					bw.Write(comment.ToCharArray());
					bw.Write(zero);

					bw.Write(new byte[] 
						{ 
							0x21,									// '!' - идентификатор начала расширения 
							0xFF,									// Тип расширения. FF - расширение стороннего приложения. 
							11,										// Размер блока расширения. 
							0x4E,0x45,0x54,0x53,0x43,0x41,0x50,0x45,// NETSCAPE
							0x32,0x2E,0x30,							// 2.0 
							3,										// Размер блока расширения.
							1, 0, 0,								// Зацикливает последовательность картинок.
							0
						});

					bw.Write(new byte[] 
						{ 
							0x21,									// '!' - идентификатор начала расширения 
							0xFF,									// Тип расширения. FF - расширение стороннего приложения. 
							11,										// Размер блока расширения. 
							0x53,0x54,0x49,0x5F,0x45,0x44,0x49,0x54,// STI_EDIT
							0x31,0x2E,0x30,							// 1.0 
							4                                    
						});

					bw.Write(shiftX);
					bw.Write(shiftY);
					bw.Write(zero);

					headerWrited = true;
				}

				// image extentions
				byte transparentIndex = (byte)(transparent ? 9 : 8);
				bw.Write(new byte[] 
					{ 
						0x21,								// '!' - идентификатор начала расширения 
						0xF9,								// Тип расширения. F9 - поведение картинки 
						4,									// Размер блока расширения. 
						transparentIndex,					// Флаги(предыдущая затирается фоном, прозрачность фона).
						(byte)(timeOut % 256),
						(byte)(timeOut / 256),				// Время задержки(в сотых секунды). 
						(byte)exBm.TransparentColorIndex,   // Индекс прозрачного цвета.
						0									// Идентификатор окончания блока.
					});

				if (exBm.ApplicationData != null)
				{
					bw.Write(new byte[] 
						{ 
							0x21,									// '!' - идентификатор начала расширения 
							0xFF,									// Тип расширения. FF - расширение стороннего приложения. 
							11,										// Размер блока расширения. 
							0x53,0x54,0x49,0x5F,0x45,0x44,0x49,0x54,// STI_EDIT
							0x31,0x2E,0x30,							// 1.0 
							(byte)AuxObjectData.SIZE                                   
						});

					exBm.ApplicationData.Save(bw.BaseStream);
					bw.Write(zero);
				}

				// image header
				bw.Write(_separator);
				bw.Write((Int16)(exBm.OffsetX + shiftX));
				bw.Write((Int16)(exBm.OffsetY + shiftY));
				bw.Write((UInt16)exBm.Frame.PixelWidth);
				bw.Write((UInt16)exBm.Frame.PixelHeight);
				bw.Write(_imageFlags);

				// local palette
				if (useLocalPalette)
					bw.Write(_palette);

				// image data
				bw.Write(_imageData);
			}

			bw.Write(_terminator);
			bw.Close();
		}

		// Анимированный GIF разбиавается покадрово на неанимированные, каждый из которых преобразуется в ExtendedBitmap.
		public static List<ExtendedBitmap> ConvertGifToBitmaps
			(string fileName, int foreshrteningNum, out bool containsLocalPalette)
		{
			List<ExtendedBitmap> result = new List<ExtendedBitmap>();
			containsLocalPalette = false;
			BitmapFrame _prevImage = null;
			using (BinaryReader br = new BinaryReader(new FileStream(fileName, FileMode.Open)))
			{
				while (br.BaseStream.Position < br.BaseStream.Length)
				{
					List<byte> header = new List<byte>();
					byte[] _startBytes = br.ReadBytes(10);
					header.AddRange(_startBytes); //Заголовок: GIF 89a, 
					UInt16 _canvasWidth = BitConverter.ToUInt16(_startBytes, 6); // ширина экрана, 
					UInt16 _canvasHeight = BitConverter.ToUInt16(_startBytes, 8); // высота экрана.

					byte flags = br.ReadByte(); header.Add(flags);
					bool isGlobalPalette = flags > 127;
					header.AddRange(br.ReadBytes(2)); // Цвет фона, форма пикселя. 
					if (isGlobalPalette)
					{
						int paletteDepth = flags % 8 + 1;
						int paletteLength = (Int32)Math.Pow(2, paletteDepth) * 3;
						header.AddRange(br.ReadBytes(paletteLength));
					}
					else
						containsLocalPalette = true;

					byte separator = br.ReadByte();
					int imageCount = 0;
					Int16 shiftX = 0;
					Int16 shiftY = 0;
					bool shiftsRead = false;
					while (!(separator == Convert.ToByte(';')))
					{
						List<byte> image = new List<byte>(header);
						AuxObjectData appData = new AuxObjectData();
						byte _transparentColorIndex = 0;
						byte _imageBehaviorFlags = 0;
						while (!(separator == Convert.ToByte(',')))
						{
							// Выкидываем расширения
							if (separator == Convert.ToByte('!'))
							{
								byte exCode = br.ReadByte();
								byte exLength = br.ReadByte();
								while (exLength != 0)
								{
									if (exCode == 0xFF)
									{
										if (new String(br.ReadChars(8)) == "STI_EDIT")
										{
											br.ReadBytes(exLength - 8);
											if (!shiftsRead)
											{
												br.ReadByte();
												shiftX = br.ReadInt16();
												shiftY = br.ReadInt16();
												shiftsRead = true;
											}
											else
											{
												br.ReadByte(); // read application data length (it is 16)
												appData.Load(br.BaseStream);
											}
											exLength = br.ReadByte();
										}
										else
										{
											br.ReadBytes(exLength - 8);
											exLength = br.ReadByte();
											exCode = 0;
										}
									}
									else if (exCode == 0xF9)
									{
										// если третий бит = 1, предудущий кадр копируется в последующий
										_imageBehaviorFlags = br.ReadByte();
										br.ReadByte(); // Время задержки в мс. В формате STCI не используется.
										_transparentColorIndex = br.ReadByte();
										br.ReadByte(); // Идентификатор окончания блока.
										exLength = br.ReadByte();
									}
									else
									{
										br.ReadBytes(exLength);
										exLength = br.ReadByte();
									}
								}
							}
							separator = br.ReadByte();
						}
						image.Add(separator);
						// Заголовок картинки.

						Int16 offsetX = BitConverter.ToInt16(br.ReadBytes(2), 0);
						Int16 offsetY = BitConverter.ToInt16(br.ReadBytes(2), 0);

						image.AddRange(new byte[4]);
						byte[] width = br.ReadBytes(2); image.AddRange(width);
						byte[] heigth = br.ReadBytes(2); image.AddRange(heigth);
						image[6] = width[0]; image[7] = width[1];
						image[8] = heigth[0]; image[9] = heigth[1];

						flags = br.ReadByte(); // флаги

						image.Add(flags);
						if (flags > 127)
						{
							containsLocalPalette = true;
							int paletteDepth = flags % 8 + 1;
							int paletteLength = (Int32)Math.Pow(2, paletteDepth) * 3;
							image.AddRange(br.ReadBytes(paletteLength));
						}
						image.Add(br.ReadByte()); // bit per pixel
						// Читаем картинку
						byte blockLength = br.ReadByte();

						while (blockLength != 0)
						{
							image.Add(blockLength);
							image.AddRange(br.ReadBytes(blockLength));
							blockLength = br.ReadByte();
						}
						image.Add(blockLength);
						image.Add(Convert.ToByte(';'));
						imageCount++;

						GifBitmapDecoder _decoder = new GifBitmapDecoder(
							new MemoryStream(image.ToArray()), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

						BitmapFrame _bf = _decoder.Frames[0];

						// draw next image over previouse
						if ((_imageBehaviorFlags >> 2) % 2 == 1)
						{
							if (_prevImage != null)
							{
								WriteableBitmap _wb = new WriteableBitmap(_prevImage);
								byte[] _buffer = new byte[_bf.PixelWidth * _bf.PixelHeight];
								_bf.CopyPixels(_buffer, _bf.PixelWidth, 0);
								var _rect = new Int32Rect(offsetX, offsetY, _bf.PixelWidth, _bf.PixelHeight);
								_wb.WritePixels(_rect, _buffer, _bf.PixelWidth, 0);
								_bf = BitmapFrame.Create(_wb);

								offsetX = (short)-shiftX;
								offsetY = (short)-shiftY;
							}

							_prevImage = _bf;
						}
						else
						{
							offsetX -= shiftX;
							offsetY -= shiftY;
						}

						ExtendedBitmap exBm = new ExtendedBitmap(_bf, offsetX, offsetY);
						exBm.ApplicationData = appData;
						exBm.TransparentColorIndex = _transparentColorIndex;

						result.Add(exBm);
						separator = br.ReadByte();
					}
				}
			}
			if (foreshrteningNum != 0)
			{
				int length = result.Count / foreshrteningNum;
				for (int i = 0; i < result.Count; i += length)
				{
					for (int j = i + 1; j < length; j++)
						result[j].ApplicationData = new AuxObjectData();

					result[i].ApplicationData.Flags = AuxObjectFlags.AUX_ANIMATED_TILE;
					result[i].ApplicationData.NumberOfFrames = (byte)length;
				}
			}
			return result;
		}
	}

	public class LocalPaletteException : Exception
	{
		public override string Message
		{
			get
			{
				return "Файл содержит локальные палитры. Конвертирование в sti невозможно.";
			}
		}
	}
}
