using Ja2Data;
using Ja2DataTest.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Ja2DataTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.tfsSlf.InitialDirName = this.FInitialFolder;
            this.tfsSti.InitialDirName = this.FInitialFolder;
        }

        private string FInitialFolder = @"C:\Program Files (x86)\Akella Games\JAGGED ALLIANCE 2 GOLD\Data";

        private SlfTestViewModel SlfViewModel
        {
            get { return (SlfTestViewModel)this.Resources["SlfViewModel"]; }
        }

        private StiTestViewModel StiViewModel
        {
            get { return (StiTestViewModel)this.Resources["StiViewModel"]; }
        }

        private JsdTestViewModel JsdViewModel
        {
            get { return (JsdTestViewModel)this.Resources["JsdViewModel"]; }
        }
    }
}
