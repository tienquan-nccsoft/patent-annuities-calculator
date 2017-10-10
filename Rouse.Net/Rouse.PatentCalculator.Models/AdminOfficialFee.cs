using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouse.PatentCalculator.Models
{
    public class AdminOfficialFee
    {
        public int Year { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Out Of Range")]
        public decimal BasicFee { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Out Of Range")]
        public decimal ClaimFee { get; set; }        
        public DateTime ValidFrom { get; set; }
    }

    public class AdminOfficialFeeModel
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
        public List<AdminOfficialFee> OfficialFees { get; set; }
    }
}
