using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.EntityClient;
using System.Configuration;
using Rouse.PatentCalculator.Data.Contracts;
using Rouse.PatentCalculator.Models;
using Rouse.PatentCalculator.DomainModels;

namespace Rouse.PatentCalculator.Data {
    public class PatUow : IUnitOfWork {
        private readonly PatCalculatorEntities context;

        public PatUow(string connectionStringName) {
            string connectionString = GetEFConnectionString(connectionStringName);
            context = new PatCalculatorEntities(connectionString);
            InitializeRepositories();
        }

        public PatUow() : this("PatConnectionString") { }

        #region Repositories

        public IRepository<Country> CountryRepository { get; private set; }

        public IRepository<Currency> CurrencyRepository { get; private set; }

        public IRepository<PatentFeeType> PatentTypeRepository { get; private set; }

        public IRepository<Tax> TaxRepository { get; private set; }

        public IAgencyFeeRepository AgencyFeeRepository { get; private set; }

        public IOfficialFeeRepository OfficialFeeRepository { get; private set; }

        #endregion

        private void InitializeRepositories() {
            OfficialFeeRepository = new OfficialFeeRepository(context);
            AgencyFeeRepository = new AgencyFeeRepository(context);
            PatentTypeRepository = new Repository<PatentFeeType>(context);
            CountryRepository = new Repository<Country>(context);
            CurrencyRepository = new Repository<Currency>(context);
            TaxRepository = new TaxRepository(context);
        }

        private string GetEFConnectionString(string connectionName) {
            var builder = new EntityConnectionStringBuilder();
            builder.Provider = "System.Data.SqlClient";
            builder.ProviderConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            builder.Metadata =
                "res://*/PatentCalculator.csdl|res://*/PatentCalculator.ssdl|res://*/PatentCalculator.msl";
            return builder.ToString();
        }

