using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EMToolBox.Mail
{
    public class EMMail
    {
        private ILog log = LogManager.GetLogger(typeof(EMMail));
        
        private static string m_serveur = "smtp.gmail.com";
        private static int m_port = 587;
        private static string[] m_user = new string[6] { "emappmailsender@gmail.com",
            "emappmailsender1@gmail.com",
            "emappmailsender2@gmail.com",
            "emappmailsender3@gmail.com",
            "emappmailsender@gmail.com",
            "emappmailsender@gmail.com" };
        private static string m_password = "3m4ppm4ailS3nder";

        private void Send(String subject, String body, String to, int attemp)
        {
            try
            {
                MailMessage message = new MailMessage();

                message.From = new MailAddress(m_user[attemp]);
                message.To.Add(to);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;

                using (SmtpClient smtp = new SmtpClient(m_serveur, m_port))
                {
                    if (m_port != 25)
                        smtp.EnableSsl = true;
                    if (!String.IsNullOrEmpty(m_user[attemp]) && !String.IsNullOrEmpty(m_password))
                        smtp.Credentials = new System.Net.NetworkCredential(m_user[attemp], m_password);
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
            while (attemps < m_user.Count())
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

        public void SendSmtpMail(String subject, String patternBody, String to, Dictionary<String, Object> parameters)
        {
            Formater formater = new Formater(patternBody, parameters);

            SendSmtpMail(subject, formater.GetFormated(), to);
        }

        public void SendSmtpMail(String subject, FileInfo patternBody, String to, Dictionary<String, Object> parameters)
        {
            String tmp;
            using (StreamReader fs = new StreamReader(patternBody.OpenRead()))
            {
                tmp = fs.ReadToEnd();
            }

            Formater formater = new Formater(tmp, parameters);
            SendSmtpMail(subject, formater.GetFormated(), to);
        }
    }
}
