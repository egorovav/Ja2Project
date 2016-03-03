using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Ja2Data
{
    public class Map
    {
        public Int32 WORLD_MAX;
        public Int32 WORLD_SIZE = 160;

        public byte NO_SOLDIER = 156;

        public Map(string mapFileName)
        {
            this.FFileName = mapFileName;
            this.WORLD_MAX = this.WORLD_SIZE * this.WORLD_SIZE;
            this.elementes = new MapElement[this.WORLD_MAX];
        }

        public Map(int size, int tileSetId)
        {
            this.WORLD_SIZE = size;
            this.WORLD_MAX = this.WORLD_SIZE * this.WORLD_SIZE;
            this.header.iTilesetID = tileSetId;

            this.elementes = new MapElement[this.WORLD_MAX];
            for (int i = 0; i < this.elementes.Length; i++)
            {
                this.elementes[i] = new MapElement();
                for (int j = 0; j < 5; j++)
                    this.elementes[i].pLevelNodes[j] = new LevelNode(0);
            }
        }

        string FFileName;

        public string FileName
        {
            get { return FFileName; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Header
        {
            public float dMajorMapVersion;
            public byte ubMinorMapVersion;
            public UInt32 uiFlags;
            public Int32 iTilesetID;
            public UInt32 uiSoldierSize;

            public MapFlags Flags
            {
                get { return (MapFlags)this.uiFlags; }
            }
        }//17

        Header header;
        MapElement[] elementes;
        public MapElement[] Elementes
        {
            get { return this.elementes; }
        }


        UInt32 uiNumWorldItems;
        bool gfBasement;
        bool gfCaves;
        byte ubAmbientLightLevel;
        byte ubNumColors;
        SGPPaletteEntry[] colors;
        UInt16 usNumLights;
        LightSprite[] lights;
        string lightTemplateName;
        MapCreateStruct mapInfo;
        BasicSoldierCreateStruct[] soldiers;
        UInt16 usNumSave;
        Dictionary<UInt16, ExitGrid> exitGrids = new Dictionary<UInt16, ExitGrid>();
        byte gubNumDoors;
        Door[] doorTable;

        UInt16 gus1stNorthEdgepointArraySize;
        UInt16 gus1stNorthEdgepointMiddleIndex;
        Int16[] gps1stNorthEdgepointArray;
        UInt16 gus1stEastEdgepointArraySize;
        UInt16 gus1stEastEdgepointMiddleIndex;
        Int16[] gps1stEastEdgepointArray;
        UInt16 gus1stSouthEdgepointArraySize;
        UInt16 gus1stSouthEdgepointMiddleIndex;
        Int16[] gps1stSouthEdgepointArray;
        UInt16 gus1stWestEdgepointArraySize;
        UInt16 gus1stWestEdgepointMiddleIndex;
        Int16[] gps1stWestEdgepointArray;

        UInt16 gus2ndNorthEdgepointArraySize;
        UInt16 gus2ndNorthEdgepointMiddleIndex;
        Int16[] gps2ndNorthEdgepointArray;
        UInt16 gus2ndEastEdgepointArraySize;
        UInt16 gus2ndEastEdgepointMiddleIndex;
        Int16[] gps2ndEastEdgepointArray;
        UInt16 gus2ndSouthEdgepointArraySize;
        UInt16 gus2ndSouthEdgepointMiddleIndex;
        Int16[] gps2ndSouthEdgepointArray;
        UInt16 gus2ndWestEdgepointArraySize;
        UInt16 gus2ndWestEdgepointMiddleIndex;
        Int16[] gps2ndWestEdgepointArray;

        byte[] gubWorldRoomInfo;

        bool fGenerateEdgePoints = false;

        byte gubScheduleID;
        byte ubNumSchedules;
        List<ScheduleNode> gpScheduleList;

        public void Load()
        {
            if (File.Exists(this.FFileName))
            {
                using (FileStream _fs = new FileStream(this.FFileName, FileMode.Open))
                {
                    this.Load(_fs);
                }
            }
            else
            {
                string _mapsFolder = Path.GetDirectoryName(this.FFileName);
                string _dataFolder = Path.GetDirectoryName(_mapsFolder);
                string _mapsSlfFile = Path.Combine(_dataFolder, "Maps.slf");
                if(File.Exists(_mapsSlfFile))
                {
                    SlfFile _mapsSlf = new SlfFile(_mapsSlfFile);
                    _mapsSlf.LoadRecords();
                    string _mapName = Path.GetFileName(this.FFileName);
                    SlfFile.Record _record = _mapsSlf.Records.
                        SingleOrDefault(x => x.FileName.ToUpperInvariant() == _mapName.ToUpperInvariant());
                    if(_record != null)
                    {
                        using(MemoryStream _ms = new MemoryStream(_record.Data))
                        {
                            this.Load(_ms);
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException(
                            String.Format("Record {0} is not found in file {1}", _mapName, _mapsSlfFile));
                    }
                }
                else
                {
                    throw new FileNotFoundException(
                        String.Format("Files {0}, {1} are not found.", this.FFileName, _mapsSlfFile));
                }

            }
        }

        public void Load(Stream input)
        {
            Deserializer deserializer = new Deserializer(input);
            this.header = new Header();
            this.header.dMajorMapVersion = deserializer.DeserializeFloat();
            this.header.ubMinorMapVersion = deserializer.DeserializeByte();
            this.header.uiFlags = deserializer.DeserializeUInt();
            this.header.iTilesetID = deserializer.DeserializeInt();
            this.header.uiSoldierSize = deserializer.DeserializeUInt();
            // FP 0x000010
            // Read height values
            for (Int32 cnt = 0; cnt < WORLD_MAX; cnt++)
            {
                MapElement element = new MapElement();
                element.sHeight = deserializer.DeserializeUShort();
                elementes[cnt] = element;
            }
            // FP 0x00c810
            // Read layer counts
            for (Int32 cnt = 0; cnt < WORLD_MAX; cnt++)
            {
                byte[] layerCountsData = deserializer.DeserializeBytes(4);
                elementes[cnt].SetLayerCounts(layerCountsData);
            }
            // FP 0x025810
            for (UInt32 cnt = 0; cnt < WORLD_MAX; cnt++)
            {
                this.elementes[cnt].pLandHead.Load(deserializer, true);
            }
            for (UInt32 cnt = 0; cnt < WORLD_MAX; cnt++)
            {
                this.elementes[cnt].pLandStart.Load(deserializer, false);
            }
            for (UInt32 cnt = 0; cnt < WORLD_MAX; cnt++)
            {
                this.elementes[cnt].pObjectHead.Load(deserializer, true);
            }
            for (UInt32 cnt = 0; cnt < WORLD_MAX; cnt++)
            {
                this.elementes[cnt].pStructHead.Load(deserializer, true);
            }
            for (UInt32 cnt = 0; cnt < WORLD_MAX; cnt++)
            {
                this.elementes[cnt].pShadowHead.Load(deserializer, true);
            }
            for (UInt32 cnt = 0; cnt < WORLD_MAX; cnt++)
            {
                this.elementes[cnt].pMercHead.Load(deserializer, true);
            }

            return;
            //for (UInt32 cnt = 0; cnt < WORLD_MAX; cnt++)
            //{
            //    this.elementes[cnt].pRoofHead.Load(deserializer, true);
            //}
            //for (UInt32 cnt = 0; cnt < WORLD_MAX; cnt++)
            //{
            //    this.elementes[cnt].pOnRoofHead.Load(deserializer, true);
            //}
            //for (UInt32 cnt = 0; cnt < WORLD_MAX; cnt++)
            //{
            //    this.elementes[cnt].pTopmostHead.Load(deserializer, true);
            //}

            if (this.header.dMajorMapVersion == 6.00 && this.header.ubMinorMapVersion < 27)
            {
                input.Seek(37, SeekOrigin.Current);
            }

            this.gubWorldRoomInfo = deserializer.DeserializeBytes(WORLD_MAX);

            if ((this.header.Flags & MapFlags.MAP_WORLDITEMS_SAVED) != 0)
            {
                this.uiNumWorldItems = deserializer.DeserializeUInt();
                // size(WORLDITEM) = 52
                input.Seek(this.uiNumWorldItems * 52, SeekOrigin.Current);
            }

            if ((this.header.Flags & MapFlags.MAP_AMBIENTLIGHTLEVEL_SAVED) != 0)
            { //Ambient light levels are only saved in underground levels
                this.gfBasement = deserializer.DeserializeBool();
                this.gfCaves = deserializer.DeserializeBool();
                this.ubAmbientLightLevel = deserializer.DeserializeByte();
            }

            if ((this.header.Flags & MapFlags.MAP_WORLDLIGHTS_SAVED) != 0)
            {
                this.ubNumColors = deserializer.DeserializeByte();
                this.colors = new SGPPaletteEntry[this.ubNumColors];
                for (int i = 0; i < this.ubNumColors; i++)
                {
                    this.colors[i] = (SGPPaletteEntry)deserializer.Deserialize(typeof(SGPPaletteEntry));
                }

                this.usNumLights = deserializer.DeserializeUShort();
                this.lights = new LightSprite[this.usNumLights];
                for (int cnt = 0; cnt < this.usNumLights; cnt++)
                {
                    this.lights[cnt] = (LightSprite)deserializer.Deserialize(typeof(LightSprite));
                    this.lightTemplateName = deserializer.DeserializeString();
                }
            }

            this.mapInfo = (MapCreateStruct)deserializer.Deserialize(typeof(MapCreateStruct));

            if ((this.header.Flags & MapFlags.MAP_FULLSOLDIER_SAVED) != 0)
            {
                byte numIndividuals = this.mapInfo.ubNumIndividuals;
                this.mapInfo.ubNumIndividuals = 0;
                this.soldiers = new BasicSoldierCreateStruct[numIndividuals];
                for (int i = 0; i < numIndividuals; i++)
                    this.soldiers[i] = (BasicSoldierCreateStruct)deserializer.Deserialize(typeof(BasicSoldierCreateStruct));
            }

            if ((this.header.Flags & MapFlags.MAP_EXITGRIDS_SAVED) != 0)
            {
                this.usNumSave = deserializer.DeserializeUShort();
                for (int i = 0; i < this.usNumSave; i++)
                {
                    UInt16 exitGridIndex = deserializer.DeserializeUShort();
                    ExitGrid exitGrid = (ExitGrid)deserializer.Deserialize(typeof(ExitGrid));
                    if (!this.exitGrids.ContainsKey(exitGridIndex))
                        this.exitGrids.Add(exitGridIndex, exitGrid);
                }
            }

            if ((this.header.Flags & MapFlags.MAP_DOORTABLE_SAVED) != 0)
            {
                this.gubNumDoors = deserializer.DeserializeByte();
                this.doorTable = new Door[this.gubNumDoors];
                for (int i = 0; i < this.gubNumDoors; i++)
                {
                    this.doorTable[i] = (Door)deserializer.Deserialize(typeof(Door));
                }
            }

            if ((this.header.Flags & MapFlags.MAP_EDGEPOINTS_SAVED) != 0)
            {
                this.gus1stNorthEdgepointArraySize = deserializer.DeserializeUShort();
                this.gus1stNorthEdgepointMiddleIndex = deserializer.DeserializeUShort();
                if (gus1stNorthEdgepointArraySize > 0)
                {
                    this.gps1stNorthEdgepointArray = new Int16[this.gus1stNorthEdgepointArraySize];
                    for (int i = 0; i < this.gus1stNorthEdgepointArraySize; i++)
                        this.gps1stNorthEdgepointArray[i] = deserializer.DeserializeShort();
                }
                this.gus1stEastEdgepointArraySize = deserializer.DeserializeUShort();
                this.gus1stEastEdgepointMiddleIndex = deserializer.DeserializeUShort();
                if (gus1stEastEdgepointArraySize > 0)
                {
                    this.gps1stEastEdgepointArray = new Int16[this.gus1stEastEdgepointArraySize];
                    for (int i = 0; i < this.gus1stEastEdgepointArraySize; i++)
                        this.gps1stEastEdgepointArray[i] = deserializer.DeserializeShort();
                }
                this.gus1stSouthEdgepointArraySize = deserializer.DeserializeUShort();
                this.gus1stSouthEdgepointMiddleIndex = deserializer.DeserializeUShort();
                if (gus1stSouthEdgepointArraySize > 0)
                {
                    this.gps1stSouthEdgepointArray = new Int16[this.gus1stSouthEdgepointArraySize];
                    for (int i = 0; i < this.gus1stSouthEdgepointArraySize; i++)
                        this.gps1stSouthEdgepointArray[i] = deserializer.DeserializeShort();
                }
                this.gus1stWestEdgepointArraySize = deserializer.DeserializeUShort();
                this.gus1stWestEdgepointMiddleIndex = deserializer.DeserializeUShort();
                if (gus1stWestEdgepointArraySize > 0)
                {
                    this.gps1stWestEdgepointArray = new Int16[this.gus1stWestEdgepointArraySize];
                    for (int i = 0; i < this.gus1stWestEdgepointArraySize; i++)
                        this.gps1stWestEdgepointArray[i] = deserializer.DeserializeShort();
                }

                if (this.mapInfo.ubMapVersion < 17)
                {	//To prevent invalidation of older maps, which only used one layer of edgepoints, and a UINT8 for 
                    //containing the size, we will preserve that paradigm, then kill the loaded edgepoints and 
                    //regenerate them.
                    //OldLoadMapEdgepoints(hBuffer);
                    //TrashMapEdgepoints();
                    this.fGenerateEdgePoints = true; //only if the map had the older edgepoint system
                    return;
                }
                else
                {
                    this.gus2ndNorthEdgepointArraySize = deserializer.DeserializeUShort();
                    this.gus2ndNorthEdgepointMiddleIndex = deserializer.DeserializeUShort();
                    if (gus2ndNorthEdgepointArraySize > 0)
                    {
                        this.gps2ndNorthEdgepointArray = new Int16[this.gus2ndNorthEdgepointArraySize];
                        for (int i = 0; i < this.gus2ndNorthEdgepointArraySize; i++)
                            this.gps2ndNorthEdgepointArray[i] = deserializer.DeserializeShort();
                    }
                    this.gus2ndEastEdgepointArraySize = deserializer.DeserializeUShort();
                    this.gus2ndEastEdgepointMiddleIndex = deserializer.DeserializeUShort();
                    if (gus2ndEastEdgepointArraySize > 0)
                    {
                        this.gps2ndEastEdgepointArray = new Int16[this.gus2ndEastEdgepointArraySize];
                        for (int i = 0; i < this.gus2ndEastEdgepointArraySize; i++)
                            this.gps2ndEastEdgepointArray[i] = deserializer.DeserializeShort();
                    }
                    this.gus2ndSouthEdgepointArraySize = deserializer.DeserializeUShort();
                    this.gus2ndSouthEdgepointMiddleIndex = deserializer.DeserializeUShort();
                    if (gus2ndSouthEdgepointArraySize > 0)
                    {
                        this.gps2ndSouthEdgepointArray = new Int16[this.gus2ndSouthEdgepointArraySize];
                        for (int i = 0; i < this.gus2ndSouthEdgepointArraySize; i++)
                            this.gps2ndSouthEdgepointArray[i] = deserializer.DeserializeShort();
                    }
                    this.gus2ndWestEdgepointArraySize = deserializer.DeserializeUShort();
                    this.gus2ndWestEdgepointMiddleIndex = deserializer.DeserializeUShort();
                    if (gus2ndWestEdgepointArraySize > 0)
                    {
                        this.gps2ndWestEdgepointArray = new Int16[this.gus2ndWestEdgepointArraySize];
                        for (int i = 0; i < this.gus2ndWestEdgepointArraySize; i++)
                            this.gps2ndWestEdgepointArray[i] = deserializer.DeserializeShort();
                    }
                }

                if (this.mapInfo.ubMapVersion < 22)
                {	//regenerate them.
                    //TrashMapEdgepoints();
                    this.fGenerateEdgePoints = true; //only if the map had the older edgepoint system
                }
            }
            else
            {
                fGenerateEdgePoints = true;
            }

            if ((this.header.Flags & MapFlags.MAP_NPCSCHEDULES_SAVED) != 0)
            {
                this.gubScheduleID = 1;
                this.ubNumSchedules = deserializer.DeserializeByte();
                this.gpScheduleList = new List<ScheduleNode>();
                for (int i = 0; i < this.ubNumSchedules; i++)
                {
                    ScheduleNode schedule = (ScheduleNode)deserializer.Deserialize(typeof(ScheduleNode));
                    schedule.ubScheduleID = gubScheduleID;
                    schedule.ubSoldierID = NO_SOLDIER;
                    this.gpScheduleList.Add(schedule);
                    this.gubScheduleID++;
                }
            }
        }

        TileObject[] tileSet = new TileObject[TileSet.NumOfTileTypes];

        public TileObject[] MapTileSet
        {
            get { return this.tileSet; }
            set { this.tileSet = value; }
        }

        public string[] TileSurfaceFilenames
        {
            get { return TileSet.TileSets[TilesetID].TileSurfaceFilenames; }
        }

        public Int32 TilesetID
        {
            get { return this.header.iTilesetID; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SGPPaletteEntry
        {
            public byte peRed;
            public byte peGreen;
            public byte peBlue;
            public byte peFlags;
        }

        // structure of light instance, or sprite (a copy of the template)
        [StructLayout(LayoutKind.Sequential)]
        public struct LightSprite
        {
            public Int16 iX, iY;
            public Int16 iOldX, iOldY;
            public Int16 iAnimSpeed;
            public Int32 iTemplate;
            public UInt32 uiFlags;
            public UInt32 uiLightType;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MapCreateStruct
        {
            //These are the mandatory entry points for a map.  If any of the values are -1, then that means that
            //the point has been specifically not used and that the map is not traversable to or from an adjacent 
            //sector in that direction.  The >0 value points must be validated before saving the map.  This is 
            //done by simply checking if those points are sittable by mercs, and that you can plot a path from 
            //these points to each other.  These values can only be set by the editor : mapinfo tab
            public Int16 sNorthGridNo;
            public Int16 sEastGridNo;
            public Int16 sSouthGridNo;
            public Int16 sWestGridNo;
            //This contains the number of individuals in the map.
            //Individuals include NPCs, enemy placements, creatures, civilians, rebels, and animals.
            public byte ubNumIndividuals;
            public byte ubMapVersion;
            public byte ubRestrictedScrollID;
            public byte ubEditorSmoothingType;  //normal, basement, or caves
            public Int16 sCenterGridNo;
            public Int16 sIsolatedGridNo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 83)]
            public byte[] bPadding;	//I'm sure lots of map info will be added
        } //99 bytes

        [StructLayout(LayoutKind.Sequential)]
        public struct BasicSoldierCreateStruct
        {
            public bool fDetailedPlacement;			//Specialized information.  Has a counterpart containing all info.
            public UInt16 usStartingGridNo;				//Where the placement position is.
            public sbyte bTeam;											//The team this individual is part of.
            public sbyte bRelativeAttributeLevel;
            public sbyte bRelativeEquipmentLevel;
            public sbyte bDirection;								//1 of 8 values (always mandatory)
            public sbyte bOrders;
            public sbyte bAttitude;
            public sbyte bBodyType;									//up to 128 body types, -1 means random
            //MAXPATROLGRIDS = 10
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            Int16[] sPatrolGrid; //possible locations to visit, patrol, etc.
            public sbyte bPatrolCnt;
            public bool fOnRoof;
            public byte ubSoldierClass;							//army, administrator, elite
            public byte ubCivilianGroup;
            public bool fPriorityExistance;			//These slots are used first
            public bool fHasKeys;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
            public byte[] PADDINGSLOTS;
        } //50 bytes

        [StructLayout(LayoutKind.Sequential)]
        public struct ExitGrid //for exit grids (object level)
        { //if an item pool is also in same gridno, then this would be a separate levelnode
            //in the object level list
            UInt16 usGridNo; //sweet spot for placing mercs in new sector.
            public byte ubGotoSectorX;
            public byte ubGotoSectorY;
            public byte ubGotoSectorZ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Door
        {
            public Int16 sGridNo;
            public bool fLocked;							// is the door locked
            public byte ubTrapLevel;					// difficulty of finding the trap, 0-10
            public byte ubTrapID;							// the trap type (0 is no trap)
            public byte ubLockID;							// the lock (0 is no lock)
            public sbyte bPerceivedLocked;			// The perceived lock value can be different than the fLocked.
            // Values for this include the fact that we don't know the status of
            // the door, etc
            public sbyte bPerceivedTrapped;		// See above, but with respect to traps rather than locked status
            public sbyte bLockDamage;					// Damage to the lock
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public sbyte[] bPadding;					// extra bytes
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ScheduleNode
        {
            //MAX_SCHEDULE_ACTIONS = 4
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public UInt16[] usTime;	//converted to minutes 12:30PM would be 12*60 + 30 = 750
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public UInt16[] usData1; //typically the gridno, but depends on the action
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public UInt16[] usData2; //secondary information, not used by most actions
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] ubAction;
            public byte ubScheduleID;
            public byte ubSoldierID;
            public UInt16 usFlags;
        }

        public class TileObject
        {
            public TileObject(StciIndexed sti, JsdFile jsd)
            {
                this.jsd = jsd;
                this.sti = sti;
            }

            StciIndexed sti;
            JsdFile jsd;

            public StciIndexed Sti
            {
                get { return this.sti; }
            }

            public JsdFile Jsd
            {
                get { return jsd; }
            }
        }

        [Flags]
        public enum MapFlags : uint
        {
            MAP_FULLSOLDIER_SAVED = 0x00000001,
            MAP_WORLDONLY_SAVED = 0x00000002,
            MAP_WORLDLIGHTS_SAVED = 0x00000004,
            MAP_WORLDITEMS_SAVED = 0x00000008,
            MAP_EXITGRIDS_SAVED = 0x00000010,
            MAP_DOORTABLE_SAVED = 0x00000020,
            MAP_EDGEPOINTS_SAVED = 0x00000040,
            MAP_AMBIENTLIGHTLEVEL_SAVED = 0x00000080,
            MAP_NPCSCHEDULES_SAVED = 0x00000100
        }

    }
}
