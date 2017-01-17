using Ja2Data;
using Ja2DataImage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Ja2DataImageTest
{
	class Program
	{
		static void Main(string[] args)
		{
			// TrimTest();

			// LoadStciIndexedTest();
			// SaveStciIndexedTest();

			// BitmapsToGifTest();

			// GifToBitmapTest("SpartanFla.gif");

			// GifToStiTest("SpartanFla.gif");

			// StiToGifTest("F_LAW.STI");

			// GifBitmapFrameTest();

			//ExtendedGifEncoderTest();

			//SmallPaletteTest();

			// GifCoderTest();

			// GifToStciWithGifCoder();

			StciToGifWithGifCoder();
		}

		private static void StciToGifWithGifCoder()
		{
			var _fileName = "M_LAW.STI";

			var _input = new FileStream(_fileName, FileMode.Open);

			var _stci = (StciIndexed)StciLoader.LoadStci(_input);
			_input.Close();

			var _gifCoder = ExtendedBitmap.ConvertStciIndexedToGifCoder(_stci, 15, true);

			var _outputFileName = Path.ChangeExtension(_fileName, ".gif");
			var _output = new FileStream(_outputFileName, FileMode.Create);
			_gifCoder.Save(_output);

			_output.Close();

			_stci = ExtendedBitmap.ConvertGifFramesToStciIndexed(_gifCoder.Frames, true, false);

			var _newStiFileName = "NEW_" + _fileName;

			_output = new FileStream(_newStiFileName, FileMode.Create);
			_stci.Save(_output);
			_output.Close();
		}

		private static void GifToStciWithGifCoder()
		{
			var _gifFileName = "";
		}

		private static void GifCoderTest()
		{
			var _gifFileName = "F_LAW.gif";

			var _input = new FileStream(_gifFileName, FileMode.Open);

			var _gifCoder = new GifBitmapCoder();
			_gifCoder.Load(_input);
			_input.Close();

			for (int i = 0; i < _gifCoder.Frames.Count; i++)
			{
				var _output = String.Format(@"result\{0}{1:d3}.gif", Path.GetFileNameWithoutExtension(_gifFileName), i);
				var _encoder = new GifBitmapEncoder();
				_encoder.Frames.Add(_gifCoder.Frames[i].Frame);

				using (var _fs = new FileStream(_output, FileMode.Create))
				{
					_encoder.Save(_fs);
				}
			}

			var _output1 = new FileStream("output.gif", FileMode.Create);
			_gifCoder.Save(_output1);
			_output1.Close();
		}

		private static void SmallPaletteTest()
		{
			var _fileName = @"result\output001_16.bmp";

			var _input = new FileStream(_fileName, FileMode.Open);
			var _decoder = new BmpBitmapDecoder(_input, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
			//_input.Close();

			var _encoder = new GifBitmapEncoder();
			_encoder.Frames.Add(_decoder.Frames[0]);

			var _output = new FileStream("output.gif", FileMode.Create);
			_encoder.Save(_output);
			_input.Close();
			_output.Close();

		}

		private static void ExtendedGifEncoderTest()
		{
			string _stiFileName = "F_LAW.STI";
			// string _stiFileName = "SpartanFla.sti";
			var _stci = (StciIndexed)StciLoader.LoadStci(_stiFileName);

			var _images = ExtendedBitmap.ConvertStciIndexedToBitmaps(_stci);
			var _encoder = new GifBitmapCoder();
			for (int  i = 0; i < _images.Count; i++)
			{
				var _frame = new GifBitmapFrame(_images[i].Frame, _images[i].OffsetX, _images[i].OffsetY);
				_frame.Delay = 10;
				_frame.DisposalMethod = GifFrameDisposalMethod.RestoreToBackgroundColor;
				_frame.UseTransparency = true;
				_frame.Extensions.Add(new GifCommentExtension("Test extension."));
				_encoder.AddFrame(_frame);
			}

			using (var _fs = new FileStream(Path.ChangeExtension(_stiFileName, ".gif"), FileMode.Create))
				_encoder.Save(_fs);
		}

		private static void GifBitmapFrameTest()
		{
			var _frame = new GifBitmapFrame(null);

			_frame.DisposalMethod = GifFrameDisposalMethod.NotDispose;
			_frame.UseTransparency = true;

			// byte _flags = _frame.GetFlags();

			//Console.WriteLine("{0:X2}", _flags);
			Console.ReadLine();
		}

		private static void TrimTest()
		{
			var _inputFile = "save0.bmp";
			ExtendedBitmap _exBm = null;
			using (var _ifs = new FileStream(_inputFile, FileMode.Open))
			{
				var decoder = new BmpBitmapDecoder(_ifs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
				var frame = decoder.Frames[0];
				_exBm = new ExtendedBitmap(frame, 0, 0);
				_exBm.Trim();
			}

			var _outputFile = "output.bmp";
			var _encoder = new BmpBitmapEncoder();
			_encoder.Frames.Add(_exBm.Frame);
			using (var _ofs = new FileStream(_outputFile, FileMode.Create))
			{
				_encoder.Save(_ofs);
			}
		}

		private static void LoadStciIndexedTest()
		{
			string _stiFileName = "F_LAW.STI";
			var _stci = (StciIndexed)StciLoader.LoadStci(_stiFileName);

			var _images = ExtendedBitmap.ConvertStciIndexedToBitmaps(_stci);

			for (int i = 0; i < _images.Count; i++)
			{
				var _output = String.Format(@"result\{0}{1:d3}.bmp", Path.GetFileNameWithoutExtension(_stiFileName), i);
				var _encoder = new BmpBitmapEncoder();
				_encoder.Frames.Add(_images[i].Frame);

				using (var _fs = new FileStream(_output, FileMode.Create))
				{
					_encoder.Save(_fs);
				}
			}
		}

		private static void SaveStciIndexedTest()
		{
			var _stiFileName = "output.sti";

			var _dir = Directory.CreateDirectory("result");
			var _images = new List<ExtendedBitmap>();
			foreach (var _file in _dir.GetFiles())
			{
				using (var _fs = new FileStream(_file.FullName, FileMode.Open))
				{
					var _decoder = 
						new BmpBitmapDecoder(_fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
					var _exBm = new ExtendedBitmap(_decoder.Frames[0], 0, 0);
					_images.Add(_exBm);
				}
			}

			var _stci = ExtendedBitmap.ConvertBitmapsToStciIndexed(_images, true, false);
			_stci.Save(_stiFileName);
		}

		private static void BitmapsToGifTest()
		{
			var _stiFileName = "F_LAW.STI";
			StciIndexed _stci = null;

			using (var _fs = new FileStream(_stiFileName, FileMode.Open))
			{
				using (var _br = new BinaryReader(_fs))
				{
					var _header = new StciHeader();
					_header.Read(_br);
					_stci = new StciIndexed(_header);

					_stci.Load(_br);
				}
			}

			var _bitmaps = ExtendedBitmap.ConvertStciIndexedToBitmaps(_stci);

			var _gifFileName = "output.gif";

			//var _encoder = new GifBitmapEncoder();
			//foreach (var _exBm in _bitmaps)
			//	_encoder.Frames.Add(_exBm.Bm);

			//using (var _fs = new FileStream(_gifFileName, FileMode.Create))
			//	_encoder.Save(_fs);

			GIF.ConvertBitmapsToGif(_bitmaps, _gifFileName, 10, true, false);
		}

		private static void GifToBitmapTest(string fileName)
		{
			bool containsLocalPalette = false;
			var _images = GIF.ConvertGifToBitmaps(fileName, 0, out containsLocalPalette);

			var _stci = ExtendedBitmap.ConvertBitmapsToStciIndexed(_images, true, false);
			Console.WriteLine(_stci);

			for (int i = 0; i < _images.Count; i++)
			{
				var _output = String.Format(@"result\{0}{1:d3}.bmp", Path.GetFileNameWithoutExtension(fileName), i);
				var _encoder = new BmpBitmapEncoder();
				_encoder.Frames.Add(_images[i].Frame);

				using (var _fs = new FileStream(_output, FileMode.Create))
				{
					_encoder.Save(_fs);
				}
			}

			Console.ReadLine();
		}

		private static void GifToStiTest(string aGifFileName)
		{
			bool containsLocalPalette = false;
			var _images = GIF.ConvertGifToBitmaps(aGifFileName, 0, out containsLocalPalette);

			var _stci = ExtendedBitmap.ConvertBitmapsToStciIndexed(_images, true, false);

			string _stiFileName = String.Format("{0}.sti", Path.GetFileNameWithoutExtension(aGifFileName));

			using (var _fs = new FileStream(_stiFileName, FileMode.Create))
				_stci.Save(_fs);
		}

		private static void StiToGifTest(string aStiFileName)
		{
			var _stci = new StciIndexed();
			using (var _fs = new FileStream(aStiFileName, FileMode.Open))
			using (var _br = new BinaryReader(_fs))
				_stci.Read(_br);
				
			var _images = ExtendedBitmap.ConvertStciIndexedToBitmaps(_stci);

			var _gifFileName = Path.ChangeExtension(aStiFileName, ".gif");

			GIF.ConvertBitmapsToGif(_images, _gifFileName, 10, true, false);
		}
	}
}
