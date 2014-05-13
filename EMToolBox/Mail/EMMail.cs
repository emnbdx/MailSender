using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EMToolBox.Mail
{
    public class EMMail
    {
        private ILog log = LogManager.GetLogger(typeof(EMMail));
        
        private EMMAILEntities _context = new EMMAILEntities();

        private List<PATTERN> _patterns;

        private List<PATTERN> Pattern
        {
            get
            {
                if (_patterns == null)
                    _patterns = _context.PATTERN.ToList();
                return _patterns;
            }
            set { _patterns = value; }
        }

        private List<SERVER> _servers;

        private List<SERVER> Server
        {
            get
            {
                if (_servers == null)
                    _servers = _context.SERVER.Where(entry => entry.ENABLE == true).ToList();
                return _servers;
            }
            set { _servers = value; }
        }
        

        private void Send(QUEUE element)
        {
            foreach (SERVER server in Server)
            {
                try
                {
                    MailMessage message = new MailMessage();

                    message.From = new MailAddress(server.LOGIN);
                    message.To.Add(element.TO);
                    message.Subject = element.SUBJECT;
                    message.IsBodyHtml = element.PATTERN.HTML;
                    message.Body = element.BODY;

                    using (SmtpClient smtp = new SmtpClient(server.IP, server.PORT))
                    {
                        if (server.SSL)
                            smtp.EnableSsl = true;
                        if (!String.IsNullOrEmpty(server.LOGIN) && !String.IsNullOrEmpty(server.PASSWORD))
                            smtp.Credentials = new System.Net.NetworkCredential(server.LOGIN, server.PASSWORD);
                        smtp.Send(message);
                    }

                    element.SEND = true;
                    element.SENDDATE = DateTime.Now;

                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    throw new Exception("Error when sending email", e);
                }
            }
        }

        /// <summary>
        /// Add Mail to queue
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="to"></param>
        /// <param name="pattern"></param>
        /// <param name="parameters"></param>
        public void Add(String subject, String to, String pattern, Dictionary<String, Object> parameters)
        {
            PATTERN pat = Pattern.First(entry => entry.NAME == pattern);
            Formater formater = new Formater(pat.CONTENT, parameters);

            _context.QUEUE.Add(new QUEUE()
            {
                PATTERN = pat,
                SUBJECT = subject,
                TO = to,
                BODY = formater.GetFormated(),
                CREATIONDATE = DateTime.Now
            });

            _context.SaveChanges();
        }

        /// <summary>
        /// Send all mail (not already send)
        /// </summary>
        public void Send()
        {
            List<QUEUE> elements = _context.QUEUE.Where(entry => entry.SEND == false).OrderBy(entry => entry.CREATIONDATE).ToList();
            log.Info("Envoi de " + elements.Count + "mails...");
            foreach (QUEUE element in elements)
                Send(element);
            log.Info("OK");
        }
    }
}
