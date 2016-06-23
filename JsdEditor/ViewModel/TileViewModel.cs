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
    public class TileViewModel : BaseViewModel
    {
        public TileViewModel(JsdTile aTile)
        {
            this.FTile = aTile;
            this.FShapeViewModel = new ShapeViewModel(this.FTile.IsHighDefenition);
            this.FShapeViewModel.Shape = this.FTile.Shape;
            this.FShapeViewModel.PropertyChanged += ShapeViewModel_PropertyChanged;
            this.PropertyChanged += TileViewModel_PropertyChanged;
        }

        public bool IsHighDefenition
        {
            get { return this.FTile.IsHighDefenition; }
        }

        private JsdTile FTile;
        public JsdTile Tile
        {
            get { return this.FTile; }
        }

        private ShapeViewModel FShapeViewModel;
        public ShapeViewModel ShapeViewModel
        {
            get 
            { 
                return this.FShapeViewModel; 
            }
        }

        public static string XPosRelToBasePropertyName = "XPosRelToBase";
        public int XPosRelToBase
        {
            get { return this.FTile.XPosRelToBase; }
            set 
            { 
                this.FTile.XPosRelToBase = value;
                NotifyPropertyChanged(XPosRelToBasePropertyName);
            }
        }

        public static string YPosRelToBasePropertyName = "YPosRelToBase";
        public int YPosRelToBase
        {
            get { return this.FTile.YPosRelToBase; }
            set 
            { 
                this.FTile.YPosRelToBase = value;
                NotifyPropertyChanged(YPosRelToBasePropertyName);
            }
        }

        public static string TileIsPassablePropertyName = "TileIsPassable";
        public bool TileIsPassable
        {
            get { return (this.FTile.Flags & JsdTile.JsdTileFlags.TILE_PASSABLE) > 0; }
            set 
            {
                if (value)
                    this.FTile.Flags |= JsdTile.JsdTileFlags.TILE_PASSABLE;
                else
                    this.FTile.Flags &= ~JsdTile.JsdTileFlags.TILE_PASSABLE;

                NotifyPropertyChanged(TileIsPassablePropertyName);
            }
        }

        public static string TileIsOnRoofPropertyName = "TileIsOnRoof";
        public bool TileIsOnRoof
        {
            get { return (this.FTile.Flags & JsdTile.JsdTileFlags.TILE_ON_ROOF) > 0; }
            set 
            {
                if (value)
                    this.FTile.Flags |= JsdTile.JsdTileFlags.TILE_ON_ROOF;
                else
                    this.FTile.Flags &= ~JsdTile.JsdTileFlags.TILE_ON_ROOF;

                NotifyPropertyChanged(TileIsOnRoofPropertyName);
            }
        }

        public static string VehicleHitLocationPropertyName = "VehicleHitLocation";
        public byte VehicleHitLocation
        {
            get { return this.FTile.VehicleHitLocation; }
            set
            {
                this.FTile.VehicleHitLocation = value;
                NotifyPropertyChanged(VehicleHitLocationPropertyName);
            }
        }

        public bool IsSelected
        {
            get;
            set;
        }

        private List<object> FReceivers = new List<object>();

        public void AttachHandler(object aReciever, PropertyChangedEventHandler aHandler)
        {
            if (!this.FReceivers.Contains(aReciever))
            {
                this.PropertyChanged += aHandler;
                this.FReceivers.Add(aReciever);
            }
        }

        public void DettachHandler(object aReciever, PropertyChangedEventHandler aHandler)
        {
            if (this.FReceivers.Contains(aReciever))
            {
                this.PropertyChanged -= aHandler;
                this.FReceivers.Remove(aReciever);
            }
        }

        private void TileViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.IsTileChanged = true;
        }

        private void ShapeViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.IsTileChanged = true;
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", this.XPosRelToBase, this.YPosRelToBase, this.TileIsOnRoof);

        }

        public bool IsTileChanged 
        { 
            get; 
            set; 
        }
    }
}
