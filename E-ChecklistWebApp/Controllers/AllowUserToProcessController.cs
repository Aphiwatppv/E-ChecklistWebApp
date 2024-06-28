using MySqlUserEngineServices.Model;
using MySqlUserEngineServices.MySqlUserService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace E_ChecklistWebApp.Controllers
{
    public class AllowUserToProcessController : Controller
    {
        private readonly IMySqlUserService _mySqlUserService;

        public AllowUserToProcessController(IMySqlUserService mySqlUserService)
        {
            _mySqlUserService = mySqlUserService;
        }

        public async Task<ActionResult> AllowUserToProcess()
        {
            var eChecklistAuthenDetails = await _mySqlUserService.getEN();
            var echecklistProcesses = await _mySqlUserService.GetActiveProcessDetails();
            var allowUserToProcessModels = new AllowUserToProcessModels
            {
                eChecklistAuthenDetails = eChecklistAuthenDetails,
                echecklistProcesses = echecklistProcesses,
                echecklistAllowingProcessDetails = null
            };

            return View(allowUserToProcessModels);
        }

        [HttpPost]
        public async Task<ActionResult> AddUserToProcess(int IdAuthen, int ProcessId)
        {
            var user = (E_ChecklistWebApp.Models.EchecklistAuthenticationWithoutHash)Session["User"];
            await _mySqlUserService.AddUserToProcess(IdAuthen, ProcessId, user.IdAuthen);

            return RedirectToAction("AllowUserToProcess");
        }

        [HttpPost]
        public async Task<ActionResult> getAllowingProcess(string EN)
        {
            var result = await _mySqlUserService.getAllowingProcess(EN);
            var eChecklistAuthenDetails = await _mySqlUserService.getEN();
            var echecklistProcesses = await _mySqlUserService.GetActiveProcessDetails();
            var allowUserToProcessModels = new AllowUserToProcessModels
            {
                eChecklistAuthenDetails = eChecklistAuthenDetails,
                echecklistProcesses = echecklistProcesses,
                echecklistAllowingProcessDetails = result
            };

            return View("AllowUserToProcess", allowUserToProcessModels);
        }
    }

    public class AllowUserToProcessModels
    {
        public IEnumerable<EChecklistAuthenDetails> eChecklistAuthenDetails { get; set; }
        public IEnumerable<EchecklistProcess> echecklistProcesses { get; set; }
        public IEnumerable<EchecklistAllowingProcess> echecklistAllowingProcessDetails { get; set; }
    }
}