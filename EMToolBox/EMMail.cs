using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EMToolBox
{
    public class EMMail
    {
        private static string m_serveur = "smtp.gmail.com";
        private static int m_port = 587;
        private static string m_user = "emappmailsender@gmail.com";
        private static string m_password = "3m4ppm4ailS3nder";

        public void SendSmtpMail(String subject, String body, String to)
        {
            try
            {
                MailMessage message = new MailMessage();

                message.From = new MailAddress(m_user);
                message.To.Add(to);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;

                using (SmtpClient smtp = new SmtpClient(m_serveur, m_port))
                {
                    if (m_port != 25)
                        smtp.EnableSsl = true;
                    if (!String.IsNullOrEmpty(m_user) && !String.IsNullOrEmpty(m_password))
                        smtp.Credentials = new System.Net.NetworkCredential(m_user, m_password);
                    smtp.Send(message);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error when sending email", e);
            }
        }
    }
}
