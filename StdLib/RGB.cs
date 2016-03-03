using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace StiLib
{
	public class RGBConverter
	{
		static byte[] convert16ARGBTo32ARGB(StciData RGBData)
		{
			byte[] RGB = RGBData.imageData;
			Int32 ARGBLength = RGB.Length * 2;
			List<byte> ARGB = new List<byte>();
			UInt16 blueMask = (UInt16)RGBData._RGB.uiBlueMask;
			UInt16 greenMask = (UInt16)RGBData._RGB.uiGreenMask;
			UInt16 redMask = (UInt16)RGBData._RGB.uiRedMask;
			UInt16 alphaMask = (UInt16)RGBData._RGB.uiAlphaMask;
			byte blueDepth = RGBData._RGB.ubBlueDepth;
			byte greenDepth = RGBData._RGB.ubGreenDepth;
			byte redDepth = RGBData._RGB.ubRedDepth;
			byte alphaDepth = RGBData._RGB.ubAlphaDepth;
			using (BinaryReader br = new BinaryReader(new MemoryStream(RGB)))
			{
				while (ARGB.Count < ARGBLength)
				{
					UInt16 word = br.ReadUInt16();
					// Такой способ декодирования должен работать для форматов
					// Format16bppArgb1555, Format16bppRgb555, Format16bppRgb565
					ARGB.Add((byte)((word & blueMask) << (8 - blueDepth)));
					ARGB.Add((byte)((word & greenMask) >> (8 - redDepth - alphaDepth)));
					ARGB.Add((byte)((word & redMask) >> (8 - alphaDepth)));
					ARGB.Add((byte)((word & alphaMask) >> 8));
				}
			}
			return ARGB.ToArray();
		}
		public static ExtendedBitmap GetBitmap(StciData RGBData)
		{
			byte[] argbFile = convert16ARGBTo32ARGB(RGBData);

			List<byte> bmpFile = new List<byte>();
			bmpFile.AddRange(BMP.CreateBmpHeader
					(
							(UInt32)argbFile.Length,
							(UInt32)RGBData.Width,
							(UInt32)RGBData.Height,
							(UInt16)32,
							(UInt32)0
					));
			bmpFile.AddRange(argbFile);

			using (MemoryStream argbStream = new MemoryStream(bmpFile.ToArray()))
			{
				Bitmap bm;
				try
				{
					bm = new Bitmap(argbStream);
				}
				catch
				{
					bm = new Bitmap(RGBData.Width, RGBData.Height);
				}
				bm.RotateFlip(RotateFlipType.Rotate180FlipX);
				return new ExtendedBitmap(bm, RGBData._RGB); //, (UInt16)RGBData.Height, (UInt16)RGBData.Width);
			}
		}

		#region Попытка сохраить файлы ширина которых в байтах не делится на 4
		//public static void ConvertBitmapToRGBData(ExtendedBitmap exBm, string fileName)
		//{
		//  UInt32 flags = 4;

		//  using (BinaryWriter bw =
		//      new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
		//  {
		//    bw.Write(Convert.ToByte('S'));
		//    bw.Write(Convert.ToByte('T'));
		//    bw.Write(Convert.ToByte('C'));
		//    bw.Write(Convert.ToByte('I'));
		//    bw.Write((UInt32)(exBm.Bm.Height * exBm.Bm.Width * 2));
		//    bw.Write((UInt32)(exBm.Bm.Height * exBm.Bm.Width * 2));
		//    bw.Write((UInt32)0);
		//    bw.Write(flags);
		//    bw.Write((UInt16)exBm.Bm.Height);
		//    bw.Write((UInt16)exBm.Bm.Width);
		//    //
		//    bw.Write((UInt32)exBm.RgbData.uiRedMask);
		//    bw.Write((UInt32)exBm.RgbData.uiGreenMask);
		//    bw.Write((UInt32)exBm.RgbData.uiBlueMask);
		//    bw.Write((UInt32)exBm.RgbData.uiAlphaMask);
		//    bw.Write((byte)exBm.RgbData.ubRedDepth);
		//    bw.Write((byte)exBm.RgbData.ubGreenDepth);
		//    bw.Write((byte)exBm.RgbData.ubBlueDepth);
		//    bw.Write((byte)exBm.RgbData.ubAlphaDepth);
		//    //
		//    bw.Write((byte)16);
		//    bw.Write(new byte[3]);
		//    bw.Write((UInt16)0);
		//    bw.Write(new byte[14]);

		//    Bitmap bm = (Bitmap)exBm.Bm.Clone();
		//    int colorDepth = 4;
		//    if (bm.PixelFormat == PixelFormat.Format24bppRgb)
		//      colorDepth = 3;

		//    Int32 height = exBm.Bm.Height;
		//    Int32 width = exBm.Bm.Width;
		//    Int32 remainder = (4 - width % 4) % 4;

		//    byte[] bmFile = new byte[(width + remainder) * height * colorDepth + 54];
		//    bm.RotateFlip(RotateFlipType.RotateNoneFlipY);
		//    using (MemoryStream ms = new MemoryStream(bmFile))
		//    {
		//      bm.Save(ms, ImageFormat.Bmp);
		//    }
		//    int columnNum = 0;
		//    for (int i = 54; i < bmFile.Length; i += colorDepth)
		//    {
		//      if (columnNum == width)
		//      {
		//        columnNum = 0;
		//        i += (remainder * colorDepth);
		//        continue;
		//      }
		//      else
		//        columnNum++;
		//      int alpha = 0;
		//      if (colorDepth == 4)
		//        alpha = ((UInt16)bmFile[i + 3]) << 8;
		//      int red = ((UInt16)bmFile[i + 2]) << (8 - exBm.RgbData.ubAlphaDepth);
		//      int green = ((UInt16)bmFile[i + 1] << (8 - exBm.RgbData.ubRedDepth - exBm.RgbData.ubAlphaDepth));
		//      int blue = ((UInt16)bmFile[i] >> (8 - exBm.RgbData.ubBlueDepth));
		//      UInt16 color = (UInt16)(alpha | red | green | blue);
		//      bw.Write(color);
		//    }
		//  }
		//}
		#endregion

		// Функция из версии 1.0.0.3 (востановлена рефлектором)
		public static void ConvertBitmapToRGBData(ExtendedBitmap exBm, string fileName)
		{
			uint num = 4;
			using (BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
			{
				writer.Write(Convert.ToByte('S'));
				writer.Write(Convert.ToByte('T'));
				writer.Write(Convert.ToByte('C'));
				writer.Write(Convert.ToByte('I'));
				writer.Write((UInt32)((exBm.Bm.Height * exBm.Bm.Width) * 2));
				writer.Write((UInt32)((exBm.Bm.Height * exBm.Bm.Width) * 2));
				writer.Write((UInt32)0);
				writer.Write(num);
				writer.Write((UInt16)exBm.Bm.Height);
				writer.Write((UInt16)exBm.Bm.Width);
				writer.Write(exBm.RgbData.uiRedMask);
				writer.Write(exBm.RgbData.uiGreenMask);
				writer.Write(exBm.RgbData.uiBlueMask);
				writer.Write(exBm.RgbData.uiAlphaMask);
				writer.Write(exBm.RgbData.ubRedDepth);
				writer.Write(exBm.RgbData.ubGreenDepth);
				writer.Write(exBm.RgbData.ubBlueDepth);
				writer.Write(exBm.RgbData.ubAlphaDepth);
				writer.Write((byte)0x10);
				writer.Write(new byte[3]);
				writer.Write((UInt16)0);
				writer.Write(new byte[14]);
				Bitmap bitmap = (Bitmap)exBm.Bm.Clone();
				int num2 = 4;
				if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
				{
					num2 = 3;
				}
				byte[] buffer = new byte[((exBm.Bm.Width * exBm.Bm.Height) * num2) + 0x36];
				bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
				using (MemoryStream stream = new MemoryStream(buffer))
				{
					bitmap.Save(stream, ImageFormat.Bmp);
				}
				RGB rgbData = exBm.RgbData;
				for (int i = 0x36; i < buffer.Length; i += num2)
				{
					int alpha = 0;
					if (num2 == 4)
					{
						alpha = buffer[i + 3] << 8;
					}
					int red = buffer[i + 2] << (8 - rgbData.ubAlphaDepth);
					int green = buffer[i + 1] << ((8 - rgbData.ubRedDepth) - rgbData.ubAlphaDepth);
					int blue = buffer[i] >> (8 - rgbData.ubBlueDepth);
					ushort num8 = (UInt16)(alpha & rgbData.uiAlphaMask | red & rgbData.uiRedMask | 
						green & rgbData.uiGreenMask | blue & rgbData.uiBlueMask);
					writer.Write(num8);
				}
			}
		}
	}
}
