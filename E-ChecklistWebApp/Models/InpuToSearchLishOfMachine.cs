using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace E_ChecklistWebApp.Models
{
    public class InpuToSearchLishOfMachine
    {
        public string plant { get; set; }
        public List<int> intsModel { get; set; }    
    }
}