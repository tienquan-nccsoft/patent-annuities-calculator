using Rouse.PatentCalculator.Web.Helpers;
using Rouse.PatentCalculator.DomainModels;
using Rouse.PatentCalculator.Models;
using System.Linq;
using System.Web.Mvc;

namespace Rouse.PatentCalculator.Web.Areas.Admin.Controllers
{
    
    public class TaxController : BaseController
    {
        // GET: Admin/TAx
        public ActionResult Index()
        {
            var model = uow.TaxRepository.GetAll().Where(s => !s.Deleted).Select(s => new TaxModel
            {
                Id = s.ID,
                CountryCode = s.CountryCode,
                CountryName = s.Country.Name,
                Type = s.Type,
                Percentage = s.Percentage,
                DefaultCurrency = s.Country.DefaultCurrency
            });
            return View(model);
        }

        public ActionResult CreateOrEdit(int? id)
        {
            ViewBag.Countries = uow.CountryRepository.GetAll();
            var model = new TaxModel();
            ViewBag.IsEdit = bool.FalseString.ToLower(); ;
            if (id.HasValue)
            {
                ViewBag.IsEdit = bool.TrueString.ToLower();
                model = uow.TaxRepository.GetAll().Where(s => !s.Deleted && s.ID == id).Select(s => new TaxModel
                {
                    Id = s.ID,
                    CountryCode = s.CountryCode,
                    CountryName = s.Country.Name,
                    Type = s.Type,
                    Percentage = s.Percentage,
                    DefaultCurrency = s.Country.DefaultCurrency
                }).FirstOrDefault();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateOrEdit(TaxModel model)
        {
            try
            {
                ViewBag.Countries = uow.CountryRepository.GetAll();
                Tax result = null;
                if (ModelState.IsValid)
                {
                    if (model.Id != 0)
                    {
                        result = uow.TaxRepository.Update(new Tax
                        {
                            ID = model.Id,
                            CountryCode = model.CountryCode,
                            Type = model.Type,
                            Percentage = model.Percentage,
                            Deleted = false
                        });
                    }
                    else
                    {
                        result = uow.TaxRepository.Add(new Tax
                        {
                            CountryCode = model.CountryCode,
                            Type = model.Type,
                            Percentage = model.Percentage,
                            Deleted = false
                        });
                    }
                    if (result != null)
                    {
                        TempData["message"] = "Add new tax successfull.";
                        return RedirectToAction("Index");
                    }
                    ModelState.AddModelError(string.Empty, "Duplicate Tax");
                }
               
                return View(model);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                uow.TaxRepository.SoftDelete(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
