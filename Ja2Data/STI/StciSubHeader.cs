using System;
using System.IO;
using System.Text;

namespace Ja2Data
{
    public class StciSubHeader
    {
        protected byte FRedDepth;
        protected byte FGreenDepth;
        protected byte FBlueDepth;

        public virtual void Read(BinaryReader aReader)
        {
            this.FRedDepth = aReader.ReadByte();
            this.FGreenDepth = aReader.ReadByte();
            this.FBlueDepth = aReader.ReadByte();
        }

        public byte RedDepth
        {
            get { return this.FRedDepth; }
            set { this.FRedDepth = value; }
        }

        public byte GreenDepth
        {
            get { return this.FGreenDepth; }
            set { this.FGreenDepth = value; }
        }

        public byte BlueDepth
        {
            get { return this.FBlueDepth; }
            set { this.FBlueDepth = value; }
        }

        public virtual void Write(BinaryWriter aWriter)
        {
            aWriter.Write(this.FRedDepth);
            aWriter.Write(this.FGreenDepth);
            aWriter.Write(this.FBlueDepth);
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine(String.Format("RedDepth - {0}", this.FRedDepth));
            _sb.AppendLine(String.Format("GreenDepth - {0}", this.FGreenDepth));
            _sb.AppendLine(String.Format("BlueDepth - {0}", this.FBlueDepth));
            return _sb.ToString();
        }
    }

    public class StciRgbHeader : StciSubHeader
    {
        public StciRgbHeader()
        {
            this.FRedMask = 63488;
            this.FGreenMask = 2016;
            this.FBlueMask = 31;
            this.FAlphaMask = 0;

            base.FRedDepth = 5;
            base.FGreenDepth = 6;
            base.FBlueDepth = 5;
            this.FAlphaDepth = 0;
        }

        UInt32 FRedMask;
        public uint RedMask
        {
            get { return this.FRedMask; }
            set { this.FRedMask = value; }
        }

        UInt32 FGreenMask;
        public uint GreenMask
        {
            get { return this.FGreenMask; }
            set { this.FGreenMask = value; }
        }

        UInt32 FBlueMask;
        public uint BlueMask
        {
            get { return this.FBlueMask; }
            set { this.FBlueMask = value; }
        }

        UInt32 FAlphaMask;
        public uint AlphaMask
        {
            get { return this.FAlphaMask; }
            set { this.FAlphaMask = value; }
        }

        byte FAlphaDepth;
        public byte AlphaDepth
        {
            get { return this.FAlphaDepth; }
            set { this.FAlphaDepth = value; }
        }

        public override void Read(BinaryReader aReader)
        {
            this.FRedMask = aReader.ReadUInt32();
            this.FGreenMask = aReader.ReadUInt32();
            this.FBlueMask = aReader.ReadUInt32();
            this.FAlphaMask = aReader.ReadUInt32();
            base.Read(aReader);
            this.FAlphaDepth = aReader.ReadByte();
        }

        public override void Write(BinaryWriter aWriter)
        {
            aWriter.Write(this.FRedMask);
            aWriter.Write(this.FGreenMask);
            aWriter.Write(this.FBlueMask);
            aWriter.Write(this.FAlphaMask);
            base.Write(aWriter);
            aWriter.Write(this.FAlphaDepth);
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine(String.Format("RedMask - {0}", this.FRedMask));
            _sb.AppendLine(String.Format("GreenMask - {0}", this.FGreenMask));
            _sb.AppendLine(String.Format("BlueMask - {0}", this.FBlueMask));
            _sb.AppendLine(String.Format("AlphaMask - {0}", this.FAlphaMask));
            _sb.Append(base.ToString());
            _sb.AppendLine(String.Format("AlphaDepth - {0}", this.FAlphaDepth));
            return _sb.ToString();
        }
    }

    public class StciIndexedHeader : StciSubHeader
    {
        public StciIndexedHeader(UInt16 aNumberOfSubImages)
        {
            base.FRedDepth = 8;
            base.FGreenDepth = 8;
            base.FBlueDepth = 8;
            this.FNumberOfColours = 256;
            this.FNumberOfSubImages = aNumberOfSubImages;
        }

        private UInt32 FNumberOfColours;
        public uint NumberOfColous
        {
            get { return this.FNumberOfColours; }
            set { this.FNumberOfColours = value; }
        }


        private UInt16 FNumberOfSubImages;
        public UInt16 NumberOfSubImages
        {
            get { return this.FNumberOfSubImages; }
            set { this.FNumberOfSubImages = value; }
        }

        private byte[] FUnused = new byte[11];
        public byte[] Unused
        {
            get { return this.FUnused; }
            set { this.FUnused = value; }
        }

        public override void Read(BinaryReader aReader)
        {
            this.FNumberOfColours = aReader.ReadUInt32();
            this.FNumberOfSubImages = aReader.ReadUInt16();
            base.Read(aReader);
            aReader.Read(this.FUnused, 0, 11);
        }

        public override void Write(BinaryWriter aWriter)
        {
            aWriter.Write(this.FNumberOfColours);
            aWriter.Write(this.FNumberOfSubImages);
            base.Write(aWriter);
            aWriter.Write(this.FUnused);
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine(String.Format("NumberOfColours - {0}", this.FNumberOfColours));
            _sb.AppendLine(String.Format("NumberOfSubImages - {0}", this.FNumberOfSubImages));
            _sb.Append(base.ToString());
            return _sb.ToString();
        }
    }
}
