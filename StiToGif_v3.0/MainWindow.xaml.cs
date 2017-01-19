using CommonWpfControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace StiToGif_v3._0
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			this.GifToStiViewModel.PropertyChanged += GifToStiViewModel_PropertyChanged;
			this.StiToGifViewModel.PropertyChanged += StiToGifViewModel_PropertyChanged;
			this.FProgress.IsCancelable = true;
		}

		void StiToGifViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == StiToGifViewModel.IsFilesNeededPropertyName)
			{
				var _files = this.GetFileNames(".sti", ".gif");
				if (_files == null)
					return;

				try
				{
					this.StiToGifViewModel.Convert(_files);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}

				this.FProgress.OperationName = "STI to GIF convertation.";
				ProgressWindow.Run(this.FProgress);
			}

			if (e.PropertyName == StiToGifViewModel.ProgressPropertyName)
			{
				if (this.FProgress.Progress >= 0)
				{
					this.FProgress.Progress = this.StiToGifViewModel.Progress;
					if (this.FProgress.Progress >= 100)
						Dispatcher.Invoke((ThreadStart)delegate { this.StiToGifViewModel.StopConvertation(); });
				}
				else
				{
					Dispatcher.Invoke((ThreadStart)delegate { this.StiToGifViewModel.StopConvertation(); });
				}
			}
		}

		private void GifToStiViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == GifToStiViewModel.IsFilesNeededPropertyName)
			{
				var _files = this.GetFileNames(".gif", ".sti");
				if (_files == null)
					return;

				try
				{
					this.GifToStiViewModel.Convert(_files);
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}

				this.FProgress.OperationName = "GIF to STI convertation.";
				ProgressWindow.Run(this.FProgress);
			}

			if (e.PropertyName == GifToStiViewModel.ProgressPropertyName)
			{
				if (this.FProgress.Progress >= 0)
				{
					this.FProgress.Progress = this.GifToStiViewModel.Progress;
					if (this.FProgress.Progress >= 100)
						Dispatcher.Invoke((ThreadStart)delegate { this.GifToStiViewModel.StopConvertation(); });
				}
				else
				{
					Dispatcher.Invoke((ThreadStart)delegate { this.GifToStiViewModel.StopConvertation(); });
				}
			}

			if (e.PropertyName == BaseViewModel.ExceptionStringPropertyName)
			{
				MessageBox.Show(
					this.GifToStiViewModel.ExceptionString, "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private ProgressHolder FProgress = new ProgressHolder();

		public StiToGifViewModel StiToGifViewModel
		{
			get { return (StiToGifViewModel)this.Resources["StiToGifViewModel"]; }
		}

		public GifToStiViewModel GifToStiViewModel
		{
			get { return (GifToStiViewModel)this.Resources["GifToStiViewModel"]; }
		}

		private string[] GetFileNames(string aExtension, string aNewExtension)
		{
			var _ofd = new OpenFileDialog();
			_ofd.Title = "Выберите файлы для конвертации.";
			_ofd.Filter = String.Format("(*{0}) | *{0}", aExtension);
			_ofd.Multiselect = true;
			if (_ofd.ShowDialog() == true)
			{
				var _sb = new StringBuilder();
				foreach (var _fileName in _ofd.FileNames)
				{
					var _newFileName = Path.ChangeExtension(_fileName, aNewExtension);
					if (File.Exists(_newFileName))
						_sb.AppendFormat("\"{0}\", ", Path.GetFileName(_newFileName));
				}

				if(_sb.Length > 0)
				{
					string _existingFileNames = String.Empty;
					if (_sb.Length > 1000)
						_existingFileNames = _sb.ToString(0, 1000) + " ... ";
					else
						_existingFileNames = _sb.ToString().TrimEnd().TrimEnd(',');

					var _message = String.Format(
						"Files {0} alredy exists. Override?", _existingFileNames);
					var _result = MessageBox.Show(_message, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
					if (_result == MessageBoxResult.No)
						return null;
				}

				return _ofd.FileNames;
			}
			return null;
		}
	}
}
