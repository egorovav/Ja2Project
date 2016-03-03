using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Ja2DataDb
{
    public class LoaderFactory
    {
        public LoaderFactory(Ja2Data.SlfFile aSlfFile, int aUserId, Ja2DataEntities aContext)
        {
            this.FSlfFile = aSlfFile;
            this.FUserId = aUserId;

            aContext.FileExtention.Load();
            this.FExtentions = aContext.FileExtention.Local.Select(x => x.Extention).ToList();

            aContext.DataType.Load();
            foreach (DataType _type in aContext.DataType.Local)
            {
                //if (_type.DataTypeName != "J2SD")
                //    continue;

                SlfRecordLoader _loader = this.CreateLoader(_type);
                this.FLoaders.Add(_loader);
            }
        }

        List<string> FExtentions;
        private Ja2Data.SlfFile FSlfFile;
        private int FUserId;

        private List<SlfRecordLoader> FLoaders = new List<SlfRecordLoader>();
        public List<SlfRecordLoader> Loaders
        {
            get { return this.FLoaders; }
        }

        private SlfRecordLoader CreateLoader(DataType aRecordType)
        {
            IEnumerable<Ja2Data.SlfFile.Record> _records =
                this.FSlfFile.Records.Where(x => 
                    aRecordType.FileExtention.Select(y => 
                        y.Extention).Contains(x.FileNameExtention));

            SlfRecordLoader _loader = null;

            switch (aRecordType.DataTypeName)
            {
                case "J2SD":
                    {
                        _loader = new JsdLoader(_records, this.FUserId);
                        break;
                    }
                case "STCI":
                    {
                        _loader = new StciLoader(_records, this.FUserId);
                        break;
                    }
                case "TEXT":
                    {
                        _loader = new TextLoader(_records, this.FUserId);
                        break;
                    }
                default:
                    {
                        _records =
                           this.FSlfFile.Records.Where(x => !this.FExtentions.Contains(x.FileNameExtention));
                        _loader = new UnstructuredLoader(_records, this.FUserId);
                        break;
                    }
            }

            return _loader;
        }


    }
}
