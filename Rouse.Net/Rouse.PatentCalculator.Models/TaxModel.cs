using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rouse.PatentCalculator.Models
{
    public class TaxModel
    {
        public int Id { get; set; }
        [DisplayName("Country")]
        [Required]
        public string CountryCode { get; set; }
        [DisplayName("Country")]
        public string CountryName { get; set; }
        [DisplayName("Default Currency")]
        public string DefaultCurrency { get; set; }
        [DisplayName("Tax Type")]
        [Required]
        public string Type { get; set; }
        [DisplayName("Tax Percentage")]
        [Required]
        [Range(0, 100, ErrorMessage = "Percentage must be of range 0 to 100")]
        public decimal Percentage { get; set; }
    }
}
