using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using stats.kjonigsen.net.Models;

namespace stats.kjonigsen.net.Controllers
{
    public class MachineStatsController : Controller
    {
        public ActionResult Index()
        {
            MachineStats stats = GetStatsData();
            IEnumerable<MachineStats> allStats = new MachineStats[] { stats };
            return View(allStats);
        }

        [HttpGet]
        public JsonResult Json()
        {
            MachineStats stats = GetStatsData();
            return Json(stats, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult JsonRef()
        {
            var result = GetLastReferers();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private MachineStats GetStatsData()
        {
            MachineStats stats = new MachineStats();
            return stats;
        }

        private IEnumerable<RefererEntry> GetLastReferers()
        {
            var db = new IISLogsDataContext();
            return db.GetLastReferers(10);
        }

        private IEnumerable<Log> GetLastLogEntries()
        {
            var db = new IISLogsDataContext();
            return db.GetLastLogEntries(10);
        }
    }
}
