using System;
using System.IO;
using System.Collections.Generic;

namespace Ja2Data
{
    public class Etrle
    {
        public static byte[] Read(BinaryReader aReader, int aHeight, int aWidth, int aDataLength)
        {
            byte[] _data = new byte[aHeight * aWidth];
            int _count = 0;
            int _readedBytesCount = 0;
            while (_readedBytesCount < aDataLength)
            {
                int _ruleByte = aReader.ReadByte();
                _readedBytesCount++;

				if (_ruleByte == 0)
				{
					continue;
				}
				else if (_ruleByte > SByte.MaxValue)
				{
					//for (byte j = 0; j < _ruleByte + SByte.MinValue; j++)
					//{
					//	_data[_count] = 0;
					//	_count++;
					//}

					// Array is zerro initialized, so just skip these bytes. 
					_count += (_ruleByte + SByte.MinValue);
				}
				else
				{
					//for (byte j = 0; j < _ruleByte; j++)
					//{
					//	byte _curByte = aReader.ReadByte();
					//	_readedBytesCount++;
					//	_data[_count] = _curByte;
					//	_count++;
					//}

					aReader.Read(_data, _count, _ruleByte);
					_count += _ruleByte;
					_readedBytesCount += _ruleByte;
				}
            }

            return _data;
        }

        public static int Write(BinaryWriter aWriter, byte[] aData, int aWidth)
        {
			int _startPosition = (int)aWriter.BaseStream.Position;
            int _zeroCount = 0;
            // List<byte> _nonZeroBytes = new List<byte>();
			byte[] _nonZeroBytes = new byte[aWidth];
			int _nonZeroCount = 0;

            for (int i = 0; i < aData.Length; i++)
            {
                byte _b = aData[i];
                if (_b == 0)
                {
                    _zeroCount++;
                    if (_nonZeroCount != 0)
                    {
                        WriteNonZeroBytes(aWriter, _nonZeroBytes, _nonZeroCount);
                        _nonZeroCount = 0;
                    }
                }
                else
                {
                    _nonZeroBytes[_nonZeroCount] = _b;
					_nonZeroCount++;
                    if (_zeroCount != 0)
                    {
                        WriteZeroBytes(aWriter, _zeroCount);
                        _zeroCount = 0;
                    }
                }

                if ((i + 1) % aWidth == 0)
                {
                    if (_nonZeroCount > 0)
                    {
                        WriteNonZeroBytes(aWriter, _nonZeroBytes, _nonZeroCount);
						_nonZeroCount = 0;
                    }

                    if (_zeroCount > 0)
                    {
                        WriteZeroBytes(aWriter, _zeroCount);
                        _zeroCount = 0;
                    }
                    aWriter.Write(Byte.MinValue);
                }
            }

			return (int)aWriter.BaseStream.Position - _startPosition;
        }

        private static int WriteZeroBytes(BinaryWriter aWriter, int aZeroCount)
        {
			int _bytesWroteCount = 0;

            int _zeroBytesSubSequenceCount = aZeroCount / SByte.MaxValue;
            int _zeroBytesLastSubsequenceLength = aZeroCount % SByte.MaxValue;

            for (int j = 0; j < _zeroBytesSubSequenceCount; j++)
                aWriter.Write(Byte.MaxValue);
			_bytesWroteCount += _zeroBytesSubSequenceCount;

			if (_zeroBytesLastSubsequenceLength > 0)
			{
				aWriter.Write((byte)(_zeroBytesLastSubsequenceLength - SByte.MinValue));
				_bytesWroteCount++;
			}

			return _bytesWroteCount;
        }

        private static int WriteNonZeroBytes(BinaryWriter aWriter, byte[] aNonZeroBytes, int aNonZeroCount)
        {
			int _bytesWroteCount = 0;

            int _nonZeroBytesSubSequenceCount = aNonZeroCount / SByte.MaxValue;
            int _nonZeroBytesLastSubSequenceLength = aNonZeroCount % SByte.MaxValue;

            for (int j = 0; j < _nonZeroBytesSubSequenceCount; j++)
            {
                aWriter.Write((byte)SByte.MaxValue);
				_bytesWroteCount++;
                //for (int k = 0; k < SByte.MaxValue; k++)
                //    aWriter.Write(aNonZeroBytes[j * SByte.MaxValue + k]);
				aWriter.Write(aNonZeroBytes, j * SByte.MaxValue, SByte.MaxValue);
				_bytesWroteCount += SByte.MaxValue;
            }
            if (_nonZeroBytesLastSubSequenceLength > 0)
            {
                aWriter.Write((byte)_nonZeroBytesLastSubSequenceLength);
				//for (int k = 0; k < _nonZeroBytesLastSubsequenceLength; k++)
				//	aWriter.Write(aNonZeroBytes[_nonZeroBytesSubSequenceCount * SByte.MaxValue + k]);
				aWriter.Write(aNonZeroBytes, _nonZeroBytesSubSequenceCount * SByte.MaxValue, _nonZeroBytesLastSubSequenceLength);
				_bytesWroteCount += _nonZeroBytesLastSubSequenceLength;
            }

			return _bytesWroteCount;
        }
    }
}
