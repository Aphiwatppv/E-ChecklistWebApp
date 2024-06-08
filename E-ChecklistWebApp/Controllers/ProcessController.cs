using MySqlUserEngineServices.MySqlUserService;
using MySqlUserEngineServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Threading.Tasks;

namespace E_ChecklistWebApp.Controllers
{
    public class ProcessController : Controller
    {

        public IEnumerable<EChecklistAllowingProcessDetail> eChecklistAllowingProcessDetail { get;set; }
 
        IMySqlUserService _mySqlUserService;
        public ProcessController(IMySqlUserService mySqlUserService)
        {
            _mySqlUserService = mySqlUserService;
        }

        

        public async Task<ActionResult> Process()
        {

            var user = (E_ChecklistWebApp.Models.EchecklistAuthenticationWithoutHash)Session["User"];

            if (user == null) 
            {
                return RedirectToAction("Index", "Home");
            }

            eChecklistAllowingProcessDetail = await _mySqlUserService.getAllowingProcessById(user.IdAuthen);
        
            return View(eChecklistAllowingProcessDetail);
        }
    }
}