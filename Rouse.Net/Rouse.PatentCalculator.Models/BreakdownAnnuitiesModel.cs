using System;
using System.Collections.Generic;

namespace Rouse.PatentCalculator.Models
{
    public class AnnuityModel
    {
        public int AnnuityNo { get; set; }
        public int NoOfClaim { get; set; }
        public decimal BasicFee { get; set; }
        public decimal ClaimFee { get; set; }
        public decimal OfficialFee { get; set; }
        public decimal AgencyFee { get; set; }
    }

    public class BreakdownAnnuitiesModel
    {
        public string ApplicationPatentNo { get; set; }
        public string PatentType { get; set; }
        public int NumberOfClaim { get; set; }
        public DateTime? FillingDate { get; set; }
        public DateTime? GrantDate { get; set; }
        public string CurrencyCode { get; set; }
        public string CreateCountry { get; set; }
        public List<AnnuityModel> AnnuitiesData { get; set; }
        public decimal SubTotalOfficialFees { get; set; }
        public decimal SubTotalAgencyFees { get; set; }
        public decimal AgencyFeeDiscountRate { get; set; }
        public decimal AgencyFeeDiscount { get; set; }
        public decimal TotalFees { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalCostEstimation { get; set; }
        public int PayableYear { get; set; }
        public decimal FXRate { get; set; }
        public string DefaultCurrencyCode { get; set; }
    }
}
