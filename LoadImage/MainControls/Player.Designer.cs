namespace dotNetStiEditor
{
    partial class Player
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.FPSScroll = new System.Windows.Forms.HScrollBar();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.AnimationTimer = new System.Windows.Forms.Timer(this.components);
            this.FPStimer = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chbForAllFrames = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.rightYNumericUpDown5 = new System.Windows.Forms.NumericUpDown();
            this.rightXNumericUpDown6 = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.leftYNumericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.leftXNumericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.сохранитьСмещенияButton3 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.hScrollBar2 = new System.Windows.Forms.HScrollBar();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.фонToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.загрузитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.отрисовыватьТайлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.прозрачностьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.прозрачностьТеньToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightYNumericUpDown5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightXNumericUpDown6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftYNumericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftXNumericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(855, 218);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // FPSScroll
            // 
            this.FPSScroll.Enabled = false;
            this.FPSScroll.Location = new System.Drawing.Point(213, 10);
            this.FPSScroll.Minimum = 1;
            this.FPSScroll.Name = "FPSScroll";
            this.FPSScroll.Size = new System.Drawing.Size(123, 20);
            this.FPSScroll.TabIndex = 1;
            this.FPSScroll.Value = 1;
            this.FPSScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.FPSScroll_Scroll);
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(339, 10);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(47, 20);
            this.textBox2.TabIndex = 2;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // AnimationTimer
            // 
            this.AnimationTimer.Tick += new System.EventHandler(this.AnimationTimer_Tick);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(213, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 25);
            this.button1.TabIndex = 3;
            this.button1.Text = "Старт";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.стартButton1_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(319, 35);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 25);
            this.button2.TabIndex = 4;
            this.button2.Text = "Стоп";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.стопButton2_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chbForAllFrames);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.rightYNumericUpDown5);
            this.panel1.Controls.Add(this.rightXNumericUpDown6);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.leftYNumericUpDown3);
            this.panel1.Controls.Add(this.leftXNumericUpDown2);
            this.panel1.Controls.Add(this.сохранитьСмещенияButton3);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.hScrollBar2);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.numericUpDown4);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.hScrollBar1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.FPSScroll);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 242);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(855, 115);
            this.panel1.TabIndex = 5;
            // 
            // chbForAllFrames
            // 
            this.chbForAllFrames.AutoSize = true;
            this.chbForAllFrames.Location = new System.Drawing.Point(213, 93);
            this.chbForAllFrames.Name = "chbForAllFrames";
            this.chbForAllFrames.Size = new System.Drawing.Size(227, 17);
            this.chbForAllFrames.TabIndex = 37;
            this.chbForAllFrames.Text = "Для всех кадров из полей \"Смещения\"";
            this.chbForAllFrames.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(574, 71);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 13);
            this.label10.TabIndex = 36;
            this.label10.Text = "Y:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(509, 71);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 35;
            this.label11.Text = "X:";
            // 
            // rightYNumericUpDown5
            // 
            this.rightYNumericUpDown5.Enabled = false;
            this.rightYNumericUpDown5.Location = new System.Drawing.Point(592, 67);
            this.rightYNumericUpDown5.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.rightYNumericUpDown5.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.rightYNumericUpDown5.Name = "rightYNumericUpDown5";
            this.rightYNumericUpDown5.Size = new System.Drawing.Size(40, 20);
            this.rightYNumericUpDown5.TabIndex = 34;
            this.rightYNumericUpDown5.ValueChanged += new System.EventHandler(this.rightYNumericUpDown5_ValueChanged);
            // 
            // rightXNumericUpDown6
            // 
            this.rightXNumericUpDown6.Enabled = false;
            this.rightXNumericUpDown6.Location = new System.Drawing.Point(529, 67);
            this.rightXNumericUpDown6.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.rightXNumericUpDown6.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.rightXNumericUpDown6.Name = "rightXNumericUpDown6";
            this.rightXNumericUpDown6.Size = new System.Drawing.Size(40, 20);
            this.rightXNumericUpDown6.TabIndex = 33;
            this.rightXNumericUpDown6.ValueChanged += new System.EventHandler(this.rightXNumericUpDown6_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(143, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 32;
            this.label8.Text = "Y:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(80, 71);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "X:";
            // 
            // leftYNumericUpDown3
            // 
            this.leftYNumericUpDown3.Enabled = false;
            this.leftYNumericUpDown3.Location = new System.Drawing.Point(161, 67);
            this.leftYNumericUpDown3.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.leftYNumericUpDown3.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.leftYNumericUpDown3.Name = "leftYNumericUpDown3";
            this.leftYNumericUpDown3.Size = new System.Drawing.Size(40, 20);
            this.leftYNumericUpDown3.TabIndex = 30;
            this.leftYNumericUpDown3.ValueChanged += new System.EventHandler(this.leftYNumericUpDown3_ValueChanged);
            // 
            // leftXNumericUpDown2
            // 
            this.leftXNumericUpDown2.Enabled = false;
            this.leftXNumericUpDown2.Location = new System.Drawing.Point(97, 67);
            this.leftXNumericUpDown2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.leftXNumericUpDown2.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.leftXNumericUpDown2.Name = "leftXNumericUpDown2";
            this.leftXNumericUpDown2.Size = new System.Drawing.Size(40, 20);
            this.leftXNumericUpDown2.TabIndex = 29;
            this.leftXNumericUpDown2.ValueChanged += new System.EventHandler(this.leftXNumericUpDown2_ValueChanged);
            // 
            // сохранитьСмещенияButton3
            // 
            this.сохранитьСмещенияButton3.Enabled = false;
            this.сохранитьСмещенияButton3.Location = new System.Drawing.Point(213, 64);
            this.сохранитьСмещенияButton3.Name = "сохранитьСмещенияButton3";
            this.сохранитьСмещенияButton3.Size = new System.Drawing.Size(206, 23);
            this.сохранитьСмещенияButton3.TabIndex = 26;
            this.сохранитьСмещенияButton3.Text = "Сохранить смещения";
            this.сохранитьСмещенияButton3.UseVisualStyleBackColor = true;
            this.сохранитьСмещенияButton3.Click += new System.EventHandler(this.сохранитьСмещенияButton3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(430, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Смещения:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Смещения:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(392, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "FPS";
            // 
            // hScrollBar2
            // 
            this.hScrollBar2.Enabled = false;
            this.hScrollBar2.Location = new System.Drawing.Point(528, 40);
            this.hScrollBar2.Maximum = 220;
            this.hScrollBar2.Minimum = 20;
            this.hScrollBar2.Name = "hScrollBar2";
            this.hScrollBar2.Size = new System.Drawing.Size(104, 20);
            this.hScrollBar2.TabIndex = 16;
            this.hScrollBar2.Value = 100;
            this.hScrollBar2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar2_Scroll);
            this.hScrollBar2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.hScrollBar2_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(430, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Масштаб:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(430, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Номер кадра:";
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.Enabled = false;
            this.numericUpDown4.Location = new System.Drawing.Point(526, 11);
            this.numericUpDown4.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(106, 20);
            this.numericUpDown4.TabIndex = 13;
            this.numericUpDown4.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown4.ValueChanged += new System.EventHandler(this.numericUpDown4_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Масштаб:";
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Enabled = false;
            this.hScrollBar1.Location = new System.Drawing.Point(97, 38);
            this.hScrollBar1.Maximum = 220;
            this.hScrollBar1.Minimum = 20;
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(104, 20);
            this.hScrollBar1.TabIndex = 7;
            this.hScrollBar1.Value = 100;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            this.hScrollBar1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.hScrollBar1_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Номер кадра:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Enabled = false;
            this.numericUpDown1.Location = new System.Drawing.Point(97, 11);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(106, 20);
            this.numericUpDown1.TabIndex = 5;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.фонToolStripMenuItem,
            this.настройкиToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(855, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Paint += new System.Windows.Forms.PaintEventHandler(this.menuStrip1_Paint);
            // 
            // фонToolStripMenuItem
            // 
            this.фонToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.загрузитьToolStripMenuItem,
            this.toolStripSeparator1});
            this.фонToolStripMenuItem.Name = "фонToolStripMenuItem";
            this.фонToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.фонToolStripMenuItem.Text = "Фон";
            // 
            // загрузитьToolStripMenuItem
            // 
            this.загрузитьToolStripMenuItem.Name = "загрузитьToolStripMenuItem";
            this.загрузитьToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.загрузитьToolStripMenuItem.Text = "Загрузить";
            this.загрузитьToolStripMenuItem.Click += new System.EventHandler(this.загрузитьФонToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(125, 6);
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.отрисовыватьТайлToolStripMenuItem,
            this.прозрачностьToolStripMenuItem,
            this.прозрачностьТеньToolStripMenuItem});
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            // 
            // отрисовыватьТайлToolStripMenuItem
            // 
            this.отрисовыватьТайлToolStripMenuItem.Checked = true;
            this.отрисовыватьТайлToolStripMenuItem.CheckOnClick = true;
            this.отрисовыватьТайлToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.отрисовыватьТайлToolStripMenuItem.Name = "отрисовыватьТайлToolStripMenuItem";
            this.отрисовыватьТайлToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.отрисовыватьТайлToolStripMenuItem.Text = "Отрисовывать тайл";
            // 
            // прозрачностьToolStripMenuItem
            // 
            this.прозрачностьToolStripMenuItem.Checked = true;
            this.прозрачностьToolStripMenuItem.CheckOnClick = true;
            this.прозрачностьToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.прозрачностьToolStripMenuItem.Name = "прозрачностьToolStripMenuItem";
            this.прозрачностьToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.прозрачностьToolStripMenuItem.Text = "Прозрачность  цвета №0 (фон)";
            // 
            // прозрачностьТеньToolStripMenuItem
            // 
            this.прозрачностьТеньToolStripMenuItem.CheckOnClick = true;
            this.прозрачностьТеньToolStripMenuItem.Name = "прозрачностьТеньToolStripMenuItem";
            this.прозрачностьТеньToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.прозрачностьТеньToolStripMenuItem.Text = "Прозрачность  цвета №254 (тень)";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Player
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Player";
            this.Size = new System.Drawing.Size(855, 357);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightYNumericUpDown5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightXNumericUpDown6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftYNumericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftXNumericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.HScrollBar FPSScroll;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Timer AnimationTimer;
        private System.Windows.Forms.Timer FPStimer;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem фонToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem загрузитьToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.HScrollBar hScrollBar2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown4;
        private System.Windows.Forms.Label label9;
			private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
			private System.Windows.Forms.Button сохранитьСмещенияButton3;
			private System.Windows.Forms.NumericUpDown leftYNumericUpDown3;
			private System.Windows.Forms.NumericUpDown leftXNumericUpDown2;
			private System.Windows.Forms.Label label7;
			private System.Windows.Forms.Label label8;
			private System.Windows.Forms.Label label10;
			private System.Windows.Forms.Label label11;
			private System.Windows.Forms.NumericUpDown rightYNumericUpDown5;
			private System.Windows.Forms.NumericUpDown rightXNumericUpDown6;
			private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
			private System.Windows.Forms.ToolStripMenuItem отрисовыватьТайлToolStripMenuItem;
			private System.Windows.Forms.ToolStripMenuItem прозрачностьToolStripMenuItem;
			private System.Windows.Forms.ToolStripMenuItem прозрачностьТеньToolStripMenuItem;
            private System.Windows.Forms.CheckBox chbForAllFrames;
    }
}
