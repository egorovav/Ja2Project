using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using StiLib;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml;
using System.Threading;
using Resources = LocalizerNameSpace.Localizer;
using System.Configuration;

namespace dotNetStiEditor
{
    public partial class WorkSpace : UserControl
    {
        public WorkSpace(EditorMainForm mainForm)
        {
            Initialize();
            this.mainForm = mainForm;


            if (File.Exists("dotNetStiEditor.exe.config"))
            {
                this.tempFolder =
                    System.Configuration.ConfigurationManager.AppSettings.Get("EditorTempDir");
            }
            if (this.tempFolder == null)
                this.tempFolder = Path.Combine(Application.StartupPath,
                    String.Format("C:\\WINDOWS\\Temp\\dotNetStiEditorTemp{0:yyyyMMddHHmm}-{1}",
                        DateTime.Now, Guid.NewGuid().ToString().Replace("-", "")));

            for (int i = 0; i < 500; i++)
            {
                DataGridViewImageColumn column = new DataGridViewImageColumn();
                column.HeaderText = i.ToString();
                column.Width = 20;
                this.workDataGridView.Columns.Add(column);
            }

            for(var i = 0; i < this.workSpace.Length; i++)
            {
                this.workSpace[i] = new List<ExtendedBitmap>();
            }

            this.workDataGridView.Rows.Add();
			this.workDataGridView.Rows.Add();
			this.workDataGridView.Rows.Add();
            this.workDataGridView.Rows.Add();
            this.workDataGridView.Rows.Add();
            this.workDataGridView.Rows.Add();

            string paletteFile = Path.Combine(Application.StartupPath, "palettes.xml");
            Stream palStream;
            XmlSerializer serializer = new XmlSerializer(typeof(PaletteRecord[]));
            List<PaletteRecord> records = new List<PaletteRecord>();
            if (File.Exists(paletteFile))
                palStream = new FileStream(paletteFile, FileMode.Open, FileAccess.Read);
            else
                palStream = new MemoryStream(Properties.Resources.palettes);
            //MessageBox.Show( String.Format(
            //    "{0} palettes.xml.",Resources.GetString("NoFile")),
            //    Resources.GetString("Attention"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            using (palStream)
            {
                PaletteRecord[] palRecords = (PaletteRecord[])serializer.Deserialize(palStream);
                records = new List<PaletteRecord>(palRecords);
            }
            foreach (PaletteRecord record in records)
                addPaletteRecord(record);
        }
        void Initialize()
        {
            InitializeComponent();

            #region SET LOCAL STRINGS
            this.bLACKToolStripMenuItem.Text = Resources.GetString("Blacks");
            this.bLACKToolStripMenuItem1.Text = Resources.GetString("Blacks");
            this.bLACKToolStripMenuItem2.Text = Resources.GetString("Black");
            this.toolStripMenuItem1.Text = Resources.GetString("Black");
            this.автообновлениеToolStripMenuItem.Text = Resources.GetString("Autoupdating");
            this.bROWToolStripMenuItem.Text = Resources.GetString("Brown");
            this.копироватьВБуферToolStripMenuItem1.Text = Resources.GetString("CopyToBuffer");
            this.dARKToolStripMenuItem.Text = Resources.GetString("Dark");
            this.bLUEToolStripMenuItem1.Text = Resources.GetString("DarkBlue");
            this.bLUEToolStripMenuItem.Text = Resources.GetString("DarkBlues");
            this.rEDBROWNToolStripMenuItem.Text = Resources.GetString("DarkRed");
            this.bROWNToolStripMenuItem.Text = Resources.GetString("Darks");
            this.dIRTBLNDEToolStripMenuItem.Text = Resources.GetString("Dirty");
            this.редактироватьToolStripMenuItem.Text = Resources.GetString("Edit");
            this.toolStripMenuItem4.Text = Resources.GetString("Editing");
            this.wHITEToolStripMenuItem.Text = Resources.GetString("GrayHaired");
            this.gREENToolStripMenuItem1.Text = Resources.GetString("Green");
            this.gREENToolStripMenuItem.Text = Resources.GetString("Greens");
            this.gREYToolStripMenuItem.Text = Resources.GetString("Grey");
            this.сераяToolStripMenuItem.Text = Resources.GetString("Grey");
            this.кислотныеЦветаToolStripMenuItem.Text = Resources.GetString("InitialPalette");
            this.вставитьИзБуфераToolStripMenuItem.Text = Resources.GetString("InsertFromBuffer");
            this.jEANToolStripMenuItem1.Text = Resources.GetString("Jeans");
            this.jEANToolStripMenuItem.Text = Resources.GetString("Jeanses");
            this.фуфайкаToolStripMenuItem.Text = Resources.GetString("Jersey");
            this.tANToolStripMenuItem.Text = Resources.GetString("Leather");
            this.bLONDToolStripMenuItem.Text = Resources.GetString("Light");
            this.палитраToolStripMenuItem.Text = Resources.GetString("Palette");
            this.pINKToolStripMenuItem.Text = Resources.GetString("Pink");
            this.rEDToolStripMenuItem1.Text = Resources.GetString("Red");
            this.удалитьToolStripMenuItem1.Text =
                String.Format("{0}...", Resources.GetString("Remove"));
            this.удалитьToolStripMenuItem.Text = Resources.GetString("Remove");
            this.rEDToolStripMenuItem.Text = Resources.GetString("Roody");
            this.сохранитьКакToolStripMenuItem.Text = Resources.GetString("SaveAs");
            this.сохранитьToolStripMenuItem.Text = Resources.GetString("Save");
            this.кожаToolStripMenuItem1.Text = Resources.GetString("Skin");
            this.tANToolStripMenuItem1.Text = Resources.GetString("Sunburnt");
            this.прозрачнаяToolStripMenuItem.Text = Resources.GetString("Transparent");
            this.кожаToolStripMenuItem.Text = Resources.GetString("Trousers");
            this.посмотретьToolStripMenuItem.Text = Resources.GetString("View");
            this.toolStripMenuItem3.Text = Resources.GetString("Violet");
            this.wHITEToolStripMenuItem1.Text = Resources.GetString("White");
            this.bEIGEToolStripMenuItem.Text = Resources.GetString("Woolen");
            this.yELLOWToolStripMenuItem.Text = Resources.GetString("Yellow");
            this.посмотретьСинхронноToolStripMenuItem.Text = Resources.GetString("ViewSynchronously");
            this.автообновлениеToolStripMenuItem.Text = Resources.GetString("Autoupdating");
            this.теньToolStripMenuItem.Text = Resources.GetString("Shadow");
            this.волосыToolStripMenuItem.Text = Resources.GetString("Hair");
            this.refreshToolStripButton3.ToolTipText = Resources.GetString("Refresh");
            this.палитраToolStripMenuItem1.Text = Resources.GetString("Palette");
            #endregion
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PaletteRecord[]));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            string paletteFile = Path.Combine(Application.StartupPath, "palettes.xml");
            using (XmlWriter xmlWriter = XmlWriter.Create(paletteFile, settings))
                serializer.Serialize(xmlWriter, userPalettes.ToArray());

