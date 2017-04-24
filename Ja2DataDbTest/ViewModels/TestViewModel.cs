using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ja2DataTest.ViewModel
{
    public class TestViewModel : BaseViewModel
    {
        public TestViewModel()
        {
        }

        public static string ErrorStringPropertyName = "ErrorString";
        private string FErrorString = String.Empty;
        public string ErrorString
        {
            get { return this.FErrorString; }
            set
            {
                this.FErrorString = value;
                NotifyPropertyChanged(ErrorStringPropertyName);
            }
        }

        public static string StatusStringPropertyName = "StatusString";
        private string FStatusString = String.Empty;
        public string StatusString
        {
            get { return this.FStatusString; }
            set
            {
                this.FStatusString = value;
                NotifyPropertyChanged(StatusStringPropertyName);
            }
        }

        public static string ResultStringPropertyName = "ResultString";
        private string FResultString = String.Empty;
        public string ResultString
        {
            get { return this.FResultString; }
            set
            {
                this.FResultString = value;
                NotifyPropertyChanged(ResultStringPropertyName);
            }
        }

        public static string FolderNamePropertyName = "FolderName";
        private string FFolderName = String.Empty;
        public virtual string FolderName
        {
            get { return this.FFolderName; }
            set
            {
                this.FFolderName = value;
                NotifyPropertyChanged(FolderNamePropertyName);
            }
        }

        public static string FileNamePropertyName = "FileName";
        private string FFileName = String.Empty;
        public virtual string FileName
        {
            get { return this.FFileName; }
            set
            {
                this.FFileName = value;
                NotifyPropertyChanged(FileNamePropertyName);
            }
        }
    }
}
