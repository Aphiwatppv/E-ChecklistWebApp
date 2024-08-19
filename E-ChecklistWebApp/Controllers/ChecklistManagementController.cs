using MySqlUserEngineServices.Model;
using MySqlUserEngineServices.MySqlUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace E_ChecklistWebApp.Controllers
{
    public class ChecklistManagementController : Controller
    {
        private readonly IMySqlUserService _mySqlUserService;

        public ChecklistManagementController(IMySqlUserService mySqlUserService)
        {
            _mySqlUserService = mySqlUserService;
        }

        public async Task<ActionResult> ChecklistManagement()
        {
            var processes = await _mySqlUserService.GetActiveProcessDetails();
            var model = new ChecklistManagementModel
            {
                echecklistProcesses = processes,
                Checklist = new List<EChecklistChecklistDetail>()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateChecklist(ChecklistManagementModel model)
        {
            if (ModelState.IsValid)
            {
                var newChecklist = new EChecklistCreateChecklist
                {
                    ChecklistName = model.ChecklistName,
                    ProcessId = model.SelectedProcessId,
                    ActiveFlag = model.ActiveFlag ? "Active" : "Inactive",
                    Period = model.Period,
                    Description = model.Description,
                    CreateBy = model.CreatedBy
                };
                await _mySqlUserService.CreateNewChecklist(newChecklist);
                return RedirectToAction("ChecklistManagement");
            }

            model.echecklistProcesses = await _mySqlUserService.GetActiveProcessDetails();
            return View("ChecklistManagement", model);
        }

        [HttpGet]
        public async Task<JsonResult> GetChecklists(int processId)
        {
            var checklists = await _mySqlUserService.getChecklistById(processId);
            return Json(checklists, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetItems(int checklistId)
        {
            var items = await _mySqlUserService.getChecklistItemById(checklistId);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> AddItemToChecklist(int indexItem, int checklistSelectItem, string itemName, string unit, string itemDescription, int itemCreateBy)
        {
            if (ModelState.IsValid)
            {
                var newItem = new EChecklistAddItemChecklist
                {
                    IndexItem = indexItem,
                    IdChecklist = checklistSelectItem,
                    Description = itemDescription,
                    CreateBy = itemCreateBy,
                    ItemName = itemName,
                    Unit = unit,
                };
                await _mySqlUserService.AddItemToChecklist(newItem);
                return RedirectToAction("ChecklistManagement");
            }

            var model = new ChecklistManagementModel
            {
                echecklistProcesses = await _mySqlUserService.GetActiveProcessDetails(),
                Checklist = new List<EChecklistChecklistDetail>()
            };

            return View("ChecklistManagement", model);
        }


        [HttpPost]
        public async Task<ActionResult> AddConstantToItem(int itemId, string description, string constantValue)
        {
            if (ModelState.IsValid)
            {
                var user = (E_ChecklistWebApp.Models.EchecklistAuthenticationWithoutHash)Session["User"];
                var newConstant = new EChecklistItemConstant
                {
                    ItemId = itemId,
                    Description = description,
                    ConstantValue = constantValue,
                    CreateBy = user.IdAuthen
                };
                await _mySqlUserService.AddConstantToItem(newConstant);
                return RedirectToAction("ChecklistManagement");
            }

            return View("ChecklistManagement");
        }





        //[HttpPost]
        //public async Task<JsonResult> ChangeItemActiveFlag(int itemId, bool isActive)
        //{
        //    var result = await _mySqlUserService.ChangeItemActiveFlag(itemId, isActive);
        //    return Json(result);
        //}
    }

    public class ChecklistManagementModel
    {
        public int SelectedProcessId { get; set; }
        public string ChecklistName { get; set; }
        public bool ActiveFlag { get; set; }
        public string Period { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public IEnumerable<EchecklistProcess> echecklistProcesses { get; set; }
        public IEnumerable<EChecklistChecklistDetail> Checklist { get; set; }
    }
}