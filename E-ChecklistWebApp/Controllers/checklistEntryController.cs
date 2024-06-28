using MySqlUserEngineServices.Model;
using MySqlUserEngineServices.MySqlUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace E_ChecklistWebApp.Controllers
{
    public class checklistEntryController : Controller
    {
        private readonly IMySqlUserService _mySqlUserService;

        private int checklistId;
        private string checklistName;
        private string checklistDescription;
        private string checklistdate;
        private int processId;

        public checklistEntryController(IMySqlUserService mySqlUserService)
        {
            _mySqlUserService = mySqlUserService;
        }

        public async Task<ActionResult> checklistEntry(int ChecklistId, string ChecklistName, string ChecklistDescription, string Checklistdate, int ProcessId)
        {

            checklistId = ChecklistId;
            checklistName = ChecklistName;
            checklistDescription = ChecklistDescription;
            checklistdate = Checklistdate;
            processId = ProcessId;


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
            var result = await _mySqlUserService.getHistorical(ChecklistId);

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

        [HttpGet]
        public async Task<ActionResult> GetchecklistRecordHistoricalbyfilter(int ChecklistId, string ChecklistName, string ChecklistDescription, string Checklistdate, int ProcessId, string MachineName, string LotNumber, DateTime? startDate, DateTime? endDate)
        {
            try
            {
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

                var filterParameters = new EchecklistHistoricalFilter
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    LotNumber = LotNumber,
                    MachineName = MachineName
                };

                var historicalRecord = await _mySqlUserService.getHistoricalRecordFilter(filterParameters, checklistId) ?? new List<EchecklistHistoricalRecord>();

                var model = new ChecklistEntryViewModel
                {
                    ChecklistName = ChecklistName,
                    ChecklistDescription = ChecklistDescription,
                    CreateDate = Checklistdate,
                    ChecklistItemsWithConstants = checklistItemsWithConstants,
                    echecklistHistoricalRecords = historicalRecord.Any() ? historicalRecord : await _mySqlUserService.getHistorical(ChecklistId)
                };

                return Json(model.echecklistHistoricalRecords, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception (use your preferred logging framework)
                System.Diagnostics.Debug.WriteLine("Error in GetchecklistRecordHistoricalbyfilter: " + ex.Message);
                return new HttpStatusCodeResult(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveChecklistEntry(int ChecklistId, string ChecklistName, string ChecklistDescription, string Checklistdate, int ProcessId, string MachineNo, string LotNo, FormCollection form)
        {
            var user = (E_ChecklistWebApp.Models.EchecklistAuthenticationWithoutHash)Session["User"];
            var createBy = user.IdAuthen;

            // Create a new historical record
            var historicalParams = new EchecklistCreateHistoricalRecordParams
            {
                ProcessId = ProcessId,
                ChecklistId = ChecklistId,
                CreateBy = createBy,
                MachineName = MachineNo,
                LotNumber = LotNo


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

        [HttpPost]
        public async Task<ActionResult> EditEchecklistRecord(string editValue, DateTime editAddingDate, string editMachineName, string editLotNo, int historicalId, int editRecordId)
        {
            try
            {


                int ChecklistId = checklistId;
                string ChecklistName = checklistName;
                string ChecklistDescription = checklistDescription;
                string Checklistdate = checklistdate;
                int ProcessId = processId;
                var input = new EchecklistRecordDetailsInput
                {
                    AddingDate = editAddingDate,
                    MachineName = editMachineName,
                    LotNumber = editLotNo,
                    Value = editValue,
                    HistoricalId = historicalId,
                    RecordId = editRecordId
                };

                await _mySqlUserService.EditEchecklistRecord(input);
                return RedirectToAction("checklistEntry", new { ChecklistId, ChecklistName, ChecklistDescription, Checklistdate, ProcessId });
            }
            catch (Exception ex)
            {

                return new HttpStatusCodeResult(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> DownloadRecords(int historicalId)
        {
            var records = await _mySqlUserService.gethistoricalById(historicalId);

            if (records == null || !records.Any())
            {
                return new HttpNotFoundResult("No records found for the provided historicalId.");
            }

            var csvContent = CsvHelper.ConvertToCsv(records);
            var bytes = Encoding.UTF8.GetBytes(csvContent);
            var fileName = "historical_records.csv";

            return File(bytes, "text/csv", fileName);
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