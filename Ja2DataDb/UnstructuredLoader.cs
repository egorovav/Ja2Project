using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ja2DataDb
{
    public class UnstructuredLoader : SlfRecordLoader
    {
        public UnstructuredLoader(IEnumerable<Ja2Data.SlfFile.Record> aFiles, int aUserId)
            : base(aFiles, aUserId)
        {
            this.FSlfRecords = new List<SlfRecord>(base.FFilesCount);
            this.FUnstructuredData = new List<UnstructuredData>(base.FFilesCount);
        }

        private List<SlfRecord> FSlfRecords;
        private List<UnstructuredData> FUnstructuredData;

        public override int AddRecordsToDataSet()
        {
            int _recCount = 0;

            _recCount += base.AddRecordsToDataSet();

            this.FContext.SlfRecord.AddRange(this.FSlfRecords);
            _recCount += this.FSlfRecords.Count;
            this.FSlfRecords.Clear();

            this.FContext.UnstructuredData.AddRange(this.FUnstructuredData);
            _recCount += this.FUnstructuredData.Count;
            this.FUnstructuredData.Clear();

            return _recCount;
        }

        public override int Upload(BinaryReader aReader, int aSlfFileId)
        {
            int _count = 0;

            foreach (Ja2Data.SlfFile.Record _file in this.FFiles)
            {
                try
                {
                    aReader.BaseStream.Position = _file.Offset;

                    SlfRecord _slfRec = new SlfRecord();
                    UnstructuredData _data = new UnstructuredData();
                    _data.Data = aReader.ReadBytes((int)_file.Length);
                    _data.UserId = this.FUserId;
                    _data.DateCreated = DateTime.Now;
                    this.FUnstructuredData.Add(_data);
                    _slfRec.UnstructuredData = _data;

                    _slfRec.SlfFileId = aSlfFileId;

                    SlfRecordHeader _recHeader = base.CreateSlfRecordHeader(_file);
                    base.FHeaders.Add(_recHeader);
                    _slfRec.SlfRecordHeader = _recHeader;
                    _slfRec.ID = _file.FileNameExtention;

                    _slfRec.UserId = this.FUserId;
                    _slfRec.DateCreated = DateTime.Now;

                    this.FSlfRecords.Add(_slfRec);

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
