//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DialogTimeStartStopWpf
{
    using System;
    using System.Collections.Generic;
    
    public partial class Lohnkategorien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lohnkategorien()
        {
            this.Personals = new HashSet<Personal>();
            this.ProjektLohnkategorieZuordnungs = new HashSet<ProjektLohnkategorieZuordnung>();
            this.RapportEintraeges = new HashSet<RapportEintraege>();
        }
    
        public string Kuerzel { get; set; }
        public string Beschreibung { get; set; }
        public decimal Standardansatz { get; set; }
        public int Lohnart { get; set; }
        public string LohnKatKontierung { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Personal> Personals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProjektLohnkategorieZuordnung> ProjektLohnkategorieZuordnungs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RapportEintraege> RapportEintraeges { get; set; }
    }
}
