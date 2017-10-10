using Rouse.PatentCalculator.Data.Contracts;
using Rouse.PatentCalculator.DomainModels;
using Rouse.PatentCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rouse.PatentCalculator.Data {
    public class AgencyFeeRepository : Repository<AgencyFee>, IAgencyFeeRepository {
        public AgencyFeeRepository(PatCalculatorEntities context) : base(context) { }

        public IQueryable<AgencyFee> GetAgencyFees(string countryCode, int patentType) {
            return context.AgencyFees.Where(a => a.CountryCode == countryCode && a.PatentTypeId == patentType);
        }
        public IQueryable<AgencyFee> GetAgencyFees(string countryCode, int patentType, string currencyCode) {
            return context.AgencyFees.Where(a => a.CountryCode == countryCode && a.PatentTypeId == patentType && a.CurrencyCode == currencyCode);
        }

        public IQueryable<AgencyFee> GetAgencyFees(string countryCode, int patentType, int year) {
            return context.AgencyFees.Where(a =>
                a.CountryCode == countryCode && a.PatentTypeId == patentType && (year == 0 || a.Year <= year));
        }
        public IQueryable<AgencyFee> GetAgencyFees(string countryCode, int patentType, string currencyCode, int year) {
            return context.AgencyFees.Where(a =>
                a.CountryCode == countryCode && a.CurrencyCode == currencyCode && a.PatentTypeId == patentType && (year == 0 || a.Year <= year));
        }

        public bool HasFeeWithCurrencyCode(string countryCode, int patentType, string currencyCode) {
            return context.AgencyFees.Any(a =>
                a.CountryCode == countryCode && a.CurrencyCode == currencyCode && a.PatentTypeId == patentType);
        }

        public IQueryable<Country> GetCountriesHasData() {
            return from agencyFee in context.AgencyFees
                group agencyFee by agencyFee.Country
                into g
                select g.Key;
        }


        public AdminAgencyFeeModel GetAgencyFeesAdmin(string countryCode, int patentTypeId, string currencyCode = "") {
            var result = new AdminAgencyFeeModel();
            var country = context.Countries.Find(countryCode);
            var patentType = context.PatentFeeTypes.Find(patentTypeId);
            var currencyCodeFind = currencyCode == "" ? country.DefaultCurrency : context.Currencies.Find(currencyCode)?.CurrencyCode;
            result.CountryCode = country.Code;
            result.CountryName = country.Name;
            result.CurrencyCode = currencyCodeFind;
            result.PatentTypeId = patentType.ID;
            result.PatentTypeName = patentType.Name;
            result.PatentTypeYears = patentType.Years;
            var agenFees = GetAgencyFees(countryCode, patentTypeId).OrderBy(p => p.Year).ToList();
            result.ValidFrom = agenFees.Count != 0 ? agenFees.FirstOrDefault().ValidFrom : DateTime.Now;
            if (agenFees != null && agenFees.Count > 0) {
                result.AgencyFees = agenFees.Select(p => new AdminAgencyFee() {
                    Year = p.Year,
                    Fee = p.Fee,
                    ValidFrom = p.ValidFrom
                }).ToList();
            } else {
                result.AgencyFees = new List<AdminAgencyFee>(new AdminAgencyFee[result.PatentTypeYears]);
            }
            return result;
        }

        public AdminAgencyFeeModel UpdateAgencyFeesAdmin(AdminAgencyFeeModel data) {
            var newPatentFees = data.AgencyFees.Select(p => new AgencyFee() {
                CountryCode = data.CountryCode,
                CurrencyCode = data.CurrencyCode,
                PatentTypeId = (byte) data.PatentTypeId,
                Year = (byte) p.Year,
                ValidFrom = data.ValidFrom,
                Fee = p.Fee
            });

            var agencyFees = GetAgencyFees(data.CountryCode, data.PatentTypeId, data.CurrencyCode);
            if (agencyFees != null && agencyFees.Any())
                dbSet.RemoveRange(agencyFees);
            dbSet.AddRange(newPatentFees);
            if (context.SaveChanges() > 0)
                return GetAgencyFeesAdmin(data.CountryCode, data.PatentTypeId, data.CurrencyCode);
            return null;
        }
    }
}