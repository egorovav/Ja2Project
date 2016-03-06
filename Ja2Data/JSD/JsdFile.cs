using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Ja2Data
{
    public class JsdFile
    {
        [Flags]
        public enum JsdFileFlags : byte
        {
            ContainsAuxImageData = 0x01,
            ContainsStructureData = 0x02,
            StructureIsHighDefenition = 0x04
        }

        public JsdFile(Header aHeader)
        {
            this.FHeader = aHeader;

            if ((this.Flags & JsdFileFlags.ContainsStructureData) != 0)
            {
                this.FStructs = new JsdStruct[aHeader.usNumberOfStructuresStored];
            }

            if ((this.Flags & JsdFileFlags.ContainsAuxImageData) != 0)
            {
                this.FAuxilarity = new AuxObjectData[aHeader.usNumberOfElements];
                if (aHeader.usNumberOfImageTileLocsStored > 0)
                    this.FTileLocData = new byte[aHeader.usNumberOfImageTileLocsStored * 2];
            }
        }

        public JsdFile(int aNumberOfStructuresStored,
                       int aNumberOfAuxDataStored,
                       int aNumberOfImageTileLocsStored,
                       bool aIsHighDefenition)
        {
            this.FHeader = new Header();

            this.FormatId = "J2SD";
            this.IsHighDefenition = aIsHighDefenition;
            this.NumberOfStructuresStored = (ushort)aNumberOfStructuresStored;
            this.NumberOfElements = (ushort)aNumberOfAuxDataStored;
            this.NumberOfImageTileLocsStored = (ushort)aNumberOfImageTileLocsStored;
        }

        public bool IsHighDefenition
        {
            get { return (this.Flags & JsdFileFlags.StructureIsHighDefenition) > 0; }
            set
            {
                if (value)
                    this.Flags |= JsdFileFlags.StructureIsHighDefenition;
                else
                    this.Flags &= ~JsdFileFlags.StructureIsHighDefenition;
            }
        }

        private Header FHeader;
        private AuxObjectData[] FAuxilarity = new AuxObjectData[0];
        public AuxObjectData[] Auxilarity
        {
            get { return this.FAuxilarity; }
        }

        // Массив знаковых байтов - sbyte. По два байта на каждый тайл изображения.
        // Размер массива - usNumberOfImageTileLocsStored * 2
        private byte[] FTileLocData = new byte[0];
        public byte[] TileLocData
        {
            get { return this.FTileLocData; }
        }

        private JsdStruct[] FStructs = new JsdStruct[0];
        public JsdStruct[] Structs
        {
            get { return this.FStructs; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Header
        {
            public const int FormatIdLength = 4;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FormatIdLength)]
            public byte[] szId;
            public UInt16 usNumberOfElements;
            public UInt16 usNumberOfStructuresStored;
            public UInt16 usStructureDataSize;
            public byte fFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] bUnused;
            public UInt16 usNumberOfImageTileLocsStored;

            public Header(
                string aId,
                UInt16 aNumberOfElements,
                UInt16 aNumberOfStructuresStored,
                UInt16 aStructureDataSize,
                byte aFlags,
                byte[] aUnused,
                UInt16 aNumberOfImageTileLocsStored
                )
            {
                this.szId = Common.StringToByteArray(aId, FormatIdLength);
                this.usNumberOfElements = aNumberOfElements;
                this.usNumberOfStructuresStored = aNumberOfStructuresStored;
                this.usStructureDataSize = aStructureDataSize;
                this.fFlags = aFlags;
                this.bUnused = aUnused;
                this.usNumberOfImageTileLocsStored = aNumberOfImageTileLocsStored;
            }
        }

        public void Save(Stream aOutput)
        {
            Serializer serializer = new Serializer(aOutput);
            this.FHeader.usStructureDataSize = this.StructureDataSize;
            this.FHeader.usNumberOfStructuresStored = this.NumberOfStructuresStored;
            this.FHeader.usNumberOfElements = this.NumberOfElements;
            this.FHeader.usNumberOfImageTileLocsStored = this.NumberOfImageTileLocsStored;
            serializer.Serialize(this.FHeader);

            if (((this.Flags & JsdFileFlags.ContainsAuxImageData) != 0))
                foreach (AuxObjectData auxData in this.FAuxilarity)
                    auxData.Save(serializer);

            if (this.FHeader.usNumberOfImageTileLocsStored > 0)
            {
                aOutput.Write(this.FTileLocData, 0, this.FTileLocData.Length);
            }

            if ((this.Flags & JsdFileFlags.ContainsStructureData) != 0)
            {
                Array.Sort(this.FStructs);
                foreach (JsdStruct jsdStruct in this.FStructs)
                    jsdStruct.Save(serializer);
            }
        }

        public ushort StructureDataSize
        {
            get
            {
                int _tileSize = IsHighDefenition ? 112 : 32;
                int _size = 0;
                foreach (JsdStruct _struct in this.Structs)
                    _size += _struct.NumberOfTiles * _tileSize + 16;
                return (ushort)_size;
            }
        }

        public static JsdFile Load(Stream aInput)
        {
            Deserializer _deserializer = new Deserializer(aInput);
            JsdFile.Header _header = (Header)_deserializer.Deserialize(typeof(Header));

            JsdFile _jsdFile = new JsdFile(_header);

            for (int i = 0; i < _jsdFile.FAuxilarity.Length; i++)
            {
                AuxObjectData data = new AuxObjectData();
                data.Load(_deserializer);
                _jsdFile.FAuxilarity[i] = data;
            }

            aInput.Read(_jsdFile.FTileLocData, 0, _jsdFile.FTileLocData.Length);

            for (int i = 0; i < _jsdFile.FStructs.Length; i++)
            {
                JsdStruct jsdStruct = new JsdStruct(_jsdFile.IsHighDefenition);
                jsdStruct.Load(_deserializer);
                _jsdFile.FStructs[i] = jsdStruct;
            }

            return _jsdFile;
        }

        public string FormatId
        {
            get { return Common.ByteArrayToString(this.FHeader.szId); }
            protected set { this.FHeader.szId = Common.StringToByteArray(value, Header.FormatIdLength); }
        }

        public ushort NumberOfElements
        {
            get
            {
                return this.FHeader.usNumberOfElements;
            }
            set
            {
                this.FHeader.usNumberOfElements = value;
            }
        }

        public UInt16 NumberOfStructuresStored
        {
            get { return (ushort)this.Structs.Length; }
            protected set
            {
                this.FHeader.usNumberOfStructuresStored = value;
                this.FStructs = new JsdStruct[value];
                for (int i = 0; i < value; i++)
                    this.FStructs[i] = new JsdStruct(this.IsHighDefenition);

                if (value > 0)
                    this.Flags |= JsdFileFlags.ContainsStructureData;
                else
                    this.Flags &= ~JsdFileFlags.ContainsStructureData;
            }
        }

        public UInt16 NumberOfImageTileLocsStored
        {
            get {  return (ushort)(this.TileLocData.Length / 2); }
            protected set
            {
                this.FHeader.usNumberOfImageTileLocsStored = value;
                this.FTileLocData = new byte[value * 2];
            }
        }

        public JsdFileFlags Flags
        {
            get { return (JsdFileFlags)this.FHeader.fFlags; }
            set { this.FHeader.fFlags = (byte)value; }
        }

        public byte[] Unused
        {
            get { return this.FHeader.bUnused; }
            set { this.FHeader.bUnused = value; }
        }

        public List<byte> GetAuxTileLocData(AuxObjectData aAuxData)
        {
            if (this.NumberOfImageTileLocsStored == 0)
                return null;

            byte[] _tileLocData = new byte[aAuxData.NumberOfTiles * 2];
            Array.Copy(this.TileLocData, aAuxData.TileLocIndex * 2, _tileLocData, 0, aAuxData.NumberOfTiles * 2);

            //return _tileLocData.ToList();
            return new List<byte>(_tileLocData);
        }

        public void BuildInfo(StringBuilder aInfoBuilder)
        {
            aInfoBuilder.AppendLine(String.Format("Flags - {0}", this.Flags));
            aInfoBuilder.AppendLine(String.Format("NumberOfImageTileLocsStored - {0}", this.NumberOfImageTileLocsStored));
            aInfoBuilder.AppendLine(String.Format("NumberOfElements - {0}", this.NumberOfElements));
            aInfoBuilder.AppendLine(String.Format("NumberOfStructuresStored - {0}", this.NumberOfStructuresStored));
            aInfoBuilder.AppendLine(String.Format("StructureDataSize - {0}", this.StructureDataSize));

            for (int i = 0; i < this.Auxilarity.Length; i++)
            {
                aInfoBuilder.AppendLine(String.Format("AUXILARITY {0}:", i));
                this.FAuxilarity[i].BuildInfo(aInfoBuilder);
            }

            for (int i = 0; i < this.NumberOfImageTileLocsStored; i += 2)
            {
                aInfoBuilder.Append(String.Format("{0}, {1};", this.FTileLocData[i], this.FTileLocData[i + 1]));
            }

            for (int i = 0; i < this.Structs.Length; i++)
            {
                aInfoBuilder.AppendLine(String.Format("STRUCTURE {0}:", i));
                this.FStructs[i].BuildInfo(aInfoBuilder);
            }

            aInfoBuilder.AppendLine();
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();

            BuildInfo(_sb);

            return _sb.ToString();
        }

        public static string ReloadJsdFile(string aFileName)
        {
            JsdFile _file = null;
            using (FileStream _fsi = new FileStream(aFileName, FileMode.Open))
                _file = JsdFile.Load(_fsi);
            
            using (FileStream _fso = new FileStream(aFileName, FileMode.Create))
                _file.Save(_fso);

            return _file.ToString();
        }

        public void ConvertToHighDefinition()
        {
            if (this.IsHighDefenition)
                return;

            this.Flags |= JsdFileFlags.StructureIsHighDefenition;

            int _tileNum = 0;
            foreach (JsdStruct _struct in this.Structs)
            {
                _tileNum += _struct.NumberOfTiles;
                foreach (JsdTile _tile in _struct.Tiles)
                {
                    _tile.ConvertToHiDefenition();
                }
            }
        }

        public static string ConvertJsdFileToHighDefinition(string aFileName)
        {
            JsdFile _file = null;
            using (FileStream _fsi = new FileStream(aFileName, FileMode.Open))
                _file = JsdFile.Load(_fsi);

            _file.ConvertToHighDefinition();

            using (FileStream _fso = new FileStream(aFileName, FileMode.Create))
                _file.Save(_fso);

            return _file.ToString();
        }

        public void AddStruct(JsdStruct aStruct)
        {
            JsdStruct[] _structs = new JsdStruct[this.NumberOfStructuresStored + 1];
            Array.Copy(this.FStructs, _structs, this.NumberOfStructuresStored);
            _structs[this.NumberOfStructuresStored] = aStruct;
            Array.Sort(_structs);
            this.FStructs = _structs;
        }

        public void RemoveStruct(JsdStruct aStruct)
        {
            List<JsdStruct> _structs = new List<JsdStruct>();
            foreach(JsdStruct _struct in this.FStructs)
            {
                if (_struct != aStruct)
                    _structs.Add(_struct);
            }
            this.FStructs = _structs.ToArray();
        }

        public void AddAuxData(AuxObjectData aAux)
        {
            AuxObjectData[] _auxs = new AuxObjectData[this.FAuxilarity.Length + 1];
            Array.Copy(this.FAuxilarity, _auxs, this.FAuxilarity.Length);
            _auxs[this.Auxilarity.Length] = aAux;
            this.FAuxilarity = _auxs;
        }

        public void RemoveAuxData(AuxObjectData aAux)
        {
            List<AuxObjectData> _auxs = new List<AuxObjectData>();
            foreach (AuxObjectData _aux in this.FAuxilarity)
            {
                if (_aux != aAux)
                    _auxs.Add(_aux);
            }
            this.FAuxilarity = _auxs.ToArray();
        }
    }
}
