using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_ChecklistWebApp.Controllers
{
    public class ErrorController : Controller
    {
        // GET: General
        public ActionResult General()
        {
            return View();
        }

        // GET: Error/NotFound
        public ActionResult NotFound()
        {
            return View();
        }

        // GET: Error/ServerError
        public ActionResult ServerError()
        {
            return View();
        }
    }
}