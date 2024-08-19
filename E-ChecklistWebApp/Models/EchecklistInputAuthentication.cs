using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_ChecklistWebApp.Models
{
    public class EchecklistInputAuthentication
    {
        public string EN { get; set; }
        public string Password { get; set; }
        public string Plant { get; set; }
        public string Role { get; set; }
        public string CreateBy { get; set; }
        public List<SelectListItem> AvailableRoles { get; set; }
        public List<SelectListItem> AvailablePlants { get; set; }

    }
}