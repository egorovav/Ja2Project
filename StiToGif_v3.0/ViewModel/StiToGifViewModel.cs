using CommonWpfControls;
using Ja2Data;
using Ja2DataImage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StiToGif_v3._0
{
	public class StiToGifViewModel : BaseViewModel
	{
		public StiToGifViewModel()
		{
			this.StiToGifCommand = new Command(() => NotifyPropertyChanged(IsFilesNeededPropertyName), true);
		}

		public static string FrameDelayPropertyName = "FrameDelay";
		private int FFrameDelay;
		public int FrameDelay
		{
			get { return this.FFrameDelay; }
			set
			{
				this.FFrameDelay = value;
				NotifyPropertyChanged(FrameDelayPropertyName);
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

		public static string ForeshotingNumberPropertyName = "ForeshotingNumber";
		private int FForeshotingNumber;
		public int ForeshotingNumber
		{
			get { return this.FForeshotingNumber; }
			set
			{
				this.FForeshotingNumber = value;
				NotifyPropertyChanged(ForeshotingNumberPropertyName);
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
			get { return this.StiToGifCommand.CanExecuteCommand; }
			set { this.StiToGifCommand.CanExecuteCommand = value; }
		}

		public void Convert(string[] aFileNames)
		{
			this.FFileNames = aFileNames;
			this.FCurrentIndex = 0;
			this.IsConvertationStoped = false;

			int _threadsCount = Environment.ProcessorCount - 1;
			// int _threadsCount = 1;

			for (int i = 0; i < _threadsCount; i++)
			{
				Task.Factory.StartNew((ViewModel) =>
				{
					var _vm = (StiToGifViewModel)ViewModel;
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
						_vm.NotifyPropertyChanged(StiToGifViewModel.ProgressPropertyName);
					}

				}, this, TaskCreationOptions.None);
			}
		}

		private void Convert(string aFileName)
		{
			var _input = new FileStream(aFileName, FileMode.Open);
			var _stci = (StciIndexed)StciLoader.LoadStci(_input);
			_input.Close();

			var _gifDecoder = Converter.ConvertStciIndexedToGif(
				_stci, (ushort)this.FrameDelay, this.IsTransparentBackground, this.ForeshotingNumber);

			using (var _output = new FileStream(Path.ChangeExtension(aFileName, ".gif"), FileMode.Create))
				_gifDecoder.Save(_output);
		}

		public void StopConvertation()
		{
			this.IsConvertationStoped = true;
		}

		public Command StiToGifCommand
		{
			get;
			protected set;
		}
	}
}
