using CommonWpfControls;
using ExtendedGifEncoder;
using Ja2Data;
using Ja2DataImage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace StiToGif_v3._0
{
	public class GifToStiViewModel : BaseViewModel
	{
		public GifToStiViewModel()
		{
			this.GifToStiCommand = new Command(()=> NotifyPropertyChanged(IsFilesNeededPropertyName), true);
		}

		public static string ForeshotingAmountPropertyName = "ForeshotingAmount";
		private int FForeshotingAmount;
		public int ForeshotingAmount
		{
			get { return this.FForeshotingAmount; }
			set
			{
				this.FForeshotingAmount = value;
				NotifyPropertyChanged(ForeshotingAmountPropertyName);
			}
		}

		public static string IsTrimBackgroundPropertyName = "IsTrimBackground";
		private bool FIsTrimBackground;
		public bool IsTrimBackground
		{
			get { return this.FIsTrimBackground; }
			set
			{
				this.FIsTrimBackground = value;
				NotifyPropertyChanged(IsTrimBackgroundPropertyName);
			}
		}

		public static string IsTransparentBackgroundPropertyName = "IsTransparentBackground";
		private bool FIsTransparentBackground;
		public bool IsTransparentBackground
		{
			get { return this.FIsTransparentBackground; }
			set
			{
				this.FIsTransparentBackground = value;
				NotifyPropertyChanged(IsTransparentBackgroundPropertyName);
			}
		}

		public static string OffsetFileNamePropertyName = "OffsetFileName";
		private string FOffsetFileName;
		private short[] FOffsetX;
		private short[] FOffsetY;
		public string OffsetFileName
		{
			get { return this.FOffsetFileName; }
			set 
			{
				this.FOffsetFileName = value;
				this.FOffsetX = null;
				this.FOffsetY = null;
				NotifyPropertyChanged(OffsetFileNamePropertyName);
			}
		}

		public static string IsFilesNeededPropertyName = "IsFilesNeeded";

		private string[] FFileNames; 
		private int FCurrentIndex = 0;
		private object FCurrentIndexKey = new object();

		public static string ProgressPropertyName = "Progress";
		public int Progress 
		{
			get { return this.FCurrentIndex * 100 / this.FFileNames.Length; }
		}

		private bool IsConvertationStoped
		{
			get { return this.GifToStiCommand.CanExecuteCommand; }
			set { this.GifToStiCommand.CanExecuteCommand = value; }
		}

		public void Convert(string[] aFileNames)
		{
			this.FFileNames = aFileNames;
			this.FCurrentIndex = 0;
			this.IsConvertationStoped = false;

			int _threadsCount = Math.Max(1, Environment.ProcessorCount - 1);
			//int _threadsCount = 1;

			for (int i = 0; i < _threadsCount; i++)
			{
				Task.Factory.StartNew((ViewModel) =>
					{
						var _vm = (GifToStiViewModel)ViewModel;
						while (!_vm.IsConvertationStoped && _vm.FCurrentIndex < _vm.FFileNames.Length)
						{
							string _fileName = null;
							lock (_vm.FCurrentIndexKey)
							{
								//if (_vm.FFileNames.Length <= _vm.FCurrentIndex)
								//	break;
								if (_vm.FCurrentIndex < _vm.FFileNames.Length)
								{
									_fileName = aFileNames[_vm.FCurrentIndex];
									_vm.FCurrentIndex++;
								}
							}

							try
							{
								if(_fileName != null)
									this.Convert(_fileName);
							}
							catch(Exception ex)
							{
								var _messageBuilder = new StringBuilder();
								_messageBuilder.AppendFormat("Error occured during convertation file {0}:\n", _fileName);
								_messageBuilder.AppendLine(ex.Message);
								_messageBuilder.AppendLine(ex.StackTrace);
								_vm.ExceptionString = _messageBuilder.ToString();
							}
							_vm.NotifyPropertyChanged(GifToStiViewModel.ProgressPropertyName);
						}

					}, this, TaskCreationOptions.None);
			}
		}

		private void Convert(string aFileName)
		{
			if(FOffsetX == null && !String.IsNullOrEmpty(this.OffsetFileName))
			{
				string _data = null;
				using(var _offsetFile = File.OpenText(this.FOffsetFileName))
				{
					_data =_offsetFile.ReadToEnd();
				}

				string[] _offsets = _data.Split(new char[] {' ', '/'}, StringSplitOptions.RemoveEmptyEntries);
				if(_offsets.Length % 2 != 0)
				{
					throw new FileFormatException("Odd numbers in offset file.");
				}

				this.FOffsetX = new short[_offsets.Length / 2];
				this.FOffsetY = new short[_offsets.Length / 2];
				for(int i = 0; i < _offsets.Length; i++)
				{
					short _offset = 0;
					if (Int16.TryParse(_offsets[i], out _offset))
					{
						if (i % 2 == 0)
						{
							this.FOffsetX[i / 2] = _offset;
						}
						else
						{
							this.FOffsetY[i / 2] = _offset;
						}
					}
					else
					{
						throw new FileFormatException("Can't parse number in offset file.");
					}
				}
			}

			var _input = new FileStream(aFileName, FileMode.Open);

			var _gifDecoder = new GifBitmapCoder();
			_gifDecoder.Load(_input);
			_input.Close();

			foreach(var _frame in _gifDecoder.Frames)
			{
				if (!_frame.UseGlobalPalette)
				{
					this.ExceptionString =
						"Внимание! В GIF файле для некоторых кадров используются локальные палитры. Изображение будет искажено.";
					break;
				}
			}

			var _stci = Converter.ConvertGifToStciIndexed(
				_gifDecoder, this.IsTransparentBackground, this.IsTrimBackground, this.ForeshotingAmount);

			if (this.FOffsetX != null)
			{
				if (this.FOffsetX.Length > 1 && _stci.Images.Length != this.FOffsetX.Length)
				{
					throw new ArgumentOutOfRangeException("Number frames in STCI is not equal to number of records in offset file");
				}

				for (int i = 0; i < _stci.Images.Length; i++)
				{
					if (this.FOffsetX.Length == 1)
					{
						_stci.Images[i].Header.OffsetX = this.FOffsetX[0];
						_stci.Images[i].Header.OffsetY = this.FOffsetY[0];
					}
					else
					{
						_stci.Images[i].Header.OffsetX = this.FOffsetX[i];
						_stci.Images[i].Header.OffsetY = this.FOffsetY[i];
					}
				}
			}

			using (var _output = new FileStream(Path.ChangeExtension(aFileName, ".sti"), FileMode.Create))
				_stci.Save(_output);
		}

		public void StopConvertation()
		{
			this.IsConvertationStoped = true;		
		}

		public Command GifToStiCommand
		{
			get;
			protected set;
		}
	}
}
