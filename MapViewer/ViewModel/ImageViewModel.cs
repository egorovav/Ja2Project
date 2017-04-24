using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonWpfControls;

namespace MapViewer
{
    class ImageViewModel : BaseViewModel
    {
        public static string ImageXPropertyName = "ImageX";
        private double FImageX;
        public double ImageX
        {
            get { return this.FImageX; }
            set
            {
                this.FImageX = value;
                NotifyPropertyChanged(ImageXPropertyName);
            }
        }

        public static string ImageYPropertyName = "ImageY";
        private double FImageY;
        public double ImageY
        {
            get { return this.FImageY; }
            set
            {
                this.FImageY = value;
                NotifyPropertyChanged(ImageYPropertyName);
            }
        }

        public static string ImageScalePropertyName = "ImageScale";
        private double FImageScale = 1;
        public double ImageScale
        {
            get { return this.FImageScale; }
            set
            {
                this.FImageScale = value;
                NotifyPropertyChanged(ImageScalePropertyName);
            }
        }

        // Mouse covered STI file name
        public static string ImageNamePropertyName = "ImageName";
        private string FImageName;
        public string ImageName
        {
            get { return this.FImageName; }
            set { this.FImageName = value; }
        }
    }
}
