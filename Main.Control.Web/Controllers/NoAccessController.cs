using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Main.Control.Web.Controllers
{
    public class NoAccessController : Controller
    {
        // GET: NoAccess
        public ActionResult SignIn()
        {
            return View();
        }
    }
}