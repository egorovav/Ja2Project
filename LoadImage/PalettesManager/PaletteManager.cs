using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Xml.Serialization;
using System.Windows.Forms;
using Resources = LocalizerNameSpace.Localizer;

namespace dotNetStiEditor
{
	public class PaletteManager
	{
		public PaletteManager()
		{
			Stream palStream = new MemoryStream(Properties.Resources.JA2PAL);
			string palFile = Path.Combine(Application.StartupPath, "JA2PAL.DAT");
			if (File.Exists(palFile))
				palStream = new FileStream("JA2PAL.DAT", FileMode.Open);

			using (BinaryReader Ja2PalReader = new BinaryReader(palStream))
			{
				colorsTypeCount = Ja2PalReader.ReadInt16();
				Ja2PalReader.ReadBytes(2); // 00 00 что-то непонятное
				typeLengths = new int[colorsTypeCount];
				for (int i = 0; i < colorsTypeCount; i++)
					typeLengths[i] = Ja2PalReader.ReadByte();
				Ja2PalReader.ReadBytes(2); // F5 FA что-то непонятное
				typeShifts = new int[colorsTypeCount];
				for (int i = 0; i < colorsTypeCount; i++)
					typeShifts[i] = Ja2PalReader.ReadByte();
				Ja2PalReader.ReadBytes(2); // DC EA что-то непонятное
				int headerLength = Ja2PalReader.ReadByte();
				Ja2PalReader.ReadBytes(4);// 00 00 00 00 что-то непонятное 
				// в файле какие-то неправильные и переставленные сдвиги:205, 219, 235, 244.
				// Должны быть такие. вроде как. 
				typeShifts = new int[] { 245, 205, 235, 220 };
				for (int i = 0; i < colorsTypeCount; i++)
				{
					for (int j = 0; j < typeLengths[i]; j++)
					{
						byte[] header = Ja2PalReader.ReadBytes(headerLength);
						string name = "";
						int index = 0;
						while (header[index] != 0)
						{
							name += Convert.ToChar(header[index]);
							index++;
						}
						Ja2PalReader.ReadBytes(2);
						int colorsCount = Ja2PalReader.ReadByte();
						ColorList colorList = new ColorList(name, typeShifts[i], colorsCount, i);
						for (int k = 0; k < colorsCount; k++)
						{
							colorList.Colors[k] = Color.FromArgb
									(255,
									Math.Min((int)(Ja2PalReader.ReadByte() * 350 / 255), 255),
									Math.Min((int)(Ja2PalReader.ReadByte() * 350 / 255), 255),
									Math.Min((int)(Ja2PalReader.ReadByte() * 350 / 255), 255));
						}
						ColorsCollection.Add(name, colorList);
						// Разделитель(?) после последнего набора не стоит.
						if (i < colorsTypeCount - 1 || j < typeLengths[i] - 1)
							Ja2PalReader.ReadByte(); // 00
					}
				}
			}
		}
		// JA2 Gold:
		//UInt16[,] gusShadeLevels = new UInt16[16, 3]
		//{ 
		//    {500, 500, 500},
		//    {450, 450, 450,},	//bright
		//    {350, 350, 350,},
		//    {300, 300, 300,},
		//    {255, 255, 255,},	//normal
		//    {231, 199, 199,},
		//    {209, 185, 185,},
		//    {187, 171, 171,},
		//    {165, 157, 157,},	//darkening
		//    {143, 143, 143,},
		//    {121, 121, 129,},
		//    {99, 99, 115 ,},
		//    {77, 77, 101,},		//night
		//    {36, 36, 244,},
		//    {18, 18, 224,},
		//    {48, 222, 48}
		//};
		public readonly Dictionary<string, ColorList> ColorsCollection = new Dictionary<string, ColorList>();
		// Текущая палитра: волосы, штаны, кожа, фуфайка. 
		public PaletteRecord currentPalette = new PaletteRecord(null, new string[4]);

		public void SetColor(string name, List<Bitmap> bitmaps)
		{
			ColorList colorList = null;
			if (ColorsCollection.TryGetValue(name, out colorList))
			{
				foreach (Bitmap bm in bitmaps)
				{
					if (defaultPalette == null)
						defaultPalette = ((Bitmap)bm.Clone()).Palette;
					ColorPalette tempPalette = bm.Palette;
					for (int i = 0; i < colorList.ColorsCount; i++)
						tempPalette.Entries[colorList.Shift + i] = colorList.Colors[i];
					bm.Palette = tempPalette;
				}
				currentPalette.ColorListNames[colorList.type] = colorList.name;
			}
		}

		public void SetColors(string[] names, List<Bitmap> bitmaps)
		{
			foreach (string name in names)
				SetColor(name, bitmaps);
		}



		public void SetShadowColor(String shadowColorName, List<Bitmap> bitmaps)
		{
			Color shadowColor = Color.Gray;
			if (shadowColorName == Resources.GetString("Grey"))
				shadowColor = Color.Gray;
			if (shadowColorName == Resources.GetString("Transparent"))
				shadowColor = Color.Transparent;


			foreach (Bitmap bm in bitmaps)
			{
				if (defaultPalette == null)
					defaultPalette = ((Bitmap)bm.Clone()).Palette;
				ColorPalette tempPalette = bm.Palette;
				tempPalette.Entries[254] = shadowColor;
				bm.Palette = tempPalette;
			}
			currentPalette.ShadowColorName = shadowColorName;
		}
		public void SetDefaultColors(List<Bitmap> bitmaps)
		{
			foreach (Bitmap bm in bitmaps)
				bm.Palette = defaultPalette;
		}

		ColorPalette defaultPalette;
		int colorsTypeCount;
		int[] typeLengths;
		int[] typeShifts;
	}
	public class ColorList
	{
		public ColorList(string name, int shift, int colorsCount, int type)
		{
			this.name = name;
			this.Shift = shift;
			this.ColorsCount = colorsCount;
			this.type = type;
			this.Colors = new Color[colorsCount];
		}
		public readonly int type;
		public readonly string name;
		public readonly int Shift;
		public readonly int ColorsCount;
		public Color[] Colors;
	}
	public class PaletteRecord
	{
		public PaletteRecord(string paletteName, string[] colorListNames)
		{
			this.ColorListNames = colorListNames;
			this.PaletteName = paletteName;
		}
		public PaletteRecord()
		{
		}
		[XmlAttributeAttribute("Name")]
		public string PaletteName;
		// Свойство для Combobox-a в форме для удаления палитр.
		public string Name
		{
			get { return PaletteName; }
		}
		public string[] ColorListNames;
		public string ShadowColorName = Resources.GetString("Grey");
	}
}