//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EMToolBox.Mail
{
    using System;
    using System.Collections.Generic;
    
    public partial class PATTERN
    {
        public PATTERN()
        {
            this.QUEUE = new HashSet<QUEUE>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public string CONTENT { get; set; }
        public bool HTML { get; set; }
    
        public virtual ICollection<QUEUE> QUEUE { get; set; }
    }
}
