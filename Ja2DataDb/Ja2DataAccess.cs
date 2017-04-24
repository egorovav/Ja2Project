using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ja2DataDb
{
    public class Ja2DataUploader : IDisposable
    {
        public Ja2DataUploader(int aUserId)
        {
            this.FUserId = aUserId;
            this.FContext = new Ja2DataEntities();
            this.FContext.Configuration.AutoDetectChangesEnabled = false;
        }

        private int FUserId;
        private Ja2DataEntities FContext;

        public List<string> GetSlfFilesNames(int aDataInfoId)
        {
            return this.FContext.SlfFile.Where(x => x.DataInfoId == aDataInfoId).Select(x => x.FileName).ToList();
        }

        SlfFile FSlfFile;
        LoaderFactory FLoaderFactory;
        List<JsdCell> FJsdCells;

        public SlfInfo UploadSlfFile(string aFileName, int aDataInfoId)
        {
            Ja2Data.SlfFile _slf = new Ja2Data.SlfFile(aFileName);
            this.FSlfFile = this.CreateSlfFile(_slf, aDataInfoId);
            this.FContext.SlfFile.Add(this.FSlfFile);
            this.FContext.SaveChanges();

            this.FLoaderFactory = new LoaderFactory(_slf, this.FUserId, this.FContext);
            int _rowsCounter = 0;
            using (FileStream _input = new FileStream(aFileName, FileMode.Open))
            using (BinaryReader _reader = new BinaryReader(_input))
            {
                foreach (SlfRecordLoader _loader in this.FLoaderFactory.Loaders)
                {
                    _rowsCounter += _loader.Upload(_reader, this.FSlfFile.SlfFileId);
                }
            }

            return new SlfInfo(this.FSlfFile, _rowsCounter);
        }

        public SlfInfo DownloadSlfFile(string aFileName, DateTime aDateCreated, int aDataInfoId)
        {
            this.FContext = new Ja2DataEntities();
            this.FContext.Configuration.AutoDetectChangesEnabled = false;

            string _fileName = Path.GetFileName(aFileName);

            SlfFile _slfRec = this.FContext.SlfFile
                .Where(x => x.FileName.ToUpper() == _fileName.ToUpper() &&
                    x.UserId == this.FUserId &&
                    x.DateCreated > aDateCreated &&
                    x.DataInfoId == aDataInfoId)
                .FirstOrDefault();

            Ja2Data.SlfFile.Header _slfHeader = new Ja2Data.SlfFile.Header();
            _slfHeader.fContainsSubDirectories = _slfRec.ContainsSubDirectories;
            _slfHeader.iEntries = _slfRec.EntriesCount;
            _slfHeader.iReserved = _slfRec.Reserved;
            _slfHeader.iSort = _slfRec.Sort;
            _slfHeader.iUsed = _slfRec.Used;
            _slfHeader.iVersion = _slfRec.Version;
            _slfHeader.LibName = _slfRec.LibName;
            _slfHeader.PathToLibrary = _slfRec.PathToLibrary;

            List<Ja2Data.SlfFile.Record> _records = new List<Ja2Data.SlfFile.Record>(_slfRec.EntriesCount);

            foreach (SlfRecordJsd _jsdRecord in _slfRec.SlfRecordJsd)
            {
                Ja2Data.SlfFile.Record _rec = this.DownloadJsdRecord(_jsdRecord.SlfRecordJsdId);
                _records.Add(_rec);
            }

            foreach (SlfRecord _unstructuredRec in _slfRec.SlfRecord)
            {
                Ja2Data.SlfFile.Record _rec = this.DownloadUnstreucturedRecord(_unstructuredRec);
                _records.Add(_rec);
            }

            foreach (SlfRecordStci _stciRecord in _slfRec.SlfRecordStci)
            {
                Ja2Data.SlfFile.Record _rec = this.DownloadStiRecord(_stciRecord.SlfRecordId);
                _records.Add(_rec);
            }

            foreach (SlfRecordText _txtRecord in _slfRec.SlfRecordText)
            {
                Ja2Data.SlfFile.Record _rec = this.DownloadTxtRecord(_txtRecord);
                _records.Add(_rec);
            }

            Ja2Data.SlfFile _slfFile = new Ja2Data.SlfFile(_slfHeader, _records);

            _slfFile.Save(aFileName);

            return new SlfInfo(_slfRec, 0);
        }

        private Ja2Data.SlfFile.Record DownloadStiRecord(long aStiRecordId)
        {
            Ja2DataEntities _context = new Ja2DataEntities();
            SlfRecordStci _stiRec = _context.SlfRecordStci.First(x => x.SlfRecordId == aStiRecordId);
            Ja2Data.SlfFile.Record _stiFile = this.DownloadStiRecord(_stiRec);
            _context = null;
            return _stiFile;
        }

        private Ja2Data.SlfFile.Record DownloadJsdRecord(long aJsdRecordId)
        {
            Ja2DataEntities _context = new Ja2DataEntities();
            SlfRecordJsd _jsdRec = _context.SlfRecordJsd.First(x => x.SlfRecordJsdId == aJsdRecordId);
            Ja2Data.SlfFile.Record _jsdFile = this.DownloadJsdRecord(_jsdRec);
            _context = null;
            return _jsdFile;
        }

        private Ja2Data.SlfFile.Record DownloadTxtRecord(SlfRecordText aTxtRecord)
        {
            Ja2Data.SlfFile.Record.Header _header = this.DownloadSlfRecordHeader(aTxtRecord.SlfRecordHeader);
            Ja2Data.SlfFile.Record _rec = new Ja2Data.SlfFile.Record(_header);
            _rec.Data = Ja2Data.Common.StringToByteArray(aTxtRecord.TextData, (int)_header.uiLength);
            return _rec;
        }

        private Ja2Data.SlfFile.Record DownloadJsdRecord(SlfRecordJsd aJsdRecord)
        {
            this.FJsdCells = new List<JsdCell>(100 * aJsdRecord.NumberOfStructuresStored);

            string _queryString = @"Select T.JsdTileId, C.RowNumber, C.CellNumber, Value
	from JsdTileCell C
	inner join JsdTile T on C.JsdTileId = T.JsdTileId
	inner join JsdStructure S on S.JsdStructureId = T.JsdStructureId
	inner join SlfRecordJsd SRJ on SRJ.SlfRecordJsdId = S.SlfRecordJsdId
	where SRJ.SlfRecordJsdId = @JsdFileId";

            DbRawSqlQuery _query = this.FContext.Database.SqlQuery(typeof(JsdCell), _queryString,
               new SqlParameter("@JsdFileId", aJsdRecord.SlfRecordJsdId));

            var _enum = _query.GetEnumerator();
            while (_enum.MoveNext())
            {
                this.FJsdCells.Add(((JsdCell)_enum.Current));
            }

            Ja2Data.SlfFile.Record.Header _header = this.DownloadSlfRecordHeader(aJsdRecord.SlfRecordHeader);
            Ja2Data.SlfFile.Record _jsdRec = new Ja2Data.SlfFile.Record(_header);
            _jsdRec.Data = new byte[_header.uiLength];

            int _flags = 0;
            foreach (JsdRecordHeaderFlag _flag in aJsdRecord.JsdRecordHeaderFlag)
            {
                _flags |= _flag.JsdHeaderFlags.Mask.Value;
            }

            Ja2Data.JsdFile.Header _jsdHeader = new Ja2Data.JsdFile.Header(
                aJsdRecord.ID,
                (ushort)aJsdRecord.NumberOfStructuresAndOrImages,
                (ushort)aJsdRecord.NumberOfStructuresStored,
                (ushort)aJsdRecord.StructureDataSize,
                (byte)_flags, 
                aJsdRecord.Unused,
                (ushort)aJsdRecord.NumberOfImageTileLocsStored
                );

            Ja2Data.JsdFile _jsdFile = new Ja2Data.JsdFile(_jsdHeader);

            if (aJsdRecord.JsdAuxData.Count > 0)
            {
                JsdAuxData _jsdAuxData = aJsdRecord.JsdAuxData.First();

                if(aJsdRecord.NumberOfImageTileLocsStored > 0)
                    Array.Copy(_jsdAuxData.TileLocationData, _jsdFile.TileLocData, _jsdFile.TileLocData.Length);

                int i = 0;
                foreach (AuxDataJsdAux _aux in _jsdAuxData.AuxDataJsdAux)
                {
                    _jsdFile.Auxilarity[i] = this.DownloadAuxObjectData(_aux.AuxObjectData);
                    i++;
                }
            }

            if (aJsdRecord.JsdStructure.Count > 0)
            {
                int i = 0;
                foreach (JsdStructure _struct in aJsdRecord.JsdStructure)
                {
                    _jsdFile.Structs[i] = this.DownloadJsdStructure(_struct, _jsdFile.IsHighDefenition);
                    i++;
                }
            }

            using (MemoryStream _ms = new MemoryStream(_jsdRec.Data))
            {
                _jsdFile.Save(_ms);
            }

            return _jsdRec;
        }

        private Ja2Data.JsdStruct DownloadJsdStructure(JsdStructure aStructRecrod, bool aIsHiDefenition)
        {
            Ja2Data.JsdStruct _struct = new Ja2Data.JsdStruct(aIsHiDefenition);

            uint _flags = 0;
            foreach (JsdStructureStructureFlag _flag in aStructRecrod.JsdStructureStructureFlag)
            {
                _flags |= (uint)_flag.JsdStructureFlags.Mask.Value;
            }
            _struct.Flags = (Ja2Data.JsdStruct.JsdStructureFlags)_flags;

            _struct.Armour = aStructRecrod.Armour;
            _struct.Density = aStructRecrod.Density;
            _struct.DestructionPartner = (sbyte)aStructRecrod.DestructionPartner;
            _struct.HitPoints = aStructRecrod.HitPoints;
            _struct.PartnerDelta = (sbyte)aStructRecrod.PartnerDelta;
            _struct.StructureNumber = (ushort)aStructRecrod.StructureNumber;
            _struct.Unused = aStructRecrod.Unused;
            _struct.WallOrientation = aStructRecrod.WallOrientation;
            _struct.ZTileOffsetX = (sbyte)aStructRecrod.ZTileOffsetX;
            _struct.ZTileOffsetY = (sbyte)aStructRecrod.ZTileOffsetY;
            _struct.NumberOfTiles = aStructRecrod.NumberOfTiles;

            int i = 0;
            foreach (JsdTile _tileRec in aStructRecrod.JsdTile)
            {
                _struct.Tiles[i] = this.DownloadJsdTile(_tileRec, aIsHiDefenition);
                i++;
            }

            return _struct;
        }

        private Ja2Data.JsdTile DownloadJsdTile(JsdTile aTileRec, bool aIsHiDefenition)
        {
            Ja2Data.JsdTile _tile = new Ja2Data.JsdTile(aIsHiDefenition);
            int _flags = 0;
            foreach (JsdTileTileFlag _flag in aTileRec.JsdTileTileFlag)
            {
                _flags |= _flag.JsdTileFlags.Mask.Value;
            }
            _tile.Flags = (Ja2Data.JsdTile.JsdTileFlags)((byte)_flags);

            _tile.PosRelToBase = aTileRec.PosRelToBase;
            _tile.Unused = aTileRec.Unused;
            _tile.VehicleHitLocation = aTileRec.VehicleHitLocation;
            _tile.XPosRelToBase = (sbyte)aTileRec.XPosRelToBase;
            _tile.YPosRelToBase = (sbyte)aTileRec.YPosRelToBase; 
                
            List<JsdCell> _cells = this.FJsdCells
                .Where(x => x.JsdTileId == aTileRec.JsdTileId)
                .ToList();

            _cells.Sort();

            _tile.Shape = _cells.Select(x => x.Value).ToArray();

            return _tile;
        }

        private Ja2Data.SlfFile.Record DownloadStiRecord(SlfRecordStci aStciRecord)
        {
            Ja2Data.SlfFile.Record.Header _header = this.DownloadSlfRecordHeader(aStciRecord.SlfRecordHeader);
            Ja2Data.SlfFile.Record _rec = new Ja2Data.SlfFile.Record(_header);
            if (aStciRecord.StciIndexed.Count != 0)
                _rec.Data = this.DownloadStiIndexed(aStciRecord, (int)_header.uiLength);
            else if (aStciRecord.StciRgb.Count != 0)
                _rec.Data = this.DownloadStciRgb(aStciRecord, (int)_header.uiLength);
            return _rec;
        }

        private byte[] DownloadStciRgb(SlfRecordStci aStciRecord, int aDataLength)
        {
            Ja2Data.StciHeader _header = DownloadStciHeader(aStciRecord);

            StciRgb _rgbRec = aStciRecord.StciRgb.First();
            Ja2Data.StciRgbHeader _subHeader = new Ja2Data.StciRgbHeader();
            _subHeader.AlphaDepth = _rgbRec.AlphaDepth;
            _subHeader.AlphaMask = (uint)_rgbRec.AlphaMask;
            _subHeader.BlueDepth = _rgbRec.BlueDepth;
            _subHeader.BlueMask = (uint)_rgbRec.BlueMask;
            _subHeader.GreenDepth = _rgbRec.GreenDepth;
            _subHeader.GreenMask = (uint)_rgbRec.GreenMask;
            _subHeader.RedDepth = _rgbRec.RedDepth;
            _subHeader.RedMask = (uint)_rgbRec.RedMask;

            _header.SubHeader = _subHeader;

            Ja2Data.StciRgb _stciRgb = new Ja2Data.StciRgb(_header);
            _stciRgb.ImageData = _rgbRec.UnstructuredData.Data;

            byte[] _buffer = new byte[aDataLength];
            using (MemoryStream _ms = new MemoryStream(_buffer))
            {
                _stciRgb.Save(_ms);
            }
            return _buffer;
        }

        private static Ja2Data.StciHeader DownloadStciHeader(SlfRecordStci aStciRecord)
        {
            Ja2Data.StciHeader _header = new Ja2Data.StciHeader();
            _header.AppDataSize = (uint)aStciRecord.AppDataSize;
            _header.CompressedImageSize = aStciRecord.StoredSize;
            _header.Depth = aStciRecord.Depth;

            int _flags = 0;
            foreach (StciRecordStciFlag _flag in aStciRecord.StciRecordStciFlag)
            {
                _flags |= _flag.StciFlags.Mask.Value;
            }

            _header.Flags = (Ja2Data.StciFlags)_flags;
            _header.ImageHeight = aStciRecord.Height;
            _header.ImageWidth = aStciRecord.Width;
            _header.TransparentColorIndex = aStciRecord.TransparentValue;
            _header.OriginalImageSize = (uint)aStciRecord.OriginalSize;
            _header.Unused = aStciRecord.Unused;
            return _header;
        }

        private byte[] DownloadStiIndexed(SlfRecordStci aStciRecord, int aDataLength)
        {
            Ja2Data.StciHeader _header = DownloadStciHeader(aStciRecord);

            StciIndexed _indexedRec = aStciRecord.StciIndexed.First();
            Ja2Data.StciIndexedHeader _subHeader = new Ja2Data.StciIndexedHeader((UInt16)_indexedRec.NumberOfSubImages);
            _subHeader.BlueDepth = _indexedRec.BlueDepth;
            _subHeader.GreenDepth = _indexedRec.GreenDepth;
            _subHeader.RedDepth = _indexedRec.RedDepth;
            _subHeader.NumberOfColous = (uint)_indexedRec.NumberOfColours;

            _header.SubHeader = _subHeader;

            Ja2Data.StciSubImage[] _subImages = new Ja2Data.StciSubImage[_indexedRec.NumberOfSubImages];
            int i = 0;
            foreach (StciIndexedSubImage _subImage in _indexedRec.StciIndexedSubImage)
            {
                _subImages[i] = this.DownloadStciSubimage(_subImage);
                i++;
            }

            Ja2Data.StciIndexed _stciIndexed = new Ja2Data.StciIndexed(_header, _indexedRec.Palette, _subImages);

            byte[] _buffer = new byte[aDataLength];
            using (MemoryStream _ms = new MemoryStream(_buffer))
            {
                _stciIndexed.Save(_ms);
            }
            return _buffer;
        }

        private Ja2Data.StciSubImage DownloadStciSubimage(StciIndexedSubImage aSubImage)
        {
            Ja2Data.StciSubImageHeader _header = new Ja2Data.StciSubImageHeader();
            _header.DataLength = (uint)aSubImage.DataLength;
            _header.DataOffset = (uint)aSubImage.DataOffset;
            _header.Height = (ushort)aSubImage.Height;
            _header.OffsetX = (short)aSubImage.OffsetX;
            _header.OffsetY = (short)aSubImage.OffsetY;
            _header.Width = (ushort)aSubImage.Width;

            Ja2Data.StciSubImage _stciSubImage = new Ja2Data.StciSubImage(_header);
            _stciSubImage.ImageData = aSubImage.UnstructuredData.Data;
            if (aSubImage.AuxObjectData != null)
                _stciSubImage.AuxData = this.DownloadAuxObjectData(aSubImage.AuxObjectData);

            return _stciSubImage;
        }

        private Ja2Data.AuxObjectData DownloadAuxObjectData(AuxObjectData aAuxObjectData)
        {
            Ja2Data.AuxObjectData _auxData = new Ja2Data.AuxObjectData();
            _auxData.CurrentFrame = aAuxObjectData.CurrentFrame;

            int _flags = 0;
            foreach (AuxObjectAuxFlag _flag in aAuxObjectData.AuxObjectAuxFlag)
            {
                _flags |= _flag.AuxDataFlags.Mask.Value;
            }

            _auxData.Flags = (Ja2Data.AuxObjectFlags)((byte)_flags);
            _auxData.NumberOfFrames = aAuxObjectData.NumberOfFrames;
            _auxData.NumberOfTiles = aAuxObjectData.NumberOfTiles;
            _auxData.TileLocIndex = (ushort)aAuxObjectData.TileLocIndex;
            _auxData.Unused = aAuxObjectData.Unused;
            _auxData.Unused1 = aAuxObjectData.Unused1;
            _auxData.WallOrientation = aAuxObjectData.WallOrientation;

            return _auxData;
        }

        private Ja2Data.SlfFile.Record DownloadUnstreucturedRecord(SlfRecord aUnstructuredRec)
        {
            Ja2Data.SlfFile.Record.Header _header = this.DownloadSlfRecordHeader(aUnstructuredRec.SlfRecordHeader);
            Ja2Data.SlfFile.Record _rec = new Ja2Data.SlfFile.Record(_header);
            _rec.Data = aUnstructuredRec.UnstructuredData.Data;
            return _rec;
        }

        private Ja2Data.SlfFile.Record.Header DownloadSlfRecordHeader(SlfRecordHeader aSlfRecordHeader)
        {
            Ja2Data.SlfFile.Record.Header _header = new Ja2Data.SlfFile.Record.Header();
            _header.FileName = aSlfRecordHeader.FileName;
            _header.FileTime = aSlfRecordHeader.FileTime;
            _header.ubReserved = aSlfRecordHeader.Reserved;
            _header.ubState = aSlfRecordHeader.State;
            _header.uiLength = (uint)aSlfRecordHeader.Length;
            _header.uiOffset = (uint)aSlfRecordHeader.Offset;
            _header.usReserved2 = (ushort)aSlfRecordHeader.Reserved2;
            return _header;
        }

        private SlfFile CreateSlfFile(Ja2Data.SlfFile aSlf, int aDataInfoId)
        {
            SlfFile _newSlfFileDb = new SlfFile();
            _newSlfFileDb.ContainsSubDirectories = aSlf.ContainsSubDirectories;
            _newSlfFileDb.EntriesCount = aSlf.Entries;
            _newSlfFileDb.LibName = aSlf.LibName;
            _newSlfFileDb.PathToLibrary = aSlf.PathToLibrary;
            _newSlfFileDb.Reserved = aSlf.Reserved;
            _newSlfFileDb.Sort = aSlf.Sort;
            _newSlfFileDb.Used = aSlf.Used;
            _newSlfFileDb.Version = aSlf.Version;
            _newSlfFileDb.FileName = aSlf.SlfFileName;
            _newSlfFileDb.DataInfoId = aDataInfoId;

            _newSlfFileDb.UserId = this.FUserId;
            _newSlfFileDb.DateCreated = DateTime.Now;

            return _newSlfFileDb;
        }


        public void Dispose()
        {
            this.FContext.Dispose();
        }
    }

    public class SlfInfo
    {
        public SlfInfo(SlfFile aSlfFile, int aRowsCount)
        {
            this.DateCreated = aSlfFile.DateCreated;
            this.UserId = aSlfFile.UserId;
            this.FileName = aSlfFile.FileName;
            this.LibName = aSlfFile.LibName;
            this.PathToLibrary = aSlfFile.PathToLibrary;
            this.EntriesCount = aSlfFile.EntriesCount;
            this.Used = aSlfFile.Used;
            this.Sort = aSlfFile.Sort;
            this.Version = aSlfFile.Version;
            this.ContainsSubDirectories = aSlfFile.ContainsSubDirectories;
            this.Reserved = aSlfFile.Reserved;

            this.RowsInserted = aRowsCount;
        }

        public System.DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public string LibName { get; set; }
        public string PathToLibrary { get; set; }
        public int EntriesCount { get; set; }
        public int Used { get; set; }
        public short Sort { get; set; }
        public short Version { get; set; }
        public byte ContainsSubDirectories { get; set; }
        public int Reserved { get; set; }

        public int RowsInserted { get; set; }
    }
}
