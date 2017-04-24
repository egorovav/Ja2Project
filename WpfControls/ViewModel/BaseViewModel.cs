using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonWpfControls
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string aPropertyName)
        {
            PropertyChangedEventHandler _handler = this.PropertyChanged;
            if(_handler != null && !String.IsNullOrEmpty(aPropertyName))
            {
                _handler(this, new PropertyChangedEventArgs(aPropertyName));
            }
        }

        public static string ExceptionStringPropertyName = "ExceptionString";
        private string FExceptionString;
        public string ExceptionString
        {
            get { return this.FExceptionString; }
            set 
            {
                this.FExceptionString = value;
                NotifyPropertyChanged(ExceptionStringPropertyName);
            }
        }
    }
}
