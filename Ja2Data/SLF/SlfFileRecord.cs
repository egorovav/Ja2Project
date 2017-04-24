using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Ja2Data
{
    public partial class SlfFile
    {
        public class Record : IComparable
        {
            [StructLayout(LayoutKind.Sequential, Size = RecordHeaderSize)]
            public struct Header
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = FileNameSize)]
                public byte[] sFileName;
                public UInt32 uiOffset;
                public UInt32 uiLength;
                public byte ubState;
                public byte ubReserved;
                public Int32 dwLowDateTime;
                public Int32 dwHighDateTime;
                public UInt16 usReserved2;

                public string FileName
                {
                    set { this.sFileName = Common.StringToByteArray(value, FileNameSize); }
                }

                public DateTime FileTime
                {
                    set
                    {
                        long _fileTime = value.ToFileTime();
                        this.dwHighDateTime = (int)(_fileTime >> 32);
                        this.dwLowDateTime = (int)_fileTime;
                    }
                }
            }

            public Record(Header aHeaderData)
            {
                this.FHeader = aHeaderData;
            }

            private Header FHeader;

            private byte[] FData;
            public byte[] Data
            {
                get { return this.FData; }
                set
                {
                    this.FData = value;
                    this.FHeader.uiLength = (uint)value.Length;
                }
            }

            public string FileName
            {
                get { return Common.ByteArrayToString(this.FHeader.sFileName); }
            }

            public uint Offset
            {
                get { return this.FHeader.uiOffset; }
            }

            public uint Length
            {
                get { return this.FHeader.uiLength; }
            }

            public byte State
            {
                get { return this.FHeader.ubState; }
            }

            public byte Reserved
            {
                get { return this.FHeader.ubReserved; }
            }

            public DateTime FileTime
            {
                get
                {
                    long _left = this.FHeader.dwHighDateTime;
                    _left <<= 32;
                    return DateTime.FromFileTime(_left + this.FHeader.dwLowDateTime);
                }
            }

            public ushort Reserved2
            {
                get { return this.FHeader.usReserved2; }
            }

            private string FDataTypeIdString = null;
            public string FileNameExtention
            {
                get 
                {
                    if (this.FDataTypeIdString == null)
                    {
                        string _extString = Path.GetExtension(this.FileName);
                        this.FDataTypeIdString = _extString.ToUpper();
                    }
                    return this.FDataTypeIdString;
                }
            }

            public void LoadData(Stream aInput)
            {
                this.FData = new byte[this.FHeader.uiLength];
                aInput.Position = this.FHeader.uiOffset;
                aInput.Read(this.FData, 0, (int)this.FHeader.uiLength);
            }

            public void WriteHeader(Serializer aSerizlizer)
            {
                aSerizlizer.Serialize(this.FHeader);
            }

            public void WriteData(Stream aOutput)
            {
                this.FHeader.uiOffset = (uint)aOutput.Position;
                this.FHeader.uiLength = (uint)this.FData.Length;
                aOutput.Write(this.FData, 0, this.FData.Length);
            }

            public override string ToString()
            {
                StringBuilder _sb = new StringBuilder();
                _sb.AppendLine(String.Format("sFileName - {0}", this.FileName));
                _sb.AppendLine(String.Format("uiOffset - {0}", this.FHeader.uiOffset));
                _sb.AppendLine(String.Format("uiLength - {0}", this.FHeader.uiLength));
                _sb.AppendLine(String.Format("ubState - {0}", this.FHeader.ubState));
                _sb.AppendLine(String.Format("ubReserved - {0}", this.FHeader.ubReserved));
                _sb.AppendLine(String.Format("FileTime - {0}", this.FileTime));
                _sb.AppendLine(String.Format("usReserved2 - {0}", this.FHeader.usReserved2));
                return _sb.ToString();
            }

            // Перед сравнением имена файлов приводим к нижнему регистру.
            public int CompareTo(object obj)
            {
                SlfFile.Record _rec = (SlfFile.Record)obj;

                byte[] _xBytes = this.FHeader.sFileName;
                byte[] _yBytes = _rec.FHeader.sFileName;

                int _byteCompareResult = 0;
                int _minLength = Math.Min(_xBytes.Length, _yBytes.Length);
                for (int i = 0; i < _minLength; i++)
                {
                    // to lower x
                    byte _xByte = _xBytes[i];
                    if (_xByte > 64 && _xByte < 91)
                        _xByte += 32;
                    // to lower y
                    byte _yByte = _yBytes[i];
                    if (_yByte > 64 && _yByte < 91)
                        _yByte += 32;

                    _byteCompareResult = _xByte.CompareTo(_yByte);
                    if (_byteCompareResult != 0)
                        break;
                }

                return _byteCompareResult;
            }
        }
    }
}
