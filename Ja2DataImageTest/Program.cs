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

			//GifTest();

			GifToBitmapTest("SpartanFla.gif");
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
				_encoder.Frames.Add(_images[i].Bm);

				using (var _fs = new FileStream(_output, FileMode.Create))
				{
					_encoder.Save(_fs);
				}
			}

			Console.ReadLine();
		}

		static void TrimTest()
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
			_encoder.Frames.Add(_exBm.Bm);
			using (var _ofs = new FileStream(_outputFile, FileMode.Create))
			{
				_encoder.Save(_ofs);
			}
		}

		static void LoadStciIndexedTest()
		{
			string _stiFileName = "F_LAW.STI";
			var _stci = (StciIndexed)StciLoader.LoadStci(_stiFileName);

			var _images = ExtendedBitmap.ConvertStciIndexedToBitmaps(_stci);

			for (int i = 0; i < _images.Count; i++)
			{
				var _output = String.Format(@"result\{0}{1:d3}.bmp", Path.GetFileNameWithoutExtension(_stiFileName), i);
				var _encoder = new BmpBitmapEncoder();
				_encoder.Frames.Add(_images[i].Bm);

				using (var _fs = new FileStream(_output, FileMode.Create))
				{
					_encoder.Save(_fs);
				}
			}
		}

		static void SaveStciIndexedTest()
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

		static void GifTest()
		{
			var _stiFileName = "output.sti";
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

			//GIF.ConvertBitmapsToGif(_bitmaps, _gifFileName, 10, true, false);
		}
	}
}
