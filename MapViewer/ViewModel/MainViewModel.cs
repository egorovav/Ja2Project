using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
        }

        private string FTilesetFolder
        {
            get { return Path.Combine(this.DataFolder, "tilesets"); }
        }

        private string FTilesetFileName
        {
            get { return Path.Combine(this.DataFolder, "Tilesets.slf"); }
        }

        private SlfFile FTilesetSlfFile;
        private SlfFile TilesetSlfFile
        {
            get
            {
                if (this.FTilesetSlfFile == null)
                {
                    this.FTilesetSlfFile = new SlfFile(this.FTilesetFileName);
                    this.FTilesetSlfFile.LoadRecords();
                }
                return this.FTilesetSlfFile;
            }
        }

        private Map FMap;
        public Map Map
        {
            get { return this.FMap; }
        }

        public static string MapInfoPropertyName = "MapInfo";
        public string MapInfo
        {
            get 
            {
                if (this.Map == null)
                    return String.Empty;
                return String.Format("File name: {0}; Tileset: {1}", this.MapFileName, this.Map.TilesetID); 
            }
        }

        public static string MapFileNamePropertyName = "MapFileName";
        private string FMapFileName;
        public string MapFileName
        {
            get { return this.FMapFileName; }
            set
            {
                this.FMapFileName = value;
                this.FMap = new Map(value);
                this.FMap.Load();

                //if (tileCache.ContainsKey(this.FMap.TilesetID))
                //    this.FMap.MapTileSet = tileCache[this.FMap.TilesetID];
                //else
                //{
                    this.LoadMapTileSet(this.FMap);
                //    this.tileCache.Add(this.FMap.TilesetID, this.FMap.MapTileSet);
                //}

                NotifyPropertyChanged(MapFileNamePropertyName);
                NotifyPropertyChanged(MapInfoPropertyName);
            }
        }

        public static string IsLoadImagePropertyName = "IsLoadImage";
        private bool FIsLoadImage = true;
        public bool IsLoadImage
        {
            get { return this.FIsLoadImage; }
            set
            {
                this.FIsLoadImage = value;
                NotifyPropertyChanged(IsLoadImagePropertyName);
            }
        }

        public static string IsLoadStructurePropertyName = "IsLoadStructure";
        private bool FIsLoadStructure = true;
        public bool IsLoadStructure
        {
            get { return this.FIsLoadStructure; }
            set
            {
                this.FIsLoadStructure = value;
                NotifyPropertyChanged(IsLoadStructurePropertyName);
            }
        }

        public static string DataFolderPropertyName = "DataFolder";
        private string FDataFolder = String.Empty;
        public string DataFolder
        {
            get 
            {
                return this.FDataFolder; 
            }
            set
            {
                this.FDataFolder = value;
                this.LoadTileSet();
                this.FTilesetSlfFile = null;
                NotifyPropertyChanged(DataFolderPropertyName);
            }
        }

        Dictionary<int, Map.TileObject[]> tileCache = new Dictionary<int, Map.TileObject[]>();

        public void LoadMapTileSet(Map aMap)
        {
            for (int k = 0; k < TileSet.NumOfTileTypes; k++)
            {
                string _fileName = TileSet.TileSets[aMap.TilesetID].TileSurfaceFilenames[k];
                int _tilesetId = aMap.TilesetID;
                if (_fileName == String.Empty)
                {
                    _fileName = TileSet.TileSets[0].TileSurfaceFilenames[k];
                    _tilesetId = 0;
                }

                _fileName = Path.Combine(_tilesetId.ToString(), _fileName);

                string _path = Path.Combine(this.FTilesetFolder, _fileName);
                StciIndexed _sti = null;
                if (File.Exists(_path))
                {
                    //using(FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read))
                        _sti = (StciIndexed)StciLoader.LoadStci(_path);
                }
                else
                {
                    if (File.Exists(this.FTilesetFileName))
                    {
                        SlfFile.Record _record = this.TilesetSlfFile.Records
                            .SingleOrDefault(x => x.State == 0 && x.FileName.ToUpperInvariant() == _fileName.ToUpperInvariant());
                        if (_record != null)
                        {
                            using(MemoryStream input = new MemoryStream(_record.Data))
                                _sti = (StciIndexed)StciLoader.LoadStci(input, _fileName);
                        }
                        else
                        {
                            throw new FileNotFoundException(
                                String.Format("Record {0} is not found in file {1}.", _fileName, this.FTilesetFileName));
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException(
                            String.Format("Files {0}, {1} are not found.", _path, this.FTilesetFileName));
                    }
                }

                JsdFile _jsd = null;
                _path = Path.ChangeExtension(_path, ".jsd");
                if (File.Exists(_path))
                {
                    using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read))
                    {
                        _jsd = JsdFile.Load(input);
                    }
                }
                else
                {
                    if (File.Exists(this.FTilesetFileName))
                    {
                        _fileName = Path.ChangeExtension(_fileName, "jsd");
                        SlfFile.Record _record = this.TilesetSlfFile.Records
                            .SingleOrDefault(x => x.FileName.ToUpperInvariant() == _fileName.ToUpperInvariant());
                        if (_record != null)
                        {
                            using (MemoryStream input = new MemoryStream(_record.Data))
                                _jsd = JsdFile.Load(input);
                        }
                        //else
                        //{
                        //    throw new FileNotFoundException(
                        //        String.Format("Record {0} is not found in file {1}.", _fileName, this.FTilesetFileName));
                        //}
                    }
                    //else
                    //{
                    //    throw new FileNotFoundException(
                    //        String.Format("Files {0}, {1} are not found.", _path, this.FTilesetFileName));
                    //}
                }

                aMap.MapTileSet[k] = new Map.TileObject(_sti, _jsd);
            }
        }

        private void LoadTileSet()
        {
            string _ja2SetData = System.IO.Path.Combine(this.DataFolder, @"BinaryData\JA2SET.DAT");

            if (File.Exists(_ja2SetData))
            {
                using (Stream _fs = new FileStream(_ja2SetData, FileMode.Open, FileAccess.Read))
                {
                    TileSet.Load(_fs);
                }
            }
            else
            {
                string _binaryDataFile = Path.Combine(this.DataFolder, "Binarydata.slf");
                if (File.Exists(_binaryDataFile))
                {
                    SlfFile _binaryDataSlf = new SlfFile(_binaryDataFile);
                    _binaryDataSlf.LoadRecords();
                    SlfFile.Record _ja2set =
                        _binaryDataSlf.Records.SingleOrDefault(x => x.FileName.ToUpperInvariant() == "JA2SET.DAT");
                    if (_ja2set != null)
                    {
                        using (MemoryStream _ms = new MemoryStream(_ja2set.Data))
                        {
                            TileSet.Load(_ms);
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException(
                            String.Format("Record JA2SET.DAT is not found in file {0}.", _binaryDataFile));
                    }
                }
                else
                {
                    throw new FileNotFoundException(
                        String.Format("Files {0}, {1} are not found.", _ja2SetData, _binaryDataFile));
                }
            }
        }
    }
}
