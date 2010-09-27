using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using stats.kjonigsen.net.Models;

namespace stats.kjonigsen.net.Controllers
{
    public class StatsController : Controller
    {
#if DEBUG
        JsonRequestBehavior _requestBehaviour = JsonRequestBehavior.AllowGet;
#else
        JsonRequestBehavior _requestBehaviour = JsonRequestBehavior.DenyGet;
#endif

        public ActionResult Index()
        {
            MachineStats stats = GetStatsData();
            return View(stats);
        }

        [HttpGet]
        public JsonResult Json()
        {
            MachineStats stats = GetStatsData();
            return Json(stats, _requestBehaviour);
        }

        private MachineStats GetStatsData()
        {
            MachineStats stats = new MachineStats();
            return stats;
        }
    }
}
