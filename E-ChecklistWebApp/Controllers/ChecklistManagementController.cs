using E_ChecklistWebApp.AuthApi;
using E_ChecklistWebApp.MachineModeApi;
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
        private readonly IMachineModelAPI _machineModelAPI;

        public ChecklistManagementController(IMySqlUserService mySqlUserService, IMachineModelAPI machineModelAPI)
        {
            _mySqlUserService = mySqlUserService;
            _machineModelAPI = machineModelAPI; 
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
            string ActiveFlag = string.Empty;
            if (model.ActiveFlag)
            {
                ActiveFlag = "Active";
            }
            else
            {
                ActiveFlag = "Inactive";
            }

            if (ModelState.IsValid)
            {
                var user = (E_ChecklistWebApp.Models.EchecklistAuthenticationWithoutHash)Session["User"];
                var newChecklist = new EChecklistCreateChecklist
                {
                    ChecklistName = model.ChecklistName,
                    ProcessId = model.SelectedProcessId,
                    ActiveFlag = model.ActiveFlag ? "Active" : "Inactive",
                    Period = model.Period,
                    Description = model.Description,
                    CreateBy = user.IdAuthen
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

        [HttpPost]
        public async Task<JsonResult> ChangeItemActiveFlag(int itemId)
        {
             await _mySqlUserService.ToggleActiveItemChecklist(itemId);
             return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> SwapItemIndex(int currentitemId, int newIndex, int ChecklistId , int currentIndex) 
        {
            await _mySqlUserService.SwapNewIndex(currentitemId, newIndex ,ChecklistId, currentIndex);  
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<JsonResult> GetConstantsByItemId(int itemId)
        {
            var constants = await _mySqlUserService.getConstantItemById(itemId);
            return Json(constants, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteConstant(int constantId)
        {
           await _mySqlUserService.DeleteConstant(constantId);
           return Json(new { success = true });
        }

        [HttpGet]
        public async Task<JsonResult> GetMachineModelByChecklistId(int checklistId)
        {
            var machines = await _mySqlUserService.GetMachineModelByChecklistId(checklistId);
            return Json(machines, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetEntireModelMachine()
        {
            var result = await _machineModelAPI.GetModelMachines();

           
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> AddMachineToChecklist(int checklistId, int pmisModelId, string pmisModelDescription)
        {
            var newMachine = new EChecklistMachineInformation
            {
                ChecklistId = checklistId,
                pmis_model_id = pmisModelId,
                pmis_model_description = pmisModelDescription,
                ActiveFlag = "Active"
            };
            await _mySqlUserService.AddMachineToChecklist(newMachine);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> ToggleMachineStatus(int machineId)
        {
            await _mySqlUserService.ToggleMachineStatus(machineId);
            return Json(new { success = true });
        }
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

