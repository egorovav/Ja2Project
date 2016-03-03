using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Ja2Data
{
		public class SerializerException : ApplicationException
		{
			public SerializerException(string msg)
				: base(msg)
			{
			}
		}

		/// <summary>
		/// Raw serializer class.  Serializes value types and structs whose length can be determined by the marshaller.
		/// </summary>
		public class Serializer
		{
			/// <summary>
			/// The binary writer instance to which value types are written.
			/// </summary>
			protected BinaryWriter bw;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="output">The output stream.</param>
			public Serializer(Stream output)
			{
				bw = new BinaryWriter(output);
			}

			public void Serialize(bool val)
			{
				bw.Write(val);
			}

			public void Serialize(byte val)
			{
				bw.Write(val);
			}

			public void Serialize(byte[] val)
			{
				//bw.Write(val.Length);
				bw.Write(val);
			}

			public void Serialize(char val)
			{
				bw.Write(val);
			}

			public void Serialize(char[] val)
			{
				bw.Write(val.Length);
				bw.Write(val);
			}

			public void Serialize(decimal val)
			{
				bw.Write(val);
			}

			public void Serialize(double val)
			{
				bw.Write(val);
			}

			public void Serialize(short val)
			{
				bw.Write(val);
			}

			public void Serialize(int val)
			{
				bw.Write(val);
			}

			public void Serialize(long val)
			{
				bw.Write(val);
			}

			public void Serialize(sbyte val)
			{
				bw.Write(val);
			}

			public void Serialize(float val)
			{
				bw.Write(val);
			}

			public void Serialize(string val)
			{
				if (val == null)
				{
					throw new SerializerException("Serialize(string val) cannot be used to serialize a null string.  Use SerializeNullable instead.");
				}

				bw.Write(val);
			}

			public void Serialize(ushort val)
			{
				bw.Write(val);
			}

			public void Serialize(uint val)
			{
				bw.Write(val);
			}

			public void Serialize(ulong val)
			{
				bw.Write(val);
			}

			public void Serialize(Guid val)
			{
				Serialize((object)val);
			}

			/// <summary>
			/// Serialize a boxed value assuming nullable is false.
			/// </summary>
			/// <param name="val">The value.</param>
			public void Serialize(object val)
			{
				if ((val == null) || (val == DBNull.Value))
				{
					throw new SerializerException("Serialize(object val) cannot be used to serialize a null value.  Use SerializeNullable instead.");
				}

				InternalSerialize(val);
			}

			protected virtual void InternalSerialize(object val)
			{
				// Is it a struct?
				if (val.GetType().IsValueType)
				{
					SerializeStruct(val);
				}
				else
				{
					throw new SerializerException("Cannot serialize " + val.GetType().AssemblyQualifiedName);
				}
			}

			/// <summary>
			/// Serialize an array of objects.
			/// </summary>
			/// <param name="objs">The array of objects.</param>
			public virtual void Serialize(object[] objs)
			{
				foreach (object obj in objs)
				{
					Serialize(obj);
				}
			}

			/// <summary>
			/// Flush the stream.
			/// </summary>
			public void Flush()
			{
				bw.Flush();
			}

			/// <summary>
			/// Close the stream.
			/// </summary>
			public void Close()
			{
				bw.Flush();
				bw.Close();
			}

			/// <summary>
			/// Virtual method to manage serializing structures, using the Marshaller.
			/// Override this method to handle structs that the marshaller doesn't.
			/// </summary>
			/// <param name="val">The struct to serialize.</param>
			protected virtual void SerializeStruct(object val)
			{
				try
				{
					// Get the size of the structure.
					byte[] bytes = new byte[Marshal.SizeOf(val.GetType())];

					// Pin the bytes so the GC doesn't move them.
					GCHandle h = GCHandle.Alloc(bytes, GCHandleType.Pinned);

					// Copy the structure into the byte array.
					Marshal.StructureToPtr(val, Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), false);

					// Unpin the memory.
					h.Free();

					// Write the byte array length and the bytes.
					//bw.Write(bytes.Length);
					bw.Write(bytes);
				}
				catch (Exception e)
				{
					throw new SerializerException(e.Message);
				}
			}
		}
	}
