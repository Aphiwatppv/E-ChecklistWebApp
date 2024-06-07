using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_ChecklistWebApp.Models
{
    public class EchecklistAllowingProcess
    {
        public int AllowingId { get; set; }
        public int IdAuthen { get; set; }
        public int ProcessId { get; set; }
        public string AllowingFlag { get; set; } // Allow or Not Allow
        public DateTime AllowingDate { get; set; }
    }
}