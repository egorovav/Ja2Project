using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using StiLib;

namespace SpriteCanvas
{
	public class Canvas : System.Windows.Forms.UserControl
	{
		private System.ComponentModel.IContainer components=null;

		public Canvas(List<ExtendedBitmap> images)
		{
			InitializeComponent();
            cPictureFile = images;
            cNumberOfFrames = images.Count;

		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

        private List<ExtendedBitmap> cPictureFile;
        private Size cSpriteSize = new Size(100, 100);
		private int cNumberOfFrames;

		#region Component Designer generated code
		private void InitializeComponent()
		{ 
			this.Name = "Canvas";
		}
		#endregion

        public List<ExtendedBitmap> PictureFile
		{
			get
			{
				return cPictureFile;
			}
			set
			{
				cPictureFile=value;
			}
		}
		public Size SpriteSize
		{
			get
			{
				return cSpriteSize;
			}
		}

		public int NumberOfFrames
		{
			get
			{
				return cNumberOfFrames;
			}
		}
	}
}
