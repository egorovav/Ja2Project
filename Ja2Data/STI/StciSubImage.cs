using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Ja2Data
{
    public class StciSubImage
    {
        public StciSubImage(StciSubImageHeader aHeader)
        {
            this.FHeader = aHeader;
        }

        StciSubImageHeader FHeader;
        public StciSubImageHeader Header
        {
            get { return this.FHeader; }
        }

        byte[] FData;
        public byte[] ImageData
        {
            get { return this.FData; }
            set { this.FData = value; }
        }

        AuxObjectData FAuxData;
        public AuxObjectData AuxData
        {
            get { return this.FAuxData; }
            set { this.FAuxData = value; }
        }

        public void ReadData(BinaryReader aReader)
        {
            this.FData = Etrle.Read(aReader, (int)this.FHeader.Height, (int)this.FHeader.Width, (int)this.FHeader.DataLength);
        }

        public void WriteData(BinaryWriter aWriter)
        {
            Etrle.Write(aWriter, this.FData, this.FHeader.Width);
        }

        public void ReadAuxData(Deserializer aReader)
        {
            this.FAuxData = new AuxObjectData();
            this.FAuxData.Load(aReader);
        }

        public void WriteAuxData(Serializer aWriter)
        {
            this.FAuxData.Save(aWriter);
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine(this.FHeader.ToString());
            _sb.AppendLine("AUXDATA:");
            if(this.FAuxData != null)
                _sb.AppendLine(this.FAuxData.ToString());
            return _sb.ToString();
        }

    }

    //    typedef struct
    //{
    //    UINT32			uiDataOffset;
    //    UINT32			uiDataLength;
    //    INT16				sOffsetX;
    //    INT16				sOffsetY;
    //    UINT16			usHeight;
    //    UINT16			usWidth;
    //} STCISubImage;

    public class StciSubImageHeader
    {
        public StciSubImageHeader()
        {

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Data
        {
            public UInt32 uiDataOffset;
            public UInt32 uiDataLength;
            public Int16 sOffsetX;
            public Int16 sOffsetY;
            public UInt16 usHeight;
            public UInt16 usWidth;
        }

        private Data FHeader;

        public UInt32 DataOffset
        {
            get { return this.FHeader.uiDataOffset; }
            set { this.FHeader.uiDataOffset = value; }
        }

        public UInt32 DataLength
        {
            get { return this.FHeader.uiDataLength; }
            set { this.FHeader.uiDataLength = value; }
        }

        public Int16 OffsetX
        {
            get { return this.FHeader.sOffsetX; }
            set { this.FHeader.sOffsetX = value; }
        }

        public Int16 OffsetY
        {
            get { return this.FHeader.sOffsetY; }
            set { this.FHeader.sOffsetY = value; }
        }

        public UInt16 Height
        {
            get { return this.FHeader.usHeight; }
            set { this.FHeader.usHeight = value; }
        }

        public UInt16 Width
        {
            get { return this.FHeader.usWidth; }
            set { this.FHeader.usWidth = value; }
        }

        public void Read(Deserializer aReader)
        {
            this.FHeader = (StciSubImageHeader.Data)aReader.Deserialize(typeof(StciSubImageHeader.Data));
        }

        public void Write(Serializer aWriter)
        {
            aWriter.Serialize(this.FHeader);
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine(String.Format("DataOffset - {0}", this.DataOffset));
            _sb.AppendLine(String.Format("DataLength - {0}", this.DataLength));
            _sb.AppendLine(String.Format("OffsetX - {0}", this.OffsetX));
            _sb.AppendLine(String.Format("OffsetY - {0}", this.OffsetY));
            _sb.AppendLine(String.Format("Heigth - {0}", this.Height));
            _sb.AppendLine(String.Format("Width - {0}", this.Width));
            return _sb.ToString();
        }
    }
}
