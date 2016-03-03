using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapViewer
{
    /// <summary>
    /// Interaction logic for GlobalMapView.xaml
    /// </summary>
    public partial class GlobalMapsView : UserControl
    {

        public GlobalMapsViewModel ViewModel
        {
            get { return (GlobalMapsViewModel)this.DataContext; }
            set { this.DataContext = value; }
        }

        public GlobalMapsView()
        {
            this.ViewModel = new GlobalMapsViewModel();
            InitializeComponent();
        }

        private void GlobalMapView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.ViewModel.SelectedSector = null;
        }

        public void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == MainViewModel.DataFolderPropertyName)
            {
                MainViewModel _mainViewModel = (MainViewModel)sender;
                this.ViewModel.DataFolder = _mainViewModel.DataFolder;
            }
        }
    }
}
