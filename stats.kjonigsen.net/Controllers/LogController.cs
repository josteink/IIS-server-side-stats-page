using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using stats.kjonigsen.net.Models;

namespace stats.kjonigsen.net.Controllers
{
    public class LogController : Controller
    {
#if DEBUG
        JsonRequestBehavior _requestBehaviour = JsonRequestBehavior.AllowGet;
#else
        JsonRequestBehavior _requestBehaviour = JsonRequestBehavior.DenyGet;
#endif


        IISLogsDataContext _db;
        int _maxEntries;

        public LogController()
            : base()
        {
            _db = new IISLogsDataContext();
            _maxEntries = 10;
        }

        //
        // GET: /Log/

        public ActionResult Index()
        {
            var data = GetLogs();
            return View(data);
        }

        public JsonResult Json()
        {
            var data = GetLogs();
            return Json(data, _requestBehaviour);
        }

        public IEnumerable<LogEntry> GetLogs()
        {
            return _db.GetLastLogEntries(_maxEntries)
                .Select((x) => LogEntry.CreateFor(x));
        }

    }
}
