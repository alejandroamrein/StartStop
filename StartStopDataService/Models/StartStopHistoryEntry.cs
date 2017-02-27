using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartStopDataService.Models
{
    public class StartStopEntry
    {
        public DateTime Datum { get; set; }
        public string Projekt { get; set; }
        public string Lohnkategorie { get; set; }
        public string Tarifkategorie { get; set; }
        public string Text { get; set; }
        public string TimeIntervall { get; set; }
    }
}