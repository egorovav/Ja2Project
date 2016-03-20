using Ja2Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapViewer
{
    public class MapImage
    {
        public MapImage(Map aMap)
        {
            this.FMap = aMap;
        }

        private Map FMap;


        public WriteableBitmap GetMapBitmap(bool[] aDrawedLayers)
        {
            PresentationSource _ps = PresentationSource.FromVisual(Application.Current.MainWindow);
            Matrix _m = _ps.CompositionTarget.TransformToDevice;
            double _dpiX = _m.M11 * 96;
            double _dpiY = _m.M22 * 96;
            int _imageHeigth = (this.FMap.WORLD_SIZE + 2) * StructureImage.TileHeight;

            WriteableBitmap _wbm = new WriteableBitmap(
                _imageHeigth * 2, _imageHeigth, _dpiX, _dpiY, PixelFormats.Bgra32, null);

            int _bytePerPixel = _wbm.Format.BitsPerPixel / 8;
            byte[] _imageData = new byte[_imageHeigth * 2 * _imageHeigth * _bytePerPixel];

            for (int aLayerNumber = 0; aLayerNumber < aDrawedLayers.Length; aLayerNumber++)
            {
                if (aDrawedLayers[aLayerNumber])
                {
                    for (int i = 0; i < this.FMap.WORLD_SIZE; i++)
                    //for (int i = this.FMap.WORLD_SIZE; i > 0; i--)
                    //    for (int j = this.FMap.WORLD_SIZE; j > 0; j--)
                        for (int j = 0; j < this.FMap.WORLD_SIZE; j++)
                        {
                            if (i + j >= 3 * this.FMap.WORLD_SIZE / 2 ||
                                i + j <= this.FMap.WORLD_SIZE / 2 ||
                                i - j >= this.FMap.WORLD_SIZE / 2 ||
                                i - j <= -this.FMap.WORLD_SIZE / 2)
                                continue;

                            MapElement _element = this.FMap.Elementes[this.FMap.WORLD_SIZE * j + i];
                            LevelNode.TileIndex[] _indexes = _element.pLevelNodes[aLayerNumber].tileIndexes;
                            for (int _index = 0; _index < _indexes.Length; _index++)
                            {
                                if (_indexes.Length > _index)
                                {
                                    LevelNode.TileIndex _tileIndex = _indexes[_index];
                                    int _subIndex = _tileIndex.usTypeSubIndex - 1;
                                    if (this.FMap.MapTileSet.Length > _tileIndex.ubType)
                                    {
                                        StciIndexed _sti = this.FMap.MapTileSet[_tileIndex.ubType].Sti;
                                        if (_sti != null && _sti.Images.Length > _subIndex)
                                        {
                                            StciSubImage _subImage = _sti.Images[_subIndex];

                                            int _x = (i - j) * StructureImage.TileWidth + _subImage.Header.OffsetX;
                                            int _y = (i + j) * StructureImage.TileHeight + _subImage.Header.OffsetY;

                                            if (aLayerNumber > 3) // крыша
                                                _y -= 50;

                                            _x += this.FMap.WORLD_SIZE * StructureImage.TileWidth / 2;
                                            _y -= this.FMap.WORLD_SIZE * StructureImage.TileHeight / 2;

                                            if (_x < 0 || _x + _subImage.Header.Width > _wbm.Width ||
                                                _y < 0 || _y + _subImage.Header.Height > _wbm.Height)
                                                continue;

                                            for (int l = 0; l < _subImage.ImageData.Length; l++)
                                            {
                                                if (_subImage.ImageData[l] != 0)
                                                {
                                                    int x = _x + l % _subImage.Header.Width;
                                                    int y = _y + l / _subImage.Header.Width;

                                                    int _offset = (int)(y * _wbm.Width + x) * _bytePerPixel;

                                                    if (_offset + 3 >= _imageData.Length)
                                                        continue;

                                                    StciColor _color = _sti.ColorPalette[_subImage.ImageData[l]];
                                                    _imageData[_offset] = _color.Blue;
                                                    _imageData[_offset + 1] = _color.Green;
                                                    _imageData[_offset + 2] = _color.Red;
                                                    _imageData[_offset + 3] = 255;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                }
            }
            Int32Rect _rect = new Int32Rect(0, 0, _imageHeigth * 2, _imageHeigth);

            int _stride = _imageHeigth * 2 * _bytePerPixel;
            _wbm.WritePixels(_rect, _imageData, _stride, 0);
            return _wbm;
        }
    }
}
