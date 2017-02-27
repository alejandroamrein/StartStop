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
    public class LoginController : ApiController
    {
        // GET api/login/{name}/{password}
        [Route("api/Login/{name}/{password}")]
        public bool GetLogin(string name, string password)
        {
            var md5 = MD5(password);
            var db = new dialogTimeEntities();
            using (db)
            {
                var q = from x in db.Personal
                        where x.PersId == name && x.Passwort == md5
                        select x;
                if (q.Any())
                {
                    return true;
                }
                return false;
            }
        }

        public static string MD5(string password)
        {
            byte[] textBytes = System.Text.Encoding.Default.GetBytes(password);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }
    }
}
