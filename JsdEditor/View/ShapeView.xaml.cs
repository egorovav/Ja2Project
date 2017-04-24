using Ja2Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ShapeView.xaml
    /// </summary>
    public partial class ShapeView : UserControl
    {
        private ShapeViewModel FViewModel = new ShapeViewModel(false);
        public ShapeViewModel ViewModel
        {
            get { return this.FViewModel; }
            set 
            {
                this.FViewModel = value;
                this.FIsLoaded = false;
                this.LoadView();
            }
        }

        public ShapeView()
        {
            InitializeComponent();
        }

        static ShapeView()
        {
            PropertyMetadata _metadata = 
                new PropertyMetadata(false, new PropertyChangedCallback(IsHighDefenitionPropertyChanged));
            IsHighDefenitionProperty =
                DependencyProperty.Register("IsHighDefenition", typeof(bool), typeof(ShapeView), _metadata);
        }

        private static void IsHighDefenitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        public static DependencyProperty IsHighDefenitionProperty;

        public bool IsHighDefenition
        {
            get { return (bool)this.GetValue(IsHighDefenitionProperty); }
            set { this.SetValue(IsHighDefenitionProperty, value); }
        }

        private int LayersNumber
        {
            get { return this.IsHighDefenition ? JsdTile.MinHeight * 2 : JsdTile.MinHeight; }
        }

        private int ShapeSize
        {
            get { return this.IsHighDefenition ? JsdTile.MinSize * 2 : JsdTile.MinSize; }
        }

        List<LayerView> FLayers;
        LayerView FCommonLayer;

        private bool FIsLoaded = false;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadView();
        }

        private void LoadView()
        {
            if (this.FIsLoaded)
                return;

            this.ugLayers.Children.Clear();
            this.sp.Children.Remove(this.FCommonLayer);
            this.ugShapeValue.Children.Clear();

            this.sp.Orientation = this.IsHighDefenition ? Orientation.Horizontal : Orientation.Vertical;

            this.ugLayers.Columns = this.IsHighDefenition ? 2 : 1;
            this.ugLayers.Rows = this.LayersNumber / this.ugLayers.Columns + 1;

            this.ugShapeValue.Columns = this.ShapeSize;
            this.ugShapeValue.Rows = this.ShapeSize;
            for (int i = 0; i < this.ShapeSize; i++)
                for (int j = 0; j < this.ShapeSize; j++)
                {
                    TextBox _tb = new TextBox();
                    _tb.MinWidth = this.IsHighDefenition ? 27 : 18;
                    this.ugShapeValue.Children.Add(_tb);
                    Binding _binding = new Binding(ShapeCellViewModel.ShapeCellValuePropertyName);
                    _binding.Source = this.ViewModel.Data[j, i];
                    _binding.Mode = BindingMode.TwoWay;
                    _tb.SetBinding(TextBox.TextProperty, _binding);
                }

            LayerViewModel _commonViewModel = new LayerViewModel(this.IsHighDefenition, -1);
            this.FCommonLayer = new LayerView(_commonViewModel);
            foreach (LayerCellViewModel _cell in this.FCommonLayer.ViewModel.Cells)
            {
                _cell.PropertyChanged += CommonCell_PropertyChanged;
            }
            this.FCommonLayer.Margin = new Thickness(2, 0, 2, 0);
            this.FCommonLayer.Header = "All Layers";
            this.sp.Children.Add(this.FCommonLayer);

            this.FLayers = new List<LayerView>(this.LayersNumber);
            for (int k = this.LayersNumber - 1; k >= 0; k--)
            {
                LayerView _layer = new LayerView(this.ViewModel.Layers[k]);
                foreach(LayerCellViewModel _cell in _layer.ViewModel.Cells)
                    _cell.PropertyChanged += Cell_PropertyChanged;
                _layer.Number = String.Format("{0}", k + 1);
                _layer.Margin = new Thickness(2, 0, 2, 0);
                this.ugLayers.Children.Add(_layer);
                this.FLayers.Add(_layer);
            }

            this.FIsLoaded = true;
        }

        private void Cell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == LayerCellViewModel.ValuePropertyName)
            {
                LayerCellViewModel _cell = (LayerCellViewModel)sender;
                bool _allIsChecked = true;
                bool _allIsUnchecked = true;
                foreach(LayerView _layerView in this.FLayers)
                {
                    LayerCellViewModel _layerCell = _layerView.ViewModel.Cells[_cell.X, _cell.Y];
                    if (!_layerCell.LayerCellValue)
                        _allIsChecked = false;

                    if (_layerCell.LayerCellValue)
                        _allIsUnchecked = false;
                }

                LayerCellViewModel _commonCell = this.FCommonLayer.ViewModel.Cells[_cell.X, _cell.Y];
                if (_allIsChecked && !_commonCell.LayerCellValue)
                    _commonCell.LayerCellValue = true;
                if (_allIsUnchecked && _commonCell.LayerCellValue)
                    _commonCell.LayerCellValue = false;
            }
        }

        private void CommonCell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == LayerCellViewModel.ValuePropertyName)
            {
                LayerCellViewModel _commonCell = (LayerCellViewModel)sender;

                if (_commonCell.LayerCellValue)
                    this.ViewModel.Data[_commonCell.X, _commonCell.Y].ShapeCellValue = (byte)((1 << this.LayersNumber) - 1);
                else
                    this.ViewModel.Data[_commonCell.X, _commonCell.Y].ShapeCellValue = 0;
            }
        }
    }
}
