using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EMToolBox
{
    public static class FormattableObject
    {
        private static ILog log = LogManager.GetLogger(typeof(FormattableObject));
        
        private static Regex regValue = new Regex(@"({)([^}]+)(})", RegexOptions.IgnoreCase);
        private static Regex regConditionnal = new Regex(@"\[(?<tag>\w*)\](?<text>.*)\[/\k<tag>\]", RegexOptions.IgnoreCase);
        private static Regex regDuplicate = new Regex(@"\#(?<tag>\w*)\#(?<text>.*)\#/\k<tag>\#", RegexOptions.IgnoreCase);

        private static void GetParamByReflexion(object source, string paramName, out Type retrievedType, out object retrievedObject)
        {
            Type type = source.GetType();

            retrievedType = null;
            retrievedObject = null;

            //first try properties
            PropertyInfo retrievedProperty = type.GetProperty(paramName);
            if (retrievedProperty != null)
            {
                retrievedType = retrievedProperty.PropertyType;
                retrievedObject = retrievedProperty.GetValue(source, null);
            }
            else //try fields
            {
                FieldInfo retrievedField = type.GetField(paramName);
                if (retrievedField != null)
                {
                    retrievedType = retrievedField.FieldType;
                    retrievedObject = retrievedField.GetValue(source);
                }
            }
        }

        private static string FormatReflexion(String paramName, Type retrievedType, object retrievedObject, string toFormat, IFormatProvider formatProvider)
        {
            try
            {
                if (toFormat == String.Empty) //no format info
                {
                    return retrievedType.InvokeMember("ToString",
                      BindingFlags.Public | BindingFlags.NonPublic |
                      BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase
                      , null, retrievedObject, null) as string;
                }
                else //format info
                {
                    return retrievedType.InvokeMember("ToString",
                      BindingFlags.Public | BindingFlags.NonPublic |
                      BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase
                      , null, retrievedObject, new object[] { toFormat, formatProvider }) as string;
                }
            }
            catch (Exception e)
            {
                log.Error("Erreur lors de la récupération du paramètre [" + paramName + "]", e);
                return String.Empty;
            }
        }
        
        private static string FormatValue(this object source, string aFormat, IFormatProvider formatProvider)
        {
            StringBuilder sb = new StringBuilder();
            
            MatchCollection mc = regValue.Matches(aFormat);
            int startIndex = 0;
            foreach (Match m in mc)
            {
                Group g = m.Groups[2]; //it's second in the match between { and }
                int length = g.Index - startIndex - 1;
                sb.Append(aFormat.Substring(startIndex, length));

                string toGet = String.Empty;
                string toFormat = String.Empty;
                int formatIndex = g.Value.IndexOf(":"); //formatting would be to the right of a :
                if (formatIndex == -1) //no formatting, no worries
                {
                    toGet = g.Value;
                }
                else //pickup the formatting
                {
                    toGet = g.Value.Substring(0, formatIndex);
                    toFormat = g.Value.Substring(formatIndex + 1);
                }

                Type retrievedType;
                object retrievedObject;
                GetParamByReflexion(source, toGet, out retrievedType, out retrievedObject);

                if (retrievedType != null) //Cool, we found something
                {
                    string result = FormatReflexion(toGet, retrievedType, retrievedObject, toFormat, formatProvider);
                    sb.Append(result);
                }
                else //didn't find a property with that name, so be gracious and put it back
                {
                    sb.Append("{");
                    sb.Append(g.Value);
                    sb.Append("}");
                }
                startIndex = g.Index + g.Length + 1;
            }
            if (startIndex < aFormat.Length) //include the rest (end) of the string
            {
                sb.Append(aFormat.Substring(startIndex));
            }
            return sb.ToString();
        }

        private static string FormatConditionnalblock(this object source, string aFormat)
        {
            MatchCollection mc = regConditionnal.Matches(aFormat);
            foreach (Match m in mc)
            {
                //Get param name 
                string paramName = m.Groups["tag"].Value;
                bool nega = paramName.StartsWith("NOT_");
                paramName = paramName.Replace("NOT_", "");

                Type retrievedType;
                object retrievedObject;
                GetParamByReflexion(source, paramName, out retrievedType, out retrievedObject);

                if (retrievedType != null) //Cool, we found something
                {
                    string result = FormatReflexion(paramName, retrievedType, retrievedObject, String.Empty, null);

                    if (!nega)
                    {
                        if (!String.IsNullOrEmpty(result) && result != "False")
                        {
                            aFormat = aFormat.Replace(m.Groups[0].Value, m.Groups[2].Value);
                        }
                        else
                        {
                            aFormat = aFormat.Replace(m.Groups[0].Value, "");
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(result) || result == "False")
                        {
                            aFormat = aFormat.Replace(m.Groups[0].Value, m.Groups[2].Value);
                        }
                        else
                        {
                            aFormat = aFormat.Replace(m.Groups[0].Value, "");
                        }
                    }
                }
            }

            return aFormat;
        }

        private static string FormatDuplicate(this object source, string aFormat)
        {
            MatchCollection mc = regDuplicate.Matches(aFormat);
            foreach (Match m in mc)
            {
                //Get param name 
                string paramName = m.Groups["tag"].Value;

                Type retrievedType;
                object retrievedObject;
                GetParamByReflexion(source, paramName, out retrievedType, out retrievedObject);

                if (retrievedType != null) //Cool, we found something
                {
                    string result = FormatReflexion(paramName, retrievedType, retrievedObject, String.Empty, null);

                    string toDuplicate = m.Groups[2].Value;

                    var values = result.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    string duplicate = "";
                    foreach (string value in values)
                    {
                        duplicate += toDuplicate.Replace("{" + paramName + "}", value);
                    }

                    aFormat = aFormat.Replace(m.Groups[0].Value, duplicate);
                }
            }

            return aFormat;
        }

        public static string ToString(this object source, string aFormat)
        {
            return FormattableObject.ToString(source, aFormat, null);
        }

        public static string ToString(this object source, string aFormat, IFormatProvider formatProvider)
        {
            aFormat = FormatConditionnalblock(source, aFormat);
            aFormat = FormatDuplicate(source, aFormat);
            aFormat = FormatValue(source, aFormat, formatProvider);

            return aFormat;
        }
    }
}
