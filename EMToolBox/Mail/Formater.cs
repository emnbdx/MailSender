using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMToolBox.Mail
{
    public class Formater
    {
        private object m_source;
        private string m_unformatedString;
        private string m_formatedString;

        public Formater(object source, string unformated)
        {
            m_source = source;
            m_unformatedString = unformated;
        }

        public string GetFormated()
        {
            if(String.IsNullOrEmpty(m_formatedString))
            {
                m_formatedString = m_source.ToString(m_unformatedString, new System.Globalization.CultureInfo("fr-fr"));
            }

            return m_formatedString;
        }
    }
}
