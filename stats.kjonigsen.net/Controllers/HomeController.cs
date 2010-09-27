using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using stats.kjonigsen.net.Models;

namespace stats.kjonigsen.net.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            CompositeStats stats = new CompositeStats();
            stats.Machine = new MachineStats();
            stats.Logs = new LogController().GetLogs();
            stats.Referers = new RefererController().GetReferers();

            return View(stats);    
        }
    }
}
