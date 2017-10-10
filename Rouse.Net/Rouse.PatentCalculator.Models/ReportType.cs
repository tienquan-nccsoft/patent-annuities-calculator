
using System.ComponentModel.DataAnnotations;

namespace Rouse.PatentCalculator.Models
{
    public enum ReportType
    {
        [Display(Name = "Total Annuities")]
        TotalAnnuities = 1,
        [Display(Name = "First Grant Payment")]
        FirstGrantPayment = 2
    }
}
