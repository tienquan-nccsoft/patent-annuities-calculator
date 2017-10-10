using Rouse.PatentCalculator.Web.Helpers;
using Rouse.PatentCalculator.Models;
using System.Web.Mvc;
using Rouse.PatentCalculator.Web.Models;

namespace Rouse.PatentCalculator.Web.Areas.Admin.Controllers
{
    public class FeesController : BaseController
    {
        // GET: Admin/Fees
        [HttpGet]
        public ActionResult Index(string countryCode, int? patentType)
        {
            ViewBag.Countries = uow.CountryRepository.GetAll();
            ViewBag.PatentTypes = uow.PatentTypeRepository.GetAll();

            //if (patentType.HasValue)
            //{
            //    var model = uow.GetPatentFeesAdmin(countryCode, patentType.Value);
            //    return View(model);
            //}
            return View();
        }

        [HttpPost]
        public ActionResult Index(AdminPatentFeeModel model)
        {
            if (ModelState.IsValid)
            {
                //show save success message
                //var result  = uow.UpdatePatentFeesAdmin(model);
                //TempData["Success"] = result != null;
                //TempData["Message"] = result != null ? "Save successfully" : "Save failed";
                return RedirectToAction("Index");
            }
            AddAlert(AlertType.DANGER, "Save falied.");

            ViewBag.Countries = uow.CountryRepository.GetAll();
            ViewBag.PatentTypes = uow.PatentTypeRepository.GetAll();
            return View(model);
        }
    }
}