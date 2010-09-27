using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stats.kjonigsen.net.Models
{
    public class CompositeStats
    {
        public IEnumerable<LogEntry> Referers { get; set; }
        public IEnumerable<LogEntry> Logs { get; set; }
        public MachineStats Machine { get; set; }
    }
}