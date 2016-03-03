using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace JsdEditor
{
    public class StructureViewModel : BaseViewModel
    {
        public StructureViewModel(JsdStruct aStructure) 
        {
            this.FRemoveTileCommand.IsCanExecuteProperty = false;
            this.FCopyTileCommand.IsCanExecuteProperty = false;
            this.PropertyChanged += StructureViewModel_PropertyChanged;
            this.Load(aStructure);
        }

        void StructureViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName != StructureViewModel.ExceptionStringPropertyName &&
                e.PropertyName != StructureViewModel.ImagePropertyName &&
                e.PropertyName != StructureViewModel.SelectedTileLocDataPropertyName && 
                e.PropertyName != StructureViewModel.SelectedTilePropertyName &&
                e.PropertyName != StructureViewModel.TileLocDataPropertyName)
            {
                this.FIsStructureChanged = true;
            }
        }

        public void Load(JsdStruct aStructure)
        {
            if (aStructure != null)
            {
                this.FStructure = aStructure;

                if (aStructure.Tiles != null)
                {
                    var _tiles = aStructure.Tiles.Select(x => new TileViewModel(x));
                    this.FTiles = new ObservableCollection<TileViewModel>(_tiles);
                    this.FTiles.CollectionChanged += Tiles_CollectionChanged;
                }

                this.FIsHighDefenition = aStructure.IsHighDefenition;
            }
            else
                this.FStructure = new JsdStruct(false);
        }

        public void Rotate(bool aIsClockwise)
        {
            this.Structure.Rotate(aIsClockwise);
            this.Load(this.Structure);
        }

        private JsdStruct FStructure;
        public JsdStruct Structure
        {
            get { return this.FStructure; }
        }

        private ObservableCollection<TileViewModel> FTiles = new ObservableCollection<TileViewModel>();
        public ObservableCollection<TileViewModel> Tiles
        {
            get { return this.FTiles; }
        }

        public static string ArmourPropertyName = "Armour";
        public int Armour
        {
            get { return (int)this.FStructure.Armour; }
            set 
            { 
                this.FStructure.Armour = (byte)value;
                NotifyPropertyChanged(ArmourPropertyName);
            }
        }

        public static string MaterialPropertyName = "Material"; 
        private JsdMaterial FMaterial;
        public JsdMaterial Material
        {
            get { return this.FMaterial; }
            set
            {
                this.FMaterial = value;
                NotifyPropertyChanged(MaterialPropertyName);
            }
        }

        public static string HitPointsPropertyName = "HitPoints";
        public byte HitPoints
        {
            get { return this.FStructure.HitPoints; }
            set 
            { 
                this.FStructure.HitPoints = value;
                NotifyPropertyChanged(HitPointsPropertyName);
            }
        }

        public static string DensityPropertyName = "Density";
        public byte Density
        {
            get { return this.FStructure.Density; }
            set 
            { 
                this.FStructure.Density = value;
                NotifyPropertyChanged(DensityPropertyName);
            }
        }

        public static string FlagsPropertyName = "Flags";
        public JsdStruct.JsdStructureFlags Flags
        {
            get { return this.FStructure.Flags; }
            set 
            { 
                this.FStructure.Flags = value;
                NotifyPropertyChanged(FlagsPropertyName);
            }
        }

        public FlagEditorViewModel FlagsViewModel
        {
            get 
            {
                FlagEditorViewModel _flagsViewModel = new FlagEditorViewModel();
                _flagsViewModel.Flags = this.Flags;
                return _flagsViewModel;
            }
            set
            {
                this.Flags = (JsdStruct.JsdStructureFlags)value.Flags;
            }
        }

        public static string StructureNumberPropertyName = "StructureNumber";
        public UInt16 StructureNumber
        {
            get { return this.FStructure.StructureNumber; }
            set 
            { 
                this.FStructure.StructureNumber = value;
                NotifyPropertyChanged(StructureNumberPropertyName);
            }
        }

        public static string WallOrientationPropertyName = "WallOrientation"; 
        public byte WallOrientation
        {
            get { return this.FStructure.WallOrientation; }
            set 
            { 
                this.FStructure.WallOrientation = value;
                NotifyPropertyChanged(WallOrientationPropertyName);
            }
        }

        public static string DestructionPartnerPropertyName = "DestructionPartner";
        public sbyte DestructionPartner
        {
            get { return this.FStructure.DestructionPartner; }
            set 
            { 
                this.FStructure.DestructionPartner = value;
                NotifyPropertyChanged(DestructionPartnerPropertyName);
            }
        }

        public static string PartnerDeltaPropertyName = "PartnerDelta";
        public sbyte PartnerDelta
        {
            get { return this.FStructure.PartnerDelta; }
            set 
            { 
                this.FStructure.PartnerDelta = value;
                NotifyPropertyChanged(PartnerDeltaPropertyName);
            }
        }

        public static string ZTileOffsetXPropertyName = "ZTileOffsetX"; 
        public sbyte ZTileOffsetX
        {
            get { return this.FStructure.ZTileOffsetX; }
            set 
            { 
                this.FStructure.ZTileOffsetX = value;
                NotifyPropertyChanged(ZTileOffsetXPropertyName);
            }
        }

        public static string ZTileOffsetYPropertyName = "ZTileOffsetY";
        public sbyte ZTileOffsetY
        {
            get { return this.FStructure.ZTileOffsetY; }
            set 
            { 
                this.FStructure.ZTileOffsetY = value;
                NotifyPropertyChanged(ZTileOffsetYPropertyName);
            }
        }

        public static string IsHighDefenitionPropertyName = "IsHighDefenition";
        private bool FIsHighDefenition;
        public bool IsHighDefenition
        {
            get { return this.FIsHighDefenition; }
        }

        public static string SelectedTilePropertyName = "SelectedTile";
        private TileViewModel FSelectedTile;
        public TileViewModel SelectedTile
        {
            get { return this.FSelectedTile; }
            set 
            { 
                this.FSelectedTile = value;
                this.FRemoveTileCommand.IsCanExecuteProperty = (value != null);
                this.FCopyTileCommand.IsCanExecuteProperty = (value != null);
                this.FRotateTileCommand.IsCanExecuteProperty = (value != null);
                this.FAntiRotateTileCommand.IsCanExecuteProperty = (value != null);

                TileViewModel _unselectedTile = this.Tiles.Where(x => x.IsSelected).SingleOrDefault();
                if (_unselectedTile != null)
                    _unselectedTile.IsSelected = false;

                if (this.FSelectedTile != null)
                {
                    this.FSelectedTile.IsSelected = true;
                }

                NotifyPropertyChanged(SelectedTilePropertyName);
            }
        }

        public static string TileLocDataPropertyName = "TileLocData";
        private ObservableCollection<RelTileLoc> FTileLocData = new ObservableCollection<RelTileLoc>();
        public ObservableCollection<RelTileLoc> TileLocData
        {
            get { return this.FTileLocData; }
            set
            {
                this.FTileLocData = value;
                NotifyPropertyChanged(TileLocDataPropertyName);
            }
        }

        public static string SelectedTileLocDataPropertyName = "SelectedTileLocData";
        private RelTileLoc FSelectedTileLocData;
        public RelTileLoc SelectedTileLocData
        {
            get { return this.FSelectedTileLocData; }
            set
            {
                this.FSelectedTileLocData = value;
                NotifyPropertyChanged(SelectedTileLocDataPropertyName);
            }
        }

        public static string ImagePropertyName = "Image";
        private StructureImage FImage;
        public StructureImage Image
        {
            get { return this.FImage; }
            set 
            {
                this.FImage = value;
                NotifyPropertyChanged(ImagePropertyName);
            }
        }

        public int MinXPosRelToBase
        {
            get 
            {
                if (this.Tiles == null || this.Tiles.Count == 0)
                {
                    if (this.TileLocData != null && this.TileLocData.Count > 0)
                        return this.TileLocData.Min(x => x.X);
                    return 0;
                }
                else
                    return this.Tiles.Min(x => x.XPosRelToBase); 
            }
        }

        public int MinYPosRelToBase
        {
            get 
            {
                if (this.Tiles == null || this.Tiles.Count == 0)
                {
                    if (this.TileLocData != null && this.TileLocData.Count > 0)
                        return this.TileLocData.Min(x => x.Y);
                    return 0;
                }
                else
                    return this.Tiles.Min(x => x.YPosRelToBase); 
            }
        }

        public int MaxXPosRelToBase
        {
            get
            {
                if (this.Tiles == null || this.Tiles.Count == 0)
                {
                    if (this.TileLocData != null && this.TileLocData.Count > 0)
                        return this.TileLocData.Max(x => x.X);
                    return 0;
                }
                else
                    return this.Tiles.Max(x => x.XPosRelToBase);
            }
        }

        public int MaxYPosRelToBase
        {
            get
            {
                if (this.Tiles == null || this.Tiles.Count == 0)
                {
                    if (this.TileLocData != null && this.TileLocData.Count > 0)
                        return this.TileLocData.Max(x => x.Y);
                    return 0;
                }
                else
                    return this.Tiles.Max(x => x.YPosRelToBase);
            }
        }

        private void Tiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                this.SelectedTile = (TileViewModel)e.NewItems[0];
            }

            this.FIsStructureChanged = true;
        }

        public static string ImageXPropertyName = "ImageX";
        private double FImageX = 0;
        public double ImageX
        {
            get { return this.FImageX; }
            set
            {
                this.FImageX = value;
                NotifyPropertyChanged(ImageXPropertyName);
            }
        }


        //private const int ImageYMax = 100;
        public static string ImageYPropertyName = "ImageY";
        private double FImageY = 0;
        public double ImageY
        {
            get { return this.FImageY; }
            set
            {
                this.FImageY = value;
                NotifyPropertyChanged(ImageYPropertyName);
            }
        }

        private AddTileCommand FAddTileCommand = new AddTileCommand();
        public AddTileCommand AddTileCommand
        {
            get { return this.FAddTileCommand; }
        }

        private RemoveTileCommand FRemoveTileCommand = new RemoveTileCommand();
        public RemoveTileCommand RemoveTileCommand
        {
            get { return this.FRemoveTileCommand;}
        }

        private CopyTileCommand FCopyTileCommand = new CopyTileCommand();
        public CopyTileCommand CopyTileCommand
        {
            get { return this.FCopyTileCommand; }
        }

        private RotateTileCommand FRotateTileCommand = new RotateTileCommand();
        public RotateTileCommand RotateTileCommand
        {
            get { return this.FRotateTileCommand; }
        }

        private AntiRotateTileCommand FAntiRotateTileCommand = new AntiRotateTileCommand();
        public AntiRotateTileCommand AntiRotateTileCommand
        {
            get { return this.FAntiRotateTileCommand; }
        }

        public void AddJsdTile(JsdTile aTile)
        {
            this.FStructure.AddTile(aTile);
            TileViewModel _tileViewModel = new TileViewModel(aTile);
            this.Tiles.Add(_tileViewModel);
            this.SelectedTile = _tileViewModel;
        }

        private bool FIsStructureChanged = false;
        public bool IsStructChanged 
        { 
            get
            {
                foreach(TileViewModel _tile in this.FTiles)
                {
                    if (_tile.IsTileChanged)
                        this.FIsStructureChanged = true;
                }
                return this.FIsStructureChanged;
            }

            set
            {
                this.FIsStructureChanged = value;
                foreach (TileViewModel _tile in this.FTiles)
                    _tile.IsTileChanged = value;
            }
        }
    }

    public class RotateTileCommand : ICommand
    {
        private bool FIsCanExecute = false;
        public bool IsCanExecuteProperty
        {
            set
            {
                this.FIsCanExecute = value;
                if (this.CanExecuteChanged != null)
                    this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.FIsCanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            StructureViewModel _viewModel = (StructureViewModel)parameter;
            TileViewModel _selectedTile = _viewModel.SelectedTile;
            JsdTile _rotatedTile = _selectedTile.Tile.Clone();
            _rotatedTile.Rotate(true);
            _viewModel.AddJsdTile(_rotatedTile);
        }
    }

    public class AntiRotateTileCommand : ICommand
    {
        private bool FIsCanExecute = false;
        public bool IsCanExecuteProperty
        {
            set
            {
                this.FIsCanExecute = value;
                if (this.CanExecuteChanged != null)
                    this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.FIsCanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            StructureViewModel _viewModel = (StructureViewModel)parameter;
            TileViewModel _selectedTile = _viewModel.SelectedTile;
            JsdTile _rotatedTile = _selectedTile.Tile.Clone();
            _rotatedTile.Rotate(false);
            _viewModel.AddJsdTile(_rotatedTile);
        }
    }

    public class AddTileCommand : ICommand
    {
        private bool FIsCanExecute = true;
        public bool IsCanExecuteProperty
        {
            set
            {
                this.FIsCanExecute = value;
                if (this.CanExecuteChanged != null)
                    this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.FIsCanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            StructureViewModel _viewModel = (StructureViewModel)parameter;
            JsdTile _tile = new JsdTile(_viewModel.IsHighDefenition);
            _viewModel.Structure.AddTile(_tile);
            TileViewModel _tileViewModel = new TileViewModel(_tile);
            _viewModel.Tiles.Add(_tileViewModel);
            _viewModel.SelectedTile = _tileViewModel;
        }
    }

    public class RemoveTileCommand : ICommand
    {
        private bool FIsCanExecute = false;
        public bool IsCanExecuteProperty
        {
            set
            {
                this.FIsCanExecute = value;
                if (this.CanExecuteChanged != null)
                    this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.FIsCanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            StructureViewModel _viewModel = (StructureViewModel)parameter;
            _viewModel.Structure.RemoveTile(_viewModel.SelectedTile.Tile);
            _viewModel.Tiles.Remove(_viewModel.SelectedTile);
        }
    }

    public class CopyTileCommand : ICommand
    {
        private bool FIsCanExecute = false;
        public bool IsCanExecuteProperty
        {
            set
            {
                this.FIsCanExecute = value;
                if (this.CanExecuteChanged != null)
                    this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.FIsCanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            StructureViewModel _viewModel = (StructureViewModel)parameter;
            JsdTile _tile = _viewModel.SelectedTile.Tile.Clone();
            _viewModel.Structure.AddTile(_tile);
            TileViewModel _tileViewModel = new TileViewModel(_tile);
            _viewModel.Tiles.Add(_tileViewModel);
            _viewModel.SelectedTile = _tileViewModel;
        }
    }
}
