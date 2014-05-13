using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMToolBox.Mail;
using EMToolBox.Services;
using log4net;
using System.Configuration;

namespace MailSender
{
    class MailSenderService : SimpleService
    {
        private static ILog log = LogManager.GetLogger(typeof(MailSenderService));
        private EMMail _mailer;
        
        public override void Initialize()
        {
            base.Initialize();
            this.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["Intervalle"]) * 1000;
            _mailer = new EMMail();
        }

        public override void Process()
        {
            base.Process();
            _mailer.Send();
        }
    }
}
