using StartStopDataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace StartStopDataService.Controllers
{
    [AllowAnonymous]
    [EnableCors("*", "*", "*")]
    public class StammController : ApiController
    {
        [Route("api/Stamm/Projekte/{name}")]
        public IEnumerable<SelectItem> GetProjekte(string name)
        {
            var db = new dialogTimeEntities();
            using (db)
            {
                var mitarbeiter = db.Personal.Find(name);
                var q = from x in mitarbeiter.MitarbeiterProjekte //.AsNoTracking()
                        where x.Aktiv != null && x.Aktiv.Value
                        orderby x.Bezeichnung 
                        select new SelectItem() { Value = x.Id.ToString(), Text = x.Bezeichnung };
                return q.ToList();
            }
        }
        [Route("api/Stamm/Lohnkategorien/{projektId}")]
        public IEnumerable<SelectItem> GetLohnkategorien(string projektId)
        {
            var db = new dialogTimeEntities();
            using (db)
            {
                var id = Guid.Parse(projektId);
                var projekt = db.Projekte.Find(id);
                var q = from x in projekt.ProjektLohnkategorieZuordnung //.AsNoTracking()
                        orderby x.Lohnkategorien.Beschreibung
                        select new SelectItem() {
                            Value = x.Lohnkategorien.Kuerzel,
                            Text = x.Lohnkategorien.Beschreibung
                        };
                return q.ToList();
            }
        }
        [Route("api/Stamm/Tarifkategorien")]
        public IEnumerable<SelectItem> GetTarifkategorien()
        {
            var db = new dialogTimeEntities();
            using (db)
            {
                var q = from x in db.Tarifkategorien.AsNoTracking()
                        orderby x.Bezeichnung
                        select new SelectItem() { Value = x.Id, Text = x.Bezeichnung };
                return q.ToList();
            }
        }
    }
}
