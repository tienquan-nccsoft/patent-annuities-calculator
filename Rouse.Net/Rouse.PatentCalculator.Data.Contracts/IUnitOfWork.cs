using System.Linq;
using Rouse.PatentCalculator.DomainModels;
using Rouse.PatentCalculator.Models;

namespace Rouse.PatentCalculator.Data.Contracts
{
    public interface IUnitOfWork
    {
        IRepository<Country> CountryRepository { get; }
        IRepository<PatentFeeType> PatentTypeRepository { get; }
        IRepository<Tax> TaxRepository { get; }
        IRepository<Currency> CurrencyRepository { get; }
        IAgencyFeeRepository AgencyFeeRepository { get; }
        IOfficialFeeRepository OfficialFeeRepository { get; }

        BreakdownAnnuitiesModel CalculatePatentFees(CalculationRequest request);
        IQueryable<Country> GetCountryHasData();
    }
}
