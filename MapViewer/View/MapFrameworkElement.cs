using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MapViewer
{
    public class MapFrameworkElement : FrameworkElement
    {
        private const int LayersNumber = 5;

        public Visual[] FVisuals = new Visual[LayersNumber];
        private DrawingVisual[] FCacheVisual = new DrawingVisual[LayersNumber];
        private int FDrewElementsCount;
        private int FTotalElementsCount;
        private ProgressHolder FProgress;

        protected override int VisualChildrenCount
        {
            get { return this.FVisuals.Length; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.FVisuals[index];
        }

        public void DrawMapByLayer(Map aMap, bool[] aDrawedLayers)
        {
            this.FDrewElementsCount = 0;
            List<MapElement> _elements = new List<MapElement>();

            for (int i = 0; i < aMap.WORLD_SIZE; i++)
                for (int j = 0; j < aMap.WORLD_SIZE; j++)
                {
                    if (i + j > 3 * aMap.WORLD_SIZE / 2 ||
                        i + j < aMap.WORLD_SIZE / 2 ||
                        i - j > aMap.WORLD_SIZE / 2 ||
                        j - i > aMap.WORLD_SIZE / 2)
                        continue;

                    _elements.Add(aMap.Elementes[aMap.WORLD_SIZE * j + i]);
                }

            this.FTotalElementsCount =
                  _elements.Select(x => x.pLevelNodes.
                      Where(y => y != null).
                      Select(y => y.tileIndexes.Length).
                      Sum()).
                  Sum();

            this.FProgress = new ProgressHolder();
            try
            {

                Thread _thr = new Thread(ProgressWindowShow);
                _thr.SetApartmentState(ApartmentState.STA);
                _thr.IsBackground = true;
                _thr.Start(this.FProgress);

                for (int i = 0; i < aDrawedLayers.Length; i++)
                {
                    DrawingVisual _layerVisual = this.DrawMapLayerByTiles(aMap, i);
                    this.FCacheVisual[i] = _layerVisual;

                    if (aDrawedLayers[i])
                    {
                        if (this.FVisuals[i] != null)
                        {
                            this.RemoveVisual(i);
                        }

                        this.AddVisual(i);
                    }
                }
            }
            finally
            {
                this.FProgress.Progress = -1;
            }
        }

        private void ProgressWindowShow(object aProgressHolder)
        {
            ProgressHolder _ph = (ProgressHolder)aProgressHolder;
            ProgressWindow _pw = new ProgressWindow(_ph);
            _pw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _pw.Topmost = true;
            _pw.Show();
            System.Windows.Threading.Dispatcher.Run();
        }

        public DrawingVisual DrawMapLayerByLevels(Map aMap, int aLayerNumber)
        {
            DrawingVisual _visual = new DrawingVisual();
            using (DrawingContext _context = _visual.RenderOpen())
            {
                int _maxLevel = aMap.Elementes.Select(x => x.pLevelNodes[aLayerNumber].tileIndexes.Length).Max();

                for (int _index = 0; _index < _maxLevel; _index++)
                {
                    this.DrawLevel(aMap, aLayerNumber, _context, _index);
                }
            }

            return _visual;
        }

        private DrawingVisual DrawMapLayerByTiles(Map aMap, int aLayerNumber)
        {
            DrawingVisual _visual = new DrawingVisual();
            using (DrawingContext _context = _visual.RenderOpen())
            {
                for (int i = 0; i < aMap.WORLD_SIZE; i++)
                    for (int j = 0; j < aMap.WORLD_SIZE; j++)
                    {
                        if (i + j > 3 * aMap.WORLD_SIZE / 2 || 
                            i + j < aMap.WORLD_SIZE / 2 ||
                            i - j > aMap.WORLD_SIZE / 2 ||
                            i - j < -aMap.WORLD_SIZE / 2)
                            continue;

                        MapElement _element = aMap.Elementes[aMap.WORLD_SIZE * j + i];
                        LevelNode.TileIndex[] _indexes = _element.pLevelNodes[aLayerNumber].tileIndexes;
                        for (int _index = 0; _index < _indexes.Length; _index++)
                        {
                            if (_indexes.Length > _index)
                            {
                                LevelNode.TileIndex _tileIndex = _indexes[_index];
                                int _subIndex = _tileIndex.usTypeSubIndex - 1;
                                if (aMap.MapTileSet.Length > _tileIndex.ubType)
                                {
                                    StciIndexed _sti = aMap.MapTileSet[_tileIndex.ubType].Sti;
                                    if (_sti != null && _sti.Images.Length > _subIndex)
                                    {
                                        List<Color> _palette = _sti.ColorPalette
                                            .Select(z => new Color() { A = 255, R = z.Red, G = z.Green, B = z.Blue })
                                            .ToList();

                                        StciSubImage _subImage = _sti.Images[_subIndex];
                                        StructureImage _image = new StructureImage(_subImage, _palette);

                                        int _x = (i - j) * StructureImage.TileWidth + _subImage.Header.OffsetX;
                                        int _y = (i + j) * StructureImage.TileHeight + _subImage.Header.OffsetY;

                                        // Надо поднимать всё кроме клифов. Как пока не понятно.

                                        //if (aLayerNumber == 1 && _element.sHeight > 0)
                                        //{
                                        //    int _heigthLevel = _element.sHeight % 256;
                                        //    int _heigth = _element.sHeight / 256;

                                        //    _y -= _heigthLevel + _heigth;
                                        //}

                                        if (aLayerNumber > 3) // крыша
                                            _y -= 50;

                                        Point point = new Point(
                                            _x + aMap.WORLD_SIZE * StructureImage.TileWidth / 2,
                                            _y - aMap.WORLD_SIZE * StructureImage.TileHeight / 2
                                        );
                                        Rect _rect = new Rect(
                                            point, new Size(_subImage.Header.Width, _subImage.Header.Height));

                                        _context.DrawImage(_image.Bitmap, _rect);
                                        this.FDrewElementsCount++;
                                        this.FProgress.Progress = FDrewElementsCount * 100 / this.FTotalElementsCount;
                                    }
                                }
                            }
                        }
                    }
            }
            return _visual;
        }

        private void DrawLevel(Map aMap, int aLayerNumber, DrawingContext aContext, int aLevel)
        {
            for (int i = 0; i < aMap.WORLD_SIZE; i++)
                for (int j = 0; j < aMap.WORLD_SIZE; j++)
                {
                    MapElement _element = aMap.Elementes[aMap.WORLD_SIZE * j + i];
                    LevelNode.TileIndex[] _indexes = _element.pLevelNodes[aLayerNumber].tileIndexes;
                    if (_indexes.Length > aLevel)
                    {
                        LevelNode.TileIndex _tileIndex = _indexes[aLevel];
                        int _subIndex = _tileIndex.usTypeSubIndex - 1;
                        if (aMap.MapTileSet.Length > _tileIndex.ubType)
                        {
                            StciIndexed _sti = aMap.MapTileSet[_tileIndex.ubType].Sti;
                            if (_sti != null && _sti.Images.Length > _subIndex)
                            {
                                List<Color> _palette = _sti.ColorPalette
                                    .Select(x => new Color() { A = 255, R = x.Red, G = x.Green, B = x.Blue })
                                    .ToList();

                                StciSubImage _subImage = _sti.Images[_subIndex];
                                StructureImage _image = new StructureImage(_subImage, _palette);

                                int _x = (i - j) * StructureImage.TileWidth / 2 + _subImage.Header.OffsetX;
                                int _y = (i + j) * StructureImage.TileHeight / 2 + _subImage.Header.OffsetY;

                                if (aLayerNumber > 3) // крыша
                                    _y -= 50;

                                Point point = new Point(
                                    _x + aMap.WORLD_SIZE * StructureImage.TileWidth / 2,
                                    _y - aMap.WORLD_SIZE * StructureImage.TileHeight / 2);
                                Rect _rect = new Rect(
                                    point, new Size(_subImage.Header.Width, _subImage.Header.Height));

                                aContext.DrawImage(_image.Bitmap, _rect);
                                this.FDrewElementsCount++;
                                this.FProgress.Progress = FDrewElementsCount * 100 / this.FTotalElementsCount;
                            }
                        }
                    }
                }
        }

        public void RemoveVisual(int aLayerNumber)
        {
            base.RemoveVisualChild(this.FVisuals[aLayerNumber]);
            base.RemoveLogicalChild(this.FVisuals[aLayerNumber]);

            this.FVisuals[aLayerNumber] = null;
        }

        public void AddVisual(int aLayerNumber)
        {
            Visual _addedVisual = this.FCacheVisual[aLayerNumber];
            if (!this.FVisuals.Contains(_addedVisual))
            {
                base.AddVisualChild(_addedVisual);
                base.AddLogicalChild(_addedVisual);

                this.FVisuals[aLayerNumber] = _addedVisual;
            }
        }

        public void DrawMap(Map aMap)
        {
            for (int k = 0; k < 5; k++)
            {
                for (int i = 0; i < aMap.WORLD_SIZE; i++)
                {
                    for (int j = 0; j < aMap.WORLD_SIZE; j++)
                    {
                        MapElement _element = aMap.Elementes[aMap.WORLD_SIZE * j + i];
                        LevelNode.TileIndex[] _indexes = _element.pLevelNodes[k].tileIndexes;
                        foreach (LevelNode.TileIndex _tileIndex in _indexes)
                        {
                            int _subIndex = _tileIndex.usTypeSubIndex - 1;
                            if (aMap.MapTileSet.Length > _tileIndex.ubType)
                            {
                                StciIndexed _sti = aMap.MapTileSet[_tileIndex.ubType].Sti;
                                if (_sti.Images.Length > _subIndex)
                                {
                                    List<Color> _palette = _sti.ColorPalette
                                        .Select(x => new Color() { A = 255, R = x.Red, G = x.Green, B = x.Blue })
                                        .ToList();

                                    StciSubImage _subImage = _sti.Images[_subIndex];
                                    StructureImage _image = new StructureImage(_subImage, _palette);

                                    int _x = (i - j) * StructureImage.TileWidth + _subImage.Header.OffsetX;
                                    int _y = (i + j) * StructureImage.TileHeight + _subImage.Header.OffsetY;

                                    if (k > 3) // крыша
                                        _y -= 50;

                                    Point point = new Point(
                                        _x + aMap.WORLD_SIZE * StructureImage.TileWidth / 2,
                                        _y - aMap.WORLD_SIZE * StructureImage.TileHeight / 2);
                                    Rect _rect = new Rect(
                                        point, new Size(_subImage.Header.Width, _subImage.Header.Height));

                                    DrawingVisual _visual = new DrawingVisual();
                                    using (DrawingContext _context = _visual.RenderOpen())
                                    {
                                        _context.DrawImage(_image.Bitmap, _rect);
                                    }
                                    this.FVisuals[k] = _visual;

                                    base.AddVisualChild(_visual);
                                    base.AddLogicalChild(_visual);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class DrawingMapInfo
    {
        public DrawingMapInfo(Map aMap, bool[] aIsDrewLayers)
        {
            this.Map = aMap;
            this.IsDrewLayers = aIsDrewLayers;
        }

        public Map Map { get; protected set; }
        public bool[] IsDrewLayers { get; protected set; }
    }
}
