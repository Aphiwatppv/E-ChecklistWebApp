using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_ChecklistWebApp.Models
{
    public class EChecklistInputLogIn
    {
        public string EN { get; set; }
        [AllowHtml]
        public string Password { get; set; }
    }
}