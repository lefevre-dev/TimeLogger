using System;
using System.ServiceProcess;
using System.Threading;

namespace TimeLoggerService
{
    public partial class Service1 : ServiceBase
    {
        private int scan_frequency = 60*10;
        Timer timer;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // démarrage du service

            // création du timer pour le scan
            timer = new Timer(OnTimer, null, 0, scan_frequency * 1000);
        }

        private void OnTimer(object sender)
        {
            timer.Dispose();
            GC.Collect();
            try
            {
                RequestLog();
            }
            catch (Exception ex)
            {
                //WriteLog("Scan error: " + ex.Message);
            }
            timer = new Timer(OnTimer, null, scan_frequency * 1000, scan_frequency * 1000);
        }

        private void RequestLog()
        {

        }

        protected override void OnStop()
        {
            timer.Dispose();
        }
    }
}
