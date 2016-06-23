using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace JsdEditor
{
    public class LayerViewModel : BaseViewModel
    {
        public LayerViewModel(int aLayerSize, int aLayerNumber)
        {
            this.LayerSize = aLayerSize;
            this.RandomFieldXSize = aLayerSize;
            this.RandomFieldYSize = aLayerSize;

            this.FData = new LayerCellViewModel[aLayerSize, aLayerSize];
            for (int i = 0; i < aLayerSize; i++)
                for (int j = 0; j < aLayerSize; j++)
                    this.FData[i, j] = new LayerCellViewModel(i, j, aLayerNumber);
        }

        public LayerViewModel(bool aIsHighDefenition, int aLayerNumber)
            : this(aIsHighDefenition ? JsdTile.MinSize * 2 : JsdTile.MinSize, aLayerNumber)
        {

        }

        public int LayerSize
        {
            get;
            protected set;
        }

        private LayerCellViewModel[,] FData;
        public LayerCellViewModel[,] Cells
        {
            get { return this.FData; }
        }

        public static string FillCellNumberPropertyName = "FillCellNumber";
        private int FFillCellNumber;
        public int FillCellNumber
        {
            get { return this.FFillCellNumber; }
            set
            {
                this.FFillCellNumber = value;
                NotifyPropertyChanged(FillCellNumberPropertyName);
            }
        }

        public static string RandomFieldXPropertyName = "RandomFieldX";
        private int FRandomFieldX;
        public int RandomFieldX
        {
            get { return this.FRandomFieldX; }
            set
            {
                this.FRandomFieldX = value;
                this.CheckRandomX();
                NotifyPropertyChanged(RandomFieldXPropertyName);
            }
        }

        public static string RandomFieldYPropertyName = "RandomFieldY";
        private int FRandomFieldY;
        public int RandomFieldY
        {
            get { return this.FRandomFieldY; }
            set
            {
                this.FRandomFieldY = value;
                this.CheckRandomY();
                NotifyPropertyChanged(RandomFieldYPropertyName);
            }
        }

        public static string RandomFieldXSizePropertyName = "RandomFieldXSize";
        private int FRandomFieldXSize;
        public int RandomFieldXSize
        {
            get { return this.FRandomFieldXSize; }
            set
            {
                this.FRandomFieldXSize = value;
                this.CheckRandomX();
                NotifyPropertyChanged(RandomFieldXSizePropertyName);
            }
        }

        public static string RandomFieldYSizePropertyName = "RandomFieldYSize";
        private int FRandomFieldYSize;
        public int RandomFieldYSize
        {
            get { return this.FRandomFieldYSize; }
            set
            {
                this.FRandomFieldYSize = value;
                this.CheckRandomY();
                NotifyPropertyChanged(RandomFieldYSizePropertyName);
            }
        }

        private int FilledCellsNumber
        {
            get { return this.FData.Cast<LayerCellViewModel>().Count(x => x.LayerCellValue); }
        }

        private void CheckRandomX()
        {
            if (this.RandomFieldX + this.RandomFieldXSize > this.LayerSize)
                this.RandomFieldXSize = this.LayerSize - this.RandomFieldX;
        }

        private void CheckRandomY()
        {
            if (this.RandomFieldY + this.RandomFieldYSize > this.LayerSize)
                this.RandomFieldYSize = this.LayerSize - this.RandomFieldY;
        }

        public void FillRandom()
        {
            Random _rnd = new Random(DateTime.Now.Millisecond);
            int _filledCellsCount = 0;
            int _xSize = this.FRandomFieldX + this.FRandomFieldXSize;
            int _ySize = this.FRandomFieldY + this.FRandomFieldYSize;
            while (_filledCellsCount < this.FillCellNumber && 
                    this.FilledCellsNumber < this.FRandomFieldXSize * this.FRandomFieldYSize)
            {
                int _rndX = _rnd.Next(this.FRandomFieldX, _xSize);
                int _rndY = _rnd.Next(this.FRandomFieldY, _ySize);

                if(!this.FData[_rndX, _rndY].LayerCellValue)
                {
                    this.FData[_rndX, _rndY].LayerCellValue = true;
                    _filledCellsCount++;
                }
            }
        }
    }

    public class LayerCellViewModel : BaseViewModel
    {
        public LayerCellViewModel(int aX, int aY, int aZ)
        {
            this.X = aX;
            this.Y = aY;
            this.Z = aZ;
        }

        public LayerCellViewModel(int aX, int aY)
        {
            this.X = aX;
            this.Y = aY;
        }

        public int X
        {
            get;
            protected set;
        }

        public int Y
        {
            get;
            protected set;
        }

        public int Z
        {
            get;
            protected set;
        }

        private List<object> FReceivers = new List<object>();

        public void AttachHandler(object aReceiver, PropertyChangedEventHandler aHandler)
        {
            if(!this.FReceivers.Contains(aReceiver))
            {
                this.PropertyChanged += aHandler;
                this.FReceivers.Add(aReceiver);
            }
        }

        public void DeattachHandler(object aReceiver, PropertyChangedEventHandler aHandler)
        {
            if (this.FReceivers.Contains(aReceiver))
            {
                this.PropertyChanged -= aHandler;
                this.FReceivers.Remove(aReceiver);
            }
        }

        public static string ValuePropertyName = "LayerCellValue";
        private bool FValue;
        public bool LayerCellValue
        {
            get { return this.FValue; }
            set 
            {
                if (this.FValue != value)
                {
                    this.FValue = value;
                    NotifyPropertyChanged(ValuePropertyName);
                }
            }
        }
    }

    [ValueConversion(typeof(bool), typeof(bool?))]
    public class BoolToNullableBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            return new Nullable<bool>((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Nullable<bool>))
                throw new ArgumentException("Nullable<bool> required.");

            Nullable<bool> _b = (Nullable<bool>)value;

            if (_b == null)
                return false;
            else
                return _b.Value;
        }
    }
}
