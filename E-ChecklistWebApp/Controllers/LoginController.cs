using E_ChecklistWebApp.AuthApi;
using E_ChecklistWebApp.Models;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace E_ChecklistWebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthAPI _authAPI;

        public LoginController(IAuthAPI authAPI)
        {
            _authAPI = authAPI;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Login(EChecklistInputLogIn model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authAPI.LoginAsync(model);

            if (result.EN != null) // Successful login
            {

                Session["Plant"] = result.Plant;
              

                Session["User"] = result;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }


        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Login", "Login");
        }
    }
}