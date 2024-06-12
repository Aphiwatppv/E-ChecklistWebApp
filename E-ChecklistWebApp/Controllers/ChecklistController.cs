using MySqlUserEngineServices.Model;
using MySqlUserEngineServices.MySqlUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace E_ChecklistWebApp.Controllers
{
    public class ChecklistController : Controller
    {
        IMySqlUserService _mySqlUserService;
        public ChecklistController(IMySqlUserService mySqlUserService)
        {
            _mySqlUserService = mySqlUserService;
        }


        public async Task<ActionResult> Checklist(int processId , string processName)
        {
            var checklists = await _mySqlUserService.getChecklistById(processId);

            ChecklistModel checklistModel = new ChecklistModel
            {
                eChecklistChecklistDetails = checklists,
                processId = processId,
                processName = processName
            };

            return View(checklistModel);
        }


    }

    public class ChecklistModel
    {
        public IEnumerable<EChecklistChecklistDetail> eChecklistChecklistDetails { get; set; }
        public int processId { get; set; }
        public string processName { get; set; }
    } 
}