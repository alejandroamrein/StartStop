using System;

namespace StartStopDataService.Models
{
    public class PutStateInfo
    {
        public string Action { get; set; }
        public string Projekt { get; set; }
        public string LohnKategorie { get; set; }
        public string TarifKategorie { get; set; }
        public string Text { get; set; }
    }
    public class PutStateInfoEx
    {
        public string Action { get; set; }
        public string Projekt { get; set; }
        public string LohnKategorie { get; set; }
        public string TarifKategorie { get; set; }
        public string Text { get; set; }
        public DateTime Datum { get; set; }
    }
}