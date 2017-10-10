using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouse.PatentCalculator.Models
{
    public class CalculationRequest : IValidatableObject {
        
        [Required]
        [DisplayName("Country")]
        public string CountryCode { get; set; }

        [Required]
        [DisplayName("Patent Type")]
        public int PatentTypeId { get; set; }

        [Required]
        [StringLength(40)]
        [DisplayName("Application / Patent No.")]
        public string ApplicationPatentNo { get; set; }

        [Required]
        [Range(1, 9999, ErrorMessage = "No of Claims must be of range 0 to 9999")]
        [DisplayName("No of Claims")]
        public int NoOfClaim { get; set; }

        [DisplayName("Filing Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FilingDate { get; set; }

        [DisplayName("Grant Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? GrantDate { get; set; }

        [DisplayName("Agency Fee Discount")]
        [Range(0, 100, ErrorMessage = "Agency Fee Discount must be of range 0 to 100")]
        public decimal AgencyFeeDiscount { get; set; }

        [DisplayName("Currency")]
        public string CurrencyCode { get; set; }

        [DisplayName("Report Type")]
        public ReportType ReportTypeId { get;set; }

        public int ExpectedPayableYear { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (ReportTypeId == ReportType.FirstGrantPayment) {
                if (!GrantDate.HasValue) {
                    yield return
                        new ValidationResult(errorMessage: "Grant Date is required in First Grant Payment",
                            memberNames: new[] {"GrantDate"});
                }
                if (!FilingDate.HasValue) {
                    yield return
                        new ValidationResult(errorMessage: "Filing Date is required in First Grant Payment",
                            memberNames: new[] {"FilingDate"});
                }
            } else {
                if(ExpectedPayableYear != 0)
                    yield return new ValidationResult(errorMessage: "Ops! Annuity year wrong!");
            }
            if (GrantDate?.Date < FilingDate?.Date) {
                yield return
                    new ValidationResult(errorMessage: "Grant Date must be equal or greater than Filing Date",
                        memberNames: new[] {"GrantDate"});
            }

        }
    }
}