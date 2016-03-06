using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ja2Data
{
    public class MapElement
    {
        public LevelNode pLandHead
        {
            get { return this.pLevelNodes[0]; }
            set { this.pLevelNodes[0] = value; }
        }						//0
        public LevelNode pLandStart
        {
            get { return this.pLevelNodes[1]; }
            set { this.pLevelNodes[1] = value; }
        }						//1
        public LevelNode pObjectHead
        {
            get { return this.pLevelNodes[2]; }
            set { this.pLevelNodes[2] = value; }
        }					//2
        public LevelNode pStructHead
        {
            get { return this.pLevelNodes[3]; }
            set { this.pLevelNodes[3] = value; }
        }					//3
        public LevelNode pShadowHead
        {
            get { return this.pLevelNodes[4]; }
            set { this.pLevelNodes[4] = value; }
        }					//4
        public LevelNode pMercHead
        {
            get { return this.pLevelNodes[5]; }
            set { this.pLevelNodes[5] = value; }
        }						//5
        public LevelNode pRoofHead
        {
            get { return this.pLevelNodes[6]; }
            set { this.pLevelNodes[6] = value; }
        }						//6
        public LevelNode pOnRoofHead
        {
            get { return this.pLevelNodes[7]; }
            set { this.pLevelNodes[7] = value; }
        }					//7
        public LevelNode pTopmostHead
        {
            get { return this.pLevelNodes[8]; }
            set { this.pLevelNodes[8] = value; }
        }					//8

        public LevelNode[] pLevelNodes = new LevelNode[9];

        public UInt16 sHeight;
        public UInt32 uiFlags;
        public byte gubWorldRoomInfo;

        public void SetLayerCounts(byte[] layerCountsData)
        {
            // Read combination of land/world flags
            byte ubCombine = layerCountsData[0];
            this.pLevelNodes[0] = new LevelNode(ubCombine & 0x0f);
            this.uiFlags |= (byte)((ubCombine & 0xf0) >> 4);
            // Read #objects, structs
            ubCombine = layerCountsData[1];
            this.pLevelNodes[1] = new LevelNode(ubCombine & 0x0f);
            this.pLevelNodes[2] = new LevelNode((ubCombine & 0xf0) >> 4);
            // Read shadows, roof
            ubCombine = layerCountsData[2];
            this.pLevelNodes[3] = new LevelNode(ubCombine & 0x0f);
            this.pLevelNodes[4] = new LevelNode((ubCombine & 0xf0) >> 4);
            // Read OnRoof, nothing
            ubCombine = layerCountsData[3];
            this.pLevelNodes[5] = new LevelNode(ubCombine & 0x0f);
        }
    }

    public class LevelNode
    {
        public LevelNode(int count)
        {
            this.tileIndexes = new TileIndex[count];
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TileIndex
        {
            public byte ubType;
            public UInt16 usTypeSubIndex;
        }

        public TileIndex[] tileIndexes;

        public void Load(Deserializer deserializer, bool isSmall)
        {
            for (int i = 0; i < this.tileIndexes.Length; i++)
            {
                TileIndex tileIndex = new TileIndex();
                tileIndex.ubType = deserializer.DeserializeByte();
                if (isSmall)
                    tileIndex.usTypeSubIndex = (ushort)deserializer.DeserializeByte();
                else
                    tileIndex.usTypeSubIndex = deserializer.DeserializeUShort();
                this.tileIndexes[i] = tileIndex;
            }
        }
    }
}
