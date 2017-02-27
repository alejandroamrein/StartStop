using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartStopDataService.Models
{
    public class StartStopState
    {
        public string State { get; set; }
        public string Projekt { get; set; }
        public string LohnKategorie { get; set; }
        public string TarifKategorie { get; set; }
        public string Text { get; set; }
        public string TimeIntervall { get; set; }
        public string Datum { get; set; }
    }
}