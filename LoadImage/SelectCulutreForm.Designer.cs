namespace dotNetStiEditor
{
	partial class SelectCulutreForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.russianRadioButton1 = new System.Windows.Forms.RadioButton();
			this.englishRadioButton2 = new System.Windows.Forms.RadioButton();
			this.deutschRadioButton3 = new System.Windows.Forms.RadioButton();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(20, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(235, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select program language please";
			// 
			// russianRadioButton1
			// 
			this.russianRadioButton1.AutoSize = true;
			this.russianRadioButton1.Location = new System.Drawing.Point(86, 53);
			this.russianRadioButton1.Name = "russianRadioButton1";
			this.russianRadioButton1.Size = new System.Drawing.Size(67, 17);
			this.russianRadioButton1.TabIndex = 1;
			this.russianRadioButton1.TabStop = true;
			this.russianRadioButton1.Tag = "ru";
			this.russianRadioButton1.Text = "Русский";
			this.russianRadioButton1.UseVisualStyleBackColor = true;
			// 
			// englishRadioButton2
			// 
			this.englishRadioButton2.AutoSize = true;
			this.englishRadioButton2.Location = new System.Drawing.Point(86, 77);
			this.englishRadioButton2.Name = "englishRadioButton2";
			this.englishRadioButton2.Size = new System.Drawing.Size(59, 17);
			this.englishRadioButton2.TabIndex = 2;
			this.englishRadioButton2.TabStop = true;
			this.englishRadioButton2.Tag = "en";
			this.englishRadioButton2.Text = "English";
			this.englishRadioButton2.UseVisualStyleBackColor = true;
			// 
			// deutschRadioButton3
			// 
			this.deutschRadioButton3.AutoSize = true;
			this.deutschRadioButton3.Location = new System.Drawing.Point(86, 101);
			this.deutschRadioButton3.Name = "deutschRadioButton3";
			this.deutschRadioButton3.Size = new System.Drawing.Size(65, 17);
			this.deutschRadioButton3.TabIndex = 3;
			this.deutschRadioButton3.TabStop = true;
			this.deutschRadioButton3.Tag = "de";
			this.deutschRadioButton3.Text = "Deutsch";
			this.deutschRadioButton3.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(78, 136);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// SelectCulutreForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(267, 171);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.deutschRadioButton3);
			this.Controls.Add(this.englishRadioButton2);
			this.Controls.Add(this.russianRadioButton1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectCulutreForm";
			this.Text = "Select culture";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton russianRadioButton1;
		private System.Windows.Forms.RadioButton englishRadioButton2;
		private System.Windows.Forms.RadioButton deutschRadioButton3;
		private System.Windows.Forms.Button button1;
	}
}