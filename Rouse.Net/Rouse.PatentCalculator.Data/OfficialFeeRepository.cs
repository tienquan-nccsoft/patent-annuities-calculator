using Rouse.PatentCalculator.Data.Contracts;
using Rouse.PatentCalculator.DomainModels;
using Rouse.PatentCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rouse.PatentCalculator.Data
{
    public class OfficialFeeRepository : Repository<OfficialFee>, IOfficialFeeRepository
    {
        public OfficialFeeRepository(PatCalculatorEntities context) : base(context)
        {
        }

        public IQueryable<OfficialFee> GetOfficialFees(string countryCode, int patentType)
        {
            return context.OfficialFees.Where(a => a.CountryCode == countryCode && a.PatentTypeId == patentType);
        }

        public IQueryable<OfficialFee> GetOfficialFees(string countryCode, int patentType, int year)
        {
            return context.OfficialFees.Where(a => a.CountryCode == countryCode && a.PatentTypeId == patentType && (year == 0 || a.Year <= year));
        }

        public IQueryable<Country> GetCountriesHasData() {
            return from agencyFee in context.AgencyFees
                group agencyFee by agencyFee.Country
                into g
                select g.Key;
        }

        public AdminOfficialFeeModel GetOfficialFeesAdmin(string countryCode, int patentTypeId)
        {
            var result = new AdminOfficialFeeModel();
            var country = context.Countries.Find(countryCode);
            var patentType = context.PatentFeeTypes.Find(patentTypeId);
            result.CountryCode = country.Code;
            result.CountryName = country.Name;
            result.CurrencyCode = country.DefaultCurrency;
            result.PatentTypeId = patentType.ID;
            result.PatentTypeName = patentType.Name;
            result.PatentTypeYears = patentType.Years;
            
            var officialFees = GetOfficialFees(countryCode, patentTypeId).OrderBy(p => p.Year).ToList();
            result.ValidFrom = officialFees.Count != 0 ? officialFees.FirstOrDefault().ValidFrom : DateTime.Now;
            if (officialFees != null && officialFees.Count > 0)
            {
                result.OfficialFees = officialFees.Select(p => new AdminOfficialFee()
                {
                    Year = p.Year,
                    BasicFee = p.BasicFee,
                    ClaimFee = p.ClaimFee,                    
                    ValidFrom = p.ValidFrom
                }).ToList();
            }
            else
            {
                result.OfficialFees = new List<AdminOfficialFee>(new AdminOfficialFee[result.PatentTypeYears]);
            }
            return result;
        }

        public AdminOfficialFeeModel UpdateOfficialFeesAdmin(AdminOfficialFeeModel data)
        {
            var newOfficialFees = data.OfficialFees.Select(p => new OfficialFee()
            {
                CountryCode = data.CountryCode,
                CurrencyCode = data.CurrencyCode,
                PatentTypeId = (byte)data.PatentTypeId,
                BasicFee = p.BasicFee,
                ClaimFee = p.ClaimFee,                
                Year = (byte)p.Year,
                ValidFrom = data.ValidFrom
            });

            var oldOfficialFees = GetOfficialFees(data.CountryCode, data.PatentTypeId);
            if (oldOfficialFees != null && oldOfficialFees.Count() > 0)
                dbSet.RemoveRange(oldOfficialFees);
            dbSet.AddRange(newOfficialFees);
            if (context.SaveChanges() > 0)
                return GetOfficialFeesAdmin(data.CountryCode, data.PatentTypeId);
            return null;
        }
    }
}
