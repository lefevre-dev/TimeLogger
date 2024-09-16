using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using TimeLoggerApp.Models;

namespace TimeLoggerApp
{
    public partial class Form1 : Form
    {
        public bool isclosed = true;
        public Form1()
        {
            InitializeComponent();
            Init();
            
        }
        public void Init()
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);

            DateTime d;
            d = DateTime.Now;
            label1.Text = d.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);

            SQLiteConnection conn = SqliteUtil.CreateConnection();
            List<DataLog> data = SqliteUtil.SelectData(conn);
            ContextMenu cm = new ContextMenu();
            for (int i = 0; i < Math.Min(data.Count, 10); i++)
            {
                cm.MenuItems.Add(data[data.Count - 1 - i].Log, Fill_Text);
            }
            button2.ContextMenu = cm;
        }

        private void Fill_Text(object sender, EventArgs e)
        {
            MenuItem menu = (MenuItem)sender;
            if (textBox1.Text == "")
            {
                textBox1.Text = menu.Text;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            isclosed = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // on verifi que le text n’est pas vide
            if(textBox1.Text == "")
            {
                // on ne fait rien
                return;
            }
            DateTime d;
            d = DateTime.Now;
            // enregistrer la date et le text dans la base sqlite
            SQLiteConnection conn = SqliteUtil.CreateConnection();
            SqliteUtil.InsertData(conn, d.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern), textBox1.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // il faut qu’il n’y ai rien d’écrit
            if (textBox1.Text == "")
            {
                SQLiteConnection conn = SqliteUtil.CreateConnection();
                List<DataLog> data = SqliteUtil.SelectData(conn);
                textBox1.Text = data[data.Count - 1].Log;
            }
        }
    }
}
