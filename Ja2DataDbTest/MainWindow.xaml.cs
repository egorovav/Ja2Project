using Ja2DataTest.ViewModel;
using System;
using System.Collections.Generic;
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

namespace Ja2DataDbTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //this.dgSlf.ItemsSource = this.SlfViewModel.DataAccess.SlfEntities;

            this.tfsSlf.InitialDirName = this.FInitialFolder;
            //this.tfsSti.InitialDirName = this.FInitialFolder;
        }

        //private string FInitialFolder = @"C:\Program Files (x86)\Akella Games\JAGGED ALLIANCE 2 GOLD\Data";
        private string FInitialFolder = @"C:\ja2\Data";

        private SlfTestViewModel SlfViewModel
        {
            get { return (SlfTestViewModel)this.Resources["SlfViewModel"]; }
        }
    }
}
