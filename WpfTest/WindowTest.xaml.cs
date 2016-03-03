using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfTest
{
    /// <summary>
    /// Interaction logic for WindowTest.xaml
    /// </summary>
    public partial class WindowTest : Window
    {
        public WindowTest(ValueHolder aValue)
        {
            InitializeComponent();

            this.Value = aValue;

            this.FTimer = new System.Timers.Timer();
            this.FTimer.Elapsed += FTimer_Elapsed;
            this.FTimer.Start();
        }

        private void FTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                this.tb.Text = Value.IntValue.ToString();
            });
        }

        private System.Timers.Timer FTimer;
        private ValueHolder Value;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.tb.Text = Value.IntValue.ToString();
        }
    }
}
