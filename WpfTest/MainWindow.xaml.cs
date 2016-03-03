using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            FValueHolder = new ValueHolder();

            FWindowTestThread = new Thread(WindowTestShow);

            FWindowTestThread.SetApartmentState(ApartmentState.STA);
            FWindowTestThread.IsBackground = true;
            FWindowTestThread.Start(FValueHolder);

            this.FTimer = new System.Timers.Timer();
            this.FTimer.Elapsed += FTimer_Elapsed;
            this.FTimer.Interval = 1000;
            this.FTimer.Start();
           
        }

        void FTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.FValueHolder.IntValue++;
        }

        Thread FWindowTestThread;
        ValueHolder FValueHolder;
        private System.Timers.Timer FTimer;

        private void WindowTestShow(object aProgressHolder)
        {
            ValueHolder _ph = (ValueHolder)aProgressHolder;
            WindowTest _pw = new WindowTest(_ph);
            _pw.Topmost = true;
            _pw.Show();
            System.Windows.Threading.Dispatcher.Run();
        }
    }
}
