using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Ja2DataDb
{
    public class StciLoader : SlfRecordWithAuxLoader
    {
        public StciLoader(IEnumerable<Ja2Data.SlfFile.Record> aFiles, int aUserId)
            : base(aFiles, aUserId)
        {
            this.FRecords = new List<SlfRecordStci>();
            this.FRecordsRgb = new List<StciRgb>();
            this.FRecordsIndexed = new List<StciIndexed>();
            this.FRecordsFlags = new List<StciRecordStciFlag>(2);
            this.FSubImages = new List<StciIndexedSubImage>(100);
            this.FUnstructuredData = new List<UnstructuredData>(100);

            this.FContext.StciFlags.Load();
            this.FStciFlags = this.FContext.StciFlags.Local
                .Select(x => new Flag() { Id = x.StciFlagsId, Mask = x.Mask.Value })
                .ToList();
        }

        private List<SlfRecordStci> FRecords;
        private List<StciRgb> FRecordsRgb;
        private List<StciRecordStciFlag> FRecordsFlags;
        private List<StciIndexed> FRecordsIndexed;
        private List<StciIndexedSubImage> FSubImages;
        private List<UnstructuredData> FUnstructuredData;

        private List<Flag> FStciFlags;

        public override int AddRecordsToDataSet()
        {
            int _recCount = 0;

            _recCount += base.AddRecordsToDataSet();

            this.FContext.SlfRecordStci.AddRange(this.FRecords);
            _recCount += this.FRecords.Count;
            this.FRecords.Clear();

            this.FContext.StciRgb.AddRange(this.FRecordsRgb);
            _recCount += this.FRecordsRgb.Count;
            this.FRecordsRgb.Clear();

            this.FContext.StciRecordStciFlag.AddRange(this.FRecordsFlags);
            _recCount += this.FRecordsFlags.Count;
            this.FRecordsFlags.Clear();

            this.FContext.StciIndexed.AddRange(this.FRecordsIndexed);
            _recCount += this.FRecordsIndexed.Count;
            this.FRecordsIndexed.Clear();

            this.FContext.StciIndexedSubImage.AddRange(this.FSubImages);
            _recCount += this.FSubImages.Count;
            this.FSubImages.Clear();

            this.FContext.UnstructuredData.AddRange(this.FUnstructuredData);
            _recCount += this.FUnstructuredData.Count;
            this.FUnstructuredData.Clear();

            return _recCount;
        }

        private SlfRecordStci LoadStciHeader(Ja2Data.StciHeader aStciHeader)
        {
            SlfRecordStci _newStciHeader = new SlfRecordStci();
            _newStciHeader.AppDataSize = (int)aStciHeader.AppDataSize;
            _newStciHeader.Depth = aStciHeader.Depth;
            _newStciHeader.Height = aStciHeader.ImageHeight;
            _newStciHeader.ID = aStciHeader.FormatId;
            _newStciHeader.OriginalSize = aStciHeader.OriginalImageSize;
            _newStciHeader.StoredSize = aStciHeader.CompressedImageSize;
            _newStciHeader.TransparentValue = aStciHeader.TransparentColorIndex;
            _newStciHeader.Unused = aStciHeader.Unused;
            _newStciHeader.Width = aStciHeader.ImageWidth;

            _newStciHeader.UserId = this.FUserId;
            _newStciHeader.DateCreated = DateTime.Now;

            var _flags = this.FStciFlags.Where(x => (x.Mask & (byte)aStciHeader.Flags) != 0);

            foreach (Flag _flag in _flags)
            {
                StciRecordStciFlag _recFlag = new StciRecordStciFlag();
                _recFlag.FlagId = (byte)_flag.Id;
                _recFlag.SlfRecordStci = _newStciHeader;

                this.FRecordsFlags.Add(_recFlag);
            }

            return _newStciHeader;
        }

        private StciIndexed LoadStciIndexed(BinaryReader aReader, Ja2Data.StciHeader aHeader)
        {
            StciIndexed _newStciIndexed = new StciIndexed();
            Ja2Data.StciIndexed _stciIndexed = new Ja2Data.StciIndexed(aHeader);
            _stciIndexed.Load(aReader);

            Ja2Data.StciIndexedHeader _subHeader = (Ja2Data.StciIndexedHeader)aHeader.SubHeader;

            _newStciIndexed.BlueDepth = _subHeader.BlueDepth;
            _newStciIndexed.GreenDepth = _subHeader.GreenDepth;
            _newStciIndexed.NumberOfColours = _subHeader.NumberOfColous;
            _newStciIndexed.NumberOfSubImages = (int)_subHeader.NumberOfSubImages;
            _newStciIndexed.RedDepth = _subHeader.RedDepth;
            _newStciIndexed.Unused = _subHeader.Unused;

            _newStciIndexed.DateCreated = DateTime.Now;
            _newStciIndexed.UserId = this.FUserId;

            _newStciIndexed.Palette = _stciIndexed.Palette;

            for (int i = 0; i < _subHeader.NumberOfSubImages; i++)
            {
                StciIndexedSubImage _subImage = this.LoadSubImage(_stciIndexed.Images[i]);
                _subImage.StciIndexed = _newStciIndexed;
                this.FSubImages.Add(_subImage);
            }

            return _newStciIndexed;
        }

        private StciIndexedSubImage LoadSubImage(Ja2Data.StciSubImage aStciSubImage)
        {
            StciIndexedSubImage _subImage = new StciIndexedSubImage();
            if (aStciSubImage.AuxData != null)
            {
                AuxObjectData _auxData = this.LoadAuxData(aStciSubImage.AuxData);

                _subImage.AuxObjectData = _auxData;
                base.FAuxData.Add(_auxData);
            }

            UnstructuredData _data = new UnstructuredData();
            _data.Data = aStciSubImage.ImageData;
            _data.UserId = this.FUserId;
            _data.DateCreated = DateTime.Now;
            this.FUnstructuredData.Add(_data);
            _subImage.UnstructuredData = _data;

            _subImage.DataLength = aStciSubImage.Header.DataLength;
            _subImage.DataOffset = aStciSubImage.Header.DataOffset;
            _subImage.Height = aStciSubImage.Header.Height;
            _subImage.OffsetX = aStciSubImage.Header.OffsetX;
            _subImage.OffsetY = aStciSubImage.Header.OffsetY;
            _subImage.Width = aStciSubImage.Header.Width;

            _subImage.DateCreated = DateTime.Now;
            _subImage.UserId = this.FUserId;

            return _subImage;
        }

        private StciRgb LoadStciRgb(BinaryReader aReader, Ja2Data.StciHeader aHeader)
        {
            StciRgb _newStciRgb = new StciRgb();
            Ja2Data.StciRgb _stciRgb = new Ja2Data.StciRgb(aHeader);

            Ja2Data.StciRgbHeader _subHeader = (Ja2Data.StciRgbHeader)_stciRgb.Header.SubHeader;
            _newStciRgb.AlphaDepth = _subHeader.AlphaDepth;
            _newStciRgb.AlphaMask = (int)_subHeader.AlphaMask;
            _newStciRgb.BlueDepth = _subHeader.BlueDepth;
            _newStciRgb.BlueMask = (int)_subHeader.BlueMask;
            _newStciRgb.GreenDepth = _subHeader.GreenDepth;
            _newStciRgb.GreenMask = (int)_subHeader.GreenMask;
            _newStciRgb.RedDepth = _subHeader.RedDepth;
            _newStciRgb.RedMask = (int)_subHeader.RedMask;

            _newStciRgb.DateCreated = DateTime.Now;
            _newStciRgb.UserId = this.FUserId;

            int _imageSize = (int)_stciRgb.Header.OriginalImageSize;

            UnstructuredData _data = new UnstructuredData();
            _data.Data = aReader.ReadBytes(_imageSize);
            _data.UserId = this.FUserId;
            _data.DateCreated = DateTime.Now;
            this.FUnstructuredData.Add(_data);
            _newStciRgb.UnstructuredData = _data;

            return _newStciRgb;
        }

        public override int Upload(BinaryReader aReader, int aSlfFileId)
        {
            int _count = 0;

            foreach (Ja2Data.SlfFile.Record _file in this.FFiles)
            {
                try
                {
                    aReader.BaseStream.Position = _file.Offset;

                    SlfRecordHeader _recHeader = this.CreateSlfRecordHeader(_file);
                    this.FHeaders.Add(_recHeader);

                    Ja2Data.StciHeader _stciHeader = new Ja2Data.StciHeader();
                    _stciHeader.Read(aReader);

                    SlfRecordStci _stciRecord = this.LoadStciHeader(_stciHeader);
                    _stciRecord.SlfRecordHeader = _recHeader;
                    _stciRecord.SlfFileId = aSlfFileId;
                    this.FRecords.Add(_stciRecord);

                    if (_stciHeader.IsIndexed)
                    {
                        StciIndexed _newStciIndexed = this.LoadStciIndexed(aReader, _stciHeader);
                        _newStciIndexed.SlfRecordStci = _stciRecord;
                        this.FRecordsIndexed.Add(_newStciIndexed);
                    }
                    else
                    {
                        StciRgb _newStciRgb = this.LoadStciRgb(aReader, _stciHeader);
                        _newStciRgb.SlfRecordStci = _stciRecord;
                        this.FRecordsRgb.Add(_newStciRgb);
                    }

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
