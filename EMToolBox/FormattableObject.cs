// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormattableObject.cs" company="Eddy MONTUS">
//   2014
// </copyright>
// <summary>
//   The formattable object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EMToolBox
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    using log4net;
    
    /// <summary>
    /// The formattable object.
    /// </summary>
    public static class FormattableObject
    {
        /// <summary>
        /// The flag.
        /// </summary>
        private const BindingFlags Flag =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod
            | BindingFlags.IgnoreCase;
        
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(FormattableObject));

        /// <summary>
        /// The reg value.
        /// </summary>
        private static readonly Regex RegValue = new Regex(@"({)([^}]+)(})", RegexOptions.IgnoreCase);

        /// <summary>
        /// The reg conditionnal.
        /// </summary>
        private static readonly Regex RegConditionnal = new Regex(@"\[(?<tag>\w*)\](?<text>.*)\[/\k<tag>\]", RegexOptions.IgnoreCase);

        /// <summary>
        /// The reg duplicate.
        /// </summary>
        private static readonly Regex RegDuplicate = new Regex(@"\#(?<tag>\w*)\#(?<text>.*)\#/\k<tag>\#", RegexOptions.IgnoreCase);

        /// <summary>
        /// The to string.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToString(this object source, string format)
        {
            return source.ToString(format, null);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="formatProvider">
        /// The format provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToString(this object source, string format, IFormatProvider formatProvider)
        {
            format = FormatConditionnalblock(source, format);
            format = FormatDuplicate(source, format);
            format = FormatValue(source, format, formatProvider);

            return format;
        }

        /// <summary>
        /// The get param by reflexion.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="paramName">
        /// The param name.
        /// </param>
        /// <param name="retrievedType">
        /// The retrieved type.
        /// </param>
        /// <param name="retrievedObject">
        /// The retrieved object.
        /// </param>
        private static void GetParamByReflexion(object source, string paramName, out Type retrievedType, out object retrievedObject)
        {
            var type = source.GetType();

            retrievedType = null;
            retrievedObject = null;

            // first try properties
            var retrievedProperty = type.GetProperty(paramName);
            if (retrievedProperty != null)
            {
                retrievedType = retrievedProperty.PropertyType;
                retrievedObject = retrievedProperty.GetValue(source, null);
            }
            else
            {
                // try fields
                var retrievedField = type.GetField(paramName);
                if (retrievedField == null)
                {
                    return;
                }

                retrievedType = retrievedField.FieldType;
                retrievedObject = retrievedField.GetValue(source);
            }
        }

        /// <summary>
        /// The format reflexion.
        /// </summary>
        /// <param name="paramName">
        /// The param name.
        /// </param>
        /// <param name="retrievedType">
        /// The retrieved type.
        /// </param>
        /// <param name="retrievedObject">
        /// The retrieved object.
        /// </param>
        /// <param name="toFormat">
        /// The to format.
        /// </param>
        /// <param name="formatProvider">
        /// The format provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FormatReflexion(string paramName, Type retrievedType, object retrievedObject, string toFormat, IFormatProvider formatProvider)
        {
            try
            {
                // no format info
                if (toFormat == string.Empty)
                {
                    return
                        retrievedType.InvokeMember(
                            "ToString",
                            Flag,
                            null,
                            retrievedObject,
                            null) as string;
                }

                return
                    retrievedType.InvokeMember(
                        "ToString",
                        Flag,
                        null,
                        retrievedObject,
                        new object[] { toFormat, formatProvider }) as string;
            }
            catch (Exception e)
            {
                Log.Debug("Erreur lors de la récupération du paramètre [" + paramName + "]", e);
                return string.Empty;
            }
        }

        /// <summary>
        /// The format value.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="aFormat">
        /// The a format.
        /// </param>
        /// <param name="formatProvider">
        /// The format provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FormatValue(this object source, string aFormat, IFormatProvider formatProvider)
        {
            var sb = new StringBuilder();

            var mc = RegValue.Matches(aFormat);
            var startIndex = 0;
            foreach (Match m in mc)
            {
                var g = m.Groups[2]; // it's second in the match between { and }
                var length = g.Index - startIndex - 1;
                sb.Append(aFormat.Substring(startIndex, length));

                string toGet;
                var toFormat = string.Empty;
                var formatIndex = g.Value.IndexOf(":", StringComparison.Ordinal); // formatting would be to the right of a :
                if (formatIndex == -1)
                {
                    // no formatting, no worries
                    toGet = g.Value;
                }
                else
                {
                    // pickup the formatting
                    toGet = g.Value.Substring(0, formatIndex);
                    toFormat = g.Value.Substring(formatIndex + 1);
                }

                Type retrievedType;
                object retrievedObject;
                GetParamByReflexion(source, toGet, out retrievedType, out retrievedObject);

                if (retrievedType != null)
                {
                    // Cool, we found something
                    var result = FormatReflexion(toGet, retrievedType, retrievedObject, toFormat, formatProvider);
                    sb.Append(result);
                }
                else
                {
                    // didn't find a property with that name, so be gracious and put it back
                    sb.Append("{");
                    sb.Append(g.Value);
                    sb.Append("}");
                }

                startIndex = g.Index + g.Length + 1;
            }

            if (startIndex < aFormat.Length)
            {
                // include the rest (end) of the string
                sb.Append(aFormat.Substring(startIndex));
            }

            return sb.ToString();
        }

        /// <summary>
        /// The format conditionnalblock.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FormatConditionnalblock(this object source, string format)
        {
            var mc = RegConditionnal.Matches(format);
            foreach (Match m in mc)
            {
                // Get param name 
                var paramName = m.Groups["tag"].Value;
                var nega = paramName.StartsWith("NOT_");
                paramName = paramName.Replace("NOT_", string.Empty);

                Type retrievedType;
                object retrievedObject;
                GetParamByReflexion(source, paramName, out retrievedType, out retrievedObject);

                if (retrievedType == null)
                {
                    continue;
                }

                var result = FormatReflexion(paramName, retrievedType, retrievedObject, string.Empty, null);

                if (!nega)
                {
                    if (!string.IsNullOrEmpty(result) && result != "False")
                    {
                        format = format.Replace(m.Groups[0].Value, m.Groups[2].Value);
                    }
                    else
                    {
                        format = format.Replace(m.Groups[0].Value, string.Empty);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(result) || result == "False")
                    {
                        format = format.Replace(m.Groups[0].Value, m.Groups[2].Value);
                    }
                    else
                    {
                        format = format.Replace(m.Groups[0].Value, string.Empty);
                    }
                }
            }

            return format;
        }

        /// <summary>
        /// The format duplicate.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FormatDuplicate(this object source, string format)
        {
            var mc = RegDuplicate.Matches(format);
            foreach (Match m in mc)
            {
                // Get param name 
                var paramName = m.Groups["tag"].Value;

                Type retrievedType;
                object retrievedObject;
                GetParamByReflexion(source, paramName, out retrievedType, out retrievedObject);

                if (retrievedType == null)
                {
                    continue;
                }

                var result = FormatReflexion(paramName, retrievedType, retrievedObject, string.Empty, null);

                var toDuplicate = m.Groups[2].Value;

                var values = result.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var duplicate = values.Aggregate(string.Empty, (current, value) => current + toDuplicate.Replace("{" + paramName + "}", value));

                format = format.Replace(m.Groups[0].Value, duplicate);
            }

            return format;
        }
    }
}
