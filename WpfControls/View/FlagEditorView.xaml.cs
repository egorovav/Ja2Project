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

namespace CommonWpfControls
{
    /// <summary>
    /// Interaction logic for FlagEditor.xaml
    /// </summary>
    public partial class FlagEditorView : UserControl
    {
        public FlagEditorViewModel ViewModel
        {
            get { return this.DataContext as FlagEditorViewModel; }
            set { this.DataContext = value; }
        }

        public FlagEditorView()
        {
            this.ViewModel = new FlagEditorViewModel();
            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            InitializeComponent();
        }

        static FlagEditorView()
        {
            PropertyMetadata _metadata = new PropertyMetadata(null, Flags_Changed);
            FlagsProperty = DependencyProperty.Register("Flags", typeof(Enum), typeof(FlagEditorView), _metadata);

            FlagsStringProperty = DependencyProperty.Register("FlagsString", typeof(String), typeof(FlagEditorView));
        }

        public static DependencyProperty FlagsProperty;
        public Enum Flags
        {
            get { return (Enum)this.GetValue(FlagsProperty); }
            set { this.SetValue(FlagsProperty, value); }
        }

        public static DependencyProperty FlagsStringProperty;
        public string FlagsString
        {
            get { return (string)this.GetValue(FlagsStringProperty); }
            set { this.SetValue(FlagsStringProperty, value); }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == FlagEditorViewModel.FlagsPropertyName)
            {
                if (this.Flags == null || this.Flags.ToString() != this.ViewModel.Flags.ToString())
                    this.Flags = this.ViewModel.Flags;
            }
        }

        private static void Flags_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FlagEditorView _flagEditor = (FlagEditorView)sender;
            if (_flagEditor.ViewModel.Flags.ToString() != args.NewValue.ToString())
                _flagEditor.ViewModel.Flags = (Enum)args.NewValue;

            if (_flagEditor.FlagsString != args.NewValue.ToString())
                _flagEditor.FlagsString = args.NewValue.ToString();
        }
    }
}
