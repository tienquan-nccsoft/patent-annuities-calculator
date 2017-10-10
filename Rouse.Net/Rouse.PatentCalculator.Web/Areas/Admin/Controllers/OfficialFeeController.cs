using Rouse.PatentCalculator.Web.Helpers;
using Rouse.PatentCalculator.Models;
using System.Web.Mvc;
using Rouse.PatentCalculator.Web.Models;

namespace Rouse.PatentCalculator.Web.Areas.Admin.Controllers
{
    
    public class OfficialFeeController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string countryCode, int? patentType)
        {
            ViewBag.Countries = uow.CountryRepository.GetAll();
            ViewBag.PatentTypes = uow.PatentTypeRepository.GetAll();

            if (patentType.HasValue)
            {
                ViewBag.Currencies = uow.CurrencyRepository.GetAll();
                var model = uow.OfficialFeeRepository.GetOfficialFeesAdmin(countryCode, patentType.Value);
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(AdminOfficialFeeModel model)
        {
            if (ModelState.IsValid)
            {
                //show save success message
                var result = uow.OfficialFeeRepository.UpdateOfficialFeesAdmin(model);
                if (result != null)
                    AddAlert(AlertType.SUCCESS, "Save successfully");
                else
                    AddAlert(AlertType.DANGER, "Save failed");
                return RedirectToAction("Index");
            }
            AddAlert(AlertType.DANGER, "Save falied.");
            ViewBag.Currencies = uow.CurrencyRepository.GetAll();
            ViewBag.PatentTypes = uow.PatentTypeRepository.GetAll();
            return View(model);
        }
    }
}