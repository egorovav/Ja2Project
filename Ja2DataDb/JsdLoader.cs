using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Ja2DataDb
{
    public class JsdLoader : SlfRecordWithAuxLoader
    {
        public JsdLoader(IEnumerable<Ja2Data.SlfFile.Record> aFiles, int aUserId)
            : base(aFiles, aUserId)
        {
            this.FJsdRecords = new List<SlfRecordJsd>();
            this.FJsdHeaderFlags = new List<JsdRecordHeaderFlag>(2);

            this.FJsdStructures = new List<JsdStructure>(this.FStrCount);
            this.FJsdAuxData = new List<JsdAuxData>(this.FStrCount);
            this.FAuxDataJsdAux = new List<AuxDataJsdAux>(this.FStrCount);
            this.FStructureStructuresFlags = new List<JsdStructureStructureFlag>(4);
            this.FJsdTile = new List<JsdTile>(this.FStrCount);
            this.FJsdTileTileFlag = new List<JsdTileTileFlag>(this.FStrCount);

            base.FContext.JsdHeaderFlags.Load();
            this.FJsdFlags = base.FContext.JsdHeaderFlags.Local
                .Select(x => new Flag() { Id = x.JsdRecordFlagId, Mask = x.Mask.Value })
                .ToList();

            base.FContext.JsdStructureFlags.Load();
            this.FStructureFlags = base.FContext.JsdStructureFlags.Local
                .Select(x => new Flag() { Id = x.JsdStructureFlagId, Mask = (int)x.Mask.Value })
                .ToList();

            base.FContext.JsdTileFlags.Load();
            this.FTilesFlags = base.FContext.JsdTileFlags.Local
                .Select(x => new Flag() { Id = x.JsdTileFlagsId, Mask = x.Mask.Value })
                .ToList();
        }

        private int FStrCount = 16;

        private List<SlfRecordJsd> FJsdRecords;
        private List<JsdRecordHeaderFlag> FJsdHeaderFlags;
        private List<JsdStructure> FJsdStructures;
        private List<JsdAuxData> FJsdAuxData;
        private List<AuxDataJsdAux> FAuxDataJsdAux;
        private List<JsdStructureStructureFlag> FStructureStructuresFlags;
        private List<JsdTile> FJsdTile;
        private List<JsdTileTileFlag> FJsdTileTileFlag;
        private List<JsdTileCell> FJsdTileCell;


        private List<Flag> FJsdFlags;
        private List<Flag> FStructureFlags;
        private List<Flag> FTilesFlags;

        private bool FJsdFileIsHiDefinition;

        private int FProfileXSize;
        private int FProfileYSize;

        public override int AddRecordsToDataSet()
        {
            int _recCount = 0;

            _recCount += base.AddRecordsToDataSet();

            base.FContext.SlfRecordJsd.AddRange(this.FJsdRecords);
            _recCount += this.FJsdRecords.Count;
            this.FJsdRecords.Clear();

            base.FContext.JsdRecordHeaderFlag.AddRange(this.FJsdHeaderFlags);
            _recCount += this.FJsdHeaderFlags.Count;
            this.FJsdHeaderFlags.Clear();

            base.FContext.JsdStructure.AddRange(this.FJsdStructures);
            _recCount += this.FJsdStructures.Count;
            this.FJsdStructures.Clear();

            base.FContext.JsdAuxData.AddRange(this.FJsdAuxData);
            _recCount += this.FJsdAuxData.Count;
            this.FJsdAuxData.Clear();

            base.FContext.AuxDataJsdAux.AddRange(this.FAuxDataJsdAux);
            _recCount += this.FAuxDataJsdAux.Count;
            this.FAuxDataJsdAux.Clear();

            base.FContext.JsdStructureStructureFlag.AddRange(this.FStructureStructuresFlags);
            _recCount += this.FStructureStructuresFlags.Count;
            this.FStructureStructuresFlags.Clear();

            base.FContext.JsdTile.AddRange(this.FJsdTile);
            _recCount += this.FJsdTile.Count;
            this.FJsdTile.Clear();

            base.FContext.JsdTileTileFlag.AddRange(this.FJsdTileTileFlag);
            _recCount += this.FJsdTileTileFlag.Count;
            this.FJsdTileTileFlag.Clear();

            base.FContext.JsdTileCell.AddRange(this.FJsdTileCell);
            _recCount += this.FJsdTileCell.Count;
            this.FJsdTileCell.Clear();

            return _recCount;
        }

        private SlfRecordJsd LoadJsd(Stream aInput)
        {
            SlfRecordJsd _jsdRecord = new SlfRecordJsd();
            Ja2Data.JsdFile _jsdFile = Ja2Data.JsdFile.Load(aInput);

            this.FJsdFileIsHiDefinition = _jsdFile.IsHighDefenition;

            this.FProfileXSize = Ja2Data.JsdTile.GetProfileXSize(this.FJsdFileIsHiDefinition);
            this.FProfileYSize = Ja2Data.JsdTile.GetProfileYSize(this.FJsdFileIsHiDefinition);

            this.FJsdTileCell = new List<JsdTileCell>(this.FStrCount * FProfileXSize * FProfileYSize);

            _jsdRecord.ID = _jsdFile.FormatId;
            _jsdRecord.NumberOfImageTileLocsStored = _jsdFile.NumberOfImageTileLocsStored;
            _jsdRecord.NumberOfStructuresStored = _jsdFile.NumberOfStructuresStored;
            _jsdRecord.NumberOfStructuresAndOrImages = _jsdFile.NumberOfElements;
            _jsdRecord.StructureDataSize = _jsdFile.StructureDataSize;
            _jsdRecord.Unused = _jsdFile.Unused;

            _jsdRecord.UserId = this.FUserId;
            _jsdRecord.DateCreated = DateTime.Now;

            var _flags = this.FJsdFlags.Where(x => (x.Mask & (byte)_jsdFile.Flags) != 0);
            foreach (Flag _flag in _flags)
            {
                JsdRecordHeaderFlag _recFlag = new JsdRecordHeaderFlag();
                _recFlag.FlagId = (byte)_flag.Id;
                _recFlag.SlfRecordJsd = _jsdRecord;
                this.FJsdHeaderFlags.Add(_recFlag);
            }

            if(_jsdFile.Auxilarity.Length > 0)
            {
                JsdAuxData _auxData = this.LoadJsdAux(_jsdFile.Auxilarity, _jsdFile.TileLocData);
                _auxData.SlfRecordJsd = _jsdRecord;
                this.FJsdAuxData.Add(_auxData);
            }


            if (_jsdFile.Structs.Length > 0)
            {
                foreach (Ja2Data.JsdStruct _struct in _jsdFile.Structs)
                {
                    JsdStructure _structRec = this.LoadStructure(_struct);
                    _structRec.SlfRecordJsd = _jsdRecord;
                    this.FJsdStructures.Add(_structRec);
                }
            }

            return _jsdRecord;
        }

        private JsdAuxData LoadJsdAux(Ja2Data.AuxObjectData[] aAuxObjectData, byte[] aTileLocData)
        {
            JsdAuxData _auxData = new JsdAuxData();
            _auxData.TileLocationData = aTileLocData;

            _auxData.UserId = this.FUserId;
            _auxData.DateCreated = DateTime.Now;

            foreach (Ja2Data.AuxObjectData _auxObject in aAuxObjectData)
            {
                AuxObjectData _auxRec = base.LoadAuxData(_auxObject);
                base.FAuxData.Add(_auxRec);

                AuxDataJsdAux _jsdAux = new AuxDataJsdAux();
                _jsdAux.AuxObjectData = _auxRec;
                _jsdAux.JsdAuxData = _auxData;
                this.FAuxDataJsdAux.Add(_jsdAux);
            }

            return _auxData;
        }

        private JsdStructure LoadStructure(Ja2Data.JsdStruct aStruct)
        {
            JsdStructure _structRec = new JsdStructure();
            _structRec.Armour = aStruct.Armour;
            _structRec.Density = aStruct.Density;
            _structRec.DestructionPartner = aStruct.DestructionPartner;
            _structRec.HitPoints = aStruct.HitPoints;
            _structRec.NumberOfTiles = aStruct.NumberOfTiles;
            _structRec.PartnerDelta = aStruct.PartnerDelta;
            _structRec.StructureNumber = aStruct.StructureNumber;
            _structRec.Unused = aStruct.Unused;
            _structRec.WallOrientation = aStruct.WallOrientation;
            _structRec.ZTileOffsetX = aStruct.ZTileOffsetX;
            _structRec.ZTileOffsetY = aStruct.ZTileOffsetY;

            _structRec.DateCreated = DateTime.Now;
            _structRec.UserId = this.FUserId;

            var _flags = this.FStructureFlags.Where(x => (x.Mask & (uint)aStruct.Flags) != 0);
            foreach (Flag _flag in _flags)
            {
                JsdStructureStructureFlag _structToFlag = new JsdStructureStructureFlag();
                _structToFlag.JsdStructure = _structRec;
                _structToFlag.FlagId = (byte)_flag.Id;
                this.FStructureStructuresFlags.Add(_structToFlag);
            }

            foreach(Ja2Data.JsdTile _tile in aStruct.Tiles)
            {
                JsdTile _tileRec = this.LoadJsdTile(_tile);
                _tileRec.JsdStructure = _structRec;
                this.FJsdTile.Add(_tileRec);
            }

            return _structRec;
        }

        private JsdTile LoadJsdTile(Ja2Data.JsdTile aTile)
        {
            JsdTile _tileRec = new JsdTile();
            _tileRec.PosRelToBase = aTile.PosRelToBase;
            _tileRec.Unused = aTile.Unused;
            _tileRec.VehicleHitLocation = aTile.VehicleHitLocation;
            _tileRec.XPosRelToBase = (short)aTile.XPosRelToBase;
            _tileRec.YPosRelToBase = (short)aTile.YPosRelToBase;

            _tileRec.DateCreated = DateTime.Now;
            _tileRec.UserId = this.FUserId;

            var _flags = this.FTilesFlags.Where(x => (x.Mask & (byte)aTile.Flags) != 0);
            foreach (Flag _flag in _flags)
            {
                JsdTileTileFlag _tileToFlag = new JsdTileTileFlag();
                _tileToFlag.JsdTile = _tileRec;
                _tileToFlag.FlagId = (byte)_flag.Id;
                this.FJsdTileTileFlag.Add(_tileToFlag);
            }

            this.LoadJsdTileCells(aTile.Shape, _tileRec);

            return _tileRec;
        }

        private void LoadJsdTileCells(byte[] aShape, JsdTile aJsdTile)
        {
            for (byte x = 0; x < this.FProfileXSize; x++)
            {
                for (byte y = 0; y < this.FProfileYSize; y++)
                {
                    JsdTileCell _cellsRec = new JsdTileCell();
                    _cellsRec.RowNumber = x;
                    _cellsRec.CellNumber = y;
                    _cellsRec.Value = aShape[x * this.FProfileYSize + y];

                    _cellsRec.JsdTile = aJsdTile;
                    this.FJsdTileCell.Add(_cellsRec);
                }
            }
        }

        public override int Upload(BinaryReader aReader, int aSlfFileId)
        {
            int _count = 0;

            foreach (Ja2Data.SlfFile.Record _file in this.FFiles)
            {
                try
                {
                    aReader.BaseStream.Position = _file.Offset;

                    SlfRecordHeader _recHeader = base.CreateSlfRecordHeader(_file);
                    this.FHeaders.Add(_recHeader);

                    SlfRecordJsd _newJsdRecord = this.LoadJsd(aReader.BaseStream);
                    _newJsdRecord.SlfFileId = aSlfFileId;
                    _newJsdRecord.SlfRecordHeader = _recHeader;
                    FJsdRecords.Add(_newJsdRecord);

                    this.AddRecordsToDataSet();
                    _count += this.SaveChanges();

                }
                catch (Exception _exc)
                {
                    string _excMess = String.Format("Uploading file {0} exception\n", _file.FileName);
                    throw new Exception(_excMess, _exc);
                }
            }

            return _count;
        }
    }
}
