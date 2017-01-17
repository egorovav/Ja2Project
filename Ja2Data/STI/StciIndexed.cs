using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ja2Data
{
    public class StciIndexed : IStci
    {
        public const int NUMBER_OF_COLORS = 256;

		public StciIndexed()
		{
		}

        public StciIndexed(StciHeader aHeader)
        {
            this.FHeader = aHeader;
        }

        public StciIndexed(StciHeader aHeader, byte[] aPalette, StciSubImage[] aImages)
           : this(aHeader)
        {
            this.FPalette = aPalette;
            this.FImages = aImages;
        }

        public StciIndexed(StciHeader aHeader, string aFileName)
            : this(aHeader)
        {
            this.FFileName = aFileName;
        }

        StciHeader FHeader = new StciHeader();
        public StciHeader Header
        {
            get { return this.FHeader; }
        }

        public StciIndexedHeader SubHeader
        {
            get { return (StciIndexedHeader)this.FHeader.SubHeader; }
        }


        private byte[] FPalette = new byte[NUMBER_OF_COLORS * 3];
        public byte[] Palette
        {
            get { return this.FPalette; }
            set { this.Palette = value; }
        }

        private StciColor[] FColorPalette;
        public StciColor[] ColorPalette
        {
            get 
            {
                if(this.FColorPalette == null)
                {
                    this.FColorPalette = new StciColor[NUMBER_OF_COLORS];
                    for(int i = 0; i < NUMBER_OF_COLORS ; i ++)
                    {
                        this.FColorPalette[i] = 
                            new StciColor(this.FPalette[i * 3], this.FPalette[i * 3 + 1], this.FPalette[i * 3 + 2]);
                    }
                }

                return this.FColorPalette;
            }
        }

        StciSubImage[] FImages;

        public StciSubImage[] Images
        {
            get { return this.FImages; }
        }

        string FFileName;
        public string FileName
        {
            get { return this.FFileName; }
        }

        public void Load(Stream aInput)
        {
            using (BinaryReader _br = new BinaryReader(aInput))
            {
                Load(_br);
            }
        }

		public void Read(BinaryReader aReader)
		{
			this.FHeader.Read(aReader);

			Load(aReader);		
		}

        public void Load(BinaryReader aReader)
        {
            aReader.Read(this.FPalette, 0, this.FPalette.Length);

            Deserializer _deserializer = new Deserializer(aReader.BaseStream);
            this.FImages = new StciSubImage[this.SubHeader.NumberOfSubImages];
            for (int i = 0; i < this.SubHeader.NumberOfSubImages; i++)
            {
                StciSubImageHeader _imageHeader = new StciSubImageHeader();
                _imageHeader.Read(_deserializer);
                this.FImages[i] = new StciSubImage(_imageHeader); ;
            }

            for (int i = 0; i < this.SubHeader.NumberOfSubImages; i++)
            {
                this.FImages[i].ReadData(aReader);
            }

            for (int i = 0; i < this.SubHeader.NumberOfSubImages; i++)
            {
                if (this.FHeader.AppDataSize != 0)
                {
                    this.FImages[i].ReadAuxData(_deserializer);
                }
            }
        }

        public void Save(string aFileName)
        {
            using (FileStream _fs = new FileStream(aFileName, FileMode.Create))
            {
                Save(_fs);
            }
        }

        public void Save(Stream aOutput)
        {
            using (BinaryWriter _bw = new BinaryWriter(aOutput))
            {
				int _compressedDataSize = 0;
				int _originalDataSize = 0;
				for (int i = 0; i < this.SubHeader.NumberOfSubImages; i++)
					_originalDataSize += this.Images[i].Header.Width * this.Images[i].Header.Height;
				byte[] _buffer = new byte[_originalDataSize];

				using (MemoryStream _memStream = new MemoryStream(_buffer))
				using (BinaryWriter _memWriter = new BinaryWriter(_memStream))
					for (int i = 0; i < this.SubHeader.NumberOfSubImages; i++)
					{
						this.Images[i].Header.DataOffset = (uint)_compressedDataSize;
						int _dataLength = this.FImages[i].WriteData(_memWriter);
						this.Images[i].Header.DataLength = (uint)_dataLength;
						_compressedDataSize += _dataLength;
					}

				this.Header.CompressedImageSize = _compressedDataSize;
				
                this.FHeader.Write(_bw);
                _bw.Write(this.Palette, 0, this.Palette.Length);
				
                Serializer _serializer = new Serializer(aOutput);
                for (int i = 0; i < this.SubHeader.NumberOfSubImages; i++)
                {
                    this.FImages[i].Header.Write(_serializer);
                }

				_bw.Write(_buffer, 0, _compressedDataSize);

                if (this.FHeader.AppDataSize != 0)
                {

                    for (int i = 0; i < this.SubHeader.NumberOfSubImages; i++)
                    {
                        this.FImages[i].WriteAuxData(_serializer);
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine(this.Header.ToString());
            int _i = 0;
            foreach (StciSubImage _ssi in this.FImages)
            {
                _sb.AppendLine(String.Format("SUBIMAGE {0}:", ++_i));
                _sb.AppendLine(_ssi.ToString());
            }
            return _sb.ToString();
        }


        public bool IsIndexed
        {
            get { return true; }
        }
    }

    public struct StciColor
    {
        public StciColor(byte aRed, byte aGreen, byte aBlue)
        {
            this.FRed = aRed; this.FGreen = aGreen; this.FBlue = aBlue;
        }

        byte FRed;
        public byte Red
        {
            get { return this.FRed; }
        }

        byte FGreen;
        public byte Green
        {
            get { return this.FGreen; }
        }

        byte FBlue;
        public byte Blue
        {
            get { return this.FBlue; }
        }
    }
}
