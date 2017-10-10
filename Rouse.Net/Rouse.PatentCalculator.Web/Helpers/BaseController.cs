using Rouse.PatentCalculator.Data;
using Rouse.PatentCalculator.Data.Contracts;
using System.Web.Mvc;
using Rouse.PatentCalculator.Web.Models;

namespace Rouse.PatentCalculator.Web.Helpers
{
   
    
    public class BaseController : Controller
    {
        protected IUnitOfWork uow;

        public BaseController()
        {
            uow = new PatUow();
        }

        protected void AddAlert(string type, string message, string tag = "")
        {
            TempData[Alert.TEMP_ALERT_KEY] = new Alert() { AlertType = type, Message = message , Tag = tag};
        }
    }
}