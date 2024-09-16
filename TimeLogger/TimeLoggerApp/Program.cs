using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using TimeLoggerApp.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TimeLoggerApp
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);

            if (procs.Count() > 1)
            {
                Application.Exit();
                return;
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            Application.Run(new MyCustomApplicationContext());
        }
    }

    public class MyCustomApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private Timer timer = new Timer();
        private Timer timer_icon = new Timer();
        private DateTime last_time = new DateTime();
        Form1 form;
        Summary summary;
        public MyCustomApplicationContext()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = Resources.icon,
                Text = "v" + fvi.FileVersion,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Exit", Exit),
                    new MenuItem("Summary", Summary),
                    new MenuItem("Log", Log),
                }),
                Visible = true
            };

            int min = ConfigFile.Instance.GetValue<int>("Time");
            timer.Interval = min * 60 * 1000;
            timer.Tick += new EventHandler(onTimer_log);
            timer.Start();

            timer_icon.Interval = 1000; // tout les 1s
            timer_icon.Tick += new EventHandler(onTimer_icon);
            timer_icon.Start();
            update_tray_icon();
        }

        private void update_tray_icon()
        {
            // mise à jour du timer en fonction de temps passé depuis le dernier log
            // int min = ConfigFile.Instance.GetValue<int>("Time"); // temps configuré
            // Datetime.Now - last_time => temps restant avant log
            // trayIcon.Icon = Resources.icon;
            // Tranche de 25%
        }

        private void onTimer_icon(object sender, EventArgs e)
        {
            update_tray_icon();
        }

        private void Log(object sender, EventArgs e)
        {
            // sauvegarde de la date de déclanchement
            last_time = DateTime.Now;
            onTimer_log(sender, e);
        }

        /// <summary>
        /// Affichage de la fenetre de log des temps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTimer_log(object sender, EventArgs e)
        {
            // demande pour logger l’activité en cours
            if (form == null || form.isclosed)
            {
                form = new Form1();
                form.Show();
                form.TopMost = true;
                form.isclosed = false;
            }
            else
            {
                DateTime d;
                d = DateTime.Now;
                SQLiteConnection conn = SqliteUtil.CreateConnection();
                SqliteUtil.InsertData(conn, d.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern), "AFK");
            }
        }

        /// <summary>
        /// Quitter l’application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        /// <summary>
        /// Affichage du résumé des temps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Summary(object sender, EventArgs e)
        {
            if (summary == null || summary.isclosed)
            {
                summary = new Summary();
                summary.Show();
                summary.isclosed = false;
            }
        }
    }
}
