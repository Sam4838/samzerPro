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
    
    public partial class userWatchList
    {
        public int id { get; set; }
        public Nullable<int> userId { get; set; }
        public Nullable<int> adId { get; set; }
        public string showDate { get; set; }
        public string isWatch { get; set; }
        public Nullable<int> packageId { get; set; }
    
        public virtual AdvertiseTbl AdvertiseTbl { get; set; }
        public virtual PackagesTbl PackagesTbl { get; set; }
        public virtual RegistrationTbl RegistrationTbl { get; set; }
    }
}
