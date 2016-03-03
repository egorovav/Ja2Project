using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace Ja2Data
{
    public partial class SlfFile
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Header
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FileNameSize)]
            public byte[] sLibName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FileNameSize)]
            public byte[] sPathToLibrary;
            public Int32 iEntries;
            public Int32 iUsed;
            public Int16 iSort;
            public Int16 iVersion;
            public byte fContainsSubDirectories;
            public Int32 iReserved;

            public string LibName
            {
                set { this.sLibName = Common.StringToByteArray(value, FileNameSize); }
            }

            public string PathToLibrary
            {
                set { this.sPathToLibrary = Common.StringToByteArray(value, FileNameSize); }
            }
        }

        const int RecordHeaderSize = 280;
        const int FileNameSize = 256;

        const byte FileOk = 0;
        const byte FileDeleted = 255;
        const byte FileOld = 1;
        const byte FileDoesNotExists = 254;

        public SlfFile(string aFullFileName)
        {
            this.FSlfFullFileName = aFullFileName;

            using (FileStream _fs = new FileStream(this.FSlfFullFileName, FileMode.Open, FileAccess.Read))
                LoadHeader(_fs);
        }

        private void LoadHeader(FileStream aInput)
        {
            Deserializer _deserializer = new Deserializer(aInput);
            this.FHeader = (Header)_deserializer.Deserialize(typeof(Header));
            this.FRecords = new Record[this.FHeader.iEntries];

            //Заголовки файлов в конце SLF-архива!!!
            aInput.Seek(-RecordHeaderSize * FHeader.iEntries, SeekOrigin.End);

            for (int i = 0; i < FHeader.iEntries; i++)
            {
                Record.Header _recordHeader =
                    (Record.Header)_deserializer.Deserialize(typeof(Record.Header));

                Record _record = new Record(_recordHeader);
                this.FRecords[i] = _record;
            }
        }

        public SlfFile(SlfFile.Header aHeader, IEnumerable<SlfFile.Record> aRecords)
        {
            this.FHeader = aHeader;
            this.FRecords = new SlfFile.Record[aHeader.iEntries];
            int i = 0;
            foreach (SlfFile.Record _record in aRecords)
            {
                this.FRecords[i] = _record;
                i++;
            }
        }

        public static SlfFile Create(DirectoryInfo aDir, SearchOption aSearchOptions) 
        {
            return SlfFile.Create(aDir, "*.*", aSearchOptions);
        }

        public static SlfFile Create(DirectoryInfo aDir, string aSearchPattern, SearchOption aSearchOptions)
        {
            FileInfo[] _files = aDir.GetFiles(aSearchPattern, aSearchOptions);
            if (_files == null || _files.Length == 0)
                return null;

            SlfFile.Header FHeader = new Header();
            FHeader.LibName = String.Format("{0}.SLF", aDir.Name);

            if (aSearchOptions == SearchOption.AllDirectories)
                FHeader.PathToLibrary = String.Format("{0}\\", aDir.Name);
            else
                FHeader.PathToLibrary = String.Empty;

            FHeader.iEntries = _files.Length;
            FHeader.iUsed = _files.Length;
            FHeader.iSort = -1;
            FHeader.iVersion = 512;

            DirectoryInfo[] _subDirs = aDir.GetDirectories();

            FHeader.fContainsSubDirectories = (byte)(_subDirs.Length > 0 ? 1 : 0);

            int _dirLength = aDir.FullName.Length + 1;
            SlfFile.Record[] _records = new SlfFile.Record[_files.Length];
            SlfFile.Record[] FRecords = new SlfFile.Record[FHeader.iEntries];
            for (int i = 0; i < _files.Length; i++)
            {
                FileInfo _file = _files[i];

                SlfFile.Record.Header _recHeader = new Record.Header();
                _recHeader.FileName = _file.FullName.Substring(_dirLength);
                _recHeader.FileTime = _file.CreationTimeUtc;
                _recHeader.uiLength = (uint)_file.Length;
                SlfFile.Record _rec = new Record(_recHeader);

                using (FileStream _fs = new FileStream(_file.FullName, FileMode.Open))
                {
                    _rec.LoadData(_fs);
                }

                FRecords[i] = _rec;
                string _key = _rec.FileName;
                if (_rec.State != FileOk)
                    _key = String.Format("{0}_{1}", _rec.FileName, _rec.State);
            }

            SlfFile _slf = new SlfFile(FHeader, FRecords);
            return _slf;
        }

        private string FSlfFullFileName;
        public string SlfFileName
        {
            get { return Path.GetFileName(this.FSlfFullFileName); }
        }

        private Header FHeader;

        private SlfFile.Record[] FRecords;
        public SlfFile.Record[] Records
        {
            get { return this.FRecords; }
        }

        public void LoadRecords()
        {
            using (FileStream _fs = new FileStream(this.FSlfFullFileName, FileMode.Open, FileAccess.Read))
            {
                this.LoadRecords(_fs);
            }
        }

        public void LoadRecords(FileStream aInput)
        {
            foreach (SlfFile.Record _rec in this.FRecords)
                _rec.LoadData(aInput);
        }

        private void Save(SlfFile.Header aHeader, SlfFile.Record[] aRecords, Stream aOutput)
        {
            Serializer _serializer = new Serializer(aOutput);
            _serializer.Serialize(aHeader);


            //Сначала пишем данные.
            //Заголовки должны быть отсортированы по именам файлов. См. SlfFile.Record.CompareTo.
            Array.Sort(aRecords);
            foreach (SlfFile.Record _rec in this.FRecords)
                _rec.WriteData(aOutput);



            //Заголовки файлов в конце SLF-архива!!!

            foreach (SlfFile.Record _rec in aRecords)
                _rec.WriteHeader(_serializer);
        }

        public void Save(string aFileName)
        {
            using (FileStream _fs = new FileStream(aFileName, FileMode.Create, FileAccess.Write))
            {
                this.Save(this.FHeader, this.FRecords, _fs);
            }
        }

        public void Save()
        {
            this.Save(this.FSlfFullFileName);
        }

        public int Extract(bool aIsRewrite)
        {
            this.LoadRecords();

            string _dirName = Path.Combine(Path.GetDirectoryName(this.FSlfFullFileName), this.PathToLibrary);
            if (!Directory.Exists(_dirName))
            {
                Directory.CreateDirectory(_dirName);
            }

            int _count = 0;

            for (int i = 0; i < this.Entries; i++)
            {
                SlfFile.Record _entry = this.FRecords[i];
                string _fileName = Path.Combine(_dirName, _entry.FileName);
                if (Path.GetFileNameWithoutExtension(_fileName) == Path.GetFileName(_fileName))
                    continue;
                string _subDirName = Path.GetDirectoryName(_fileName);
                if (!Directory.Exists(_subDirName))
                {
                    Directory.CreateDirectory(_subDirName);
                }

                if (!aIsRewrite && File.Exists(_fileName))
                    continue;

                using (FileStream _ofs = new FileStream(_fileName, FileMode.Create))
                {
                    _entry.WriteData(_ofs);
                }

                _count++;
            }

            return _count;
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine(String.Format("sLibName - {0}", this.LibName));
            _sb.AppendLine(String.Format("sPathToLibrary - {0}", this.PathToLibrary));
            _sb.AppendLine(String.Format("iEntries - {0}", this.FHeader.iEntries));
            _sb.AppendLine(String.Format("iUsed - {0}", this.FHeader.iUsed));
            _sb.AppendLine(String.Format("iSort - {0}", this.FHeader.iSort));
            _sb.AppendLine(String.Format("iVersion - {0}", this.FHeader.iVersion));
            _sb.AppendLine(String.Format("fContainsSubDirectories - {0}", this.FHeader.fContainsSubDirectories));
            _sb.AppendLine(String.Format("iReserved - {0}", this.FHeader.iReserved));
            for (int i = 0; i < this.FRecords.Length; i++)
            {
                Record _srh = this.FRecords[i];
                _sb.AppendLine("-------------------");
                _sb.AppendLine(_srh.ToString());
            }
            return _sb.ToString();
        }

        public string ToShortString()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine(String.Format("sLibName - {0}", this.LibName));
            _sb.AppendLine(String.Format("sPathToLibrary - {0}", this.PathToLibrary));
            _sb.AppendLine(String.Format("iEntries - {0}", this.FHeader.iEntries));
            _sb.AppendLine("-------------------");
            for (int i = 0; i < this.FRecords.Length; i++)
            {
                Record _srh = this.FRecords[i];
                _sb.AppendLine(_srh.FileName);
            }
            return _sb.ToString();
        }

        #region = Properties =

        public string LibName
        {
            get { return Common.ByteArrayToString(this.FHeader.sLibName); }
            //set { this.FHeader.sLibName = Common.StringToByteArray(value, FileNameSize); }
        }

        public string PathToLibrary
        {
            get { return Common.ByteArrayToString(this.FHeader.sPathToLibrary); }
            //set { this.FHeader.sPathToLibrary = Common.StringToByteArray(value, FileNameSize); }
        }

        public int Entries
        {
            get { return this.FHeader.iEntries; }
            //set { this.FHeader.iEntries = value; }
        }

        public int Used
        {
            get { return this.FHeader.iUsed; }
            //set { this.FHeader.iUsed = value; }
        }

        public short Sort
        {
            get { return this.FHeader.iSort; }
            //set { this.FHeader.iSort = value; }
        }

        public short Version
        {
            get { return this.FHeader.iVersion; }
            //set { this.FHeader.iVersion = value; }
        }

        public byte ContainsSubDirectories
        {
            get { return this.FHeader.fContainsSubDirectories; }
            //set { this.FHeader.fContainsSubDirectories = value; }
        }

        public int Reserved
        {
            get { return this.FHeader.iReserved; }
            //set { this.FHeader.iReserved = value; }
        }

        #endregion
    }
}
