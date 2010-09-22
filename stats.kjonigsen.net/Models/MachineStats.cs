using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace stats.kjonigsen.net.Models
{
    public class MachineStats
    {
        private PerformanceCounter _cpuStats;
        private PerformanceCounter _ramStats;
        private PerformanceCounter _iisStats;

        public string CPU
        {
            get { return GetValue(_cpuStats, "{0:N1}%"); }
        }

        public string RAM
        {
            get { return GetValue(_ramStats, "{0}MB"); }
        }

        public string WebConnections
        {
            get { return GetValue(_iisStats); }
        }

        public MachineStats()
        {
            _cpuStats = GetCounter("Processor", "% Processor Time", "_Total");
            _ramStats = GetCounter("Memory", "Available MBytes");
            _iisStats = GetCounter("W3SVC_WP", "Requests / Sec", "_Total");
        }

        private PerformanceCounter GetCounter(string objectName, string counterName)
        {
            PerformanceCounter counter = null;
            try
            {
                counter = new PerformanceCounter(objectName, counterName, true);
            }
            catch { }
            return counter;
        }

        private PerformanceCounter GetCounter(string objectName, string counterName, string instanceName)
        {
            PerformanceCounter counter = null;
            try
            {
                counter = new PerformanceCounter(objectName, counterName, instanceName, true);
            }
            catch { }
            return counter;
        }

        private string GetValue(PerformanceCounter counter)
        {
            return GetValue(counter, "{0}");
        }

        private string GetValue(PerformanceCounter counter, string formatString)
        {
            string result = "N/A";

            if (counter != null)
            {
                float value = counter.NextValue();
                result = string.Format(formatString, value);
            }

            return result;
        }
    }
}