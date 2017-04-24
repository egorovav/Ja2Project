using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Ja2DataDb
{
    public class SlfRecordWithAuxLoader : SlfRecordLoader
    {
        public SlfRecordWithAuxLoader(IEnumerable<Ja2Data.SlfFile.Record> aFiles, int aUserId)
            : base(aFiles, aUserId)
        {
            this.FAuxData = new List<AuxObjectData>(100);
            this.FAuxDataFlags = new List<AuxObjectAuxFlag>(8);

            this.FContext.AuxDataFlags.Load();
            this.FAuxFlags = this.FContext.AuxDataFlags.Local
                .Select(x => new Flag() { Id = x.AuxDataFlagsId, Mask = x.Mask.Value })
                .ToList();
        }

        protected List<AuxObjectData> FAuxData;
        protected List<AuxObjectAuxFlag> FAuxDataFlags;

        private List<Flag> FAuxFlags;

        protected AuxObjectData LoadAuxData(Ja2Data.AuxObjectData aAuxObjectData)
        {
            AuxObjectData _auxData = new AuxObjectData();
            _auxData.CurrentFrame = aAuxObjectData.CurrentFrame;
            _auxData.NumberOfFrames = aAuxObjectData.NumberOfFrames;
            _auxData.NumberOfTiles = aAuxObjectData.NumberOfTiles;
            _auxData.TileLocIndex = aAuxObjectData.TileLocIndex;
            _auxData.Unused = aAuxObjectData.Unused;
            _auxData.Unused1 = aAuxObjectData.Unused1;
            _auxData.WallOrientation = aAuxObjectData.WallOrientation;

            _auxData.DateCreated = DateTime.Now;
            _auxData.UserId = this.FUserId;

            var _flags = this.FAuxFlags.Where(x => (x.Mask & (int)aAuxObjectData.Flags) != 0);
            foreach (Flag _flag in _flags)
            {
                AuxObjectAuxFlag _auxRecFlag = new AuxObjectAuxFlag();
                _auxRecFlag.AuxObjectData = _auxData;
                _auxRecFlag.FlagId = (byte)_flag.Id;
                this.FAuxDataFlags.Add(_auxRecFlag);
            }

            return _auxData;
        }

        public override int AddRecordsToDataSet()
        {
            int _recCount = 0;
            _recCount += base.AddRecordsToDataSet();

            this.FContext.AuxObjectData.AddRange(this.FAuxData);
            _recCount += this.FAuxData.Count;
            this.FAuxData.Clear();

            this.FContext.AuxObjectAuxFlag.AddRange(this.FAuxDataFlags);
            _recCount += this.FAuxDataFlags.Count;
            this.FAuxDataFlags.Clear();

            return _recCount;
        }
    }
}
