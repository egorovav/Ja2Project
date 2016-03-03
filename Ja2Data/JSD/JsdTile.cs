using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Ja2Data
{
    public class JsdTile : IComparable
    {
        public static int MinSize = 5;
        public static int MinHeight = 4;

        [Flags]
        public enum JsdTileFlags : byte
        {
            TILE_ON_ROOF = 0x01,
            TILE_PASSABLE = 0x02
        }

        public static int GetProfileXSize(bool aIsHiDefenition)
        {
            return aIsHiDefenition ? JsdTile.MinSize * 2 : JsdTile.MinSize; 
        }

        public static int GetProfileYSize(bool aIsHiDefenition)
        {
            return aIsHiDefenition ? JsdTile.MinSize * 2 : JsdTile.MinSize;
        }

        public static int GetProfileZSize(bool aIsHiDefenition)
        {
            return aIsHiDefenition ? JsdTile.MinHeight * 2 : JsdTile.MinHeight;
        }

        public static int GetUnusedDataSize(bool aIsHiDefenition)
        {
            return aIsHiDefenition ? 6 : 1;
        }

        public JsdTile(bool aIsHighDefentition)
        {
            this.IsHighDefenition = aIsHighDefentition;
        }

        public int ProfileXSize { get; protected set; }
        public int ProfileYSize { get; protected set; }
        public int ProfileZSize { get; protected set; }
        public int UnusedDataSize { get; protected set; }

        public int ProfileSize
        {
            get { return ProfileXSize * ProfileYSize; }
        }

        public struct Data
        {
            public Int16 sPosRelToBase;
            public sbyte bXPosRelToBase;
            public sbyte bYPosRelToBase;
            public byte[] shape;
            public byte fFlags;
            public byte ubVehicleHitLocation;
            public byte[] bUnused;
        }

        private bool FIsHighDefenition;
        public bool IsHighDefenition
        {
            get { return this.FIsHighDefenition; }
            protected set 
            { 
                this.FIsHighDefenition = value;
                this.ProfileXSize = GetProfileXSize(value);
                this.ProfileYSize = GetProfileYSize(value);
                this.ProfileZSize = GetProfileZSize(value);
                this.UnusedDataSize = GetUnusedDataSize(value);
                this.FData.bUnused = new byte[this.UnusedDataSize];
                this.Shape = new byte[this.ProfileSize];
            }
        }

        private Data FData;

        public Int16 PosRelToBase
        {
            get { return this.FData.sPosRelToBase; }
            set { this.FData.sPosRelToBase = value; }
        }
        public int XPosRelToBase
        {
            get { return this.FData.bXPosRelToBase; }
            set { this.FData.bXPosRelToBase = (sbyte)value; }
        }
        public int YPosRelToBase
        {
            get { return this.FData.bYPosRelToBase; }
            set { this.FData.bYPosRelToBase = (sbyte)value; }
        }

        public byte[] Shape
        {
            get { return this.FData.shape; }
            set { this.FData.shape = value; }
        }

        public JsdTileFlags Flags
        {
            get { return (JsdTileFlags)this.FData.fFlags; }
            set { this.FData.fFlags = (byte)value; }
        }

        public byte VehicleHitLocation
        {
            get { return this.FData.ubVehicleHitLocation; }
            set { this.FData.ubVehicleHitLocation = value; }
        }

        public byte Unused
        {
            get { return this.FData.bUnused[0]; }
            set { this.FData.bUnused[0] = value; }
        }

        public void Load(Deserializer deserializer)
        {
            this.FData.sPosRelToBase = deserializer.DeserializeShort();
            this.FData.bXPosRelToBase = deserializer.DeserializeSByte();
            this.FData.bYPosRelToBase = deserializer.DeserializeSByte();
            this.FData.shape = deserializer.DeserializeBytes(this.ProfileSize);
            this.FData.fFlags = deserializer.DeserializeByte();
            this.FData.ubVehicleHitLocation = deserializer.DeserializeByte();
            this.FData.bUnused = deserializer.DeserializeBytes(this.UnusedDataSize);
        }

        public void Save(Serializer serializer)
        {
            serializer.Serialize(this.FData.sPosRelToBase);
            serializer.Serialize(this.FData.bXPosRelToBase);
            serializer.Serialize(this.FData.bYPosRelToBase);
            serializer.Serialize(this.FData.shape);
            serializer.Serialize(this.FData.fFlags);
            serializer.Serialize(this.FData.ubVehicleHitLocation);
            serializer.Serialize(this.FData.bUnused);
        }

        public JsdTile Clone()
        {
            JsdTile _newTile = new JsdTile(this.IsHighDefenition);

            Data _data = new Data();
            _data.bUnused = this.FData.bUnused;
            _data.bXPosRelToBase = this.FData.bXPosRelToBase;
            _data.bYPosRelToBase = this.FData.bYPosRelToBase;
            _data.fFlags = this.FData.fFlags;
            _data.shape = new byte[this.FData.shape.Length];
            Array.Copy(this.FData.shape, _data.shape, _data.shape.Length);
            _data.sPosRelToBase = this.FData.sPosRelToBase;
            _data.ubVehicleHitLocation = this.FData.ubVehicleHitLocation;

            _newTile.FData = _data;

            return _newTile;
        }

        public void BuildInfo(StringBuilder aInfoBuilder)
        {
            aInfoBuilder.AppendLine(String.Format("Flags - {0}", this.Flags));
            aInfoBuilder.AppendLine(String.Format("PosRelToBase - {0}", this.PosRelToBase));
            aInfoBuilder.AppendLine(String.Format("VehicleHitLocation - {0}", this.VehicleHitLocation));
            aInfoBuilder.AppendLine(String.Format("XPosRelToBase - {0}", this.XPosRelToBase));
            aInfoBuilder.AppendLine(String.Format("YPosRelToBase - {0}", this.YPosRelToBase));
            for (int i = 0; i < ProfileSize; i += ProfileXSize)
            {
                for (int j = 0; j < ProfileYSize; j++)
                    if (this.ProfileZSize == MinHeight * 2)
                        aInfoBuilder.AppendFormat("{0:d3} ", this.Shape[i + j]);
                    else
                        aInfoBuilder.AppendFormat("{0:d2} ", this.Shape[i + j]);
                aInfoBuilder.AppendLine();
            }
            aInfoBuilder.AppendLine();
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();

            BuildInfo(_sb);

            return _sb.ToString();
        }

        // Базовый тайл должен быть первым.
        public int CompareTo(object obj)
        {
            JsdTile _tile = obj as JsdTile;
            int _result = 0;
            if(_tile != null)
            {
                if (_tile.XPosRelToBase == 0 && _tile.YPosRelToBase == 0)
                    _result = -1;
            }

            return _result;
        }

        public static byte DublicateBits(byte aOrigin)
        {
            if (aOrigin >= 1 << (JsdTile.MinHeight + 1))
                throw new Exception("Origin byte is too match to dublicate.");

            int _dublicatedByte = 0;

            for(int i = 0; i < JsdTile.MinHeight; i++)
            {
                int _masked = aOrigin & 1 << i;
                _dublicatedByte += _masked << i;
                _dublicatedByte += _masked << i + 1;
            }

            return (byte)_dublicatedByte;
        }

        public void ConvertToHiDefenition()
        {
            int _hiDefenitionXSize = GetProfileXSize(true);
            int _hiDefenitionYSize = GetProfileYSize(true);

            List<byte> _row = new List<byte>(_hiDefenitionXSize);
            List<byte> _newShape = new List<byte>(_hiDefenitionXSize * _hiDefenitionYSize);

            for(int i = 0; i < this.Shape.Length; i++)
            {
                if(i != 0 && i % this.ProfileXSize == 0)
                {
                    _newShape.AddRange(_row);
                    _newShape.AddRange(_row);
                    _row = new List<byte>(_hiDefenitionXSize);
                }

                byte _dublicatedBitsCellValue = DublicateBits(this.Shape[i]);

                _row.Add(_dublicatedBitsCellValue);
                _row.Add(_dublicatedBitsCellValue);
            }

            _newShape.AddRange(_row);
            _newShape.AddRange(_row);

            int _hiDefenitionUnusedDataSize = GetUnusedDataSize(true);
            this.FData.bUnused = new byte[_hiDefenitionUnusedDataSize];

            this.Shape = _newShape.ToArray();
        }

        public void Rotate(bool aIsClockwise)
        {
            byte[] _shape = new byte[this.ProfileSize];
            Array.Copy(this.Shape, _shape, this.ProfileSize);
            for(int i = 0; i < this.ProfileSize; i++)
            {
                int _x = i % this.ProfileXSize;
                int _y = i / this.ProfileXSize;

                int _index = aIsClockwise ? 
                    _x * this.ProfileXSize + this.ProfileXSize - _y - 1 :
                    (this.ProfileXSize - _x - 1) * this.ProfileXSize + _y;
                this.Shape[i] = _shape[_index];
            }
        }
    }
}
