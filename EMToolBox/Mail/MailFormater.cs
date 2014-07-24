using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMToolBox.Mail
{
    public class MailFormater
    {
        private string _formated;

        public MailFormater(string pattern, object source)
        {
            if (source != null)
                _formated = source.ToString(pattern, new System.Globalization.CultureInfo("fr-fr"));
        }

        public MailFormater(string pattern, Dictionary<String, object> optParam = null)
        {
            if (optParam != null)
            {
                _formated = pattern;

                foreach (String key in optParam.Keys)
                {
                    object tmp = optParam[key];

                    string result;
                    if (tmp is Int32)
                        result = Convert.ToInt32(tmp).ToString();
                    else if (tmp is Decimal)
                        result = Convert.ToDecimal(tmp).ToString();
                    else if (tmp is DateTime)
                        result = Convert.ToDateTime(tmp).ToString("dd/MM/yyyy HH:mm:ss");
                    else if (tmp is Boolean)
                        result = Convert.ToBoolean(tmp) ? "Oui" : "Non";
                    else
                        result = Convert.ToString(tmp);

                    _formated = _formated.Replace(key, result);
                }
            }
        }

        public String Formated { get { return _formated; } }
    }
}
