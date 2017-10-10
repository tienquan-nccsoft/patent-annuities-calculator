using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouse.PatentCalculator.Models
{
    public class AdminAgencyFee
    {
        public int Year { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Out Of Range")]
        public decimal Fee { get; set; }        
        public DateTime ValidFrom { get; set; }
    }

    public class AdminAgencyFeeModel
    {
        [Required]
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public int PatentTypeId { get; set; }
        public string PatentTypeName { get; set; }
        public int PatentTypeYears { get; set; }
        [Required]
        public string CurrencyCode { get; set; }
        public DateTime ValidFrom { get; set; }
        public List<AdminAgencyFee> AgencyFees { get; set; }
    }
}
