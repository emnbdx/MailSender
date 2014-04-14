using log4net;
using System;
using System.Collections.Generic;
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
        
        private static int m_port = 587;
        private static string[,] m_login = new string[7, 3] { 
            { "contact@lbcalerter.com", "vicpt240", "auth.smtp.1and1.fr"},
            { "emappmailsender@gmail.com", "3m4ppm4ailS3nder", "smtp.gmail.com"},
            {"emappmailsender1@gmail.com", "3m4ppm4ailS3nder", "smtp.gmail.com"},
            {"emappmailsender2@gmail.com", "3m4ppm4ailS3nder", "smtp.gmail.com"},
            {"emappmailsender3@gmail.com", "3m4ppm4ailS3nder", "smtp.gmail.com"},
            {"emappmailsender4@gmail.com", "3m4ppm4ailS3nder", "smtp.gmail.com"},
            {"emappmailsender5@gmail.com", "3m4ppm4ailS3nder", "smtp.gmail.com"} };
        private void Send(String subject, String body, String to, int attemp)
        {
            try
            {
                MailMessage message = new MailMessage();

                message.From = new MailAddress(m_login[attemp, 0]);
                message.To.Add(to);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;

                using (SmtpClient smtp = new SmtpClient(m_login[attemp, 2], m_port))
                {
                    if (m_port != 25)
                        smtp.EnableSsl = true;
                    if (!String.IsNullOrEmpty(m_login[attemp, 0]) && !String.IsNullOrEmpty(m_login[attemp, 1]))
                        smtp.Credentials = new System.Net.NetworkCredential(m_login[attemp, 0], m_login[attemp, 1]);
                    smtp.Send(message);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error when sending email", e);
            }
        }

        public void SendSmtpMail(String subject, String body, String to)
        {
            int attemps = 0;
            while (attemps < m_login.GetLength(0))
            {
                try
                {
                    if (attemps != 0)
                        log.Info("Nouvelle tentative #" + attemps + " après erreur");

                    Send(subject, body, to, attemps);
                    break;
                }
                catch (Exception e)
                {
                    log.Error(e.Message + "\r\n" + e.InnerException);
                }
                attemps++;
            }
        }

        public void SendSmtpMail(String subject, String to, String pattern, Dictionary<String, Object> parameters)
        {
            Formater formater = new Formater(pattern, parameters);
            SendSmtpMail(subject, formater.GetFormated(), to);
        }
    }
}
