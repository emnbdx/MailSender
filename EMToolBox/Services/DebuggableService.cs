using log4net;
using System;
using System.ServiceProcess;
using System.Threading;

namespace EMToolBox.Services
{
    public partial class DebuggableService : ServiceBase
    {
        ILog log = LogManager.GetLogger(typeof(TimedService));

        private TimedService m_simpleService;
        private EventWaitHandle m_stopEvent = null;
        private EventWaitHandle m_threadStoppedEvent = null;
        private Thread m_serviceThread = null;

        public DebuggableService(Type simpleServiceType)
        {
            InitializeComponent();
            m_simpleService = (TimedService)Activator.CreateInstance(simpleServiceType);
            m_simpleService.Service = this;
        }

        protected override void OnStart(string[] args)
        {
            m_stopEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
            m_threadStoppedEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
            m_serviceThread = new Thread(new ThreadStart(ServiceThread));
            m_serviceThread.SetApartmentState(m_simpleService.ThreadApartmentState);
            m_serviceThread.Start();
        }

        protected override void OnStop()
        {
            m_stopEvent.Set();
            m_simpleService.StopProcessing();
            // Attend la fin de la boucle en cours
            m_threadStoppedEvent.WaitOne();
            m_serviceThread = null;
        }

        private void ServiceThread()
        {
            try
            {
                m_simpleService.Initialize();

                bool firstProcess = true;
                bool finished = false;
                while (!finished)
                {
                    m_simpleService.Process();
                    m_simpleService.Process(firstProcess);
                    firstProcess = false;
                    finished = m_stopEvent.WaitOne(m_simpleService.Interval, true);
                }
                this.ExitCode = 0;
                m_threadStoppedEvent.Set();
            }
            catch (Exception e)
            {
                log.Error("Arrêt du service suite à une erreur non gérée:\r\n" + e.ToString());
                m_simpleService.Crash(e);
            }

            // A tout hasard, pour être sûr de ne pas bloquer le thread principal...
            m_threadStoppedEvent.Set();
        }

        public bool WaitForEnding(TimeSpan timeout)
        {
            return m_threadStoppedEvent.WaitOne(timeout);
        }

        public void StopDebug()
        {
            this.OnStop();
        }

        public new void Stop()
        {
            base.Stop();
        }

        public void Crash(Exception e)
        {
            m_simpleService.Crash(e);
        }

        public int Interval { get { return m_simpleService.Interval; } set { m_simpleService.Interval = value; } }
    }
}
