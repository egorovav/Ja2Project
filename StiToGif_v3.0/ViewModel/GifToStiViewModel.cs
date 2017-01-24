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
using System.Windows.Input;

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

		public static string IsFilesNeededPropertyName = "IsFilesNeeded";

		private string[] FFileNames; 
		private int FCurrentIndex = 0;
		private object FCurrentIndexKey = new object();

		public static string ProgressPropertyName = "Progress";
		public int Progress 
		{
			get { return (this.FCurrentIndex + 1) * 100 / this.FFileNames.Length; }
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

			int _threadsCount = Environment.ProcessorCount - 1;
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
								if (_vm.FCurrentIndex >= _vm.FFileNames.Length)
									return;

								_fileName = aFileNames[_vm.FCurrentIndex];
								_vm.FCurrentIndex++;
							}

							this.Convert(_fileName);
							_vm.NotifyPropertyChanged(GifToStiViewModel.ProgressPropertyName);
						}

					}, this, TaskCreationOptions.None);
			}
		}

		private void Convert(string aFileName)
		{
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
