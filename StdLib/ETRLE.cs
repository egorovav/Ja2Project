using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
//using System.Windows.Forms;

namespace StiLib
{
	public class IndexedConverter
	{
		public static byte[] GlobalPalette = new byte[768];
 
		const UInt32 IMAGE_ALLIMAGEDATA = 0x000C;
		const UInt32 IMAGE_ALLDATA = 0x001C;
		const UInt32 IMAGE_COMPRESSED = 0x0001;
		const UInt32 IMAGE_TRLECOMPRESSED = 0x0002;
		const UInt32 IMAGE_PALETTE = 0x0004;
		const UInt32 IMAGE_BITMAPDATA = 0x0008;
		const UInt32 IMAGE_APPDATA = 0x0010;

		const UInt32 STCI_PALETTE_ELEMENT_SIZE = 3;
		const UInt32 STCI_8BIT_PALETTE_SIZE = 768;
		const UInt32 STCI_SUBIMAGE_SIZE = 16;

		const UInt32 STCI_ETRLE_COMPRESSED = 0x0020;
		const UInt32 STCI_ZLIB_COMPRESSED = 0x0010;

		public static ETRLEData LoadIndexedImageData(StciData data)
		{
			UInt32 uiFileSectionSize;
			UInt32 fContents = IMAGE_ALLDATA; //IMAGE_ALLIMAGEDATA;
			UInt32 numberOfColours = data._Indexed.UiNumberOfColours;
			UInt32 numberOfSubImages = data._Indexed.UsNumberOfSubImages;
			byte[] imageData = data.imageData;

			//    Color[] palette = new Color[numberOfColours];
			byte[] palette = new byte[numberOfColours * 4];
			ETRLEData result = new ETRLEData(palette);

			using (BinaryReader dataReader =
					new BinaryReader(new MemoryStream(imageData, 0, imageData.Length)))
			{
				if ((fContents & IMAGE_PALETTE) != 0)
				{
					// Allocate memory for reading in the palette
					uiFileSectionSize = numberOfColours * STCI_PALETTE_ELEMENT_SIZE;

					for (Int32 i = 0; i < numberOfColours; i++)
					{
						palette[i * 4 + 3] = 0;
						byte b = dataReader.ReadByte();
						palette[i * 4 + 2] = b;
						//if (IndexedConverter.GlobalPalette[i * 3 + 2] == 0)
						//  IndexedConverter.GlobalPalette[i * 3 + 2] = b;
						b = dataReader.ReadByte();
						palette[i * 4 + 1] = b;
						//if (IndexedConverter.GlobalPalette[i * 3 + 1] == 0)
						//  IndexedConverter.GlobalPalette[i * 3 + 1] = b;
						b = dataReader.ReadByte();
						palette[i * 4 + 0] = b;
						//if (IndexedConverter.GlobalPalette[i * 3 + 0] == 0)
						//  IndexedConverter.GlobalPalette[i * 3 + 0] = b;

						// Color.FromArgb(dataReader.ReadByte(), dataReader.ReadByte(), dataReader.ReadByte());
					}
					//hImage->fFlags |= IMAGE_PALETTE;
				}
				if ((fContents & IMAGE_BITMAPDATA) != 0)
				{
					if ((data.FFlags & STCI_ETRLE_COMPRESSED) != 0)
					{
						result.Compressed = true;
						// load data for the subimage (object) structures
						result.usNumberOfObjects = (UInt16)numberOfSubImages;
						uiFileSectionSize = numberOfSubImages * STCI_SUBIMAGE_SIZE;

						ETRLEObject[] objects = new ETRLEObject[numberOfSubImages];
						for (Int32 i = 0; i < numberOfSubImages; i++)
						{
							objects[i] = new ETRLEObject
							(
									dataReader.ReadUInt32(),
									dataReader.ReadUInt32(),
									dataReader.ReadInt16(),
									dataReader.ReadInt16(),
									dataReader.ReadUInt16(),
									dataReader.ReadUInt16()
							);
						}
						result.uiSizePixData = data.UiStoredSize;
						result.ETRLEObjects = objects;
						//  hImage->fFlags |= IMAGE_TRLECOMPRESSED;

					}
					result.imageData = dataReader.ReadBytes((Int32)data.UiStoredSize);
					//  hImage->fFlags |= IMAGE_BITMAPDATA;
				}
				else if ((fContents & IMAGE_APPDATA) != 0) // then there's a point in seeking ahead
				{
					dataReader.BaseStream.Seek(data.UiStoredSize, SeekOrigin.Current);
				}

				if ((fContents & IMAGE_APPDATA) != 0 && data.UiAppDataSize > 0)
				{
					// load application-specific data
					result.pAppData = dataReader.ReadBytes((Int32)data.UiAppDataSize);
					result.uiAppDataSize = data.UiAppDataSize;
					//  hImage->fFlags |= IMAGE_APPDATA;
				}
				else
				{
					result.pAppData = null;
					result.uiAppDataSize = 0;
				}
			}
			result.HeaderData = data;
			return result;
		}

