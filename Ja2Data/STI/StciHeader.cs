using System;
using System.IO;
using System.Text;

namespace Ja2Data
{
    public class StciHeader
    {
        public StciHeader()
        {

        }

        public StciHeader(
            UInt32 aOriginalImageSize,
            UInt16 aImageHeight,
            UInt16 aImageWidth,
            StciRgbHeader aSubHeader)
        {
            this.FFormatId = "STCI";
            this.FOriginalImageSize = aOriginalImageSize;
            this.FCompressedImageSize = aOriginalImageSize;
            this.FFlags = StciFlags.STCI_RGB;

            this.FImageHeight = aImageHeight;
            this.FImageWidth = aImageWidth;

            this.FSubHeader = aSubHeader;

            this.FDepth = 16;
        }

        public StciHeader(
            UInt32 aCompressedImageSize,
            UInt32 aTransparentColorIndex,
            UInt32 aAppDataSize,
            StciIndexedHeader aSubHeader)
        {
            this.FFormatId = "STCI";

            this.FCompressedImageSize = aCompressedImageSize;
            this.FTransparentColorIndex = aTransparentColorIndex;
            this.FFlags = StciFlags.STCI_ETRLE_COMPRESSED | StciFlags.STCI_INDEXED;

            this.FSubHeader = aSubHeader;

            this.FImageHeight = 480;
            this.FImageWidth = 640;
            this.FOriginalImageSize = (UInt32)(this.FImageHeight * this.FImageWidth);

            this.FAppDataSize = aAppDataSize;
            this.FDepth = 8;
        }

        string FFormatId = "STCI";
        public string FormatId
        {
            get { return this.FFormatId; }
        }

        UInt32 FCompressedImageSize;
        public long CompressedImageSize
        {
            get { return this.FCompressedImageSize; }
            set { this.FCompressedImageSize = (uint)value; }
        }

        UInt32 FTransparentColorIndex;
        public long TransparentColorIndex
        {
            get { return this.FTransparentColorIndex; }
            set { this.FTransparentColorIndex = (uint)value; }
        }

        StciFlags FFlags;
        public StciFlags Flags
        {
            get { return this.FFlags; }
            set { this.FFlags = value; }
        }

        UInt16 FImageHeight;
        public int ImageHeight
        {
            get { return this.FImageHeight; }
            set { this.FImageHeight = (UInt16)value; }
        }

        UInt16 FImageWidth;
        public int ImageWidth
        {
            get { return this.FImageWidth; }
            set { this.FImageWidth = (UInt16)value; }
        }

        UInt32 FOriginalImageSize;
        public UInt32 OriginalImageSize
        {
            get { return this.FOriginalImageSize; }
            set { this.FOriginalImageSize = value; }
        }

        public bool IsIndexed
        {
            get { return (this.FFlags & StciFlags.STCI_INDEXED) != 0; }
        }

        StciSubHeader FSubHeader;
        public StciSubHeader SubHeader
        {
            get
            {
                if (this.FSubHeader == null && this.FFlags != StciFlags.None)
                {
                    if (this.IsIndexed)
                        this.FSubHeader = new StciIndexedHeader(0);
                    else
                        this.FSubHeader = new StciRgbHeader();
                }
                return this.FSubHeader;
            }
            set { this.FSubHeader = value; }
        }

        byte FDepth;
        public byte Depth
        {
            get { return this.FDepth; }
            set { this.FDepth = value; }
        }

        UInt32 FAppDataSize;
        // 15 байт не используется.

        public UInt32 AppDataSize
        {
            get { return this.FAppDataSize; }
            set { this.FAppDataSize = value; }
        }

        byte[] FUnused = new byte[15];
        public byte[] Unused
        {
            get { return this.FUnused; }
            set { this.FUnused = value; }
        }

        public void Read(Stream aInput)
        {
            using (BinaryReader _br = new BinaryReader(aInput))
            {

                Read(_br);
            }
        }

        public void Read(BinaryReader _br)
        {
            this.FFormatId = new String(_br.ReadChars(4));
            this.FOriginalImageSize = _br.ReadUInt32();
            this.FCompressedImageSize = _br.ReadUInt32();
            this.FTransparentColorIndex = _br.ReadUInt32();
            this.FFlags = (StciFlags)_br.ReadUInt32();
            this.FImageHeight = _br.ReadUInt16();
            this.FImageWidth = _br.ReadUInt16();

            this.SubHeader.Read(_br);

            this.FDepth = _br.ReadByte();
            _br.ReadBytes(3).CopyTo(this.FUnused, 0);
            this.FAppDataSize = _br.ReadUInt32();
            _br.ReadBytes(12).CopyTo(this.FUnused, 3);
        }

        public void Write(BinaryWriter aWriter)
        {
            aWriter.Write(Encoding.ASCII.GetBytes(this.FFormatId));
            aWriter.Write(this.FOriginalImageSize);
            aWriter.Write(this.FCompressedImageSize);
            aWriter.Write(this.FTransparentColorIndex);
            aWriter.Write((uint)this.FFlags);
            aWriter.Write(this.FImageHeight);
            aWriter.Write(this.FImageWidth);

            this.SubHeader.Write(aWriter);

            aWriter.Write(this.FDepth);
            aWriter.Write(new byte[3]);
            aWriter.Write(this.FAppDataSize);
            aWriter.Write(new byte[12]);
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine("STCI HEADER:");
            _sb.AppendLine(String.Format("FormatId - {0}", this.FFormatId));
            _sb.AppendLine(String.Format("OriginalImageSize - {0}", this.FOriginalImageSize));
            _sb.AppendLine(String.Format("CompressedImageSize - {0}", this.FCompressedImageSize));
            _sb.AppendLine(String.Format("TransparentColorIndex - {0}", this.FTransparentColorIndex));
            _sb.AppendLine(String.Format("Flags - {0}", this.FFlags));
            _sb.AppendLine(String.Format("ImageHeight - {0}", this.FImageHeight));
            _sb.AppendLine(String.Format("ImageWidth - {0}", this.FImageWidth));
            _sb.Append(this.SubHeader.ToString());
            _sb.AppendLine(String.Format("Depth - {0}", this.FDepth));
            _sb.AppendLine(String.Format("AppDataSize - {0}", this.FAppDataSize));
            return _sb.ToString();
        }
    }

    [Flags]
    public enum StciFlags : uint
    {
        None = 0,
        STCI_TRANSPARENT = 1,
        STCI_ALPHA = 2,
        STCI_RGB = 4,
        STCI_INDEXED = 8,
        STCI_ZLIB_COMPRESSED = 16,
        STCI_ETRLE_COMPRESSED = 32
    }
}
