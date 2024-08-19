using E_ChecklistWebApp.AuthApi;
using E_ChecklistWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace E_ChecklistWebApp.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IAuthAPI _authAPI;

        public RegisterController(IAuthAPI authAPI)
        {
            _authAPI = authAPI;
        }

        [HttpGet]
        public ActionResult Register()
        {
            var model = new EchecklistInputAuthentication
            {
                AvailableRoles = new List<SelectListItem>
                {
                    new SelectListItem { Value = "User", Text = "User" },
                    new SelectListItem { Value = "ENGINEER", Text = "ENGINEER" }
                },
                AvailablePlants = new List<SelectListItem>
                {
                    new SelectListItem { Value = "UTL1", Text = "UTL1" },
                    new SelectListItem { Value = "UTL2", Text = "UTL2" },
                    new SelectListItem { Value = "UTL3", Text = "UTL3" }
                }
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Register(EchecklistInputAuthentication model)
        {
            var user = (E_ChecklistWebApp.Models.EchecklistAuthenticationWithoutHash)Session["User"];
            model.CreateBy = user.EN; // Regist by ?
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = new List<SelectListItem>
                {
                    new SelectListItem { Value = "User", Text = "User" },
                    new SelectListItem { Value = "ENGINEER", Text = "ENGINEER" }
                };
                model.AvailablePlants = new List<SelectListItem>
                {
                    new SelectListItem { Value = "UTL1", Text = "UTL1" },
                    new SelectListItem { Value = "UTL2", Text = "UTL2" },
                    new SelectListItem { Value = "UTL3", Text = "UTL3" }
                };
                return View(model);
            }

            var result = await _authAPI.RegisterAsync(model);

            if (result == "success")
            {
                TempData["AlertMessage"] = "Registration successful.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Login", "Login");
            }
            else
            {
                TempData["AlertMessage"] = "Registration failed. Please try again.";
                TempData["AlertType"] = "alert-danger";
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
            model.AvailableRoles = new List<SelectListItem>
            {
                new SelectListItem { Value = "User", Text = "User" },
                new SelectListItem { Value = "ENGINEER", Text = "ENGINEER" }
            };
            model.AvailablePlants = new List<SelectListItem>
            {
                new SelectListItem { Value = "UTL1", Text = "UTL1" },
                new SelectListItem { Value = "UTL2", Text = "UTL2" },
                new SelectListItem { Value = "UTL3", Text = "UTL3" }
            };
            return View(model);
        }
    }
}
