using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ja2Data
{
    public class JsdStruct : IComparable
    {
        [Flags]
        public enum JsdStructureFlags : uint
        {
            None              = 0x00000000,
            BASE_TILE         = 0x00000001,
            OPEN              = 0x00000002,
            OPENABLE          = 0x00000004,
            // synonyms for STRUCTURE_OPENABLE
            CLOSEABLE         = 0x00000004,
            SEARCHABLE        = 0x00000004,
            HIDDEN            = 0x00000008,
            MOBILE            = 0x00000010,
            // STRUCTURE_PASSABLE is set for each structure instance where
            // the tile flag TILE_PASSABLE is set
            PASSABLE          = 0x00000020,
            EXPLOSIVE         = 0x00000040,
            TRANSPARENT       = 0x00000080,
            GENERIC           = 0x00000100,
            TREE              = 0x00000200,
            FENCE             = 0x00000400,
            WIREFENCE         = 0x00000800,
            // ATE: HASITEM: struct has item on top of it
            HAS_ITEM_ON_TOP   = 0x00001000,
            SPECIAL           = 0x00002000,
            LIGHTSOURCE       = 0x00004000,
            VEHICLE           = 0x00008000,
            WALL              = 0x00010000,
            WALLNWINDOW       = 0x00020000,
            SLIDINGDOOR       = 0x00040000,
            DOOR              = 0x00080000,
            // a "multi" structure (as opposed to multitiled) is composed of multiple graphics & structures
            MULTI =			    0x00100000,
            CAVEWALL          = 0x00200000,
            DDOOR_LEFT        = 0x00400000,
            DDOOR_RIGHT       = 0x00800000,
            NORMAL_ROOF       = 0x01000000,
            SLANTED_ROOF      = 0x02000000,
            TALL_ROOF         = 0x04000000,
            SWITCH   		  = 0x08000000,
            ON_LEFT_WALL      = 0x10000000,
            ON_RIGHT_WALL     =	0x20000000,
            CORPSE  		  = 0x40000000,
            PERSON            = 0x80000000
        }

        private Header FHeader;

        public JsdStruct()
        {

        }

        public JsdStruct(bool aIsHighDefenition)
        {
            this.FIsHighDefenition = aIsHighDefenition;
        }

        private bool FIsHighDefenition;
        public bool IsHighDefenition
        {
            get { return this.FIsHighDefenition; }
            set { this.FIsHighDefenition = value; }
        }

        private JsdTile[] FTiles;
        public JsdTile[] Tiles
        {
            get 
            {
                return this.FTiles; 
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Header
        {
            public byte ubArmour;
            public byte ubHitPoints;
            public byte ubDensity;
            public byte ubNumberOfTiles;
            public UInt32 fFlags;
            public UInt16 usStructureNumber;
            public byte ubWallOrientation;
            public sbyte bDestructionPartner;
            public sbyte bPartnerDelta;
            public sbyte bZTileOffsetX;
            public sbyte bZTileOffsetY;
            public byte bUnused;
        }

        public void Save(Serializer serializer)
        {
            this.FHeader.ubNumberOfTiles = this.NumberOfTiles;
            serializer.Serialize(this.FHeader);

            if (this.FTiles == null || this.FTiles.Length == 0)
                return;

            // Базовый тайл должен быть первым.
            if (this.FTiles[0].XPosRelToBase != 0 || this.FTiles[0].YPosRelToBase != 0)
            {
                JsdTile _temp = this.FTiles[0];
                for(int i = 0; i < this.FTiles.Length; i++)
                {
                    if(this.FTiles[i].XPosRelToBase == 0 && this.FTiles[i].YPosRelToBase == 0)
                    {
                        this.FTiles[0] = this.FTiles[i];
                        this.FTiles[i] = _temp;
                        break;
                    }
                }
            }

            foreach (JsdTile jsdTile in this.FTiles)
                jsdTile.Save(serializer);
        }

        public void Load(Deserializer deserializer)
        {
            this.FHeader = (Header)deserializer.Deserialize(typeof(Header));
            this.FTiles = new JsdTile[this.FHeader.ubNumberOfTiles];
            for (int i = 0; i < this.FHeader.ubNumberOfTiles; i++)
            {
                JsdTile _tile = new JsdTile(this.FIsHighDefenition);
                _tile.Load(deserializer);
                this.FTiles[i] = _tile;
            }
        }

        public byte Armour
        {
            get { return this.FHeader.ubArmour; }
            set { this.FHeader.ubArmour = value; }
        }

        public byte HitPoints
        {
            get { return this.FHeader.ubHitPoints; }
            set { this.FHeader.ubHitPoints = value; }
        }

        public byte Density
        {
            get { return this.FHeader.ubDensity; }
            set { this.FHeader.ubDensity = value; }
        }

        public byte NumberOfTiles
        {
            get 
            {
                if (this.FTiles == null)
                    return 0;

                return (byte)this.FTiles.Length; 
            }

            set
            {
                this.FTiles = new JsdTile[value];
            }
        }

        public JsdStructureFlags Flags
        {
            get { return (JsdStructureFlags)this.FHeader.fFlags; }
            set { this.FHeader.fFlags = (uint)value; }
        }
        public UInt16 StructureNumber
        {
            get { return this.FHeader.usStructureNumber; }
            set { this.FHeader.usStructureNumber = value; }
        }

        public byte WallOrientation
        {
            get { return this.FHeader.ubWallOrientation; }
            set { this.FHeader.ubWallOrientation = value; }
        }

        public sbyte DestructionPartner
        {
            get { return this.FHeader.bDestructionPartner; }
            set { this.FHeader.bDestructionPartner = value; }
        }

        public sbyte PartnerDelta
        {
            get { return this.FHeader.bPartnerDelta; }
            set { this.FHeader.bPartnerDelta = value; }
        }

        public sbyte ZTileOffsetX
        {
            get { return this.FHeader.bZTileOffsetX; }
            set { this.FHeader.bZTileOffsetX = value; }
        }

        public sbyte ZTileOffsetY
        {
            get { return this.FHeader.bZTileOffsetY; }
            set { this.FHeader.bZTileOffsetY = value; }
        }

        public byte Unused
        {
            get { return this.FHeader.bUnused; }
            set { this.FHeader.bUnused = value; }
        }

        public void AddTile(JsdTile aJsdTile)
        {
            JsdTile[] _tiles = new JsdTile[this.NumberOfTiles + 1];
            if(this.FTiles != null)
                Array.Copy(this.FTiles, _tiles, this.NumberOfTiles);
            _tiles[this.NumberOfTiles] = aJsdTile;
            this.FTiles = _tiles;
        }

        public void RemoveTile(JsdTile aTile)
        {
            List<JsdTile> _tiles = new List<JsdTile>(this.NumberOfTiles - 1);
            for(int i = 0; i < this.NumberOfTiles; i++)
            {
                if (this.FTiles[i] == aTile)
                    continue;

                _tiles.Add(this.FTiles[i]);
            }
            this.FTiles = _tiles.ToArray();
        }

        public void BuildInfo(StringBuilder aInfoBuilder)
        {
            aInfoBuilder.AppendLine(String.Format("Armour - {0}", this.Armour));
            aInfoBuilder.AppendLine(String.Format("Density - {0}", this.Density));
            aInfoBuilder.AppendLine(String.Format("DestructionPartner - {0}", this.DestructionPartner));
            aInfoBuilder.AppendLine(String.Format("Flags - {0}", this.Flags));
            aInfoBuilder.AppendLine(String.Format("HitPoints - {0}", this.HitPoints));
            aInfoBuilder.AppendLine(String.Format("NumberOfTiles - {0}", this.NumberOfTiles));
            aInfoBuilder.AppendLine(String.Format("PartnerDelta - {0}", this.PartnerDelta));
            aInfoBuilder.AppendLine(String.Format("StructureNumber - {0}", this.StructureNumber));
            aInfoBuilder.AppendLine(String.Format("WallOrientation - {0}", this.WallOrientation));
            aInfoBuilder.AppendLine(String.Format("ZTileOffsetX - {0}", this.ZTileOffsetX));
            aInfoBuilder.AppendLine(String.Format("ZTileOffsetY - {0}", this.ZTileOffsetY));

            for (int i = 0; i < this.NumberOfTiles; i++)
            {
                aInfoBuilder.AppendLine(String.Format("TILE {0}:", i));
                this.FTiles[i].BuildInfo(aInfoBuilder);
            }

            aInfoBuilder.AppendLine();
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();

            BuildInfo(_sb);

            return _sb.ToString();
        }

        public int CompareTo(object obj)
        {
            JsdStruct _struct = obj as JsdStruct;
            if (_struct != null)
            {
                return this.StructureNumber.CompareTo(_struct.StructureNumber);
            }
            else
                return 0;
        }

        public JsdStruct Clone()
        {
            JsdStruct _newStruct = new JsdStruct(this.IsHighDefenition);

            Header _header = new Header();
            _header.bDestructionPartner = this.FHeader.bDestructionPartner;
            _header.bPartnerDelta = this.FHeader.bPartnerDelta;
            _header.bUnused = this.FHeader.bUnused;
            _header.bZTileOffsetX = this.FHeader.bZTileOffsetX;
            _header.bZTileOffsetY = this.FHeader.bZTileOffsetY;
            _header.fFlags = this.FHeader.fFlags;
            _header.ubArmour = this.FHeader.ubArmour;
            _header.ubDensity = this.FHeader.ubDensity;
            _header.ubHitPoints = this.FHeader.ubHitPoints;
            _header.ubNumberOfTiles = this.FHeader.ubNumberOfTiles;
            _header.ubWallOrientation = this.FHeader.ubWallOrientation;
            _header.usStructureNumber = this.FHeader.usStructureNumber;

            _newStruct.FHeader = _header;
            if (this.FTiles == null)
            {
                _newStruct.FTiles = null;
            }
            else
            {
                _newStruct.FTiles = new JsdTile[this.FTiles.Length];
                for (int i = 0; i < this.FTiles.Length; i++)
                    _newStruct.FTiles[i] = this.FTiles[i].Clone();
            }

            return _newStruct;
        }

        public void Rotate(bool aIsClockwise)
        {
            if (this.FTiles == null)
                return;

            //int _maxX = 0;
            //int _maxY = 0;

            foreach(JsdTile _tile in this.FTiles)
            {
                if (aIsClockwise)
                {
                    int _temp = _tile.XPosRelToBase;
                    _tile.XPosRelToBase = -_tile.YPosRelToBase;
                    _tile.YPosRelToBase = _temp;
                }
                else
                {
                    int _temp = _tile.XPosRelToBase;
                    _tile.XPosRelToBase = _tile.YPosRelToBase;
                    _tile.YPosRelToBase = -_temp;
                }

                //if (_tile.XPosRelToBase + _tile.YPosRelToBase > _maxX + _maxY)
                //{
                //    _maxX = _tile.XPosRelToBase;
                //    _maxY = _tile.YPosRelToBase;
                //}
                _tile.Rotate(aIsClockwise);
            }

            //foreach (JsdTile _tile in this.FTiles)
            //{
            //    _tile.XPosRelToBase -= _maxX;
            //    _tile.YPosRelToBase -= _maxY;
            //}
        }

        public class BaseTileMissingException : Exception
        {
            public override string Message
            {
                get
                {
                    return "Structure doesn't have base tile. Structute mast have a tile with XPosRelToBase = 0 and YPosRelToBase = 0.";
                }
            }
        }
    }
}
