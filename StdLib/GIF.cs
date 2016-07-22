using System;
using System.Collections.Generic;
//using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace StiLib
{
	public class GIF
	{
		public static void ConvertBitmapsToGif
			(List<ExtendedBitmap> exBms, string fileName, UInt16 timeOut, bool transparent, bool useLocalPalette)
		{
			using (BinaryWriter bw = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
			{
				UInt16 screenWidth = 0;
				UInt16 screenHeight = 0;
				Int16 shiftX = 0;
				Int16 shiftY = 0;
				bool headerWrited = false;
				long endHeaderPosition = 0;

				foreach (ExtendedBitmap exBm in exBms)
				{
					Bitmap bm = exBm.Bm;
					UInt16 curWidth = (UInt16)bm.Width;
					UInt16 curHeight = (UInt16)bm.Height;

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
				foreach (ExtendedBitmap exBm in exBms)
				{
					Bitmap bm = exBm.Bm;
					using (MemoryStream ms = new MemoryStream())
					{
						bm.Save(ms, ImageFormat.Gif);
						ms.Position = 0;
						byte[] localPalette = new byte[256 * 3];
						using (BinaryReader br = new BinaryReader(ms))
						{
							if (!headerWrited)
							{
								bw.Write(br.ReadBytes(6)); // Сигнатура, версия: GIF, 89a.
								br.ReadBytes(4);
								bw.Write(screenWidth);
								bw.Write(screenHeight);
								byte flags = br.ReadByte(); bw.Write(flags);
								int paletteDepth = flags % 8 + 1;
								bw.Write(br.ReadBytes(2)); // Цвет фона, форма пикселя.
								int paletteLength = (int)Math.Pow(2, paletteDepth) * 3;
								localPalette = br.ReadBytes(paletteLength);
								bw.Write(localPalette);
								
                                //// Расширение.
                                //if (!useLocalPalette)
                                {
                                    string comment = "Egorov A. V. aka pipetz for www.ja2.su";
                                    bw.Write(new byte[] 
                                { 
                                    0x21,   // '!' - идентификатор начала расширения 
                                    0xFE,   // Тип расширения. FE - комментарий. 
                                    (byte)comment.Length,     // Размер блока расширения.
                                });
                                    bw.Write(comment.ToCharArray());
                                    bw.Write((byte)0);
                                }

								// Расширение.
								bw.Write(new byte[] 
                                { 
                                    0x21,  // '!' - идентификатор начала расширения 
                                    0xFF,  // Тип расширения. FF - расширение стороннего приложения. 
                                    11,    // Размер блока расширения. 
                                    0x4E,0x45,0x54,0x53,0x43,0x41,0x50,0x45,   // NETSCAPE
                                    0x32,0x2E,0x30,                            // 2.0 
                                    3,      // Размер блока расширения.
                                    1, 0, 0,// Зацикливает последовательность картинок.
                                    0
                                });
								//if (!useLocalPalette)
								//{
									bw.Write(new byte[] 
                                { 
                                    0x21,  // '!' - идентификатор начала расширения 
                                    0xFF,  // Тип расширения. FF - расширение стороннего приложения. 
                                    11,    // Размер блока расширения. 
                                    0x53,0x54,0x49,0x5F,0x45,0x44,0x49,0x54,     // STI_EDIT
                                    0x31,0x2E,0x30,                              // 1.0 
                                    4                                    
                                });

									bw.Write(shiftX);
									bw.Write(shiftY);
									bw.Write(byte.MinValue);
								//}

								headerWrited = true;
								endHeaderPosition = br.BaseStream.Position;
							}
							else
							{
								if (useLocalPalette)
								{
									br.BaseStream.Position = 13;
									localPalette = br.ReadBytes(256 * 3);
								}
								// Пропускаем заголовок если он уже записан.
								//else
								//{
									br.BaseStream.Position = endHeaderPosition;
								//}
							}
							byte transparentIndex = (byte)(transparent ? 9 : 8);
                            //byte transparentIndex = (byte)(transparent ? 5 : 4);
							//if (useLocalPalette)
							//{
								//transparentIndex = byte.MinValue;
								//timeOut = 20;
							//}
							// Расширение.
							bw.Write(new byte[] 
                                { 
                                    0x21,  // '!' - идентификатор начала расширения 
                                    0xF9,  // Тип расширения. F9 - поведение картинки 
                                    4,     // Размер блока расширения. 
                                    transparentIndex, // Флаги(предыдущая затирается фоном, прозрачность фона).
                                    (byte)(timeOut % 256),
                                    (byte)(timeOut / 256),// Время задержки(в сотых секунды). 
                                    (byte)exBm.TransparentColorIndex,    // Индекс прозрачного цвета.
                                    0     // Идентификатор окончания блока.
                                });
							if (exBm.ApplicationData != null)
							{
								bw.Write(new byte[] 
                                { 
                                    0x21, // '!' - идентификатор начала расширения 
                                    0xFF, // Тип расширения. FF - расширение стороннего приложения. 
                                    11,   // Размер блока расширения. 
                                    0x53,0x54,0x49,0x5F,0x45,0x44,0x49,0x54,      // STI_EDIT
                                    0x31,0x2E,0x30,                               // 1.0 
                                    (byte)exBm.ApplicationData.Length                                    
                                });
								bw.Write(exBm.ApplicationData);
								bw.Write(byte.MinValue);
							}
							bw.Write(br.ReadByte());  // ',' - начало описания картинки
							br.ReadInt16(); bw.Write((Int16)(exBm.OffsetX + shiftX));
							br.ReadInt16(); bw.Write((Int16)(exBm.OffsetY + shiftY));
							if (useLocalPalette)
							{
								bw.Write(br.ReadInt16());
								bw.Write(br.ReadInt16());
								bw.Write((byte)(br.ReadByte() + 128 + 7));
								//br.ReadByte();  bw.Write((byte)0xC7);
								bw.Write(localPalette);
							}
							//Читаем до конца. Терминатор не считываем
							bw.Write(br.ReadBytes((Int32)(ms.Length - ms.Position - 1)));
						}
					}
				}
				bw.Write(';'); // Терминатор
			}
		}

        // Анимированный GIF разбиавается покадрово на неанимированные, каждый из которых преобразуется в ExtendedBitmap.
		public static List<ExtendedBitmap> ConvertGifToBitmaps
			(string fileName, int foreshrteningNum, out bool containsLocalPalette)
		{
			List<ExtendedBitmap> result = new List<ExtendedBitmap>();
			bool itIsStiEditFile = false;
			containsLocalPalette = false;
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
						byte[] appData = new byte[16];
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
											itIsStiEditFile = true;
											br.ReadBytes(exLength - 8);
											if (!shiftsRead)
											{
												br.ReadByte();
												shiftX = br.ReadInt16();
												shiftY = br.ReadInt16();
												shiftsRead = true;
											}
											else
												appData = br.ReadBytes(br.ReadByte());
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
                                        // если третий ? бит = 1, предудущий кадр копируется в последующий
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

						Int16 offsetX = (Int16)(BitConverter.ToInt16(br.ReadBytes(2), 0) - shiftX);
						Int16 offsetY = (Int16)(BitConverter.ToInt16(br.ReadBytes(2), 0) - shiftY);

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
						image.Add(br.ReadByte()); // кол-во цветов
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
						Bitmap bm = new Bitmap(new MemoryStream(image.ToArray()));

						ExtendedBitmap exBm = new ExtendedBitmap(bm, offsetX, offsetY);
						exBm.ApplicationData = appData;
                        exBm.TransparentColorIndex = _transparentColorIndex;
                        exBm.BehaviorFlags = _imageBehaviorFlags;
						result.Add(exBm);
						separator = br.ReadByte();
					}
				}
			}
			if (!itIsStiEditFile && foreshrteningNum != 0)
			{
				int length = result.Count / foreshrteningNum;
                for (int i = 0; i < result.Count; i += length)
				{
					result[i].ApplicationData =
							new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, (byte)length, 2, 0, 0, 0, 0, 0, 0 };
					for (int j = i + 1; j < length; j++)
						result[j].ApplicationData = new byte[16];
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
