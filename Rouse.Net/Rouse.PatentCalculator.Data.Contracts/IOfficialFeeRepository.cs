using Rouse.PatentCalculator.DomainModels;
using Rouse.PatentCalculator.Models;
using System.Linq;

namespace Rouse.PatentCalculator.Data.Contracts
{
    public interface IOfficialFeeRepository: IRepository<OfficialFee>
    {
        IQueryable<OfficialFee> GetOfficialFees(string countryCode, int patentType);
        IQueryable<OfficialFee> GetOfficialFees(string countryCode, int patentType, int year);
        IQueryable<Country> GetCountriesHasData();
        AdminOfficialFeeModel GetOfficialFeesAdmin(string countryCode, int patentTypeId);
        AdminOfficialFeeModel UpdateOfficialFeesAdmin(AdminOfficialFeeModel data);
    }
}
