using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ja2Data
{
    public class TileSet
    {
        const int FileNameSize = 32;
        /// <summary>
        ///  Число наборов тайлов (tile set) в стандартном файле Ja2Set.dat
        /// </summary>
        public static byte NumOfTileSets;
        /// <summary>
        ///  Число типов тайлов. 
        /// </summary>
        public static int NumOfTileTypes;

        public static TileSet[] TileSets;


        public TileSet()
        {
        }
        public TileSet(int namesNum)
        {
            this.tileSurfaceFilenames = new string[namesNum];
        }

        public struct Data
        {
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] zNameData;
            public byte ubAmbientID;
            // 32 x 151 = 4832
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = NumOfTileTypes * 32)]
            public byte[] fileNamesData;
        }

        public static void Load(Stream input)
        {
            Deserializer deserializer = new Deserializer(input);

            NumOfTileSets = deserializer.DeserializeByte();
            NumOfTileTypes = deserializer.DeserializeInt();

            TileSets = new TileSet[NumOfTileSets];

            Data data = new Data();
            for (int i = 0; i < NumOfTileSets; i++)
            {
                data.zNameData = deserializer.DeserializeBytes(FileNameSize);
                data.ubAmbientID = deserializer.DeserializeByte();
                data.fileNamesData = deserializer.DeserializeBytes(NumOfTileTypes * FileNameSize);

                TileSet tileSet = new TileSet();
                tileSet.zName = Common.ByteArrayToString(data.zNameData);
                tileSet.ubAmbientID = data.ubAmbientID;
                for (int j = 0; j < NumOfTileTypes; j++)
                {
                    tileSet.tileSurfaceFilenames[j] =
                        Common.ByteArrayToString(data.fileNamesData, j * FileNameSize, FileNameSize);
                }
                TileSet.TileSets[i] = tileSet;
            }
        }

        public string zName;
        public string ZName
        {
            get { return zName; }
            set { zName = value; }
        }

        string[] tileSurfaceFilenames = new string[NumOfTileTypes];
        public string[] TileSurfaceFilenames
        {
            get { return tileSurfaceFilenames; }
            set { tileSurfaceFilenames = value; }
        }

        public string Tiles
        {
            get { return String.Join(";", TileSurfaceFilenames); }
        }
        public byte ubAmbientID;
        public byte UbAmbientID
        {
            get { return ubAmbientID; }
        }

        public static string TileSetsString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < NumOfTileSets; i++)
                {
                    sb.AppendLine(TileSet.TileSets[i].Tiles);
                }
                return sb.ToString();
            }
        }
        //public TILESET_CALLBACK MovementCostFnc;
    }
}
