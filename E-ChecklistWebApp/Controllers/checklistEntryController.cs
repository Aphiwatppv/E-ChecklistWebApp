using MySqlUserEngineServices.Model;
using MySqlUserEngineServices.MySqlUserService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace E_ChecklistWebApp.Controllers
{
    public class checklistEntryController : Controller
    {
        private readonly IMySqlUserService _mySqlUserService;

        public checklistEntryController(IMySqlUserService mySqlUserService)
        {
            _mySqlUserService = mySqlUserService;
        }

        public async Task<ActionResult> checklistEntry(int ChecklistId, string ChecklistName, string ChecklistDescription, string Checklistdate , int ProcessId)
        {
            ViewBag.ChecklistId = ChecklistId; 
            ViewBag.ChecklistName = ChecklistName; 
            ViewBag.ChecklistDescription = ChecklistDescription; 
            ViewBag.Checklistdate = Checklistdate;
            ViewBag.ProcessId = ProcessId;
            var checklistItems = await _mySqlUserService.getChecklistItemById(ChecklistId);
            var checklistItemsWithConstants = new List<ChecklistItemWithConstants>();

            foreach (var item in checklistItems)
            {
                var constants = await _mySqlUserService.getConstantItemById(item.ItemId);
                checklistItemsWithConstants.Add(new ChecklistItemWithConstants
                {
                    ChecklistItem = item,
                    Constants = constants
                });
            }
            var result = await _mySqlUserService.getHistorical();

            var model = new ChecklistEntryViewModel
            {
                ChecklistName = ChecklistName,
                ChecklistDescription = ChecklistDescription,
                CreateDate = Checklistdate,
                ChecklistItemsWithConstants = checklistItemsWithConstants,
                echecklistHistoricalRecords = result

            };

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetEchecklistRecordsByHistoricalId(int historicalId)
        {
            var records = await _mySqlUserService.gethistoricalById(historicalId);
            return Json(records, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SaveChecklistEntry(int ChecklistId, string ChecklistName, string ChecklistDescription, string Checklistdate, int ProcessId, FormCollection form)
        {
            var user = (E_ChecklistWebApp.Models.EchecklistAuthenticationWithoutHash)Session["User"];
            var createBy = user.IdAuthen;

            // Create a new historical record
            var historicalParams = new EchecklistCreateHistoricalRecordParams
            {
                ProcessId = ProcessId,
                ChecklistId = ChecklistId,
                CreateBy = createBy
            };

            int historicalId = await _mySqlUserService.CreateHistoricalId(historicalParams);

            // Save each checklist item value
            foreach (var key in form.AllKeys)
            {
                if (key.StartsWith("item-"))
                {
                    int itemId = int.Parse(key.Split('-')[1]);
                    string value = form[key];

                    var recordParams = new EChecklistRecordChecklistValueParams
                    {
                        ChecklistId = ChecklistId,
                        HistoricalId = historicalId,
                        ItemId = itemId,
                        Value = value,
                        AddingBy = createBy
                    };

                    await _mySqlUserService.SaveRecordAsync(recordParams);
                }
            }
            return RedirectToAction("checklistEntry", new { ChecklistId, ChecklistName, ChecklistDescription, Checklistdate, ProcessId });
   
        }

    }

    public class ChecklistEntryViewModel
    {
        public string ChecklistName { get; set; }
        public string ChecklistDescription { get; set; }
        public string CreateDate { get; set; }
        public IEnumerable<ChecklistItemWithConstants> ChecklistItemsWithConstants { get; set; }
        public IEnumerable<EchecklistHistoricalRecord> echecklistHistoricalRecords { get; set; }
    }

    public class ChecklistItemWithConstants
    {
        public EchecklistChecklistItem ChecklistItem { get; set; }
        public IEnumerable<EChecklistItemConstant> Constants { get; set; }
    }



}