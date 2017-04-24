using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsdEditor
{
    public class ShapeViewModel : BaseViewModel
    {
        public ShapeViewModel(bool aIsHighDefenition)
        {
            this.FSize = aIsHighDefenition ? JsdTile.MinSize * 2 : JsdTile.MinSize;
            this.FHeight = aIsHighDefenition ? JsdTile.MinHeight * 2 : JsdTile.MinHeight;

            this.FCells = new LayerCellViewModel[this.FSize, this.FSize, this.FHeight];
            this.FLayers = new LayerViewModel[this.FHeight];
            for (int k = 0; k < this.FHeight; k++)
            {
                this.FLayers[k] = new LayerViewModel(aIsHighDefenition, k);

                for (int i = 0; i < this.FSize; i++)
                    for (int j = 0; j < this.FSize; j++)
                    {
                        LayerCellViewModel _cell = this.FLayers[k].Cells[i, j];
                        this.SetLayerCell(_cell, k);
                    }
            }

            this.Shape = new byte[this.FSize * this.FSize];
        }

        public static string ShapePropertyName = "Shape";

        private int FSize;
        private int FHeight;

        private byte[] FShape;
        public byte[] Shape
        {
            get { return this.FShape; }
            set 
            {
                this.FShape = value;
                if(this.FData == null)
                    this.FData = new ShapeCellViewModel[this.FSize, this.FSize];

                for (int i = 0; i < this.FSize; i++)
                    for (int j = 0; j < this.FSize; j++)
                    {
                        if (this.FData[i, j] == null)
                        {
                            ShapeCellViewModel _shapeCell = new ShapeCellViewModel(i, j, this.FShape);
                            _shapeCell.PropertyChanged += ShapeCell_PropertyChanged;
                            this.FData[i, j] = _shapeCell;
                        }
                        else
                            this.FData[i, j].Data = this.FShape;
                    }
            }
        }

        private LayerViewModel[] FLayers;
        public LayerViewModel[] Layers
        {
            get { return this.FLayers; }
        }

        private LayerCellViewModel[,,] FCells;
        public LayerCellViewModel[,,] Cells
        {
            get 
            { 
                return this.FCells; 
            }
        }

        private ShapeCellViewModel[,] FData;
        public ShapeCellViewModel[,] Data
        {
            get { return this.FData; }
        }

        public void SetLayerCell(LayerCellViewModel aCell, int aLayerNumber)
        {
            aCell.PropertyChanged += LayerCell_PropertyChanged;
            this.FCells[aCell.X, aCell.Y, aLayerNumber] = aCell;
        }

        public int MaxX
        {
            get
            {
                int _maxX = 0;
                foreach (LayerCellViewModel _cell in this.Cells)
                    if (_cell.LayerCellValue && _cell.X > _maxX)
                        _maxX = _cell.X;

                return _maxX;
            }
        }


        public int MaxY
        {
            get
            {
                int _maxY = 0;
                foreach (LayerCellViewModel _cell in this.Cells)
                    if (_cell.LayerCellValue && _cell.Y > _maxY)
                        _maxY = _cell.Y;

                return _maxY;
            }
        }

        public int MaxZ
        {
            get
            {
                int _maxZ = 0;
                foreach (LayerCellViewModel _cell in this.Cells)
                    if (_cell.LayerCellValue && _cell.Z > _maxZ)
                        _maxZ = _cell.Z;

                return _maxZ;
            }
        }

        public int MinX
        {
            get
            {
                int _minX = this.FSize;
                foreach (LayerCellViewModel _cell in this.Cells)
                    if (_cell.LayerCellValue && _cell.X < _minX)
                        _minX = _cell.X;

                return _minX;
            }
        }


        public int MinY
        {
            get
            {
                int _minY = this.FSize;
                foreach (LayerCellViewModel _cell in this.Cells)
                    if (_cell.LayerCellValue && _cell.Y < _minY)
                        _minY = _cell.Y;

                return _minY;
            }
        }

        public int MinZ
        {
            get
            {
                int _minZ = this.Layers.Length;
                foreach (LayerCellViewModel _cell in this.Cells)
                    if (_cell.LayerCellValue && _cell.Z < _minZ)
                        _minZ = _cell.Z;

                return _minZ;
            }
        }

        public void ExpandData()
        {
            if (this.FSize == JsdTile.MinSize * 2)
                return;

            byte[] _expandedData = new byte[JsdTile.MinSize * 2 * JsdTile.MinSize * 2];
            for (int i = 0; i < JsdTile.MinSize; i++)
                {
                    byte _ijValue = JsdTile.DublicateBits(this.Shape[i]);
                    _expandedData[i * 2] = _ijValue;
                    _expandedData[i * 2 + 1] = _ijValue;
                    _expandedData[i * 2 + this.FSize] = _ijValue;
                    _expandedData[i * 2 + this.FSize + 1] = _ijValue;
                }

            this.FSize = JsdTile.MinSize * 2;
            this.FHeight = JsdTile.MinHeight * 2;

            this.Shape = _expandedData;
        }

        private bool FHandled = false;

        private void ShapeCell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ShapeCellViewModel.ShapeCellValuePropertyName)
            {
                ShapeCellViewModel _shapeCell = (ShapeCellViewModel)sender;
                this.SetCell(_shapeCell);
                this.FHandled = false;
            }
        }

        private void SetCell(ShapeCellViewModel aShapeCell)
        {
            int _cellValue = aShapeCell.ShapeCellValue;
            this.FHandled = true;
            for (int k = 0; k < this.FHeight; k++)
            {
                bool _layerCellValue = (_cellValue & 1 << k) > 0;
                this.FCells[aShapeCell.X, aShapeCell.Y, k].LayerCellValue = _layerCellValue;
            }
        }

        private void LayerCell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.FHandled)
                return;

            if (e.PropertyName == LayerCellViewModel.ValuePropertyName)
            {
                LayerCellViewModel _layerCell = (LayerCellViewModel)sender;
                int _shapeCellValue = 0;
                for (int k = 0; k < this.FHeight; k++)
                {
                    if (this.FCells[_layerCell.X, _layerCell.Y, k].LayerCellValue)
                        _shapeCellValue += 1 << k;
                }

                if (this.FData[_layerCell.X, _layerCell.Y].ShapeCellValue != (byte)_shapeCellValue)
                {
                    this.FData[_layerCell.X, _layerCell.Y].ShapeCellValue = (byte)_shapeCellValue;
                    NotifyPropertyChanged(ShapePropertyName);
                }
            }
        }
    }

    public class ShapeCellViewModel : BaseViewModel
    {
        public ShapeCellViewModel(int aX, int aY, byte[] aData)
        {
            this.X = aX;
            this.Y = aY;
            this.Data = aData;
        }

        private byte[] FData; 
        public byte[] Data
        {
            get { return this.FData; }
            set 
            { 
                this.FData = value;
                this.FIndex = this.X * (int)Math.Sqrt(this.FData.Length) + this.Y;
                NotifyPropertyChanged(ShapeCellValuePropertyName);
            }
        }

        private int FIndex;

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

        public static string ShapeCellValuePropertyName = "ShapeCellValue";
        public byte ShapeCellValue
        {
            get { return this.Data[this.FIndex]; }
            set
            {
                if (this.Data[this.FIndex] != value)
                {
                    this.Data[this.FIndex] = value;
                    NotifyPropertyChanged(ShapeCellValuePropertyName);
                }
            }
        }
    }
}
