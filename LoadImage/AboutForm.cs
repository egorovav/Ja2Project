using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Localizer;
using dotNetStiEditor.Properties;

namespace dotNetStiEditor
{
	public partial class AboutForm : Form
	{
		public AboutForm()
		{
			InitializeComponent();
            this.Text = LocalizerNameSpace.Localizer.GetString("About");
			StringBuilder about = new StringBuilder();

			about.AppendLine();
			about.AppendLine("                          .NET Sti Editor версия 1.0.5");
		//	about.AppendLine(" _________________________________________________");
			about.AppendLine();
			about.AppendLine(" Авторы:");
			about.AppendLine(" Егоров Алексей аки pipetz - программирование;");
			about.AppendLine(" Избачков Юрий аки Strax5 - техзадание, тестирование");
			about.AppendLine(" Алексей аки Zar XaplYch - локализация");
			about.AppendLine(" ________________________________________________");
			about.AppendLine(" При создании программы использован исходный код от");
			about.AppendLine(" Sasha Djurovic, djurovic@nyc.rr.com");
			about.AppendLine(" Stephane Rodriguez, http://xlsgen.arstdesign.com/");
			about.AppendLine(" ________________________________________________");
			about.AppendLine(" Поддержка / Support / Unterstützung:");
			about.AppendLine(" www.ja2.su, egorov_av@mail.ru:");
			about.AppendLine();
			about.AppendLine(" Москва, 19.02.2016");

			this.label1.Text = about.ToString();
			//this.textBox1.Text = about.ToString();
			//this.textBox1.Select(0, 0);
		}
	}
}