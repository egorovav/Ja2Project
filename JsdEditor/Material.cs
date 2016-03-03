using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using LocalizerNameSpace;
using CommonWpfControls;

namespace JsdEditor
{
	/// <summary>
	/// Description of Material.
	/// </summary>
	public class JsdMaterial : BaseViewModel
	{
		public JsdMaterial()
		{
		}

        public static string IndexPropertyName = "Index";
		int index;
		public int Index
		{
			get 
			{
				return this.index; 
			}
			set 
			{ 
				this.index = value;
                NotifyPropertyChanged(IndexPropertyName);
			}
		}

        public static string NamePropertyName = "Name";
		string name;
		public string Name
		{
			get { return this.name; }
			set 
            { 
                this.name = value;
                NotifyPropertyChanged(NamePropertyName);
                NotifyPropertyChanged(DisplayNamePropertyName);
            }
		}

        LocalName[] localNames;
        public LocalName[] LocalNames
        {
            get { return this.localNames; }
            set { this.localNames = value; }
        }
		
		bool notUsed;
		public bool NotUsed
		{
			get { return this.notUsed; }
			set { this.notUsed = value; }
		}
		
		int armor;
		public int Armor
		{
			get { return this.armor; }
			set { this.armor = value; }
		}

        public string DisplayNamePropertyName = "DisplayName";
        string displayName;
		public string DisplayName
		{
            get
            {
                if (this.displayName == null)
                {
                    string name = LocalizerNameSpace.Localizer.GetString(this.name);
                    this.displayName = String.Format("{0}({1})", name, this.armor);
                }
                return this.displayName;
            }
		}
	}		
}