        public BreakdownAnnuitiesModel CalculatePatentFees(CalculationRequest request) {
            // Display value by report type "Total Calculation" or "First Payment Calculation"
            int payableYear = 0;
            if (request.ReportTypeId == ReportType.FirstGrantPayment && request.FilingDate.HasValue &&
                request.GrantDate.HasValue) {
                int protectionYear = request.GrantDate.Value.Year - request.FilingDate.Value.Year;
                if (request.GrantDate.Value.Month > request.FilingDate.Value.Month
                    || (request.GrantDate.Value.Month == request.FilingDate.Value.Month &&
                        request.GrantDate.Value.Day >= request.FilingDate.Value.Day))
                    protectionYear++;

                payableYear = protectionYear + 1;
            }

            var country = CountryRepository.GetByID(request.CountryCode);
            var patentFees = new List<Models.PatentFee>();
            var officialFees = OfficialFeeRepository
                .GetOfficialFees(request.CountryCode, request.PatentTypeId, payableYear).ToList();

            var hasAgencyFeeWithCurrency =
                AgencyFeeRepository.HasFeeWithCurrencyCode(request.CountryCode, request.PatentTypeId,
                    request.CurrencyCode);

            var agencyFees = hasAgencyFeeWithCurrency
                ? AgencyFeeRepository.GetAgencyFees(request.CountryCode, request.PatentTypeId, request.CurrencyCode, payableYear).ToList()
                : AgencyFeeRepository.GetAgencyFees(request.CountryCode, request.PatentTypeId, payableYear).ToList();

            string agencyCurrencyCode =
                agencyFees.Any() ? agencyFees.FirstOrDefault().CurrencyCode : country.DefaultCurrency;
            if (string.IsNullOrEmpty(agencyCurrencyCode))
                agencyCurrencyCode = request.CurrencyCode;

            int maxYear = Math.Max(officialFees.Any() ? officialFees.Max(o => o.Year) : 0,
                agencyFees.Any() ? agencyFees.Max(o => o.Year) : 0);
            if (maxYear > 0) {
                var expectedYear = request.ExpectedPayableYear;
                if (expectedYear > 0) {
                    var index = expectedYear - 1;
                    var patentFee = new Models.PatentFee();
                    patentFee.PatentTypeId = (byte) request.PatentTypeId;
                    patentFee.Year = (byte) (expectedYear);
                    patentFee.CountryCode = request.CountryCode;
                    patentFee.AgencyCurrencyCode = agencyCurrencyCode;
                    patentFee.BasicFee = index < officialFees.Count && officialFees[index] != null
                        ? officialFees[index].BasicFee
                        : 0;
                    patentFee.ClaimFee = index < officialFees.Count && officialFees[index] != null
                        ? officialFees[index].ClaimFee
                        : 0;
                    patentFee.AgencyFee = index < agencyFees.Count && agencyFees[index] != null
                        ? agencyFees[index].Fee
                        : 0;
                    patentFees.Add(patentFee);
                } else {
                    for (int i = 0; i < maxYear; i++) {
                        var patentFee = new Models.PatentFee();
                        patentFee.PatentTypeId = (byte) request.PatentTypeId;
                        patentFee.Year = (byte) (i + 1);
                        patentFee.CountryCode = request.CountryCode;
                        patentFee.AgencyCurrencyCode = agencyCurrencyCode;
                        patentFee.BasicFee = i < officialFees.Count && officialFees[i] != null
                            ? officialFees[i].BasicFee
                            : 0;
                        patentFee.ClaimFee = i < officialFees.Count && officialFees[i] != null
                            ? officialFees[i].ClaimFee
                            : 0;
                        patentFee.AgencyFee = i < agencyFees.Count && agencyFees[i] != null ? agencyFees[i].Fee : 0;
                        patentFees.Add(patentFee);
                    }
                }
            }

            if (patentFees != null && patentFees.Count > 0) {
                var result = new BreakdownAnnuitiesModel() {
                    ApplicationPatentNo = request.ApplicationPatentNo,
                    PatentType = ((PatentTypes) request.PatentTypeId).ToString(),
                    NumberOfClaim = request.NoOfClaim,
                    AnnuitiesData = new List<AnnuityModel>(),
                    FillingDate = request.FilingDate,
                    GrantDate = request.GrantDate,
                    PayableYear = payableYear,
                    AgencyFeeDiscountRate = request.AgencyFeeDiscount,
                    CreateCountry = CountryRepository.GetByID(request.CountryCode)?.Name
                };

                decimal rateForAgencyFee = 1;
                decimal rateForOfficialFee = 1;
                // Calculate by currency
                if (!string.IsNullOrEmpty(request.CurrencyCode)) {
                    result.DefaultCurrencyCode = country.DefaultCurrency;
                    result.CurrencyCode = request.CurrencyCode;
                    if (!hasAgencyFeeWithCurrency) {
                        var requestCurrency = CurrencyRepository.GetByID(request.CurrencyCode);
                        if (requestCurrency != null && requestCurrency.Rate.HasValue) {
                            var currency = CurrencyRepository.GetByID(agencyCurrencyCode);
                            if (currency.Rate.HasValue) {
                                rateForAgencyFee = requestCurrency.Rate.Value / currency.Rate.Value;
                            }
                        }
                    }
                    if (country.DefaultCurrency != request.CurrencyCode) {
                        var requestCurrency = CurrencyRepository.GetByID(request.CurrencyCode);
                        if (requestCurrency != null && requestCurrency.Rate.HasValue) {
                            var currency = CurrencyRepository.GetByID(country.DefaultCurrency);
                            if (currency.Rate.HasValue) {
                                rateForOfficialFee = requestCurrency.Rate.Value / currency.Rate.Value;
                                result.FXRate = rateForOfficialFee;
                            }
                        }
                    }
                }

                result.AnnuitiesData = patentFees.Select(p => new AnnuityModel() {
                    AnnuityNo = p.Year,
                    NoOfClaim = request.NoOfClaim,
                    BasicFee = p.BasicFee,
                    ClaimFee = p.ClaimFee * request.NoOfClaim,
                    AgencyFee = p.AgencyFee * rateForAgencyFee,
                    OfficialFee = (p.BasicFee + p.ClaimFee * request.NoOfClaim)
                }).ToList();

                result.SubTotalOfficialFees = result.AnnuitiesData.Sum(a => a.OfficialFee) * rateForOfficialFee;
                result.SubTotalAgencyFees = result.AnnuitiesData.Sum(a => a.AgencyFee);
                result.AgencyFeeDiscount = result.SubTotalAgencyFees * request.AgencyFeeDiscount / 100;
                result.TotalFees = result.SubTotalAgencyFees + result.SubTotalOfficialFees - result.AgencyFeeDiscount;

                var tax = context.Taxes.FirstOrDefault(t => t.CountryCode == request.CountryCode && !t.Deleted);
                if (tax != null) {
                    result.TaxRate = tax.Percentage;
                    result.Taxes = result.TotalFees * tax.Percentage / 100;
                }
                result.TotalCostEstimation = result.TotalFees + result.Taxes;


                return result;
            }
            return null;
        }

        public IQueryable<Country> GetCountryHasData() {
            return from agencyCountries in AgencyFeeRepository.GetCountriesHasData()
                join officalCountries in OfficialFeeRepository.GetCountriesHasData() on agencyCountries.Code equals
                    officalCountries.Code
                select agencyCountries;
        }
    }
}