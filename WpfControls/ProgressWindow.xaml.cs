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

namespace CommonWpfControls
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public ProgressWindow(ProgressHolder aProgressHolder)
        {
            InitializeComponent();

            this.FProgressHolder = aProgressHolder;
            this.FTimer.Elapsed += FTimer_Elapsed;
            this.FTimer.Start();

            if (aProgressHolder.IsCancelable)
                this.btnCancel.Visibility = System.Windows.Visibility.Visible;
        }

        private bool FIsCancelable;

        void FTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.FProgressHolder.Duration.Add(TimeSpan.FromMilliseconds(this.FTimer.Interval));
            Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                this.pbDrawingProgress.Value = this.FProgressHolder.Progress;
                if (this.FProgressHolder.Progress < 0 || this.FProgressHolder.Progress >= 100)
                    this.Close();
            });
        }

        private System.Timers.Timer FTimer = new System.Timers.Timer();
        private ProgressHolder FProgressHolder;

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.FProgressHolder.Progress = -1;
        }

        public static void ProgressWindowShow(object aProgressHolder)
        {
            ProgressHolder _ph = (ProgressHolder)aProgressHolder;
            ProgressWindow _pw = new ProgressWindow(_ph);
            _pw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _pw.Topmost = true;
            _pw.Show();
            System.Windows.Threading.Dispatcher.Run();
        }

        public static void Run(object aProgressHolder)
        {
            Thread _thr = new Thread(ProgressWindowShow);
            _thr.SetApartmentState(ApartmentState.STA);
            _thr.IsBackground = true;
            _thr.Start(aProgressHolder);
        }
    }

    public class ProgressHolder
    {
        public bool IsCancelable
        {
            get;
            set;
        }

        public TimeSpan Duration
        {
            get;
            set;
        }

        public string OperationName
        {
            get;
            set;
        }

        public int Progress
        {
            get;
            set;
        }
    }
}
