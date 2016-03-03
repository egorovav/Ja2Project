using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ja2Data
{
	public class Common
	{

        public static Encoding DefaultEncoding =  ASCIIEncoding.Default;

        public static string ByteArrayToString(byte[] aBytes)
        {
            return ByteArrayToString(aBytes, 0, aBytes.Length);
        }

        public static byte[] StringToByteArray(string aString, int aArraySize)
        {
            byte[] _arr = new byte[aArraySize];
            DefaultEncoding.GetBytes(aString, 0, aString.Length, _arr, 0);
            return _arr;
        }

        public static string ByteArrayToString(byte[] aBytes, int aStartIndex, int aLength)
        {
            int _index = aStartIndex;
            while (_index < aStartIndex + aLength && aBytes[_index] != 0)
                _index++;

            string _str = String.Empty;
            if (_index > 0)
                _str = DefaultEncoding.GetString(aBytes, aStartIndex, _index - aStartIndex);

            return _str;
        }

        public static string GetErrorString(Exception exc)
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine(exc.Message);

            Exception _temp = exc;
            while (_temp.InnerException != null)
            {
                _temp = _temp.InnerException;
                _sb.AppendLine(_temp.Message);
            }
            _sb.AppendLine(exc.StackTrace);
            return _sb.ToString();
        }

        // Obsolete. Use
        // aDirectory.GetFiles("*.*", SearchOption.AllDirectories);
        public static void GetAllFiles(DirectoryInfo aDirectory, List<FileInfo> aFiles)
        {
            FileInfo[] _files = aDirectory.GetFiles();
            foreach (FileInfo _file in _files)
                aFiles.Add(_file);

            DirectoryInfo[] _subDirs = aDirectory.GetDirectories();
            foreach (DirectoryInfo _subDir in _subDirs)
                GetAllFiles(_subDir, aFiles);
        }

        public static int StringByByteCompare(string x, string y)
        {
            byte[] _xBytes = Common.StringToByteArray(x.ToLower(), x.Length + 1);
            byte[] _yBytes = Common.StringToByteArray(y.ToLower(), y.Length + 1);

            int _byteCompareResult = 0;
            int _minLength = Math.Min(_xBytes.Length, _yBytes.Length);
            for (int i = 0; i < _minLength; i++)
            {
                _byteCompareResult = _xBytes[i].CompareTo(_yBytes[i]);
                if (_byteCompareResult != 0)
                    return _byteCompareResult;
            }

            return _byteCompareResult;
        }

        public static string CompareByLine(string aLeft, string aRight)
        {
            StringBuilder _sb = new StringBuilder();
            using(StringReader _srl = new StringReader(aLeft))
            using(StringReader _srr = new StringReader(aRight))
            {
                string _leftLine = String.Empty;
                string _rightLine = String.Empty;
                while (_leftLine != null || _rightLine != null)
                {
                    _leftLine = _srl.ReadLine();
                    _rightLine = _srr.ReadLine();
                    if(_leftLine != _rightLine)
                    {
                        _sb.AppendLine(String.Format("{0}\t{1}", _leftLine, _rightLine));
                    }
                }
            }

            return _sb.ToString();
        }

    }
}
