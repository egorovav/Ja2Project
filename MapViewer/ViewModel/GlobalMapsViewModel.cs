using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapViewer
{
    public class GlobalMapsViewModel : BaseViewModel
    {
        public GlobalMapsViewModel()
        {

        }

        public static string DataFolderPropertyName = "DataFolder";
        private string FDataFolder;
        public string DataFolder
        {
            get 
            { 
                return this.FDataFolder; 
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    this.FDataFolder = value;
                    this.FGlobalMapsFile = null;
                    this.LoadGlobalMaps();
                    NotifyPropertyChanged(DataFolderPropertyName);
                }
            }
        }

        private string FGlobalMapsFolder
        {
            get { return Path.Combine(this.DataFolder, "Interface"); }
        }

        private string FGlobalMapsSlfFileName
        {
            get { return Path.Combine(this.DataFolder, "Interface.slf"); }
        }


        private SlfFile FGlobalMapsFile;
        private SlfFile SlfFile
        {
            get
            {
                if(this.FGlobalMapsFile == null)
                {
                    this.FGlobalMapsFile = new SlfFile(this.FGlobalMapsSlfFileName);
                    this.FGlobalMapsFile.LoadRecords();
                }
                return this.FGlobalMapsFile;
            }
        }

        public static string GlobalMapPropertyName = "GlobalMap";
        private ImageSource FGlobalMap;
        public ImageSource GlobalMap
        {
            get { return this.FGlobalMap; }
            protected set 
            {
                if (value != null)
                {
                    this.FGlobalMap = value;
                    NotifyPropertyChanged(GlobalMapPropertyName);
                }
            }
        }

        public static string GlobalMap1PropertyName = "GlobalMap1";
        private ImageSource FGlobalMap1;
        public ImageSource GlobalMap1
        {
            get { return this.FGlobalMap1; }
            protected set
            {
                if (value != null)
                {
                    this.FGlobalMap1 = value;
                    NotifyPropertyChanged(GlobalMap1PropertyName);
                }
            }
        }

        public static string GlobalMap2PropertyName = "GlobalMap2";
        private ImageSource FGlobalMap2;
        public ImageSource GlobalMap2
        {
            get { return this.FGlobalMap2; }
            protected set
            {
                if (value != null)
                {
                    this.FGlobalMap2 = value;
                    NotifyPropertyChanged(GlobalMap2PropertyName);
                }
            }
        }

        public static string GlobalMap3PropertyName = "GlobalMap3";
        private ImageSource FGlobalMap3;
        public ImageSource GlobalMap3
        {
            get { return this.FGlobalMap3; }
            protected set
            {
                if (value != null)
                {
                    this.FGlobalMap3 = value;
                    NotifyPropertyChanged(GlobalMap3PropertyName);
                }
            }
        }

        public static string SelectedSectorPropertyName = "SelectedSector";
        private SectorNumber FSelectedSector;
        public SectorNumber SelectedSector
        {
            get { return this.FSelectedSector; }
            set 
            {
                this.FSelectedSector = value;
                NotifyPropertyChanged(SelectedSectorPropertyName);
            }
        }

        public void LoadGlobalMaps()
        {
            this.SetGlobalMap();
            this.GlobalMap1 = this.GetGlobalMapImagefFromSti("MINE_1.STI");
            this.GlobalMap2 = this.GetGlobalMapImagefFromSti("MINE_2.STI");
            this.GlobalMap3 = this.GetGlobalMapImagefFromSti("MINE_3.STI");
        }

        private void SetGlobalMap()
        {
            ImageSource _mapImage = this.GetGlobalMapImagefFromPcx("B_MAP.PCX");
            if(_mapImage == null)
            {
                _mapImage = this.GetGlobalMapImagefFromSti("b_map.sti");
            }

            if (_mapImage == null)
            {
                ImageSourceConverter _isc = new ImageSourceConverter();
                _mapImage = (ImageSource)_isc.ConvertFromString("MAP_STUB.bmp");
            }

            this.GlobalMap = _mapImage;
        }

        private ImageSource GetGlobalMapImagefFromPcx(string aFileName)
        {
            string _path = Path.Combine(this.FGlobalMapsFolder, aFileName);
            PcxObject _pcx = null;
            if (File.Exists(_path))
            {
                _pcx = new PcxObject(aFileName);
                using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read))
                    _pcx.Load(input);
            }
            else
            {
                if (this.SlfFile != null)
                {
                    SlfFile.Record _record = this.SlfFile.Records
                        .SingleOrDefault(x => x.FileName.ToUpperInvariant() == aFileName.ToUpperInvariant());
                    if (_record != null)
                    {
                        _pcx = new PcxObject(aFileName);
                        using (MemoryStream input = new MemoryStream(_record.Data))
                            _pcx.Load(input);
                    }
                }
            }

            if (_pcx == null)
            {
                return null;
            }

            List<Color> _palette = _pcx.ColorPalette
                .Select(x => new Color() { A = 255, R = x.Red, G = x.Green, B = x.Blue })
                .ToList();

            BitmapPalette _pb = new BitmapPalette(_palette);
            PixelFormat _pf = PixelFormats.Indexed8;

            BitmapSource _bitmap = BitmapSource.Create(
                _pcx.Width,
                _pcx.Height,
                96,
                96,
                _pf,
                _pb,
                _pcx.ImageData,
                _pcx.Width * _pf.BitsPerPixel / 8);

            return _bitmap;
        }

        private ImageSource GetGlobalMapImagefFromSti(string aFileName)
        {
            string _path = Path.Combine(this.FGlobalMapsFolder, aFileName);
            IStci _sti = null;
            if (File.Exists(_path))
            {
                using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read))
                    _sti = StciLoader.LoadStci(input);
            }
            else
            {
                if (this.SlfFile != null)
                {
                    SlfFile.Record _record = this.SlfFile.Records
                        .SingleOrDefault(x => x.FileName.ToUpperInvariant() == aFileName.ToUpperInvariant());
                    if (_record != null)
                    {
                        using (MemoryStream input = new MemoryStream(_record.Data))
                            _sti = StciLoader.LoadStci(input);
                    }
                }
            }

            if (_sti == null)
            {

                ImageSourceConverter _isc = new ImageSourceConverter();
                ImageSource _is = (ImageSource)_isc.ConvertFromString("MAP_STUB.bmp");
                return _is;
            }
            else if (_sti is StciIndexed)
            {
                StciIndexed _stciIndexed = (StciIndexed)_sti;
                List<Color> _palette = _stciIndexed.ColorPalette
                    .Select(x => new Color() { A = 255, R = x.Red, G = x.Green, B = x.Blue })
                    .ToList();

                StciSubImage _subImage = _stciIndexed.Images[0];
                StructureImage _image = new StructureImage(_subImage, _palette);
                BitmapPalette _pb = new BitmapPalette(_palette);
                PixelFormat _pf = PixelFormats.Indexed8;

                BitmapSource _bitmap = BitmapSource.Create(
                    _subImage.Header.Width,
                    _subImage.Header.Height,
                    96,
                    96,
                    _pf,
                    _pb,
                    _subImage.ImageData,
                    _subImage.Header.Width * _pf.BitsPerPixel / 8);
                return _bitmap;
            }
            else if(_sti is StciRgb)
            {
                StciRgb _stciRgb = (StciRgb)_sti;
                PixelFormat _pf = PixelFormats.Bgr565;

                BitmapSource _bitmap = BitmapSource.Create(
                    _stciRgb.Header.ImageWidth,
                    _stciRgb.Header.ImageHeight,
                    96,
                    96,
                    _pf,
                    null,
                    _stciRgb.ImageData,
                    _stciRgb.Header.ImageWidth * _pf.BitsPerPixel / 8);
                return _bitmap;
            }

            return null;
        }
    }

    public class SectorNumber
    {
        public SectorNumber(int aLevel, int aX, int aY)
        {
            this.Level = aLevel;
            this.X = aX;
            this.Y = aY;
        }

        public int Level { get; protected set; }
        public int X { get; protected set; }
        public int Y { get; protected set; }

        public override string ToString()
        {
            string _mapFileName =
                String.Format("{0}{1}", (char)('A' + this.Y), this.X + 1);
            if (this.Level > 0)
            {
                _mapFileName += String.Format("_B{0}", this.Level);
            }
            return _mapFileName;
        }
    }
}
