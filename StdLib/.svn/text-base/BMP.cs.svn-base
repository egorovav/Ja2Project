using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace StiLib
{
    public class BMP
    {
        public static byte[] CreateBmpHeader
            (
                UInt32 imageLength, 
                UInt32 width, 
                UInt32 heigth, 
                UInt16 bitPerPixel, 
                UInt32 paletteLength
            )
        {
            UInt32 headerLength = 54;
            UInt32 bmpFileLength = imageLength + headerLength + paletteLength;
            byte[] bmpHeader = new byte[headerLength];

            using (BinaryWriter bw = new BinaryWriter(new MemoryStream(bmpHeader)))
            {

                bw.Write(Convert.ToByte('B'));
                bw.Write(Convert.ToByte('M'));
                bw.Write(bmpFileLength);
                bw.Write((UInt32)0);  // зарезервировано
                bw.Write(headerLength + paletteLength); // смещение от начала файла до картинки

                UInt32 structLength = 40;
                bw.Write(structLength);
                bw.Write(width);
                bw.Write(heigth);
                bw.Write((UInt16)1); // Число плоскостей
                bw.Write(bitPerPixel);// Бит на пиксель
                bw.Write((UInt32)0); // Алгоритм сжатия 
                bw.Write((UInt32)0); // Размер буфера сжатия 
                bw.Write((UInt32)0); // Горизонтальное разрешение 
                bw.Write((UInt32)0); // Вертикальное разрешение 
                bw.Write((UInt32)256); // Кол-во используемых цветов
                bw.Write((UInt32)256); // Кол-во важных цветов 
            }
            return bmpHeader;
        }

        public static ColorPalette MakeTransparentPalette(ColorPalette palette, Color bC)
        {
            List<byte> paletteFile = new List<byte>();
            paletteFile.AddRange(BMP.CreateBmpHeader
                (
                    (UInt32)4,
                    (UInt32)4,
                    (UInt32)1,
                    (UInt16)8,
                    (UInt32)(palette.Entries.Length * 4)
                ));
            paletteFile.AddRange(new byte[] { bC.B, bC.G, bC.R, bC.A });
            for (int i = 1; i < palette.Entries.Length; i++)
            {
                Color c = palette.Entries[i];
                paletteFile.AddRange(new byte[] { c.B, c.G, c.R, c.A });
            }
            paletteFile.AddRange(new byte[4]);
            Bitmap bmPalette;
            using (MemoryStream argbStream = new MemoryStream(paletteFile.ToArray()))
                try
                {
                    bmPalette = new Bitmap(argbStream);
                }
                catch { return null; }

            return bmPalette.Palette;
        }

        static ColorPalette getPalette(byte[] palette)
        {
            List<byte> paletteFile = new List<byte>();
            paletteFile.AddRange(BMP.CreateBmpHeader
                (
                    (UInt32)4,
                    (UInt32)4,
                    (UInt32)1,
                    (UInt16)8,
                    (UInt32)palette.Length
                ));
            paletteFile.AddRange(palette);
            paletteFile.AddRange(new byte[4]);
            Bitmap bmPalette;
            using (MemoryStream argbStream = new MemoryStream(paletteFile.ToArray()))
                try
                {
                    bmPalette = new Bitmap(argbStream);
                }
                catch { return null; }

            return bmPalette.Palette;
        }
    }
}
