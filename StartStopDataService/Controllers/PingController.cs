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
    public class PingController : ApiController
    {
        // GET api/ping
        [Route("api/Ping")]
        public string Get()
        {
            return "ok";
        }

        // GET api/ping/db/test
        [Route("api/Ping/db/test")]
        public string GetDbTest()
        {
            var db = new dialogTimeEntities();
            using (db)
            {
                try
                {                    
                    return db.Personal.First().PersId;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        // GET api/ping/dbcs
        [Route("api/Ping/db/cs")]
        public string GetDbConStr()
        {
            var db = new dialogTimeEntities();
            using (db)
            {
                var str = db.Database.Connection.ConnectionString;
                return str;
            }
        }
    }
}
