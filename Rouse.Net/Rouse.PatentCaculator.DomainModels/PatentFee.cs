//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rouse.PatentCalculator.DomainModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class PatentFee
    {
        public long ID { get; set; }
        public string CountryCode { get; set; }
        public byte PatentTypeId { get; set; }
        public byte Year { get; set; }
        public string CurrencyCode { get; set; }
        public System.DateTime ValidFrom { get; set; }
        public decimal BasicFee { get; set; }
        public decimal ClaimFee { get; set; }
        public decimal AgencyFee { get; set; }
        public bool Deleted { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual PatentFeeType PatentFeeType { get; set; }
    }
}
