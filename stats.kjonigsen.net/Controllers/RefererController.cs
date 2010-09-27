using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using stats.kjonigsen.net.Models;

namespace stats.kjonigsen.net.Controllers
{
    public class RefererController : Controller
    {
#if DEBUG
        JsonRequestBehavior _requestBehaviour = JsonRequestBehavior.AllowGet;
#else
        JsonRequestBehavior _requestBehaviour = JsonRequestBehavior.DenyGet;
#endif

        IISLogsDataContext _db;
        int _maxEntries;

        public RefererController()
            : base()
        {
            _db = new IISLogsDataContext();
            _maxEntries = 10;
        }


        public ActionResult Index()
        {
            var data = GetReferers();
            return View(data);
        }

        public JsonResult Json()
        {
            var data = GetReferers();
            return Json(data, _requestBehaviour);
        }

        public IEnumerable<LogEntry> GetReferers()
        {
            return _db.GetLastReferers(_maxEntries)
                .Select((x) => LogEntry.CreateFor(x));
        }
    }
}
