using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_ChecklistWebApp.Models
{
    public class EchecklistAuthenticationWithoutHash
    {
        public int IdAuthen { get; set; }
        public string EN { get; set; }
        public string Plant { get; set; }
        public string Role { get; set; }
        public string ActiveFlag { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
    }
}