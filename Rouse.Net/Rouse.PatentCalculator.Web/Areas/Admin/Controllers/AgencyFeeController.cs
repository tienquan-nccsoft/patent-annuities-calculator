using Rouse.PatentCalculator.Web.Helpers;
using Rouse.PatentCalculator.Models;
using System.Web.Mvc;
using Rouse.PatentCalculator.Web.Models;

namespace Rouse.PatentCalculator.Web.Areas.Admin.Controllers
{
    public class AgencyFeeController : BaseController
    {
        // GET: Admin/Fees
        [HttpGet]
        public ActionResult Index(string countryCode, int? patentType, string currencyCode)
        {
            ViewBag.Countries = uow.CountryRepository.GetAll();
            ViewBag.PatentTypes = uow.PatentTypeRepository.GetAll();
            ViewBag.Currencies = uow.CurrencyRepository.GetAll();
            if (patentType.HasValue) {
                var defaultCurrencyCode = string.IsNullOrEmpty(currencyCode)? uow.CountryRepository.GetByID(countryCode).DefaultCurrency : currencyCode;
                var model = uow.AgencyFeeRepository.GetAgencyFeesAdmin(countryCode, patentType.Value, defaultCurrencyCode);
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(AdminAgencyFeeModel model)
        {
            if (ModelState.IsValid)
            {
                //show save success message
                var result = uow.AgencyFeeRepository.UpdateAgencyFeesAdmin(model);
                if (result != null)
                    AddAlert(AlertType.SUCCESS, "Save successfully");
                else
                    AddAlert(AlertType.DANGER, "Save failed");
                return RedirectToAction("Index");
            }
            AddAlert(AlertType.DANGER, "Save failed");

            ViewBag.Countries = uow.CountryRepository.GetAll();
            ViewBag.PatentTypes = uow.PatentTypeRepository.GetAll();
            ViewBag.Currencies = uow.CurrencyRepository.GetAll();
            return View(model);
        }
    }
}