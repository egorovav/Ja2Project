using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ja2Data
{
    public class StciRgb : IStci
    {
        public StciRgb(StciHeader aHeader)
        {
            this.FHeader = aHeader;
        }

        public StciRgb(StciHeader aHeader, byte[] aImageData) 
            : this(aHeader)
        {
            this.FImageData = aImageData;
        }

        public StciRgb(StciHeader aHeader, string aFileName)
            : this(aHeader)
        {
            this.FFileName = aFileName;
        }

        StciHeader FHeader = new StciHeader();
        public StciHeader Header
        {
            get { return this.FHeader; }
        }

        byte[] FImageData;
        public byte[] ImageData
        {
            get { return this.FImageData; }
            set { this.FImageData = value; }
        }

        string FFileName;
        public string FileName
        {
            get { return this.FFileName; }
        }

        public void Load(string aFileName)
        {
            using (FileStream _fs = new FileStream(aFileName, FileMode.Open))
            {
                this.Load(_fs);
            }
        }

        public void Load(Stream aInput)
        {
            using (BinaryReader _br = new BinaryReader(aInput, Encoding.ASCII))
            {
                this.Load(_br);
            }
        }

        public void Load(BinaryReader aReader)
        {           
            this.FImageData = aReader.ReadBytes((int)this.FHeader.OriginalImageSize);
        }

        public void Save(string aFileName)
        {
            using (FileStream _fs = new FileStream(aFileName, FileMode.Create))
            {
                Load(_fs);
            }
        }

        public void Save(Stream aOutput)
        {
            using (BinaryWriter _bw = new BinaryWriter(aOutput))
            {
                this.FHeader.Write(_bw);
                _bw.Write(this.FImageData);
            }
        }

        public override string ToString()
        {
            return this.FHeader.ToString();
        }


        public bool IsIndexed
        {
            get { return false; }
        }
    }
}
