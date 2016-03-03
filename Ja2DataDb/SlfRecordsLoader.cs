using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Ja2DataDb
{
    public class SlfRecordLoader
    {
        public SlfRecordLoader(IEnumerable<Ja2Data.SlfFile.Record> aFiles, int aUserId)
        {
            this.FFiles = aFiles;
            this.FContext = new Ja2DataEntities();
            this.FUserId = aUserId;

            this.FFilesCount = aFiles.Count();
            this.FHeaders = new List<SlfRecordHeader>(this.FFilesCount);
        }

        protected int FFilesCount;
        protected IEnumerable<Ja2Data.SlfFile.Record> FFiles;
        protected Ja2DataEntities FContext;
        protected int FUserId;
        protected SlfFile FSlfFile;

        protected List<SlfRecordHeader> FHeaders;

        protected SlfRecordHeader CreateSlfRecordHeader(Ja2Data.SlfFile.Record aRecord)
        {
            SlfRecordHeader _newSlfRecordHeader = new SlfRecordHeader();
            _newSlfRecordHeader.FileNameExtention = aRecord.FileNameExtention;
            _newSlfRecordHeader.FileName = aRecord.FileName;
            _newSlfRecordHeader.FileTime = aRecord.FileTime;
            _newSlfRecordHeader.Length = aRecord.Length;
            _newSlfRecordHeader.Offset = aRecord.Offset;
            _newSlfRecordHeader.Reserved = aRecord.Reserved;
            _newSlfRecordHeader.Reserved2 = aRecord.Reserved2;

            _newSlfRecordHeader.UserId = this.FUserId;
            _newSlfRecordHeader.DateCreated = DateTime.Now;

            return _newSlfRecordHeader;
        }

        public virtual int SaveChanges()
        {
            return this.FContext.SaveChanges();
        }

        public virtual int AddRecordsToDataSet()
        {
            this.FContext = new Ja2DataEntities();
            this.FContext.Configuration.AutoDetectChangesEnabled = false;

            this.FContext.SlfRecordHeader.AddRange(this.FHeaders);
            int _recCount = this.FHeaders.Count;
            this.FHeaders.Clear();
            return _recCount;
        }

        public virtual int Upload(BinaryReader aRader, int aSlfFileId)
        {
            return 0;
        }
    }

    public struct Flag
    {
        public int Id;
        public int Mask;
    }
}
