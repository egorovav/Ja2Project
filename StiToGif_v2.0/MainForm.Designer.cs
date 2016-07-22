namespace StiToGif
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnStiToGif = new System.Windows.Forms.Button();
            this.gbStiToGif = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudForeshorteningIndex = new System.Windows.Forms.NumericUpDown();
            this.chbTransparentBackground = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudDelay = new System.Windows.Forms.NumericUpDown();
            this.gbGifToSti = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.nudForeshorteningCount = new System.Windows.Forms.NumericUpDown();
            this.btnGifToSti = new System.Windows.Forms.Button();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.ttDelay = new System.Windows.Forms.ToolTip(this.components);
            this.ttTransparent = new System.Windows.Forms.ToolTip(this.components);
            this.chbTrim = new System.Windows.Forms.CheckBox();
            this.gbStiToGif.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudForeshorteningIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDelay)).BeginInit();
            this.gbGifToSti.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudForeshorteningCount)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStiToGif
            // 
            this.btnStiToGif.Location = new System.Drawing.Point(9, 19);
            this.btnStiToGif.Name = "btnStiToGif";
            this.btnStiToGif.Size = new System.Drawing.Size(100, 30);
            this.btnStiToGif.TabIndex = 0;
            this.btnStiToGif.Text = "STI => GIF";
            this.btnStiToGif.UseVisualStyleBackColor = true;
            this.btnStiToGif.Click += new System.EventHandler(this.btnStiToGif_Click);
            // 
            // gbStiToGif
            // 
            this.gbStiToGif.Controls.Add(this.label2);
            this.gbStiToGif.Controls.Add(this.nudForeshorteningIndex);
            this.gbStiToGif.Controls.Add(this.chbTransparentBackground);
            this.gbStiToGif.Controls.Add(this.label1);
            this.gbStiToGif.Controls.Add(this.nudDelay);
            this.gbStiToGif.Controls.Add(this.btnStiToGif);
            this.gbStiToGif.Location = new System.Drawing.Point(12, 12);
            this.gbStiToGif.Name = "gbStiToGif";
            this.gbStiToGif.Size = new System.Drawing.Size(288, 169);
            this.gbStiToGif.TabIndex = 1;
            this.gbStiToGif.TabStop = false;
            this.gbStiToGif.Text = "Экспорт из STI в GIF.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(127, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 39);
            this.label2.TabIndex = 5;
            this.label2.Text = "Номер выгружаемого \r\nракурса. Если нужны\r\nвсе передавайте 0.";
            // 
            // nudForeshorteningIndex
            // 
            this.nudForeshorteningIndex.Location = new System.Drawing.Point(130, 143);
            this.nudForeshorteningIndex.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudForeshorteningIndex.Name = "nudForeshorteningIndex";
            this.nudForeshorteningIndex.Size = new System.Drawing.Size(120, 20);
            this.nudForeshorteningIndex.TabIndex = 4;
            // 
            // chbTransparentBackground
            // 
            this.chbTransparentBackground.AutoSize = true;
            this.chbTransparentBackground.Location = new System.Drawing.Point(129, 61);
            this.chbTransparentBackground.Name = "chbTransparentBackground";
            this.chbTransparentBackground.Size = new System.Drawing.Size(112, 17);
            this.chbTransparentBackground.TabIndex = 3;
            this.chbTransparentBackground.Text = "Прозрачный фон";
            this.chbTransparentBackground.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(126, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Задержка кадра в мс.";
            // 
            // nudDelay
            // 
            this.nudDelay.Location = new System.Drawing.Point(129, 35);
            this.nudDelay.Name = "nudDelay";
            this.nudDelay.Size = new System.Drawing.Size(120, 20);
            this.nudDelay.TabIndex = 1;
            // 
            // gbGifToSti
            // 
            this.gbGifToSti.Controls.Add(this.chbTrim);
            this.gbGifToSti.Controls.Add(this.label3);
            this.gbGifToSti.Controls.Add(this.nudForeshorteningCount);
            this.gbGifToSti.Controls.Add(this.btnGifToSti);
            this.gbGifToSti.Location = new System.Drawing.Point(10, 187);
            this.gbGifToSti.Name = "gbGifToSti";
            this.gbGifToSti.Size = new System.Drawing.Size(290, 90);
            this.gbGifToSti.TabIndex = 2;
            this.gbGifToSti.TabStop = false;
            this.gbGifToSti.Text = "Экспорт из GIF в STI.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(129, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Колличество ракурсов.";
            // 
            // nudForeshorteningCount
            // 
            this.nudForeshorteningCount.Location = new System.Drawing.Point(131, 29);
            this.nudForeshorteningCount.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.nudForeshorteningCount.Name = "nudForeshorteningCount";
            this.nudForeshorteningCount.Size = new System.Drawing.Size(120, 20);
            this.nudForeshorteningCount.TabIndex = 2;
            // 
            // btnGifToSti
            // 
            this.btnGifToSti.Location = new System.Drawing.Point(9, 19);
            this.btnGifToSti.Name = "btnGifToSti";
            this.btnGifToSti.Size = new System.Drawing.Size(100, 30);
            this.btnGifToSti.TabIndex = 0;
            this.btnGifToSti.Text = "GIF => STI";
            this.btnGifToSti.UseVisualStyleBackColor = true;
            this.btnGifToSti.Click += new System.EventHandler(this.btnGifToSti_Click);
            // 
            // ofd
            // 
            this.ofd.Filter = "GIF файлы (*.gif)|*.gif";
            this.ofd.Multiselect = true;
            // 
            // chbTrim
            // 
            this.chbTrim.AutoSize = true;
            this.chbTrim.Location = new System.Drawing.Point(131, 55);
            this.chbTrim.Name = "chbTrim";
            this.chbTrim.Size = new System.Drawing.Size(148, 17);
            this.chbTrim.TabIndex = 3;
            this.chbTrim.Text = "Обрезать фон по краям";
            this.chbTrim.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 289);
            this.Controls.Add(this.gbGifToSti);
            this.Controls.Add(this.gbStiToGif);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "STI <=> GIF";
            this.gbStiToGif.ResumeLayout(false);
            this.gbStiToGif.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudForeshorteningIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDelay)).EndInit();
            this.gbGifToSti.ResumeLayout(false);
            this.gbGifToSti.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudForeshorteningCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStiToGif;
        private System.Windows.Forms.GroupBox gbStiToGif;
        private System.Windows.Forms.GroupBox gbGifToSti;
        private System.Windows.Forms.CheckBox chbTransparentBackground;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudDelay;
        private System.Windows.Forms.Button btnGifToSti;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.NumericUpDown nudForeshorteningCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudForeshorteningIndex;
        private System.Windows.Forms.ToolTip ttDelay;
        private System.Windows.Forms.ToolTip ttTransparent;
        private System.Windows.Forms.CheckBox chbTrim;
    }
}

