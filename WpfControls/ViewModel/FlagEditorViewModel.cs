using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonWpfControls
{
    public class FlagEditorViewModel : BaseViewModel
    {
        public FlagEditorViewModel()
        {
        }

        private Type FEnumType;

        public static string FlagsPropertyName = "Flags";
        public Enum Flags
        {
            get
            {
                if (this.FEnumType == null)
                    return null;

                return (Enum)Enum.ToObject(this.FEnumType, this.FlagsValue);
            }

            set
            {
                this.FEnumType = value.GetType();
                string[] _flagNames = Enum.GetNames(this.FEnumType);
                Array _flagValues = Enum.GetValues(this.FEnumType);

                for (int i = 0; i < _flagNames.Length; i++)
                {
                    UInt64 _flagValue = Convert.ToUInt64(_flagValues.GetValue(i));
                    bool _flag = (_flagValue & Convert.ToUInt64(value)) > 0;
                    FlagViewModel _fvm = new FlagViewModel(_flagNames[i], _flag, _flagValue);
                    _fvm.PropertyChanged += FlagViewModel_PropertyChanged;
                    this.FFlagsViewModel.Add(_fvm);
                }                    

                NotifyPropertyChanged(FlagsPropertyName);
            }
        }

        public UInt64 FlagsValue
        {
            get 
            {
                var _flagValues = this.FFlagsViewModel.Where(x => x.Flag).Select(x => x.FlagValue);

                UInt64 _flags = 0;
                foreach (UInt64 _flag in _flagValues)
                    _flags |= _flag;

                return _flags;
            }
        }

        private List<FlagViewModel> FFlagsViewModel = new List<FlagViewModel>();
        public List<FlagViewModel> FlagsViewModel
        {
            get { return this.FFlagsViewModel; }
        }

        private void FlagViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == FlagViewModel.FlagPropertyName)
            {
                NotifyPropertyChanged(FlagsPropertyName);
            }
        }
    }

    public class FlagViewModel : BaseViewModel
    {
        public FlagViewModel(string aName, bool aFlag, UInt64 aFlagValue)
        {
            this.FFlagName = aName;
            this.FFlag = aFlag;
            this.FlagValue = aFlagValue;
        }

        public UInt64 FlagValue
        {
            get;
            protected set;
        }

        public static string FlagNamePropertyName = "FlagName";
        private string FFlagName;
        public string FlagName
        {
            get { return this.FFlagName; }
            set
            {
                this.FFlagName = value;
                NotifyPropertyChanged(FlagNamePropertyName);
            }
        }

        public static string FlagPropertyName = "Flag";
        private bool FFlag;
        public bool Flag
        {
            get { return this.FFlag; }
            set
            {
                this.FFlag = value;
                NotifyPropertyChanged(FlagPropertyName);
            }
        }

        public static string DescriptionPropertyName = "Description";
        private string FDescription;
        public string Description
        {
            get { return this.FDescription; }
            set
            {
                this.FDescription = value;
                NotifyPropertyChanged(DescriptionPropertyName);
            }
        }
    }
}
