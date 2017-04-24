using CommonWpfControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace JsdEditor
{
    /// <summary>
    /// Interaction logic for JsdFileView.xaml
    /// </summary>
    public partial class JsdFileView : UserControl
    {
        public JsdFileViewModel ViewModel
        {
            get { return this.DataContext as JsdFileViewModel; }
            set 
            { 
                this.DataContext = value;

                if (this.ViewModel != null)
                {
                    this.ViewModel.PropertyChanged += JsdFileViewModel_PropertyChanged;
                    if(this.ViewModel.SelectedStruct != null)
                        this.svJsdShape3D.ViewModel = new StructureViewModel3D(this.ViewModel.SelectedStruct);
                }
            }
        }

        private void JsdFileViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == JsdFileViewModel.SelectedStructPropertyName)
            {
                if(this.ViewModel.SelectedStruct != null)
                    this.dgStruct.ScrollIntoView(this.ViewModel.SelectedStruct);
                this.svJsdShape3D.ViewModel = new StructureViewModel3D(this.ViewModel.SelectedStruct);
            }

            if(e.PropertyName == JsdFileViewModel.SelectedAuxDataPropertyName)
            {
                if(this.ViewModel.SelectedAuxData != null)
                    this.dgAuxData.ScrollIntoView(this.ViewModel.SelectedAuxData);
            }

            if(e.PropertyName == JsdFileViewModel.ImageSelectedIndexPropertyName)
            {
                if(this.ViewModel.ImageSelectedIndex >= 0 && this.ViewModel.ImageSelectedIndex < this.lbImages.Items.Count)
                    this.lbImages.ScrollIntoView(this.lbImages.Items[this.ViewModel.ImageSelectedIndex]);
            }
        }

        public JsdFileView()
        {
            InitializeComponent();
            this.dgcbcMaterial.ItemsSource = Materials;
        }

        private static List<JsdMaterial> FMaterials;
        private static List<JsdMaterial> Materials
        {
            get 
            {
                if(FMaterials == null)
                {
                    string materialsFile = System.IO.Path.Combine(Environment.CurrentDirectory, "Ja2Materials.xml");
                    XmlSerializer serializer = new XmlSerializer(typeof(JsdMaterial[]));
                    if (File.Exists(materialsFile))
                    {
                        using (Stream _fs = new FileStream(materialsFile, FileMode.Open, FileAccess.Read))
                        {
                            JsdMaterial[] materials = (JsdMaterial[])serializer.Deserialize(_fs);
                            FMaterials = materials.Where(x => !x.NotUsed).ToList();
                        }
                    }
                    else
                    {
                        using (StringReader _sr = new StringReader(Properties.Resources.Ja2Materials))
                        {
                            JsdMaterial[] materials = (JsdMaterial[])serializer.Deserialize(_sr);
                            FMaterials = materials.Where(x => !x.NotUsed).ToList();
                        }
                    }
                }
                return FMaterials;
            }
        }

        private void StructFlagsButton_Click(object sender, RoutedEventArgs e)
        {
            this.FTargetStructure = this.ViewModel.SelectedStruct;
            this.fevStruct.DataContext = this.FTargetStructure.FlagsViewModel;
            this.puStructFlags.IsOpen = true;
        }

        StructureViewModel FTargetStructure;

        private void puStructFlags_Closed(object sender, EventArgs e)
        {
            this.FTargetStructure.FlagsViewModel = (FlagEditorViewModel)this.fevStruct.DataContext;
        }

        private void AuxFlagsButton_Click(object sender, RoutedEventArgs e)
        {
            this.FTargetAuxData = this.ViewModel.SelectedAuxData;
            this.fevAux.DataContext = this.FTargetAuxData.FlagsViewModel;
            this.puAuxFlags.IsOpen = true;
        }

        AuxDataViewModel FTargetAuxData;

        private void puAuxFlags_Closed(object sender, EventArgs e)
        {
            this.FTargetAuxData.FlagsViewModel = (FlagEditorViewModel)this.fevAux.DataContext;
        }
    }
}