            if (Directory.Exists(tempFolder))
            {
                try
                {
                    Directory.Delete(this.tempFolder, true);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // public List<ExtendedBitmap> workSpace = new List<ExtendedBitmap>();
        public List<ExtendedBitmap>[] workSpace = new List<ExtendedBitmap>[6];
        List<Bitmap> workSpaceBitmaps
        {
            get
            {
                //return workSpace.ConvertAll<Bitmap>(delegate(ExtendedBitmap exBm) { return exBm.Bm; });

                var _list = new List<Bitmap>();
                for(var i = 0; i < this.workDataGridView.Rows.Count; i++)
                {
                    var _bms = workSpace[i].ConvertAll<Bitmap>(delegate (ExtendedBitmap exBm) { return exBm.Bm; });
                    _list.AddRange(_bms);                 
                }

                return _list;
            }
        }
        PaletteManager palManamger = new PaletteManager();
        PaletteRecord currentPalette
        {
            get { return palManamger.currentPalette; }
        }
        EditorMainForm mainForm;

        public void Add(ExtendedBitmap exBm)
        {
            workSpace[0].Add(exBm);
            rebindWorkSpace();
        }

        List<ExtendedBitmap> getExtendedMovieFromWorkCells(IList cells)
        {
            List<ExtendedBitmap> result = new List<ExtendedBitmap>();
            foreach (DataGridViewCell cell in cells)
                if (cell.Value != null)
                {
                    int colIndex = cell.ColumnIndex;
                    int rowIndex = cell.RowIndex;
                    result.Add(workSpace[rowIndex][colIndex]);
                }
                else
                    break;
            return result;
        }

        List<Bitmap> getMovieFromWorkCells(IList cells)
        {
            return expendedBitmapsToBitmaps(getExtendedMovieFromWorkCells(cells));
        }

        List<string> tempFiles = new List<string>();
        string tempFolder;
        string editorPath;
        void edit(ExtendedBitmap editingExBm, string tempFile)
        {
            Bitmap image = editingExBm.Bm;
            try
            {
                using (FileStream fs = File.OpenWrite(tempFile))
                {
                    image.Save(fs, ImageFormat.Bmp);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.StackTrace);
            }
            ProcessStartInfo startInfo = new ProcessStartInfo(this.editorPath, tempFile);
            try
            {
                Process paintProcess = Process.Start(startInfo);
                paintProcess.EnableRaisingEvents = true;
                paintProcess.Exited += new EventHandler(paintProcess_Exited);
            }
            catch
            {
                MessageBox.Show(@"Invalid grafics editor. Please, select another.
For example, MS Paint or Adobe Fotoshop.", Resources.GetString("Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.editorPath = null;
            }
        }
        void paintProcess_Exited(object sender, EventArgs e)
        {
            refresh();
            //string tempFile = ((Process)sender).StartInfo.Arguments;
            //try
            //{
            //  File.Delete(tempFile);
            //  if (tempFiles.Count > 0)
            //    foreach (string file in new List<string>(tempFiles))
            //      File.Delete(file);
            //}
            //catch
            //{
            //  tempFiles.Add(tempFile);
            //}
        }

        private void редактированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.editorPath == null)
            {
                if (File.Exists(Application.StartupPath + @"\dotNetStiEditor.EXE.config"))
                {
                    this.editorPath =
                        System.Configuration.ConfigurationManager.AppSettings.Get("EditorPath");
                }
                if (this.editorPath == null || !File.Exists(this.editorPath))
                {
                    if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        EditorMainForm.configExists = false;
                        this.editorPath = this.openFileDialog1.FileName;
                        EditorMainForm.config.AppSettings.Settings.Remove("EditorPath");
                        EditorMainForm.config.AppSettings.Settings.Add("EditorPath", this.editorPath);
                    }
                }
            }

            this.refreshTimer.Tick -= new System.EventHandler(this.refreshTimer_Tick);

            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);
            tempFiles.Clear();
            foreach (DataGridViewCell cell in workDataGridView.SelectedCells)
            {
                int colIndex = cell.ColumnIndex;
                int rowIndex = cell.RowIndex;
                if (colIndex < workSpace[rowIndex].Count)
                {
                    ExtendedBitmap editingExBm = workSpace[rowIndex][colIndex];
                    string tempFile = String.Format("Temp_{0}_{1}_{2}.bmp", rowIndex, colIndex, editingExBm.Id);
                    tempFile = Path.Combine(tempFolder, tempFile);
                    edit(editingExBm, tempFile);
                }
                else
                    return;
            }

            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            refreshTimer.Start();
        }

        void rebindWorkSpace()
        {
            for (int j = 0; j < this.workSpace.Length; j++)
            {
                DataGridViewRow workRow = workDataGridView.Rows[j];

                for (int i = 0; i < workDataGridView.Columns.Count; i++)
                {
                    if (i < workSpace[j].Count)
                    {
                        Bitmap bm = workSpace[j][i].Bm;
                        try
                        {
                            workDataGridView.Columns[i].Width = bm.Width;
                        }
                        catch { }
                        if (workRow.Height < bm.Height + 5)
                            try
                            {
                                workRow.Height = bm.Height + 5;
                            }
                            catch { }
                        workRow.Cells[i].Value = bm;
                    }
                    else
                        workRow.Cells[i].Value = null;
                }
            }
        }
        private void просмотретьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            List<DataGridViewCell> sortedList = sortSelectedCells(workDataGridView.SelectedCells);
            mainForm.Play(getExtendedMovieFromWorkCells(sortedList), true);
        }
        private void копироватьВБуферToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            List<DataGridViewCell> sortedList = sortSelectedCells(workDataGridView.SelectedCells);
            mainForm.Buffer = new List<ExtendedBitmap>();
            foreach (ExtendedBitmap exBm in getExtendedMovieFromWorkCells(sortedList))
                mainForm.Buffer.Add(exBm.Clone());
        }
        private void просмотретьСинхронноToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            List<DataGridViewCell> sortedList = sortSelectedCells(workDataGridView.SelectedCells);
            mainForm.Play(getExtendedMovieFromWorkCells(sortedList), false);
        }
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<DataGridViewCell> sortedList = sortSelectedCells(workDataGridView.SelectedCells);
            sortedList.Reverse();
            foreach (DataGridViewCell cell in sortedList)
                workSpace[cell.RowIndex].RemoveAt(cell.ColumnIndex);
            rebindWorkSpace();
            if (workSpace[0].Count == 0)
            {
                foreach (object item in this.палитраToolStripMenuItem.DropDownItems)
                    if (item is ToolStripMenuItem)
                        ((ToolStripMenuItem)item).Checked = false;
                ((ToolStripMenuItem)this.палитраToolStripMenuItem.DropDownItems[6]).Checked = true;
            }
        }
        private void вставитьИзБуфераToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<DataGridViewCell> sortedList = sortSelectedCells(workDataGridView.SelectedCells);
            int colIndex = 0;
            int rowIndex = 0;

            if (sortedList.Count > 0)
            {
                rowIndex = sortedList[0].RowIndex;
                colIndex = Math.Min(sortedList[0].ColumnIndex, workSpace[rowIndex].Count);
            }

            workSpace[rowIndex].InsertRange(colIndex, mainForm.Buffer);
            rebindWorkSpace();
        }
        List<DataGridViewCell> sortSelectedCells(DataGridViewSelectedCellCollection selectedCells)
        {
            List<DataGridViewCell> result = new List<DataGridViewCell>();
            foreach (DataGridViewCell cell in selectedCells)
            {
                //if (cell.Value != null)
                    result.Add(cell);
            }
            result.Sort(delegate(DataGridViewCell x, DataGridViewCell y)
                    { return x.ColumnIndex.CompareTo(y.ColumnIndex); });
            return result;
        }
        private void сохранитьВSTIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<ExtendedBitmap> movie =
                    getExtendedMovieFromWorkCells(sortSelectedCells(this.workDataGridView.SelectedCells));
            if (movie.Count > 0 && movie[0].RgbData != null)
            {
                this.saveFileDialog1.AddExtension = true;
                this.saveFileDialog1.DefaultExt = "sti";
                this.saveFileDialog1.Filter = "Image Files (*.sti)|*.sti";
                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    RGBConverter.ConvertBitmapToRGBData(movie[0], this.saveFileDialog1.FileName);
                }
            }
            else
            {
                Form form = new Foreshortening(movie);
                form.Show();
            }
        }
        List<Bitmap> expendedBitmapsToBitmaps(List<ExtendedBitmap> exBms)
        {
            return new List<Bitmap>(Array.ConvertAll<ExtendedBitmap, Bitmap>(exBms.ToArray(),
                    delegate(ExtendedBitmap exBm) { return exBm.Bm; }));
        }
        private void сохранитьВTiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.AddExtension = true;
            this.saveFileDialog1.DefaultExt = "tif";
            this.saveFileDialog1.Filter = "Image Files (*.tif)|*.tif";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<ExtendedBitmap> movie =
                        getExtendedMovieFromWorkCells(sortSelectedCells(this.workDataGridView.SelectedCells));
                TIFF.ConvertBitmapsToTiff(movie, this.saveFileDialog1.FileName);
            }
        }
        private void сохранитьВBMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Bitmap> movie =
                    getMovieFromWorkCells(sortSelectedCells(this.workDataGridView.SelectedCells));
            string directoryName =
                    String.Format("C:\\StiEditorSave\\Save_{0:yy.MM.dd HH.mm}", DateTime.Now);
            Directory.CreateDirectory(directoryName);
            for (int i = 0; i < movie.Count; i++)
            {
                string fileName = Path.Combine(directoryName, "save" + i.ToString() + ".bmp");
                movie[i].Save(fileName, ImageFormat.Bmp);
            }
        }
        private void сохранитьВGIFToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            //this.saveFileDialog1.AddExtension = true;
            //this.saveFileDialog1.DefaultExt = "gif";
            //if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            List<ExtendedBitmap> movie =
                getExtendedMovieFromWorkCells(sortSelectedCells(this.workDataGridView.SelectedCells));
            new GifSaveForm(movie).ShowDialog();
        }

        private void изменитьЦветаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = ((ToolStripMenuItem)sender).Tag.ToString();
            palManamger.SetColor(name, workSpaceBitmaps);
            workDataGridView.Invalidate();
        }

        private void цветТениToolStripMenuItem_Click(object sender, EventArgs e)
        {
            palManamger.SetShadowColor(((ToolStripMenuItem)sender).Text, workSpaceBitmaps);
            workDataGridView.Invalidate();
        }

        private void кислотныеЦветаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (workSpaceBitmaps.Count > 0)
            {
                foreach (object item in this.палитраToolStripMenuItem.DropDownItems)
                {
                    if (item is ToolStripMenuItem)
                        ((ToolStripMenuItem)item).Checked = false;
                }
                ((ToolStripMenuItem)sender).Checked = true;
                palManamger.SetDefaultColors(workSpaceBitmaps);
                workDataGridView.Invalidate();
            }
        }

        List<PaletteRecord> userPalettes = new List<PaletteRecord>();

        void addPaletteRecord(PaletteRecord record)
        {
            userPalettes.Add(record);
            this.палитраToolStripMenuItem.DropDownItems.Add(record.PaletteName, null,
                    delegate(object sender, EventArgs e)
                    {
                        string[] colors = new string[4];
                        record.ColorListNames.CopyTo(colors, 0);
                        if (workSpace[0].Count > 0)
                        {
                            foreach (object item in this.палитраToolStripMenuItem.DropDownItems)
                            {
                                if (item is ToolStripMenuItem)
                                    ((ToolStripMenuItem)item).Checked = false;
                            }
                            ((ToolStripMenuItem)sender).Checked = true;
                            currentPalette.PaletteName = record.PaletteName;
                            palManamger.SetColors(colors, workSpaceBitmaps);
                            palManamger.SetShadowColor(record.ShadowColorName, workSpaceBitmaps);
                            workDataGridView.Invalidate();
                        }
                    });
        }

        private void удалитьПалитруStripButton2_Click(object sender, EventArgs e)
        {
            DeletePaletteForm deleteForm = new DeletePaletteForm(userPalettes);
            if (deleteForm.ShowDialog() == DialogResult.OK)
            {
                int index = deleteForm.PaletteIndex;
                if (MessageBox.Show(String.Format("{1}: {0}?",
                        userPalettes[index].PaletteName, Resources.GetString("RemovePalette")),
                        Resources.GetString("Attention"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    this.userPalettes.RemoveAt(index);
                    this.палитраToolStripMenuItem.DropDownItems.RemoveAt(index + 10);
                    this.workDataGridView.Invalidate();
                }
            }
        }

        void refresh()
        {
            bool rebindNeeded = false;
            DirectoryInfo dir = Directory.CreateDirectory(tempFolder);
            List<Bitmap> updatedBitmaps = new List<Bitmap>();

            foreach(var _item in workSpace)
            foreach (ExtendedBitmap exBm in _item)
            {
                if (dir != null)
                {
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        if (file.Name.Contains(exBm.Id))
                        {
                            Bitmap bm;
                            try // Файл может быть занят
                            {
                                using (FileStream fs = File.OpenRead(file.FullName))
                                {
                                    bm = new Bitmap(fs);
                                    bm.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    bm.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    updatedBitmaps.Add(bm);
                                    exBm.Bm = bm;
                                    rebindNeeded = true;
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            //if (palName != currentPalette.PaletteName)
            //{
            //  palName = currentPalette.PaletteName;
            //  if (palName != null)
            //  {
            //    foreach (string name in currentPalette.ColorListNames)
            //      palManamger.SetColors(name, updatedBitmaps);
            //    palManamger.SetShadowColor(currentPalette.ShadowColorName, updatedBitmaps);
            //  }
            //}
            if (rebindNeeded)
                rebindWorkSpace();
        }
        //string palName;

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            refresh();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavePaletteForm savePaletteForm = new SavePaletteForm();
            if (savePaletteForm.ShowDialog() == DialogResult.OK)
            {
                string[] colors = new string[4];
                currentPalette.ColorListNames.CopyTo(colors, 0);
                PaletteRecord palRecord =
                    new PaletteRecord(savePaletteForm.PaletteName, colors);
                palRecord.ShadowColorName = currentPalette.ShadowColorName;
                addPaletteRecord(palRecord);
            }
        }

        private void автообновлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            if (item.Checked)
            {
                this.refreshTimer.Stop();
                item.Checked = false;
            }
            else
            {
                this.refreshTimer.Start();
                item.Checked = true;
            }
        }

        private void workDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            редактированиеToolStripMenuItem_Click(sender, new EventArgs());
        }

        private void pNGToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void палитраToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(workDataGridView.SelectedCells.Count > 0)
            {
                int colIndex = workDataGridView.SelectedCells[0].ColumnIndex;
                int rowIndex = workDataGridView.SelectedCells[0].RowIndex;
                if (workSpace[rowIndex].Count > colIndex)
                {
                    ExtendedBitmap _ebm = workSpace[rowIndex][colIndex];
                    if (_ebm.Bm.Palette.Entries.Length > 0)
                    {
                        PaletteForm _pf = new PaletteForm(_ebm.Bm);
                        if (_pf.ShowDialog() == DialogResult.OK)
                        {
                            int _count = _ebm.Bm.Palette.Entries.Length;
                            ColorPalette _tempPalette = _ebm.Bm.Palette;
                            for (int i = 0; i < _count; i++)
                            {
                                _tempPalette.Entries[i] = _pf.Colors[i];
                            }

                            for (int i = 0; i < workSpace[rowIndex].Count; i++)
                            {
                                workSpace[rowIndex][i].Bm.Palette = _tempPalette;
                            }

                            workDataGridView.Refresh();
                        }
                    }
                }
            }
        }
    }
}
