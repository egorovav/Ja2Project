using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ja2DataDb
{
    public class TextLoader : SlfRecordLoader
    {

        public TextLoader(IEnumerable<Ja2Data.SlfFile.Record> aFiles, int aUserId)
            : base(aFiles, aUserId)
        {
            this.FSlfRecordText = new List<SlfRecordText>(base.FFilesCount);
        }

        private List<SlfRecordText> FSlfRecordText;

        public override int AddRecordsToDataSet()
        {
            int _recCount = 0;

            _recCount += base.AddRecordsToDataSet();

            this.FContext.SlfRecordText.AddRange(this.FSlfRecordText);
            _recCount += this.FSlfRecordText.Count;
            this.FSlfRecordText.Clear();

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

                    SlfRecordText _textRec = new SlfRecordText();

                    _textRec.SlfFileId = aSlfFileId;

                    SlfRecordHeader _recHeader = base.CreateSlfRecordHeader(_file);
                    base.FHeaders.Add(_recHeader);
                    _textRec.TextData = Ja2Data.Common.ByteArrayToString(aReader.ReadBytes((int)_recHeader.Length));
                    _textRec.SlfRecordHeader = _recHeader;
                    _textRec.ID = _file.FileNameExtention;



                    _textRec.UserId = this.FUserId;
                    _textRec.DateCreated = DateTime.Now;

                    this.FSlfRecordText.Add(_textRec);

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
