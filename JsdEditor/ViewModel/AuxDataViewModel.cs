using CommonWpfControls;
using Ja2Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace JsdEditor
{
    public class AuxDataViewModel : BaseViewModel
    {
        public AuxDataViewModel(AuxObjectData aAuxData, List<byte> aTileLocData)
        {
            if (aAuxData != null)
                this.FAuxData = aAuxData;
            else
                this.FAuxData = new AuxObjectData();

            if (aTileLocData != null)
            {
                List<RelTileLoc> _points = new List<RelTileLoc>();
                for (int i = 0; i < aTileLocData.Count; i += 2)
                {
                    _points.Add(new RelTileLoc(aTileLocData[i], aTileLocData[i + 1]));
                }
                this.TileLocData = new ObservableCollection<RelTileLoc>(_points);
            }
        }

        private AuxObjectData FAuxData;
        public AuxObjectData AuxData
        {
            get { return this.FAuxData; }
        }

        public static string WallOrientationPropertyName = "WallOrientation";
        public byte WallOrientation
        {
            get { return this.FAuxData.WallOrientation; }
            set 
            { 
                this.FAuxData.WallOrientation = value;
                NotifyPropertyChanged(WallOrientationPropertyName);
            }
        }

        public static string NumberOfTilesPropertyName = "NumberOfTiles";
        public byte NumberOfTiles
        {
            get { return this.FAuxData.NumberOfTiles; }
            set 
            { 
                this.FAuxData.NumberOfTiles = value;
                NotifyPropertyChanged(NumberOfTilesPropertyName);
            }
        }

        public static string TileLocIndexPropertyName = "TileLocIndex";
        public UInt16 TileLocIndex
        {
            get { return this.FAuxData.TileLocIndex; }
            set 
            { 
                this.FAuxData.TileLocIndex = value;
                NotifyPropertyChanged(TileLocIndexPropertyName);
            }
        }

        public static string CurrentFramePropertyName = "CurrentFrame";
        public byte CurrentFrame
        {
            get { return this.FAuxData.CurrentFrame; }
            set 
            { 
                this.FAuxData.CurrentFrame = value;
                NotifyPropertyChanged(CurrentFramePropertyName);
            }
        }

        public static string NumberOfFramesPropertyName = "NumberOfFrames";
        public byte NumberOfFrames
        {
            get { return this.FAuxData.NumberOfFrames; }
            set 
            {
                this.FAuxData.NumberOfFrames = value;
                NotifyPropertyChanged(NumberOfFramesPropertyName);
            }
        }

        public static string FlagsPropertyName = "Flags";
        public AuxObjectFlags Flags
        {
            get { return this.FAuxData.Flags; }
            set 
            { 
                this.FAuxData.Flags = value;
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
                this.Flags = (AuxObjectFlags)value.Flags;
            }
        }

        public ObservableCollection<RelTileLoc> TileLocData
        {
            get;
            protected set;
        }
    }

    public class RelTileLoc : BaseViewModel
    {
        public RelTileLoc()
        {

        }

        public RelTileLoc(byte aX, byte aY)
        {
            this.FX = (sbyte)aX;
            this.FY = (sbyte)aY;
        }

        public static string XPropertyName = "X";
        private sbyte FX;
        public sbyte X
        {
            get { return this.FX; }
            set
            {
                this.FX = value;
                NotifyPropertyChanged(XPropertyName);
            }
        }

        public static string YPropertyName = "Y";
        private sbyte FY;
        public sbyte Y
        {
            get { return this.FY; }
            set
            {
                this.FY = value;
                NotifyPropertyChanged(YPropertyName);
            }
        }
    }
}
