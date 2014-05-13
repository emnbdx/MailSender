using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMToolBox.Mail
{
    public class Formater
    {
        private Dictionary<String, Object> m_params;
        private string m_unformatedString;
        private string m_formatedString;

        public Formater(string unformated, Dictionary<String, Object> parameters)
        {
            m_unformatedString = unformated;
            m_params = parameters;
        }

        public string GetFormated()
        {
            if(String.IsNullOrEmpty(m_formatedString))
            {
                m_formatedString = m_unformatedString;

                if (m_params != null)
                {
                    foreach (String key in m_params.Keys)
                    {
                        String t = m_params[key].GetType().Name;
                        switch (t)
                        {
                            /*case "Int":
                                break;
                             * 
                             * ...
                             * 
                             */
                            default:
                                m_formatedString = m_formatedString.Replace(key, Convert.ToString(m_params[key]));
                                break;
                        }
                    }
                }
            }

            return m_formatedString;
        }
    }
}
