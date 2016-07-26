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

        public static byte[] CreateBmpHeader(
            Int32 imageLength, Int32 width, Int32 height, Int32 bitPerPixel, Int32 paletteLength)
        {
            return BMP.CreateBmpHeader(
                (UInt32)imageLength, (UInt32)width, (UInt32)height, (UInt16)bitPerPixel, (UInt32)paletteLength); 
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

        public static Bitmap Convert32argbToIndexed(Bitmap argbFile, ColorPalette palette)
        {
            // if palette = null, create palette. 
            // if file contains more than 256 different colors throw excepiton
            int _headerSize = 54;
            int _colorNum = 256;
            int _paletteSize = 256 * 4;
            int _width = argbFile.Width;
            int _height = argbFile.Height;
            int _imageDataSize = _width * _height;

            byte[] _indexedBmData = new byte[_headerSize + _paletteSize + _imageDataSize];
            bool _isNewPalette = false;
            if(palette == null)
            {
                byte[] _paletteData = new byte[_paletteSize];
                palette = BMP.getPalette(_paletteData);
                _isNewPalette = true;
            }

            int _paletteIndex = 0;
            
            byte[] _argbBmData = new byte[_headerSize + _imageDataSize * 4];

            Bitmap _indexedBm = null;

            using (MemoryStream _argbStream = new MemoryStream(_argbBmData))
            {
                argbFile.Save(_argbStream, ImageFormat.Bmp);
                using (MemoryStream _indexedStream = new MemoryStream(_indexedBmData))
                {
                    byte[] _header = BMP.CreateBmpHeader(
                        _imageDataSize, argbFile.Width, argbFile.Height, 8, _colorNum);
                    _indexedStream.Write(_header, 0, _headerSize);

                    _indexedStream.Seek(_paletteSize, SeekOrigin.Current);
                    _argbStream.Seek(_headerSize, SeekOrigin.Begin);

                    for (int i = 0; i < _height; i++)
                    {
                        for (int j = 0; j < _width; j++)
                        {
                            // argb data is reversed
                            // TODO GetPixel works very slowly. Use stream instead.
                            //Color _c1 = argbFile.GetPixel(_width - j - 1, _height - i - 1);
                            int _b = _argbStream.ReadByte();
                            int _g = _argbStream.ReadByte();
                            int _r = _argbStream.ReadByte();
                            int _a = _argbStream.ReadByte();
                            Color _c = Color.FromArgb(_a, _r, _g, _b);

                            int _ci = Array.IndexOf(palette.Entries, _c);
                            if (_ci >= 0)
                            {
                                _indexedStream.WriteByte((byte)_ci);
                            }
                            else
                            {
                                if (_isNewPalette)
                                {
                                    if (_paletteIndex < _colorNum)
                                    {
                                        palette.Entries[_paletteIndex] = _c;
                                        _paletteIndex++;
                                    }
                                    else
                                    {
                                        string _msg = String.Format(
                                            "There are more than {0} colors in input file.", _colorNum);
                                        throw new ArgumentException(_msg, "argbFile");
                                    }
                                }
                                else
                                {
                                    string _msg = String.Format(
                                        "Color R={0} G={1} B={2} is not found in palette.", _c.R, _c.G, _c.B);
                                    throw new ArgumentException(_msg, "palette");
                                }
                            }
                        }
                    }

                    _indexedBm = new Bitmap(_indexedStream);
                    _indexedBm.Palette = palette;
                }
            }

            return _indexedBm;
        }

        public static Bitmap ConvertIndexedToArgb32(Bitmap indexedFile)
        {
            int _headerSize = 54;
            int _colorNum = 256;
            int _paletteSize = _colorNum * 4;
            int _height = indexedFile.Height;
            int _width = indexedFile.Width;
            int _rem = (4 - _width % 4) % 4;
            _width = _width + _rem;

            int _imageDataSize = _width * _height;

            byte[] _indexedBmData = new byte[_headerSize + _paletteSize + _imageDataSize];
            byte[] _argbBmData = new byte[_headerSize + _imageDataSize * 4];

            Bitmap _argbBm = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
            using (MemoryStream _argbStream = new MemoryStream(_argbBmData))
            {
                _argbBm.Save(_argbStream, ImageFormat.Bmp);
                using (MemoryStream _indexedStream = new MemoryStream(_indexedBmData))
                {
                    indexedFile.Save(_indexedStream, ImageFormat.Bmp);
                    _indexedStream.Seek(_headerSize + _paletteSize, SeekOrigin.Begin);
                    _argbStream.Seek(_headerSize, SeekOrigin.Begin);

                    for (int i = 0; i < _height; i++)
                    {
                        for (int j = 0; j < _width; j++)
                        {
                            int _ci = 0;
                            int _index = _indexedStream.ReadByte(); 
                            if (j < indexedFile.Width)
                            {
                                _ci = _index;
                            }
                            Color _c = indexedFile.Palette.Entries[_ci];

                            // argb data is reversed

                            _argbStream.WriteByte((byte)_c.B);
                            _argbStream.WriteByte((byte)_c.G);
                            _argbStream.WriteByte((byte)_c.R);
                            _argbStream.WriteByte((byte)_c.A);
                        }
                    }
                }

                _argbBm = new Bitmap(_argbStream);
            }

            // some unexplained, if don't do it file is incorrect
            _argbBm.RotateFlip(RotateFlipType.RotateNoneFlipX);
            _argbBm.RotateFlip(RotateFlipType.RotateNoneFlipX);
            /////////////////////////////////////////////////////

            return _argbBm;
        }
    }
}
