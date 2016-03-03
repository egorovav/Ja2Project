using System;
using System.ComponentModel;
using System.Text;

namespace Ja2DataTest
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null && !String.IsNullOrEmpty(propertyName))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
