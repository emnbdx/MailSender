using System;
using System.Collections.Generic;
using System.Text;

namespace EMToolBox.Services
{
    /// <summary>
    /// Fournis un ensemble de fonctionnalit�s permettant de lire les param�tres
    /// de la ligne de commande.
    /// </summary>
    public static class CommandLine
    {
        #region Private
        #endregion Private
        #region Public

        /// <summary>
        /// Retourne la valeur du param�tre de la ligne de commande dont le nom est sp�cifi�.
        /// La casse n'est pas respect�e.
        /// </summary>
        /// <remarks>
        /// Les formats reconnus sont:
        /// 
        ///     {nom_param�tre}={valeur_param�tre}
        ///     /{nom_param�tre}={valeur_param�tre}
        ///     -{nom_param�tre}={valeur_param�tre}
        ///     {nom_param�tre} {valeur_param�tre}
        ///     /{nom_param�tre} {valeur_param�tre}
        ///     -{nom_param�tre} {valeur_param�tre}
        /// 
        /// Ex: Passage de la valeur 'srv-sql-w2s' par le param�tre 'server'
        /// 
        ///     C:\toto.exe server srv-sql-w2s
        ///     C:\toto.exe /server=srv-sql-w2s
        /// 
        /// </remarks>
        /// <param name="parameterName">Nom du param�tre recherch�</param>
        /// <returns>Valeur associ�e � ce param�tre</returns>
        public static string GetValue(string parameterName)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i=0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.StartsWith("-") || arg.StartsWith("/"))
                    arg = arg.Substring(1);
                string[] nameValue = arg.Split(new char[] { '=' });
                if ((nameValue.Length > 0) && (nameValue[0].ToUpper() == parameterName.ToUpper()))
                {
                    if (arg.Length == parameterName.Length)
                    {
                        if (args.Length > i + 1) return args[i + 1]; // ex: C:\toto.exe /PARAM VALUE
                        return ""; // La valeur du param�tre n'est pas renseign�e. ex: C:\toto.exe /PARAM ou C:\toto.exe /PARAM=
                    }
                    else
                    {
                        // C:\toto.exe /PARAM=VALUE
                        return arg.Substring(parameterName.Length + 1, arg.Length - parameterName.Length - 1);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Retourne la valeur du premier param�tre trouv�
        /// </summary>
        /// <param name="parameterNames">Liste des noms de param�tre</param>
        /// <returns></returns>
        public static string GetValue(string[] parameterNames)
        {
            foreach (string parameterName in parameterNames)
            {
                string result = GetValue(parameterName);
                if (result != null) return result;
            }
            // Aucune valeur n'a �t� trouv�e
            return null;
        }

        /// <summary>
        /// D�termine si la ligne de commande contient un switch. La casse n'est pas respect�e.
        /// </summary>
        /// <param name="switchName">Nom du switch recherch�</param>
        /// <returns>True si le switch apparait sur la ligne de commande</returns>
        public static bool ContainsSwitch(string switchName)
        {
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.Replace("/", "").Replace("-", "").ToUpper() == switchName.ToUpper()) return true;
            }
            return false;
        }

        /// <summary>
        /// D�termine si la ligne de commande contient un des switchs. La casse n'est pas respect�e.
        /// </summary>
        /// <param name="switchName">Nom du switch recherch�</param>
        /// <returns>True si le switch apparait sur la ligne de commande</returns>
        public static bool ContainsSwitch(string[] switchNames)
        {
            foreach (string switchName in switchNames)
            {
                if (ContainsSwitch(switchName)) return true;
            }
            return false;
        }

        /// <summary>
        /// D�termine si le premier switch de la ligne de commande est le switch sp�cifi�
        /// </summary>
        /// <param name="switchName">Nom du switch recherch�</param>
        /// <returns></returns>
        public static bool FirstSwitchIs(string switchName)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 2) return false;
            return (args[1].Replace("/", "").Replace("-", "").ToUpper() == switchName.ToUpper());
        }

        /// <summary>
        /// D�termine si le premier switch de la ligne de commande est le switch sp�cifi�
        /// </summary>
        /// <param name="switchName">Nom du switch recherch�</param>
        /// <returns></returns>
        public static bool FirstSwitchIs(string[] switchNames)
        {
            foreach (string switchName in switchNames)
            {
                if (FirstSwitchIs(switchName)) return true;
            }
            return false;
        }

        /// <summary>
        /// Nombre de param�tres sur la ligne de commande
        /// </summary>
        public static int Count
        {
            get { return Environment.GetCommandLineArgs().Length; }
        }

        /// <summary>
        /// Retourne l'argument de la ligne de commande portant l'index sp�cifi�
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetValue(int index)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (index >= args.Length) throw new ArgumentOutOfRangeException("index", "Nombre d'arguments sur la ligne de commande : [" + args.Length.ToString() + "]\r\nValeur du param�tre 'Index': [" + index.ToString() + "]");
            return args[index]; 
        }

        #endregion Public
    }
}
