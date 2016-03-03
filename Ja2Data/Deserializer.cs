using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Ja2Data
{
	public class Deserializer
	{
		protected BinaryReader br;

		public Deserializer(Stream input)
		{
			br = new BinaryReader(input);
		}

		public bool DeserializeBool()
		{
			return br.ReadBoolean();
		}

		public byte DeserializeByte()
		{
			return br.ReadByte();
		}

		public byte[] DeserializeBytes()
		{
			int count = br.ReadInt32();
			return br.ReadBytes(count);
		}

		public byte[] DeserializeBytes(int count)
		{
			return br.ReadBytes(count);
		}

		public char DeserializeChar()
		{
			return br.ReadChar();
		}

		public char[] DeserializeChars()
		{
			int count = br.ReadInt32();
			return br.ReadChars(count);
		}

        public char[] DeserializeChars(int count)
        {
            return br.ReadChars(count);
        }

		public decimal DeserializeDecimal()
		{
			return br.ReadDecimal();
		}

		public double DeserializeDouble()
		{
			return br.ReadDouble();
		}

		public short DeserializeShort()
		{
			return br.ReadInt16();
		}

		public int DeserializeInt()
		{
			return br.ReadInt32();
		}

		public long DeserializeLong()
		{
			return br.ReadInt64();
		}

		public sbyte DeserializeSByte()
		{
			return br.ReadSByte();
		}

		public float DeserializeFloat()
		{
			return br.ReadSingle();
		}

		public string DeserializeString()
		{
			return br.ReadString();
		}

		public ushort DeserializeUShort()
		{
			return br.ReadUInt16();
		}

		public uint DeserializeUInt()
		{
			return br.ReadUInt32();
		}

		public ulong DeserializeULong()
		{
			return br.ReadUInt64();
		}

		public Guid DeserializeGuid()
		{
			return (Guid)Deserialize(typeof(Guid));
		}

		public DateTime DeserializeDateTime()
		{
			return (DateTime)Deserialize(typeof(DateTime));
		}

		public object Deserialize(Type type)
		{
			if (type.IsValueType)
			{
				//int count = br.ReadInt32();
				int count = Marshal.SizeOf(type);
				if (br.BaseStream.Position + count <= br.BaseStream.Length)
				{
					byte[] data = br.ReadBytes(count);
					return Deserialize(data, type);
				}
				else
				{
					throw new SerializerException("Cannot deserialize " + type.AssemblyQualifiedName + ". End of stream.");
				}
			}
			else
			{
				throw new SerializerException("Cannot deserialize " + type.AssemblyQualifiedName + " is a reference type");
			}
		}

		protected virtual object Deserialize(byte[] bytes, Type type)
		{
			object structure = null;

			try
			{
				GCHandle h = GCHandle.Alloc(bytes, GCHandleType.Pinned);
				structure = Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), type);
				h.Free();
			}
			catch (Exception e)
			{
				throw new SerializerException(e.Message);
			}

			return structure;
		}

		public object Deserialize(Type type, int count)
		{
			object structure = null;
			try
			{
				byte[] bytes = this.DeserializeBytes(count);
				structure = this.Deserialize(bytes, type);
			}
			catch (Exception e)
			{
				throw new SerializerException(e.Message);
			}

			return structure;
		}
	}
}
