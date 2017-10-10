using Rouse.PatentCalculator.DomainModels;
using Rouse.PatentCalculator.Models;
using System.Linq;

namespace Rouse.PatentCalculator.Data.Contracts
{
    public interface IAgencyFeeRepository: IRepository<AgencyFee> {
        IQueryable<AgencyFee> GetAgencyFees(string countryCode, int patentType);
        IQueryable<AgencyFee> GetAgencyFees(string countryCode, int patentType, string currencyCode);
        IQueryable<AgencyFee> GetAgencyFees(string countryCode, int patentType, int year);
        IQueryable<AgencyFee> GetAgencyFees(string countryCode, int patentType, string currencyCode, int year);
        bool HasFeeWithCurrencyCode(string countryCode, int patentType, string currencyCode);
        IQueryable<Country> GetCountriesHasData();
        AdminAgencyFeeModel GetAgencyFeesAdmin(string countryCode, int patentTypeId, string currencyCode= "");
        AdminAgencyFeeModel UpdateAgencyFeesAdmin(AdminAgencyFeeModel data);
    }
}
