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
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(EchecklistInputAuthentication model)
        {
            var user = (E_ChecklistWebApp.Models.EchecklistAuthenticationWithoutHash)Session["User"];
            model.CreateBy = user.EN; // Regist by ?
            if (!ModelState.IsValid)
            {
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
            return View(model);
        }
    }
}