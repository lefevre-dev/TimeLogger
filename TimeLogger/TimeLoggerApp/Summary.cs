using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeLoggerApp.Models;

namespace TimeLoggerApp
{
    public partial class Summary : Form
    {
        public bool isclosed = true;
        public Summary()
        {
            InitializeComponent();

            SQLiteConnection conn = SqliteUtil.CreateConnection();
            List<DataLog> data = SqliteUtil.SelectData(conn);

            // remplissage de la combobox
            List<string> dates = new List<string>();
            foreach (DataLog log in data)
            {
                string date_dd_mm_yyyy = log.Date.Split(' ')[0];
                if (!dates.Contains(date_dd_mm_yyyy))
                {
                    dates.Add(date_dd_mm_yyyy);
                }
            }
            
            dates.Sort(SortByNameAscending);
            comboBox1.Items.AddRange(dates.ToArray());

            // Selection de la combo
            try
            {
                comboBox1.SelectedItem = comboBox1.Items[0];
            }
            catch (Exception)
            {
            }

            // affichage du tableau
            DisplayTable();

        }

        public int SortByNameAscending(string name1, string name2)
        {
            DateTime d1 = DateTime.Parse(name1);
            DateTime d2 = DateTime.Parse(name2);
            return DateTime.Compare(d2, d1);
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public void DisplayTable()
        {
            string selected_combo = comboBox1.Text;
            dataGridView1.Rows.Clear();
            SQLiteConnection conn = SqliteUtil.CreateConnection();
            List<DataLog> data = SqliteUtil.SelectData(conn);
            foreach (DataLog log in data)
            {
                string date_dd_mm_yyyy = log.Date.Split(' ')[0];
                string date_hh_mm = log.Date.Split(' ')[1];
                if (date_dd_mm_yyyy == selected_combo)
                {
                    dataGridView1.Rows.Add(date_hh_mm, log.Log);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            isclosed = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayTable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // ouvre l’explorateur de fichier a l’emplacement de la base de données
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string program_data = Path.Combine(appdata, "TimeLogger");
            Process.Start(program_data);
        }

        /// <summary>
        /// Bouton précédent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            // on descend dans la combo
            if (comboBox1.SelectedIndex < comboBox1.Items.Count - 1)
            {
                comboBox1.SelectedIndex += 1;
            }
        }

        /// <summary>
        /// Bouton suivant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            // on monte dans la combo
            if (comboBox1.SelectedIndex > 0)
            {
                comboBox1.SelectedIndex -= 1;
            }
        }
    }
}
