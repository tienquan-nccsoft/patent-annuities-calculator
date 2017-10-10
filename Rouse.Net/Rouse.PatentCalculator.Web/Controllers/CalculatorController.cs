using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Rouse.PatentCalculator.Web.Helpers;
using Rouse.PatentCalculator.Models;
using System.Web;
using Rouse.PatentCalculator.Web.Models;


namespace Rouse.PatentCalculator.Web.Controllers {
    public class CalculatorController : BaseController {
        // GET: Calculator
        public ActionResult Index() {
            var model = new CalculationRequest();
            SetViewBagData();
            model.CountryCode = "ID";
            model.CurrencyCode = "IDR";
            model.ReportTypeId = ReportType.TotalAnnuities;
            return View(model);
        }


        [HttpPost]
        public ActionResult Index(CalculationRequest request, bool isBack = false) {
            ViewBag.Request = request;
            if (isBack) {
                ViewBag.Currencies = uow.CurrencyRepository.GetAll();
                ViewBag.Countries = uow.GetCountryHasData();
                ViewBag.PatentFeeTypes = uow.PatentTypeRepository.GetAll();
                return View(request);
            }
            if (ModelState.IsValid) {
                var model = uow.CalculatePatentFees(request);
                if (model != null) {
                    ViewBag.DropdownSource = ComputeDropdownSource(model);
                    ViewBag.CreateCountry = uow.CountryRepository.GetByID(request.CountryCode).Name;
                    model.ApplicationPatentNo = HttpUtility.HtmlDecode(model.ApplicationPatentNo);
                    return View("Report", model);
                }
            }
            AddAlert(AlertType.DANGER,"There is no setup for Offical Fee/Agency Fee. Please go to Admin section to setup!");
            SetViewBagData();
            return View(request);
        }

        private void SetViewBagData() {
            ViewBag.Currencies = uow.CurrencyRepository.GetAll();
            var countries = uow.GetCountryHasData();
            ViewBag.Countries = countries;
            if (!countries.Any())
                AddAlert(AlertType.DANGER,"There is no setup for Offical Fee/Agency Fee. Please go to Admin section to setup!", AlertTag.DATA_EMPTY);
            ViewBag.PatentFeeTypes = uow.PatentTypeRepository.GetAll();
        }

        private IEnumerable<SelectListItem> ComputeDropdownSource(BreakdownAnnuitiesModel model) {
            var isFirstPayment = model.FillingDate.HasValue && model.GrantDate.HasValue;
            var result = new List<SelectListItem>() {
                new SelectListItem() {
                    Text = "Total Annuity",
                    Value = "TotalAnnuities"
                },
                new SelectListItem() {
                    Text = "First Grant Payment",
                    Value = "FirstGrantPayment",
                    Disabled = !isFirstPayment
                }
            };
            if (!isFirstPayment)
                return result;
            int payableYear = 0;
            int protectionYear = model.GrantDate.Value.Year - model.FillingDate.Value.Year;
            if (model.GrantDate.Value.Month > model.FillingDate.Value.Month
                || (model.GrantDate.Value.Month == model.FillingDate.Value.Month &&
                    model.GrantDate.Value.Day >= model.FillingDate.Value.Day))
                protectionYear++;
            payableYear = protectionYear + 1;

            if (payableYear > 0) {
                for (int i = 1; i <= payableYear; ++i)
                    result.Add(new SelectListItem() {
                        Text = $"Annuity year {i}",
                        Value = i.ToString()
                    });
            }
            return result;
        }


        [HttpPost]
        public PartialViewResult BodyResult(CalculationRequest request) {
            if (ModelState.IsValid) {
                ViewBag.Request = request;
                request.ApplicationPatentNo = HttpUtility.HtmlDecode(request.ApplicationPatentNo);
                var model = uow.CalculatePatentFees(request);
                if (model != null) {
                    ViewBag.Country = uow.CountryRepository.GetByID(request.CountryCode).Name;
                    model.ApplicationPatentNo = HttpUtility.HtmlDecode(model.ApplicationPatentNo);
                    return PartialView("BodyReport", model);
                }
            }
            return null;
        }
    }
}