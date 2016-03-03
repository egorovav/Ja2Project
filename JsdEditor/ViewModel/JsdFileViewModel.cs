using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JsdEditor
{
    public class JsdFileViewModel : BaseViewModel
    {
        public JsdFileViewModel(JsdFile aJsdFile, string aFileName)
        {
            this.FJsdFile = aJsdFile;
            this.FileName = aFileName;

            this.FStructs = new ObservableCollection<StructureViewModel>(
                aJsdFile.Structs.Select(x => new StructureViewModel(x)));

            this.FStructs.CollectionChanged += FStructs_CollectionChanged;

            this.FAuxData = new ObservableCollection<AuxDataViewModel>(
                aJsdFile.Auxilarity.Select(x => new AuxDataViewModel(x, aJsdFile.GetAuxTileLocData(x))));

            this.FAuxData.CollectionChanged += FAuxData_CollectionChanged;
            this.PropertyChanged += JsdFileViewModel_PropertyChanged;
        }

        void JsdFileViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != JsdFileViewModel.ImagesPropertyName &&
                e.PropertyName != JsdFileViewModel.SelectedStructPropertyName &&
                e.PropertyName != JsdFileViewModel.ImageSelectedIndexPropertyName && 
                e.PropertyName != JsdFileViewModel.AuxDataSelectedIndexPropertyName &&
                e.PropertyName != JsdFileViewModel.ExceptionStringPropertyName && 
                e.PropertyName != JsdFileViewModel.SelectedAuxDataPropertyName)
            {
                this.FIsFileChanged = true;
            }
        }

        private bool FIsFileChanged = false;
        public bool IsFileChanged
        {
            get 
            { 
                foreach(StructureViewModel _struct in this.FStructs)
                {
                    if (_struct.IsStructChanged)
                        this.FIsFileChanged = true;
                }
                return this.FIsFileChanged; 
            }

            set 
            { 
                this.FIsFileChanged = value;
                foreach (StructureViewModel _struct in this.FStructs)
                {
                    if (_struct.IsStructChanged)
                        _struct.IsStructChanged = value;
                }
            }
        }

        public string FileName
        {
            get;
            protected set;
        }

        public string FullFileName
        {
            get;
            set;
        }

        void FAuxData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.FAuxData.Count > 0)
                this.FJsdFile.Flags |= JsdFile.JsdFileFlags.ContainsAuxImageData;
            else
                this.FJsdFile.Flags &= ~JsdFile.JsdFileFlags.ContainsAuxImageData;

            this.FIsFileChanged = true;
            NotifyPropertyChanged(NumberOfImagesPropertyName);
        }

        void FStructs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.FStructs.Count > 0)
                this.FJsdFile.Flags |= JsdFile.JsdFileFlags.ContainsStructureData;
            else
                this.FJsdFile.Flags &= ~JsdFile.JsdFileFlags.ContainsStructureData;

            this.FIsFileChanged = true;
            NotifyPropertyChanged(NumberOfStructuresPropertyName);
        }

        private JsdFile FJsdFile;
        public JsdFile JsdFile
        {
            get { return this.FJsdFile; }
        }

        public static string StciFilePropertyName = "StciFile"; 
        private StciIndexed FStciFile;
        public StciIndexed StciFile
        {
            get { return this.FStciFile; }
            set 
            {
                this.FStciFile = value;
                if(this.SelectedStruct != null)
                    this.SelectedStruct.Image = this.Images[this.ImageSelectedIndex];
                NotifyPropertyChanged(ImagesPropertyName);
            }
        }

        public static string ImagesPropertyName = "Images";
        //private StructureImage[] FImages;
        public StructureImage[] Images
        {
            get
            {
                if (this.StciFile != null)
                {
                    List<Color> _palette = this.StciFile.ColorPalette
                        .Select(x => new Color() { A = 255, R = x.Red, G = x.Green, B = x.Blue })
                        .ToList();

                    _palette[0] = Colors.Transparent;

                    if ((this.StciFile.Header.Flags & StciFlags.STCI_TRANSPARENT) == 0)
                    {
                        return this.StciFile.Images
                            .Select(x => new StructureImage(x, _palette)).ToArray();
                    }
                    else
                    {
                       // если файл анимированный загружаем только последние кадры каждого ракурса
                       List<StructureImage> _images = new List<StructureImage>(); 
                       for(int i = 0; i < this.StciFile.Images.Length; i++)
                       {
                           StciSubImage _subImage = this.StciFile.Images[i];
                           if(_subImage.AuxData != null && _subImage.AuxData.NumberOfFrames > 0)
                           {
                               int _index = _subImage.AuxData.NumberOfFrames + i - 1;
                               if (_index < this.StciFile.Images.Length)
                               {
                                   i = _index;
                                   _images.Add(new StructureImage(this.StciFile.Images[_index], _palette, true));
                               }
                           }
                       }
                       return _images.ToArray();
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public static string ImageSelectedIndexPropertyName = "ImageSelectedIndex";
        private int FImageSelectedIndex;
        public int ImageSelectedIndex
        {
            get { return this.FImageSelectedIndex; }
            set
            {
                if (this.FImageSelectedIndex != value)
                {
                    this.FImageSelectedIndex = value;

                    if(this.Structs != null)
                    {
                        var _struct = this.Structs.Where(x => x.StructureNumber == value).FirstOrDefault();
                        if (_struct != null)
                            this.SelectedStruct = _struct;
                    }
                    if (this.AuxData != null && this.AuxData.Count > value)
                        this.AuxDataSelectedIndex = value;

                    NotifyPropertyChanged(ImageSelectedIndexPropertyName);
                }
            }
        }
        
        private ObservableCollection<StructureViewModel> FStructs;
        public ObservableCollection<StructureViewModel> Structs
        {
            get { return this.FStructs; }
        }

        public static string SelectedStructPropertyName = "SelectedStruct";
        private StructureViewModel FSelectedStruct;
        public StructureViewModel SelectedStruct
        {
            get { return this.FSelectedStruct; }
            set 
            {

                this.FRemoveStructureCommand.IsCanExecuteProperty = (value != null);
                this.FCopyStructureCommand.IsCanExecuteProperty = (value != null);
                this.FRotateStructureCommand.IsCanExecuteProperty = (value != null);
                this.FAntiRotateStructureCommand.IsCanExecuteProperty = (value != null);

                if (this.FSelectedStruct != value)
                {
                    this.FSelectedStruct = value;
                    if (value != null)
                    {
                        if (this.AuxData != null && this.AuxData.Count > value.StructureNumber)
                            this.SelectedAuxData = this.AuxData[value.StructureNumber];
                        if (this.Images != null && this.Images.Length > value.StructureNumber)
                            this.ImageSelectedIndex = value.StructureNumber;
                    }
                    NotifyPropertyChanged(SelectedStructPropertyName);
                }
            }
        }

        private ObservableCollection<AuxDataViewModel> FAuxData;
        public ObservableCollection<AuxDataViewModel> AuxData
        {
            get { return this.FAuxData; }
        }

        public static string SelectedAuxDataPropertyName = "SelectedAuxData";
        private AuxDataViewModel FSelectedAuxData;
        public AuxDataViewModel SelectedAuxData
        {
            get { return this.FSelectedAuxData; }
            set 
            {
                this.FRemoveAuxDataCommand.IsCanExecuteProperty = (value != null);

                if (this.FSelectedAuxData != value)
                {
                    this.FSelectedAuxData = value;
                    NotifyPropertyChanged(SelectedAuxDataPropertyName);
                }
            }
        }

        public static string AuxDataSelectedIndexPropertyName = "AuxDataSelectedIndex";
        private int FAuxDataSelectedIndex;
        public int AuxDataSelectedIndex
        {
            get { return this.FAuxDataSelectedIndex; }
            set 
            {
                if (this.FAuxDataSelectedIndex != value)
                {
                    this.FAuxDataSelectedIndex = value;
                    if (this.Structs != null)
                    {
                        var _struct = this.Structs.Where(x => x.StructureNumber == value).FirstOrDefault();
                        if (_struct != null)
                            this.SelectedStruct = _struct;
                    }
                    if (this.Images != null && this.Images.Length > value)
                        this.ImageSelectedIndex = value;
                    NotifyPropertyChanged(AuxDataSelectedIndexPropertyName);
                }
            }
        }

        public static string NumberOfStructuresPropertyName = "NumberOfStructures";
        public int NumberOfStructures
        {
            get { return this.FStructs.Count; }
        }

        public static string NumberOfImagesPropertyName = "NumberOfImages";
        public ushort NumberOfImages
        {
            //get { return this.FAuxData.Count == 0 ? this.NumberOfStructures : this.FAuxData.Count; }
            get { return this.JsdFile.NumberOfElements; }
            set 
            { 
                this.JsdFile.NumberOfElements = value;
                NotifyPropertyChanged(NumberOfImagesPropertyName);
            }
        }

        public static string MaterialsPropertyName = "Materials";
        private List<JsdMaterial> FMaterials;
        public List<JsdMaterial> Materials
        {
            get { return this.FMaterials; }
            set
            {
                this.FMaterials = value;
                NotifyPropertyChanged(MaterialsPropertyName);
            }
        }

        public static string IsHighDefenitionPropertyName = "IsHighDefenition"; 
        public bool IsHighDefenition
        {
            get { return (this.FJsdFile.Flags & JsdFile.JsdFileFlags.StructureIsHighDefenition) > 0; }
            set 
            {
                if (value != this.IsHighDefenition)
                {
                    if (value)
                        this.FJsdFile.Flags |= JsdFile.JsdFileFlags.StructureIsHighDefenition;
                    else
                        this.FJsdFile.Flags &= ~JsdFile.JsdFileFlags.StructureIsHighDefenition;
                    NotifyPropertyChanged(IsHighDefenitionPropertyName);
                }
            }
        }

        public void AddJsdStructure(JsdStruct aJsdStruct)
        {
            this.JsdFile.AddStruct(aJsdStruct);
            StructureViewModel _structureViewModel = new StructureViewModel(aJsdStruct);
            this.Structs.Add(_structureViewModel);
            this.SelectedStruct = _structureViewModel;
        }

        private AddStructureCommand FAddStructureCommand = new AddStructureCommand();
        public AddStructureCommand AddStructureCommand
        {
            get { return this.FAddStructureCommand; }
        }

        private RemoveStructureCommand FRemoveStructureCommand = new RemoveStructureCommand();
        public RemoveStructureCommand RemoveStructureCommand
        {
            get { return this.FRemoveStructureCommand; }
        }

        private CopyStructureCommand FCopyStructureCommand = new CopyStructureCommand();
        public CopyStructureCommand CopyStructureCommand
        {
            get { return this.FCopyStructureCommand; }
        }

        private RotateStructureCommand FRotateStructureCommand = new RotateStructureCommand();
        public RotateStructureCommand RotateStructureCommand
        {
            get { return this.FRotateStructureCommand; }
        }

        private AntiRotateStructureCommand FAntiRotateStructureCommand = new AntiRotateStructureCommand();
        public AntiRotateStructureCommand AntiRotateStructureCommand
        {
            get { return this.FAntiRotateStructureCommand; }
        }

        private AddAuxDataCommand FAddAuxDataCommand = new AddAuxDataCommand();
        public AddAuxDataCommand AddAuxDataCommand
        {
            get { return this.FAddAuxDataCommand; }
        }

        private RemoveAuxDataCommand FRemoveAuxDataCommand = new RemoveAuxDataCommand();
        public RemoveAuxDataCommand RemoveAuxDataCommand
        {
            get { return this.FRemoveAuxDataCommand; }
        }
    }

    public class AddStructureCommand : ICommand
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
            JsdFileViewModel _jsdViewModel = (JsdFileViewModel)parameter;
            JsdStruct _struct = new JsdStruct(_jsdViewModel.IsHighDefenition);
            _jsdViewModel.AddJsdStructure(_struct);
        }
    }

    public class CopyStructureCommand : ICommand
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
            JsdFileViewModel _jsdViewModel = (JsdFileViewModel)parameter;
            JsdStruct _struct = _jsdViewModel.SelectedStruct.Structure.Clone();
            _jsdViewModel.AddJsdStructure(_struct);
        }
    }

    public class RemoveStructureCommand : ICommand
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
            JsdFileViewModel _viewModel = (JsdFileViewModel)parameter;
            _viewModel.JsdFile.RemoveStruct(_viewModel.SelectedStruct.Structure);
            _viewModel.Structs.Remove(_viewModel.SelectedStruct);
        }
    }

    public class RotateStructureCommand : ICommand
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
            JsdFileViewModel _viewModel = (JsdFileViewModel)parameter;
            StructureViewModel _selectedStruct = _viewModel.SelectedStruct;
            JsdStruct _rotatedStruct = _selectedStruct.Structure.Clone();
            _rotatedStruct.Rotate(true);
            _viewModel.AddJsdStructure(_rotatedStruct);
        }
    }

    public class AntiRotateStructureCommand : ICommand
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
            JsdFileViewModel _viewModel = (JsdFileViewModel)parameter;
            StructureViewModel _selectedStruct = _viewModel.SelectedStruct;
            JsdStruct _rotatedStruct = _selectedStruct.Structure.Clone();
            _rotatedStruct.Rotate(false);
            _viewModel.AddJsdStructure(_rotatedStruct);
        }
    }

    public class AddAuxDataCommand : ICommand
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
            JsdFileViewModel _viewModel = (JsdFileViewModel)parameter;
            AuxObjectData _aux = new AuxObjectData();
            _viewModel.JsdFile.AddAuxData(_aux);
            AuxDataViewModel _auxViewModel = new AuxDataViewModel(_aux, null);
            _viewModel.AuxData.Add(_auxViewModel);
            _viewModel.SelectedAuxData = _auxViewModel;
        }
    }

    public class RemoveAuxDataCommand : ICommand
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
            JsdFileViewModel _viewModel = (JsdFileViewModel)parameter;
            _viewModel.JsdFile.RemoveAuxData(_viewModel.SelectedAuxData.AuxData);
            _viewModel.AuxData.Remove(_viewModel.SelectedAuxData);
        }
    }
}
