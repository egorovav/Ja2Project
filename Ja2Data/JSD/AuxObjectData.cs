using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Ja2Data
{
    public class AuxObjectData
    {
		public const int SIZE = 16;

        [StructLayout(LayoutKind.Sequential)]
        public struct Header
        {
            public byte ubWallOrientation;
            public byte ubNumberOfTiles;
            public UInt16 usTileLocIndex;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] ubUnused1;
            public byte ubCurrentFrame;
            public byte ubNumberOfFrames;
            public byte fFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] ubUnused;
        }

        Header FHeader;

        public byte WallOrientation
        {
            get { return this.FHeader.ubWallOrientation; }
            set { this.FHeader.ubWallOrientation = value; }
        }
        public byte NumberOfTiles
        {
            get { return this.FHeader.ubNumberOfTiles; }
            set { this.FHeader.ubNumberOfTiles = value; }
        }
        public UInt16 TileLocIndex
        {
            get { return this.FHeader.usTileLocIndex; }
            set { this.FHeader.usTileLocIndex = value; }
        }
        public byte CurrentFrame
        {
            get { return this.FHeader.ubCurrentFrame; }
            set { this.FHeader.ubCurrentFrame = value; }
        }
        public byte NumberOfFrames
        {
            get { return this.FHeader.ubNumberOfFrames; }
            set { this.FHeader.ubNumberOfFrames = value; }
        }
        public AuxObjectFlags Flags
        {
            get { return (AuxObjectFlags)this.FHeader.fFlags; }
            set { this.FHeader.fFlags = (byte)value; }
        }

        public byte[] Unused
        {
            get { return this.FHeader.ubUnused; }
            set { this.FHeader.ubUnused = value; }
        }

        public byte[] Unused1
        {
            get { return this.FHeader.ubUnused1; }
            set { this.FHeader.ubUnused1 = value; }
        }

        public void Load(Deserializer deserializer)
        {
            this.FHeader = (Header)deserializer.Deserialize(typeof(Header));
        }

		public void Load(Stream stream)
		{
			Deserializer ds = new Deserializer(stream);
			Load(ds);
		}

        public void Save(Serializer serializer)
        {
            serializer.Serialize(this.FHeader);
        }

		public void Save(Stream stream)
		{
			Serializer s = new Serializer(stream);
			Save(s);
		}

        public void BuildInfo(StringBuilder aInfoBuilder)
        {
            aInfoBuilder.AppendLine(String.Format("WallOrientation - {0}", this.WallOrientation));
            aInfoBuilder.AppendLine(String.Format("NumberOfTiles - {0}", this.NumberOfTiles));
            aInfoBuilder.AppendLine(String.Format("TileLocIndex - {0}", this.TileLocIndex));
            aInfoBuilder.AppendLine(String.Format("CurrentFrame - {0}", this.CurrentFrame));
            aInfoBuilder.AppendLine(String.Format("NumberOfFrames - {0}", this.NumberOfFrames));
            aInfoBuilder.AppendLine(String.Format("Flags - {0}", this.Flags));
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();

            this.BuildInfo(_sb);

            return _sb.ToString();
        }
	}

    [Flags]
    public enum AuxObjectFlags : byte
    {
        None = 0,
        AUX_FULL_TILE = 1,
        AUX_ANIMATED_TILE = 2,
        AUX_DYNAMIC_TILE = 4,
        AUX_INTERACTIVE_TILE = 8,
        AUX_IGNORES_HEIGHT = 16,
        AUX_USES_LAND_Z = 32
    }
}