		static List<ExtendedBitmap> ConvertEtrleDataToBitmaps(ETRLEData data)
		{
			return ConvertEtrleDataToBitmaps((UInt16)0, data.usNumberOfObjects, data);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="_foreshorteningIndex">Номер загружаетого ракурса. Если надо загрузить все ракурсы предаётся 0.</param>
        /// <returns></returns>
		public static List<ExtendedBitmap> ConvertEtrleDataToBitmaps(ETRLEData data, int _foreshorteningIndex)
		{
			byte[] appData = data.pAppData;
			if (_foreshorteningIndex == 0 || appData == null)
				return ConvertEtrleDataToBitmaps(data);

			UInt16 startIndex = 0;
			UInt16 length = 0;
			int fsCount = 0;
			for (UInt16 i = 9; i < appData.Length; i += 16)
			{
				if (appData[i] != 0)
				{
					fsCount++;
					if (fsCount == _foreshorteningIndex)
					{
						do
						{
							length++;
							i += 16;
						}
						while (i < appData.Length && appData[i] == 0);
						break;
					}
				}
				startIndex++;
			}
			return ConvertEtrleDataToBitmaps(startIndex, length, data);
		}

		static List<ExtendedBitmap> ConvertEtrleDataToBitmaps
				(UInt16 startIndex, UInt16 length, ETRLEData data)
		{
			byte[] palette = data.palette;
			List<ExtendedBitmap> bitmaps = new List<ExtendedBitmap>();
			for (int objIndex = startIndex; objIndex < startIndex + length; objIndex++)
			{
				if (objIndex >= data.ETRLEObjects.Length)
					return bitmaps;

				ETRLEObject obj = data.ETRLEObjects[objIndex];

				byte[] file = new byte[obj.uiDataLength];
				Array.Copy(data.imageData, obj.uiDataOffset, file, 0, obj.uiDataLength);
				Int32 remainder = (4 - obj.usWidth % 4) % 4;

				List<byte> argbFile = new List<byte>();

				using (BinaryReader br = new BinaryReader(new MemoryStream(file)))
				{
					while (br.BaseStream.Position < br.BaseStream.Length)
					{
						Int32 firstSectionByte = br.ReadByte();
						if (firstSectionByte == 0)
						{
							for (int i = 0; i < remainder; i++)
							{
								argbFile.Add(0);
							}
						}
						else if (firstSectionByte > 127)
						{
							for (int j = 0; j < firstSectionByte - 128; j++)
								argbFile.Add(0);

						}
						else
						{
							for (int j = 0; j < firstSectionByte; j++)
							{
								byte index = br.ReadByte();
								argbFile.Add(index);
							}
						}
					}
				}
				List<byte> bmpFile = new List<byte>();
				bmpFile.AddRange(BMP.CreateBmpHeader
						(
								(UInt32)argbFile.Count,
								(UInt32)(obj.usWidth),
								(UInt32)obj.usHeight,
								(UInt16)8,
								(UInt32)palette.Length
						));
				bmpFile.AddRange(palette);
				bmpFile.AddRange(argbFile);

				byte[] bFile = bmpFile.ToArray();
				using (MemoryStream argbStream = new MemoryStream(bFile))
				{
					Bitmap bm;
					try
					{
						bm = new Bitmap(argbStream);
					}
					catch
					{
						bm = new Bitmap(obj.usWidth, obj.usHeight);
					}
					bm.RotateFlip(RotateFlipType.RotateNoneFlipY);
					ExtendedBitmap exBm = new ExtendedBitmap(bm, obj.sOffsetX, obj.sOffsetY);
					if (data.pAppData != null)
					{
						exBm.ApplicationData = new byte[16];
						Array.Copy(data.pAppData, objIndex * 16, exBm.ApplicationData, 0, 16);
					}
					bitmaps.Add(exBm);
				}
			}
			return bitmaps;
		}

        public static void ConvertBitmapsToEtrleData(List<ExtendedBitmap> bitmaps, string fileName)
        {
            ConvertBitmapsToEtrleData(bitmaps, fileName, false);
        }

		public static void ConvertBitmapsToEtrleData(List<ExtendedBitmap> bitmaps, string fileName, bool isTrim)
		{
			ETRLEData data = ConvertBitmapToETRLEObjects(bitmaps, isTrim);
			List<byte> appData = new List<byte>();
			UInt32 flags = 40;
			foreach (ExtendedBitmap exBm in bitmaps)
			{
				if (exBm.ApplicationData != null)
				{
					flags = 41;
					appData.AddRange(exBm.ApplicationData);
				}
			}

			using (BinaryWriter bw =
					new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
			{
				bw.Write(Convert.ToByte('S'));
				bw.Write(Convert.ToByte('T'));
				bw.Write(Convert.ToByte('C'));
				bw.Write(Convert.ToByte('I'));
				bw.Write(data.uiSizePixData);
				bw.Write(data.uiSizePixData);
				bw.Write((UInt32)0);
				bw.Write(flags);
				bw.Write((UInt16)768);
				bw.Write((UInt16)1024);
				//
				bw.Write((UInt32)256);
				bw.Write(data.usNumberOfObjects);
				bw.Write((byte)8);
				bw.Write((byte)8);
				bw.Write((byte)8);
				bw.Write(new byte[11]);
				//
				bw.Write((byte)8);
				bw.Write(new byte[3]);
				bw.Write((UInt16)appData.Count);
				bw.Write(new byte[14]);
				bw.Write(data.palette);
				foreach (ETRLEObject obj in data.ETRLEObjects)
				{
					bw.Write(obj.uiDataOffset);
					bw.Write(obj.uiDataLength);
					bw.Write(obj.sOffsetX);
					bw.Write(obj.sOffsetY);
					bw.Write(obj.usHeight);
					bw.Write(obj.usWidth);
				}
				bw.Write(data.imageData);
				bw.Write(appData.ToArray());
			}
		}

        static ETRLEData ConvertBitmapToETRLEObjects(List<ExtendedBitmap> bitmaps)
        {
            return ConvertBitmapToETRLEObjects(bitmaps, false);
        }

		static ETRLEData ConvertBitmapToETRLEObjects(List<ExtendedBitmap> bitmaps, bool isTrim)
		{
			UInt32 dataOffset = 0;
			List<ETRLEObject> objects = new List<ETRLEObject>();
			ETRLEData etrleData = null;
			List<byte> imageData = new List<byte>();

            byte[] _previousImageFile = null;
            int _previousImageWidth = 0;
            int _previousImageHeight = 0;
            int _previousRemainder = 0;
            short _previousOffsetX = 0;
            short _previousOffsetY = 0;
            
			foreach (ExtendedBitmap bitmap in bitmaps)
			{
                if (isTrim)
                    bitmap.Trim();

				Int32 paletteLength = bitmap.Bm.Palette.Entries.Length * 4;
				Int32 height = bitmap.Bm.Height;
				Int32 width = bitmap.Bm.Width;
				Int32 remainder = (4 - width % 4) % 4;
				byte[] bmFile = new byte[(width + remainder) * height + 1024 + 54];

				Bitmap tempBitmap = (Bitmap)bitmap.Bm.Clone();
				tempBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
				using (MemoryStream ms = new MemoryStream(bmFile))
				{
					tempBitmap.Save(ms, ImageFormat.Bmp);
				}

				byte[] palette = new byte[1024];
				Array.Copy(bmFile, 54, palette, 0, paletteLength);
				List<byte> rgbPaletteList = new List<byte>();
				for (int i = 0; i < 1024; i += 4)
				{
					rgbPaletteList.Add(palette[i + 2]);
					rgbPaletteList.Add(palette[i + 1]);
					rgbPaletteList.Add(palette[i]);
				}
				byte[] rgbPalette = rgbPaletteList.ToArray();
				if (etrleData == null)
					etrleData = new ETRLEData(rgbPalette);

				byte[] imageFile = new byte[(width + remainder) * height];
				Array.Copy(bmFile, 54 + paletteLength, imageFile, 0, imageFile.Length);

                short _offsetX = bitmap.OffsetX;
                short _offsetY = bitmap.OffsetY;

                if(_previousImageFile != null)
                {
                    for(int i = 0; i < height; i++)
                    {
                        int _offsetPrev = 
                            (_previousImageWidth + _previousRemainder) * (bitmap.OffsetY + i) + bitmap.OffsetX;
                        int _offsetCurr = (width + remainder) * i;

                        Array.Copy(imageFile, _offsetCurr, _previousImageFile, _offsetPrev, width);
                    }

                    imageFile = _previousImageFile;
                    width = _previousImageWidth;
                    height = _previousImageHeight;
                    remainder = _previousRemainder;
                    _offsetX = _previousOffsetX;
                    _offsetY = _previousOffsetY;
                }


                if ((bitmap.BehaviorFlags >> 2) % 2 == 1)
                {
                    _previousImageFile = imageFile;
                    _previousImageWidth = width;
                    _previousImageHeight = height;
                    _previousRemainder = remainder;
                    _previousOffsetX = _offsetX;
                    _previousOffsetY = _offsetY;
                }
                else
                    _previousImageFile = null;

				List<byte> compressedImage = new List<byte>();
				byte zeroCount = 0;
				byte nonZeroCount = 0;
				int rowPosition = 0;
				using (BinaryReader br = new BinaryReader(new MemoryStream(imageFile)))
				{
					List<byte> buffer = new List<byte>();
					while (br.BaseStream.Position < br.BaseStream.Length + 1)
					{
						if (rowPosition != 0 && rowPosition % width == 0)
						{
							if (zeroCount != 0)
							{
								compressedImage.Add((byte)(zeroCount + 128));
								zeroCount = 0;
							}
							if (nonZeroCount != 0)
							{
								compressedImage.Add(nonZeroCount);
								compressedImage.AddRange(buffer);
								buffer = new List<byte>();
								nonZeroCount = 0;
							}
							compressedImage.Add(0);
							br.BaseStream.Position += remainder;
							if (br.BaseStream.Position >= br.BaseStream.Length)
								break;
						}

						if (zeroCount == 127)
						{
							compressedImage.Add((byte)(zeroCount + 128));
							zeroCount = 0;
						}
						if (nonZeroCount == 127)
						{
							compressedImage.Add(nonZeroCount);
							compressedImage.AddRange(buffer);
							buffer = new List<byte>();
							nonZeroCount = 0;
						}

						byte next = br.ReadByte();
						if (next == 0)
						{
							if (zeroCount == 0 && nonZeroCount != 0)
							{
								compressedImage.Add(nonZeroCount);
								compressedImage.AddRange(buffer);
								buffer = new List<byte>();
								nonZeroCount = 0;
							}
							zeroCount++;
						}
						else
						{
							if (nonZeroCount == 0 && zeroCount != 0)
							{
								compressedImage.Add((byte)(zeroCount + 128));
								zeroCount = 0;
							}
							buffer.Add(next);
							nonZeroCount++;
						}
						rowPosition++;
					}
				}
				imageData.AddRange(compressedImage);
				UInt32 dataLength = (UInt32)compressedImage.Count;
				ETRLEObject obj = new ETRLEObject
                        (dataOffset, dataLength, _offsetX, _offsetY, (UInt16)height, (UInt16)width);
				objects.Add(obj);
				dataOffset += dataLength;
			}
			etrleData.imageData = imageData.ToArray();
			etrleData.ETRLEObjects = objects.ToArray();
			etrleData.usNumberOfObjects = (UInt16)bitmaps.Count;
			etrleData.uiSizePixData = (UInt32)imageData.Count;
			return etrleData;
		}

	}
	// TRLE subimage structure, mirroring that of ST(C)I
	public class ETRLEObject
	{
		public ETRLEObject
		(
				UInt32 uiDataOffset,
				UInt32 uiDataLength,
				short sOffsetX,
				short sOffsetY,
				UInt16 usHeight,
				UInt16 usWidth
		)
		{
			this.uiDataOffset = uiDataOffset;
			this.uiDataLength = uiDataLength;
			this.sOffsetX = sOffsetX;
			this.sOffsetY = sOffsetY;
			this.usHeight = usHeight;
			this.usWidth = usWidth;
		}
		public readonly UInt32 uiDataOffset; // 4
		public readonly UInt32 uiDataLength; // 4
		public readonly short sOffsetX;    // 2
		public readonly short sOffsetY;    // 2
		public readonly UInt16 usHeight;   // 2
		public readonly UInt16 usWidth;    // 2
		// Итого: 16
	}

	public class ETRLEData
	{
		public ETRLEData
		(
				byte[] palette
		)
		{
			this.palette = palette;
		}
		public StciData HeaderData;
		public string FileName { get { return HeaderData.FileName; } }
		public UInt32 uiAppDataSize;
		public UInt32 uiSizePixData;
		public ETRLEObject[] ETRLEObjects;
		public UInt16 usNumberOfObjects;
		public byte[] imageData;
		public byte[] pAppData;
		public string AppData
		{
			get
			{
				if (pAppData == null)
					return "";
				return arrayToString(pAppData, false);
			}
		}
		public string OffsetX
		{
			get
			{
				int[] array = Array.ConvertAll<ETRLEObject, int>(ETRLEObjects,
						delegate(ETRLEObject obj) { return Math.Abs(obj.sOffsetX) + obj.usWidth; });
				StringBuilder sb = new StringBuilder();
				Int32 max = Int16.MinValue;
				Int32 min = Int16.MaxValue;
				for (Int32 i = 0; i < array.Length; i++)
				{
					Int32 obj = array[i];
					if (max.CompareTo(obj) < 0)
						max = obj;
					if (min.CompareTo(obj) > 0)
						min = obj;
					sb = new StringBuilder(String.Format("MAX = {0}, MIN = {1}", max.ToString(), min.ToString()));
				}
				return sb.ToString();
			}
		}
		public string OffsetY
		{
			get
			{
				int[] array = Array.ConvertAll<ETRLEObject, int>(ETRLEObjects,
						delegate(ETRLEObject obj) { return Math.Abs(obj.sOffsetY) + obj.usHeight; });
				StringBuilder sb = new StringBuilder();
				Int32 max = Int16.MinValue;
				Int32 min = Int16.MaxValue;
				for (Int32 i = 0; i < array.Length; i++)
				{
					Int32 obj = array[i];
					if (max.CompareTo(obj) < 0)
						max = obj;
					if (min.CompareTo(obj) > 0)
						min = obj;
					sb = new StringBuilder(String.Format("MAX = {0}, MIN = {1}", max.ToString(), min.ToString()));
				}
				return sb.ToString();
			}
		}
		public byte[] palette;
		public bool Compressed;

		private string arrayToString(Array array, bool extremum)
		{
			StringBuilder sb = new StringBuilder();
			Int32 linesCount = 0;
			for (Int32 i = 0; i < array.Length; i++)
			{
				object obj = array.GetValue(i);
				if (obj.ToString() != "0")
				{
					if (linesCount != 0)
					{
						sb.Append("|" + linesCount.ToString() + "|");
						linesCount = 0;
					}
					sb.Append(obj.ToString());
					sb.Append(", ");
				}
				if (i != 0 && i % 16 == 0)
					linesCount++;
			}
			sb.Append("|" + (linesCount + 1).ToString() + "|");
			return sb.ToString();
		}
	}

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

        private byte FBehaviorFlags = 0;
        public byte BehaviorFlags
        {
            get { return this.FBehaviorFlags; }
            set { this.FBehaviorFlags = value; }
        }


		public Bitmap Bm;
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
            newExBm.BehaviorFlags = this.BehaviorFlags;
			return newExBm;
		}

        // Trim background pixels
        public void Trim()
        {
            Color bc = this.Bm.Palette.Entries[0];
            int _top = 0;
            int _left = this.Bm.Width;

            for (int i = 0; i < this.Bm.Height; i++)
            {
                for (int j = 0; j < this.Bm.Width; j++)
                {
                    Color c = this.Bm.GetPixel(j, i);
                    if (c != bc)
                    {
                        if (_top == 0)
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

            this.Bm = this.Bm.Clone(new Rectangle(_left, _top, _width, _height), PixelFormat.Format8bppIndexed);
            this.OffsetX -= (short)_left;
            this.OffsetY -= (short)_top;
        }
	}
}
