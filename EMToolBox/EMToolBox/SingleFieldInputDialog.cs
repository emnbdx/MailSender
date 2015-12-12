using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EMToolBox
{
    public partial class SingleFieldInputDialog : Form
    {
        #region Private

        /// <summary>
        /// Constructeur
        /// </summary>
        private SingleFieldInputDialog()
        {
            InitializeComponent();
        }

        private DialogResult Execute(IWin32Window owner, string title, string description, string label, string defaultValue, int maxLength, out string value)
        {
            lblDescription.Text = description;
            
            lblValue.Top = lblDescription.Top + ComputeTextHeight(description) + 10;
            txtValue.Top = lblValue.Top - 3;
            this.Height = txtValue.Top + 108;

            lblValue.Text = label;
            lblValue.Width = ComputeTextWidth(label);
            if (maxLength > 0)
            {
                txtValue.Width = ComputeTextWidth(new string('m', Math.Max(maxLength + 1, 25)));
                txtValue.MaxLength = maxLength;
            }
            txtValue.Left = lblValue.Right + 5;
            this.Width = txtValue.Right + lblValue.Left;
            txtValue.Anchor |= AnchorStyles.Right;
            txtValue.Text = defaultValue;
            this.Text = title;
            
            DialogResult dr = this.ShowDialog(owner);
            value = txtValue.Text;

            return dr;
        }

        /// <summary>
        /// Retourne la longueur (en pixels) n�c�ssaire pour afficher le texte sp�cifi�
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private int ComputeTextWidth(string text)
        {
            SizeF sz = Graphics.FromHwnd(this.Handle).MeasureString(text, this.Font);
            return Convert.ToInt32(sz.Width);
        }

        /// <summary>
        /// Retourne la hauteur(en pixels) n�c�ssaire pour afficher le texte sp�cifi�
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private int ComputeTextHeight(string text)
        {
            SizeF sz = Graphics.FromHwnd(this.Handle).MeasureString(text, this.Font);
            return Convert.ToInt32(sz.Height);
        }

        #endregion Private
        #region Public

        /// <summary>
        /// Affiche une fen�tre proposant un libell�, un champ de saisie libre, des boutons 'Ok' et 'Annuler'.
        /// La valeur saisie est retourn�e par le param�tre en sortie 'value'
        /// </summary>
        /// <param name="owner">Fen�tre parente</param>
        /// <param name="title">Titre de la fen�tre</param>
        /// <param name="label">Libell� de la zone de saisie</param>
        /// <param name="defaultValue">Valeur � afficher par d�faut dans la zone de saisie</param>
        /// <param name="maxLength">Taille maximale de texte � saisir</param>
        /// <param name="value">Valeur de retour</param>
        /// <returns>DialogResult selon le bouton s�lectionn�</returns>
        public static DialogResult Show(IWin32Window owner, string title, string description, string label, string defaultValue, int maxLength, out string value)
        {
            SingleFieldInputDialog dlg = new SingleFieldInputDialog();
            return dlg.Execute(owner, title, description, label, defaultValue, maxLength, out value);
        }
        
        /// <summary>
        /// Affiche une fen�tre proposant un libell�, un champ de saisie libre, des boutons 'Ok' et 'Annuler'.
        /// La valeur saisie est retourn�e par le param�tre en sortie 'value'
        /// </summary>
        /// <param name="owner">Fen�tre parente</param>
        /// <param name="title">Titre de la fen�tre</param>
        /// <param name="label">Libell� de la zone de saisie</param>
        /// <param name="defaultValue">Valeur � afficher par d�faut dans la zone de saisie</param>
        /// <param name="maxLength">Taille maximale de texte � saisir</param>
        /// <param name="value">Valeur de retour</param>
        /// <returns>DialogResult selon le bouton s�lectionn�</returns>
        public static DialogResult Show(IWin32Window owner, string title, string label, string defaultValue, int maxLength, out string value)
        {
            return Show(
                owner,
                title,
                "",
                label,
                defaultValue,
                maxLength,
                out value);
        }

        /// <summary>
        /// Affiche une fen�tre proposant un libell�, un champ de saisie libre, des boutons 'Ok' et 'Annuler'.
        /// La valeur saisie est retourn�e par le param�tre en sortie 'value'
        /// </summary>
        /// <param name="title">Titre de la fen�tre</param>
        /// <param name="value">Valeur de retour</param>
        /// <returns>DialogResult selon le bouton s�lectionn�</returns>
        public static DialogResult Show(string title, out string value)
        {
            return Show(
                null,
                title,
                "",
                "Valeur",
                "",
                0,
                out value);
        }

        #endregion Public
    }
}