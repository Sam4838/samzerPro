//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GTM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class StateTbl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StateTbl()
        {
            this.CityTbls = new HashSet<CityTbl>();
        }
    
        public int StateId { get; set; }
        public string StateName { get; set; }
        public Nullable<int> CountryId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CityTbl> CityTbls { get; set; }
        public virtual CountryTbl CountryTbl { get; set; }
    }
}
