using Ja2Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JsdEditor
{
    /// <summary>
    /// Interaction logic for LayerView.xaml
    /// </summary>
    public partial class LayerView : UserControl
    {
        public LayerViewModel ViewModel
        {
            get { return this.DataContext as LayerViewModel; }
            set { this.DataContext = value; }
        }

        public LayerView(LayerViewModel aViewModel)
        {
            this.ViewModel = aViewModel;
            InitializeComponent();
        }

        private CheckBox[,] FCellCheckBoxes;
        private CheckBox[] FFillRowCheckBoxes;
        private CheckBox[] FFillColumnCheckBoxes;
        private CheckBox FFillLayerCheckBox;


        public int LayerSize
        {
            get { return this.ViewModel.LayerSize; }
        }

        public string Header
        {
            get { return this.lblHeader.Content.ToString(); }
            set { this.lblHeader.Content = value; }
        }

        public string Number
        {
            get { return this.lblNumber.Content.ToString(); }
            set { this.lblNumber.Content = value; }
        }

        private double FCheckBoxSize = 15;
        private bool FIsLoaded = false;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.FIsLoaded)
                return;

            this.ugShapeLayer.Rows = this.LayerSize + 1;
            this.ugShapeLayer.Columns = this.LayerSize + 1;
            this.ugShapeLayer.Width = this.ugShapeLayer.Rows * this.FCheckBoxSize;
            this.ugShapeLayer.Height = this.ugShapeLayer.Height * this.FCheckBoxSize;

            this.FCellCheckBoxes = new CheckBox[this.LayerSize, this.LayerSize];
            this.FFillRowCheckBoxes = new CheckBox[this.LayerSize];
            this.FFillColumnCheckBoxes = new CheckBox[this.LayerSize];

            for(int i = 0; i < this.LayerSize; i++)
            {
                int _rowIndex = i;
                for(int j = 0; j < this.LayerSize; j++)
                {
                    int _columnIndex = j;
                    CheckBox _cb = this.GetCheckBox();
                    Binding _binding = new Binding();
                    _binding.Mode = BindingMode.TwoWay;
                    _binding.Source = this.ViewModel.Cells[_columnIndex, _rowIndex];
                    _binding.Path = new PropertyPath(LayerCellViewModel.ValuePropertyName, null);
                    _binding.Converter = new BoolToNullableBoolConverter();
                    _cb.SetBinding(CheckBox.IsCheckedProperty, _binding);


                   //_cb.Checked += (s, arg) => this.CellCheckedChanged(_rowIndex, _columnIndex, _cb.IsChecked);
                   //_cb.Unchecked += (s, arg) => this.CellCheckedChanged(_rowIndex, _columnIndex, _cb.IsChecked);
                    this.FCellCheckBoxes[_columnIndex, _rowIndex] = _cb;
                    this.ugShapeLayer.Children.Add(_cb);
                }

                CheckBox _frcb = this.GetFillCheckBox();
                _frcb.Checked += (s, arg) => this.FillRowCheckedChanged(_rowIndex, _frcb.IsChecked);
                _frcb.Unchecked += (s, arg) => this.FillRowCheckedChanged(_rowIndex, _frcb.IsChecked);
                this.FFillRowCheckBoxes[i] = _frcb;
                this.ugShapeLayer.Children.Add(_frcb);
            }

            for(int i = 0; i < this.LayerSize; i++)
            {
                int _colIndex = i;
                CheckBox _fccb = this.GetFillCheckBox();
                _fccb.Checked += (s, arg) => this.FillColmnCheckedChanged(_colIndex, _fccb.IsChecked);
                _fccb.Unchecked += (s, arg) => this.FillColmnCheckedChanged(_colIndex, _fccb.IsChecked);
                this.FFillColumnCheckBoxes[i] = _fccb;
                this.ugShapeLayer.Children.Add(_fccb);
            }

            CheckBox _flcb = this.GetFillCheckBox();
            _flcb.Checked += (s, arg) => this.FillLayerCheckedChanged(_flcb.IsChecked);
            _flcb.Unchecked += (s, arg) => this.FillLayerCheckedChanged(_flcb.IsChecked);
            this.FFillLayerCheckBox = _flcb;
            this.ugShapeLayer.Children.Add(_flcb);

            this.FIsLoaded = true;
        }

        private CheckBox GetCheckBox()
        {
            CheckBox _cb = new CheckBox();
            return _cb;
        }

        private CheckBox GetFillCheckBox()
        {
            CheckBox _cb = this.GetCheckBox();
            _cb.Background = Brushes.LightGreen;
            return _cb;
        }

        private bool FHandled = false;

        private void CellCheckedChanged(int aRowNum, int aColNum, bool? aIsChecked)
        {
            if (this.FHandled)
                return;

            bool _allIsChecked = true;
            bool _allIsUnchecked = true;
            for (int i = 0; i < this.LayerSize; i++)
            {
                if (this.FCellCheckBoxes[i, aRowNum].IsChecked != true)
                    _allIsChecked = false;

                if (this.FCellCheckBoxes[i, aRowNum].IsChecked != false)
                    _allIsUnchecked = false;
            }

            if (_allIsChecked && !this.FFillRowCheckBoxes[aRowNum].IsChecked.Value)
                this.FFillRowCheckBoxes[aRowNum].IsChecked = true;

            if (_allIsUnchecked && this.FFillRowCheckBoxes[aRowNum].IsChecked.Value)
                this.FFillRowCheckBoxes[aRowNum].IsChecked = false;

            _allIsChecked = true;
            _allIsUnchecked = true;
            for (int i = 0; i < this.LayerSize; i++)
            {
                if (this.FCellCheckBoxes[aColNum, i].IsChecked != true)
                    _allIsChecked = false;

                if (this.FCellCheckBoxes[aColNum, i].IsChecked != false)
                    _allIsUnchecked = false;
            }

            if (_allIsChecked && !this.FFillColumnCheckBoxes[aColNum].IsChecked.Value)
                this.FFillColumnCheckBoxes[aColNum].IsChecked = true;

            if (_allIsUnchecked && this.FFillColumnCheckBoxes[aColNum].IsChecked.Value)
                this.FFillColumnCheckBoxes[aColNum].IsChecked = false;
        }

        private void FillRowCheckedChanged(int aRowNum, bool? aIsChecked)
        {         
            for (int i = 0; i < this.LayerSize; i++)
                if(this.FCellCheckBoxes[i, aRowNum].IsChecked != aIsChecked)
                    this.FCellCheckBoxes[i, aRowNum].IsChecked = aIsChecked;           
        }

        private void FillColmnCheckedChanged(int aColumnNum, bool? aIsChecked)
        {
            if (this.FHandled)
                return;

            bool _allIsChecked = true;
            bool _allIsUnchecked = true;
            for(int i = 0; i < this.LayerSize; i++)
            {
                if (this.FFillColumnCheckBoxes[i].IsChecked != true)
                    _allIsChecked = false;

                if (this.FFillColumnCheckBoxes[i].IsChecked != false)
                    _allIsUnchecked = false;
            }

            if (_allIsChecked && !this.FFillLayerCheckBox.IsChecked.Value)
                this.FFillLayerCheckBox.IsChecked = true;

            if (_allIsUnchecked && this.FFillLayerCheckBox.IsChecked.Value)
                this.FFillLayerCheckBox.IsChecked = false;

            for (int i = 0; i < this.LayerSize; i++)
                if (this.FCellCheckBoxes[aColumnNum, i].IsChecked != aIsChecked)
                    this.FCellCheckBoxes[aColumnNum, i].IsChecked = aIsChecked;           
        }

        private void FillLayerCheckedChanged(bool? aIsChecked)
        {
            this.FHandled = true;

            for (int i = 0; i < this.LayerSize; i++)
                if(this.FFillRowCheckBoxes[i].IsChecked != aIsChecked)
                    this.FFillRowCheckBoxes[i].IsChecked = aIsChecked;

            for (int i = 0; i < this.LayerSize; i++)
                if (this.FFillColumnCheckBoxes[i].IsChecked != aIsChecked)
                    this.FFillColumnCheckBoxes[i].IsChecked = aIsChecked;

            this.FHandled = false;
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.FillRandom();
        }
    }
}
