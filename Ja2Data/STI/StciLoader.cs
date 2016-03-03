using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ja2Data
{
    public interface IStci
    {
        void Load(Stream aInput);
        void Load(BinaryReader aInput);
    
        void Save(Stream aOutput);

        bool IsIndexed { get; }
        string FileName { get; }
    }

    public class StciLoader
    {
        public static string ReloadStci(string aFileName)
        {
            IStci _stci = LoadStci(aFileName);
            using(FileStream _fs = new FileStream(aFileName, FileMode.Create))
                _stci.Save(_fs);
            return _stci.ToString();
        }

        public static IStci LoadStci(string aFileName)
        {
            using (FileStream _fs = new FileStream(aFileName, FileMode.Open))
            {
                using (BinaryReader _br = new BinaryReader(_fs))
                {
                    IStci _stci = null;
                    StciHeader _header = new StciHeader();
                    _header.Read(_br);

                    if (_header.IsIndexed)
                    {
                        _stci = new StciIndexed(_header, aFileName);
                        _stci.Load(_br);
                    }
                    else
                    {
                        _stci = new StciRgb(_header, aFileName);
                        _stci.Load(_br);
                    }
                    return _stci;
                }
            }
        }

        public static IStci LoadStci(Stream _fs)
        {
            using (BinaryReader _br = new BinaryReader(_fs))
            {
                return LoadStci(_br);
            }
        }

        private static IStci LoadStci(BinaryReader _br)
        {
            IStci _stci = null;
            StciHeader _header = new StciHeader();
            _header.Read(_br);

            if (_header.IsIndexed)
            {
                _stci = new StciIndexed(_header);
                _stci.Load(_br);
            }
            else
            {
                _stci = new StciRgb(_header);
                _stci.Load(_br);
            }
            return _stci;
        }

        public static IStci LoadStci(Stream aFs, string aFileName)
        {
            using (BinaryReader _br = new BinaryReader(aFs))
            {
                return LoadStci(_br, aFileName);
            }
        }

        private static IStci LoadStci(BinaryReader _br, string aFileName)
        {
            IStci _stci = null;
            StciHeader _header = new StciHeader();
            _header.Read(_br);

            if (_header.IsIndexed)
            {
                _stci = new StciIndexed(_header, aFileName);
                _stci.Load(_br);
            }
            else
            {
                _stci = new StciRgb(_header, aFileName);
                _stci.Load(_br);
            }
            return _stci;
        }
    }
}
