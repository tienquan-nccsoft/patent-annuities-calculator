using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rouse.PatentCalculator.Models
{
    public class AdminPatentFee {
        public int Year { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Out Of Range")]
        public decimal BasicFee { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Out Of Range")]
        public decimal ClaimFee { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Out Of Range")]
        public decimal AgencyFee { get; set; }
        public DateTime ValidFrom { get; set; }
    }

    public class AdminPatentFeeModel {
        [Required]
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public int PatentTypeId { get; set; }
        public string PatentTypeName { get; set; }
        public int PatentTypeYears { get; set; }
        [Required]
        public string CurrencyCode { get; set; }
        public List<AdminPatentFee> AgencyFees { get; set; }
    }


}
