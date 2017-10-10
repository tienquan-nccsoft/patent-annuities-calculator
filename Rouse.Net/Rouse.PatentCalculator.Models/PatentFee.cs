namespace Rouse.PatentCalculator.Models
{
    public class PatentFee
    {
        public string CountryCode { get; set; }
        public byte PatentTypeId { get; set; }
        public byte Year { get; set; }
        public string AgencyCurrencyCode { get; set; }
        public decimal BasicFee { get; set; }
        public decimal ClaimFee { get; set; }
        public decimal AgencyFee { get; set; }
    }
}
