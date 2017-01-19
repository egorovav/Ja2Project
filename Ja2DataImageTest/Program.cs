using ExtendedGifEncoder;
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

			// StciToGifWithGifCoder();

			DisposalMethodTest();
		}

		private static void DisposalMethodTest()
		{
			// var _fileName = "SpartanFla.gif";
			var _fileName = "output.gif";

			var _input = new FileStream(_fileName, FileMode.Open);

			var _gifCoder = new GifBitmapCoder();
			_gifCoder.Load(_input);
			_input.Close();

			foreach (var _frame in _gifCoder.Frames)
				// _frame.DisposalMethod = GifFrameDisposalMethod.RestoreToPrevious;
				_frame.DisposalMethod = GifFrameDisposalMethod.NotDispose;

			var _output = new FileStream("output.gif", FileMode.Create);
			_gifCoder.Save(_output);
			_output.Close();
		}

		private static void StciToGifWithGifCoder()
		{
			var _fileName = "M_LAW.STI";

			var _input = new FileStream(_fileName, FileMode.Open);

			var _stci = (StciIndexed)StciLoader.LoadStci(_input);
			_input.Close();

			var _gifCoder = Converter.ConvertStciIndexedToGifCoder(_stci, 15, true);

			var _outputFileName = Path.ChangeExtension(_fileName, ".gif");
			var _output = new FileStream(_outputFileName, FileMode.Create);
			_gifCoder.Save(_output);

			_output.Close();

			_stci = Converter.ConvertGifFramesToStciIndexed(_gifCoder, true, false, 0);

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

		private static void GifBitmapFrameTest()
		{
			var _frame = new GifBitmapFrame(null);

			_frame.DisposalMethod = GifFrameDisposalMethod.NotDispose;
			_frame.UseTransparency = true;

			// byte _flags = _frame.GetFlags();

			//Console.WriteLine("{0:X2}", _flags);
			Console.ReadLine();
		}
	}
}
