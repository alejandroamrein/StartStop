using StartStopDataService.Helpers;
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
    public class StartStopController : ApiController
    {
        [Route("api/StartStop/History/{name}/{max}")]
        public List<StartStopEntry> GetHistory(string name, int max = 5)
        {
            var list = new List<StartStopEntry>();
            var db = new dialogTimeEntities();
            using (db)
            {
                var q = from x in db.StartStops
                        where x.PersId == name && x.Datum != null
                        orderby x.Datum descending
                        select new StartStopEntry()
                        {
                            Datum = x.Datum.Value,
                            Lohnkategorie = x.Lohnkategorie.Beschreibung,
                            Projekt = x.Projekt.Bezeichnung,
                            Tarifkategorie = x.Tarifkategorie.Bezeichnung,
                            Text = x.Text,
                            TimeIntervall = x.TimeIntervall
                        };
                if (q.Any())
                {
                    list = q.Take(max).ToList();
                }
                return list;
            }
        }

        [Route("api/StartStop/State/{name}")]
        public StartStopState GetState(string name)
        {
            var db = new dialogTimeEntities();
            using (db)
            {
                var q = from x in db.StartStops.AsNoTracking()
                        where x.PersId == name
                        orderby x.Id descending
                        select x;
                if (q.Any())
                {
                    var first = q.First();
                    var last = new StartStopState()
                    {
                        State = first.TimeIntervall.EndsWith("-") ? "started" : (first.TimeIntervall.EndsWith("p") ? "paused" : "stopped"),
                        Datum = first.Datum.Value.ToShortDateString(),
                        Projekt = first.ProjektId.ToString(),
                        LohnKategorie = first.LohnkategorieKuerzel,
                        TarifKategorie = first.TarifkategorieId,
                        TimeIntervall = first.TimeIntervall,
                        Text = first.Text
                    };
                    return last;
                }
                else
                {
                    var last = new StartStopState()
                    {
                        State = "stopped",
                        Datum = null,
                        Projekt = null,
                        LohnKategorie = null,
                        TarifKategorie = null,
                        TimeIntervall = "",
                        Text = ""
                    };
                    return last;
                }
            }
        }

        private StateResponse _putStop(dialogTimeEntities db, string persId, StartStop entry, DateTime datum, bool manuell = false)
        {
            if (entry == null)
            {
                return new Models.StateResponse()
                {
                    Success = false,
                    Error = "Kann man nicht stoppen wenn nicht gestarted. Bitte melden!"
                };
            }
            var timeIntervall = entry.TimeIntervall;
            if (timeIntervall.EndsWith("-") || timeIntervall.EndsWith("p"))
            {
                if (timeIntervall.EndsWith("-"))
                {
                    entry.TimeIntervall = timeIntervall + string.Format("{0:00}:{1:00}", datum.Hour, datum.Minute);
                }
                else
                {
                    entry.TimeIntervall = timeIntervall.Substring(0, timeIntervall.Length - 1);
                }
                if (manuell)
                {
                    entry.StartStopManuell = manuell;
                }
                var ii = new Intervalls(entry.TimeIntervall);
                var jj = new Intervalls();
                var q2 = from x in db.RapportEintraege
                         where x.PersId == persId && x.Datum == datum.Date
                         select x;
                foreach (var e in q2)
                {
                    jj.AddRange(new Intervalls(e.TimeIntervall));
                }
                ii.CutWith(jj);
                var eintrag = new RapportEintrag()
                {
                    Id = Guid.NewGuid(),
                    AnsatzExtern = 0,
                    AnsatzIntern = 0,
                    ArbeitsRapportNr = 0,
                    Aufwand = Math.Round(ii.EllapsedAsDouble, 1),
                    Datum = datum.Date,
                    ErfDatum = DateTime.Now,
                    ErfName = persId,
                    LohnkategorieKuerzel = entry.LohnkategorieKuerzel,
                    LohnKatKontierung = "",
                    MandantId = Guid.Parse("331A58AF-C3F6-42BE-BF55-0AE0C5F26C87"),
                    MutDatum = DateTime.Now,
                    MutName = persId,
                    PersId = persId,
                    ProjektId = entry.ProjektId,
                    TarifkategorieId = entry.TarifkategorieId,
                    Text = entry.Text,
                    TimeIntervall = ii.ToString(),
                    Verrechnet = 0,
                    Zuschlag = 0,
                    StartStopManuell = entry.StartStopManuell
                };
                db.RapportEintraege.Add(eintrag);
                db.SaveChanges();
                return new Models.StateResponse()
                {
                    Success = true,
                    TimeIntervall = eintrag.TimeIntervall,
                    Error = null
                };
            }
            else
            {
                return new Models.StateResponse()
                {
                    Success = false,
                    Error = "Kann man nicht stoppen wenn nicht gestarted. Bitte melden!"
                };
            }
        }

        private StateResponse _putCancel(dialogTimeEntities db, string persId, StartStop entry)
        {
            if (entry == null)
            {
                return new Models.StateResponse()
                {
                    Success = false,
                    Error = "Kann man nicht stornieren wenn nicht gestarted. Bitte melden!"
                };
            }
            db.StartStops.Remove(entry);
            db.SaveChanges();
            return new Models.StateResponse()
            {
                Success = true,
                TimeIntervall = string.Empty,
                Error = null
            };
        }

        private StateResponse _putStart(dialogTimeEntities db, string persId, StartStop entry, PutStateInfoEx data, bool manuell = false)
        {
            if (entry != null && (entry.TimeIntervall.EndsWith("-") || entry.TimeIntervall.EndsWith("p")))
            {
                return new StateResponse()
                {
                    Success = false,
                    Error = "Kann man nicht starten wenn nicht gestoppt. Bitte melden!"
                };
            }
            else
            {
                var x = new StartStop()
                {
                    Datum = data.Datum.Date,
                    LohnkategorieKuerzel = data.LohnKategorie,
                    PersId = persId,
                    ProjektId = Guid.Parse(data.Projekt),
                    TarifkategorieId = data.TarifKategorie,
                    Text = data.Text,
                    TimeIntervall = string.Format("{0:00}:{1:00}-", data.Datum.Hour, data.Datum.Minute),
                    StartStopManuell = manuell
                };
                db.StartStops.Add(x);
                db.SaveChanges();
                return new Models.StateResponse()
                {
                    Success = true,
                    TimeIntervall = x.TimeIntervall,
                    Error = null
                };
            }

        }

        [Route("api/StartStop/State/{name}")]
        [HttpPut]
        public StateResponse PutState(string name, [FromBody] PutStateInfo data)
        {
            // Ejemplo
            // api/StartStop/State/Fritz     
            //    { Action="start", Projekt="", LohnKategorie="", TarifKategorie="", Text="" }
            //    { Action="stop" }
            //    { Action="pause" }
            //    { Action="resume" }
            StartStop entry = null;
            var db = new dialogTimeEntities();
            using (db)
            {
                try
                {
                    var q = from x in db.StartStops
                            where x.PersId == name
                            orderby x.Id descending
                            select x;
                    if (q.Any())
                    {
                        entry = q.First();
                    }
                    // ---------------------------------------------
                    switch (data.Action)
                    {
                        case "start":
                            return _putStart(db, name, entry, new PutStateInfoEx()
                                {
                                    Action = "start",
                                    Datum = DateTime.Now,
                                    LohnKategorie = data.LohnKategorie,
                                    Projekt = data.Projekt,
                                    TarifKategorie = data.TarifKategorie,
                                    Text = data.Text
                                });
                        case "pause":
                            {
                                if (entry == null)
                                {
                                    return new Models.StateResponse()
                                    {
                                        Success = false,
                                        Error = "Kann man nicht pausen wenn nicht gestartet. Bitte melden!"
                                    };
                                }
                                var timeIntervall = entry.TimeIntervall;
                                if (timeIntervall.EndsWith("-"))
                                {
                                    entry.TimeIntervall = timeIntervall + string.Format("{0:00}:{1:00}p", DateTime.Now.Hour, DateTime.Now.Minute);
                                    db.SaveChanges();
                                    return new StateResponse()
                                    {
                                        Success = true,
                                        Error = null,
                                        TimeIntervall = entry.TimeIntervall
                                    };
                                }
                                else
                                {
                                    return new Models.StateResponse()
                                    {
                                        Success = false,
                                        Error = "Kann man nicht pausen wenn nicht gestartet. Bitte melden!"
                                    };
                                }
                            }
                        case "resume":
                            {
                                if (entry == null)
                                {
                                    return new Models.StateResponse()
                                    {
                                        Success = false,
                                        Error = "Kann man nicht resumen wenn nicht gepaused. Bitte melden!"
                                    };
                                }
                                var timeIntervall = entry.TimeIntervall;
                                if (timeIntervall.EndsWith("p"))
                                {
                                    entry.TimeIntervall = timeIntervall.Substring(0, timeIntervall.Length - 1) +
                                        string.Format(",{0:00}:{1:00}-", DateTime.Now.Hour, DateTime.Now.Minute);
                                    db.SaveChanges();
                                    return new Models.StateResponse()
                                    {
                                        Success = true,
                                        TimeIntervall = entry.TimeIntervall,
                                        Error = null
                                    };
                                }
                                else
                                {
                                    return new Models.StateResponse()
                                    {
                                        Success = false,
                                        Error = "Kann man nicht resumen wenn nicht gepaused. Bitte melden!"
                                    };
                                }
                            }
                        case "stop":
                            return _putStop(db, name, entry, DateTime.Now);
                        case "cancel":
                            return _putCancel(db, name, entry);
                        default:
                            return new Models.StateResponse()
                            {
                                Success = false,
                                Error = "Keine Aktion vorhanden. Bitte melden!"
                            };
                    }
                }
                catch (Exception ex)
                {
                    return new Models.StateResponse()
                    {
                        Success = false,
                        Error = ex.Message
                    };
                }
            }
        }

        [Route("api/StartStop/StateEx/{name}")]
        [HttpPut]
        public StateResponse PutStateEx(string name, [FromBody] PutStateInfoEx data)
        {
            StartStop entry = null;
            var db = new dialogTimeEntities();
            using (db)
            {
                try
                {
                    var q = from x in db.StartStops
                            where x.PersId == name
                            orderby x.Id descending
                            select x;
                    if (q.Any())
                    {
                        entry = q.First();
                    }
                    // ---------------------------------------------
                    switch (data.Action)
                    {
                        case "start":
                            return _putStart(db, name, entry, data, true);
                        case "stop":
                            return _putStop(db, name, entry, data.Datum, true);
                        default:
                            return new Models.StateResponse()
                            {
                                Success = false,
                                Error = "Keine Aktion vorhanden. Bitte melden!"
                            };
                    }
                }
                catch (Exception ex)
                {
                    return new Models.StateResponse()
                    {
                        Success = false,
                        Error = ex.Message
                    };
                }
            }
        }
    }
}
