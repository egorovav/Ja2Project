using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Ja2Data
{
    public class PcxObject
    {
        public PcxObject(string aFileName)
        {
            this.FFileName = aFileName;
        }

        private const int HeaderSize = 128;
        private const int PaletteSize = NumberOfColors * 3;
        private const int NumberOfColors = 256;

        private const byte PcxBF = 191;
        private const byte Pcx3F = 63;

        private string FFileName;

        private PcxHeader FHeader;

        private byte[] pPcxBuffer;
        private byte[] ubPalette = new byte[PaletteSize];

        public byte[] ImageData
        {
            get;
            protected set;
        }

        private UInt16 usWidth;
        public int Width
        {
            get { return this.usWidth; }
        }

        private UInt16 usHeight;
        public int Height
        {
            get { return this.usHeight; }
        }


        private UInt32 uiBufferSize;
        private PcxFlags usPcxFlags;

        private StciColor[] FColorPalette;
        public StciColor[] ColorPalette
        {
            get
            {
                if (this.FColorPalette == null)
                {
                    this.FColorPalette = new StciColor[NumberOfColors];
                    for (int i = 0; i < NumberOfColors; i++)
                    {
                        this.FColorPalette[i] =
                            new StciColor(this.ubPalette[i * 3], this.ubPalette[i * 3 + 1], this.ubPalette[i * 3 + 2]);
                    }
                }

                return this.FColorPalette;
            }
        }

        public void Load()
        {
            if (!File.Exists(this.FFileName))
                return;

            using (FileStream _fs = new FileStream(this.FFileName, FileMode.Open))
                this.Load(_fs);
        }

        public void Load(Stream aStream)
        {

            long _bufferSize = aStream.Length - HeaderSize - PaletteSize;
            this.pPcxBuffer = new byte[_bufferSize];
            this.uiBufferSize = (UInt32)_bufferSize;

            Deserializer _deserializer = new Deserializer(aStream);
            PcxHeader _header = (PcxHeader)_deserializer.Deserialize(typeof(PcxHeader));
            this.FHeader = _header;

            if (_header.ubManufacturer != 10 || _header.ubEncoding != 1)
                throw new Exception("Invalid PCX format.");

            if (_header.ubBitsPerPixel == 8)
                this.usPcxFlags |= PcxFlags.PCX_256COLOR;
            else
                this.usPcxFlags = 0;

            this.usWidth = (UInt16)(1 + (_header.usRight - _header.usLeft));
            this.usHeight = (UInt16)(1 + (_header.usBottom - _header.usTop));

            aStream.Read(this.pPcxBuffer, 0, this.pPcxBuffer.Length);
            aStream.Read(this.ubPalette, 0, PaletteSize);

            this.BlitPcxToBuffer(false);
        }

        private bool BlitPcxToBuffer(bool aTransparent)
        {
            byte ubCurrentByte = 0;
            int uiImageSize = this.usWidth * this.usHeight;
            PcxFlags ubMode = PcxFlags.PCX_NORMAL;
            int uiOffset = 0;
            byte ubRepCount = 0;

            this.ImageData = new byte[uiImageSize];

            if (aTransparent)
            {
                for (int uiIndex = 0; uiIndex < uiImageSize; uiIndex++)
                {
                    if (ubMode == PcxFlags.PCX_NORMAL)
                    {
                        ubCurrentByte = this.pPcxBuffer[uiOffset++];
                        if (ubCurrentByte > PcxBF)
                        {
                            ubRepCount = (byte)(ubCurrentByte & Pcx3F);
                            ubCurrentByte = this.pPcxBuffer[uiOffset++];
                            if (--ubRepCount > 0)
                                ubMode = PcxFlags.PCX_RLE;
                        }
                    }
                    else
                    {
                        if (--ubRepCount > 0)
                            ubMode = PcxFlags.PCX_NORMAL;
                    }

                    if (ubCurrentByte != 0)
                        this.ImageData[uiIndex] = ubCurrentByte;
                }
            }
            else
            {
                for (int uiIndex = 0; uiIndex < uiImageSize; uiIndex++)
                {
                    if (ubMode == PcxFlags.PCX_NORMAL)
                    {
                        ubCurrentByte = this.pPcxBuffer[uiOffset++];
                        if (ubCurrentByte > PcxBF)
                        {
                            ubRepCount = (byte)(ubCurrentByte & Pcx3F);
                            ubCurrentByte = this.pPcxBuffer[uiOffset++];
                            if (--ubRepCount > 0)
                                ubMode = PcxFlags.PCX_RLE;
                        }
                    }
                    else
                    {
                        if (--ubRepCount == 0)
                            ubMode = PcxFlags.PCX_NORMAL;
                    }

                    this.ImageData[uiIndex] = ubCurrentByte;
                }
            }

            return true;
        }

        private bool BlitPcxToBuffer(ushort aBufferWidth, ushort aBufferHeight, ushort aX, ushort aY, bool aTransparent)
        {
            byte ubRepCount = 0;
            int usMaxX, usMaxY;
            int uiImageSize;
            byte ubCurrentByte = 0;
            PcxFlags ubMode = PcxFlags.PCX_NORMAL; ;
            ushort usCurrentX = aX; 
            ushort usCurrentY = aY;
            int uiOffset = 0;
            int uiNextLineOffset, uiStartOffset, uiCurrentOffset;

            if (this.usWidth + aX >= aBufferWidth)
            {
                this.usPcxFlags |= PcxFlags.PCX_X_CLIPPING;
                usMaxX = aBufferWidth - 1;
            }
            else
                usMaxX = this.usWidth + aX;

            if (this.usHeight + aY >= aBufferHeight)
            {
                this.usPcxFlags |= PcxFlags.PCX_Y_CLIPPING;
                uiImageSize = this.usWidth * (aBufferHeight - aY);
                usMaxY = aBufferHeight - 1;
            }
            else
            {
                uiImageSize = this.usWidth * this.usHeight;
                usMaxY = this.usHeight + aY;
            }

            this.ImageData = new byte[uiImageSize];

            if (aTransparent)
            {
                for (int uiIndex = 0; uiIndex < uiImageSize; uiIndex++)
                {
                    if (ubMode == PcxFlags.PCX_NORMAL)
                    {
                        ubCurrentByte = this.pPcxBuffer[uiOffset++];
                        if (ubCurrentByte == PcxBF)
                        {
                            ubRepCount = (byte)(ubCurrentByte & Pcx3F);
                            ubCurrentByte = this.pPcxBuffer[uiOffset++];
                            if (--ubRepCount > 0)
                                ubMode = PcxFlags.PCX_RLE;
                        }
                    }
                    else
                    {
                        if (--ubRepCount == 0)
                            ubMode = PcxFlags.PCX_NORMAL;
                    }

                    if (ubCurrentByte != 0)
                        this.ImageData[usCurrentY * aBufferWidth + usCurrentX] = ubCurrentByte;

                    usCurrentX++;
                    if (usCurrentX > usMaxX)
                    {
                        usCurrentX = aX;
                        usCurrentY++;
                    }
                }
            }
            else
            {
                uiStartOffset = (usCurrentY * aBufferWidth) + usCurrentX;
                uiNextLineOffset = uiStartOffset + aBufferWidth;
                uiCurrentOffset = uiStartOffset;

                for (int uiIndex = 0; uiIndex < uiImageSize; uiIndex++)
                {

                    if (ubMode == PcxFlags.PCX_NORMAL)
                    {
                        ubCurrentByte = this.pPcxBuffer[uiOffset++];
                        if (ubCurrentByte > 0x0BF)
                        {
                            ubRepCount = (byte)(ubCurrentByte & Pcx3F);
                            ubCurrentByte = this.pPcxBuffer[uiOffset++];
                            if (--ubRepCount > 0)
                                ubMode = PcxFlags.PCX_RLE;
                        }
                    }
                    else
                    {
                        if (--ubRepCount == 0)
                            ubMode = PcxFlags.PCX_NORMAL;
                    }

                    if (usCurrentX < usMaxX)
                    {
                        this.ImageData[uiCurrentOffset] = ubCurrentByte;
                        uiCurrentOffset++;
                        usCurrentX++;
                    }
                    else
                    {
                        if ((uiCurrentOffset + 1) < uiNextLineOffset)
                        {
                            uiCurrentOffset++;
                        }
                        else
                        {
                            usCurrentX = aX;
                            usCurrentY++;
                            if (usCurrentY > usMaxY)
                            {
                                break;
                            }
                            uiStartOffset = (usCurrentY * aBufferWidth) + usCurrentX;
                            uiNextLineOffset = uiStartOffset + aBufferWidth;
                            uiCurrentOffset = uiStartOffset;
                        }
                    }
                }
            }
            return false;
        }

        public static PcxObject LoadPcx(string aFileName)
        {
            PcxObject _pcx = new PcxObject(aFileName);
            _pcx.Load();
            return _pcx;
        }

        public void BuildInfo(StringBuilder aStringBuilder)
        {
            aStringBuilder.AppendLine(String.Format("Manufacturer - {0}", this.FHeader.ubManufacturer));
            aStringBuilder.AppendLine(String.Format("Version - {0}", this.FHeader.ubVersion));
            aStringBuilder.AppendLine(String.Format("Encoding - {0}", this.FHeader.ubEncoding));

            aStringBuilder.AppendLine(String.Format("BitsPerPixel - {0}", this.FHeader.ubBitsPerPixel));
            aStringBuilder.AppendLine(String.Format("Left - {0}", this.FHeader.usLeft));
            aStringBuilder.AppendLine(String.Format("Top - {0}", this.FHeader.usTop));
            aStringBuilder.AppendLine(String.Format("Right - {0}", this.FHeader.usRight));
            aStringBuilder.AppendLine(String.Format("Bottom - {0}", this.FHeader.usBottom));
            aStringBuilder.AppendLine(String.Format("HorRez - {0}", this.FHeader.usHorRez));
            aStringBuilder.AppendLine(String.Format("VerRez - {0}", this.FHeader.usVerRez));
            aStringBuilder.AppendLine(String.Format("Reserved - {0}", this.FHeader.ubReserved));
            aStringBuilder.AppendLine(String.Format("ColorPlanes - {0}", this.FHeader.ubColorPlanes));
            aStringBuilder.AppendLine(String.Format("PaletteType - {0}", this.FHeader.usPaletteType));
            aStringBuilder.AppendLine(String.Format("Width - {0}", this.usWidth));
            aStringBuilder.AppendLine(String.Format("Height - {0}", this.usHeight));
            aStringBuilder.AppendLine(String.Format("BufferSize - {0}", this.uiBufferSize));
            aStringBuilder.AppendLine(String.Format("PcxFlags - {0}", this.usPcxFlags));
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();
            this.BuildInfo(_sb);
            return _sb.ToString();
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PcxHeader
    {
        public byte ubManufacturer;
        public byte ubVersion;
        public byte ubEncoding;
        public byte ubBitsPerPixel;
        public UInt16 usLeft; 
        public UInt16 usTop;
        public UInt16 usRight; 
        public UInt16 usBottom;
        public UInt16 usHorRez; 
        public UInt16 usVerRez;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] ubEgaPalette;
        public byte ubReserved;
        public byte ubColorPlanes;
        public UInt16 usBytesPerLine;
        public UInt16 usPaletteType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 58)]
        public byte[] ubFiller; // 128
    }

    [Flags]
    public enum PcxFlags : ushort 
    {
        PCX_NORMAL = 1,
        PCX_RLE = 2,
        PCX_256COLOR = 4,
        PCX_TRANSPARENT = 8,
        PCX_CLIPPED = 16,
        PCX_REALIZEPALETTE = 32,
        PCX_X_CLIPPING = 64,
        PCX_Y_CLIPPING = 128,
        PCX_NOTLOADED = 256
    }
}
