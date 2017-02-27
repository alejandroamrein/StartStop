using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartStopDataService.Models
{
    public class StateResponse
    {
        public bool Success { get; set; } 
        public string Error { get; set; }
        public string TimeIntervall { get; set; }
    }
}