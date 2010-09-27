using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace stats.kjonigsen.net.Models
{
    public class MachineStats
    {
        public string CPU
        {
            get
            {
                var counter = GetCounter("Processor", "% Processor Time", "_Total");
                return GetValue(counter, "{0:N1}%");
            }
        }

        public string RAM
        {
            get
            {
                var counter = GetCounter("Memory", "Available MBytes");
                return GetValue(counter, "{0}MB");
            }
        }

        public string WebConnections
        {
            get
            {
                var counter = GetCounter("W3SVC_WP", "Requests / Sec", "_Total");
                return GetValue(counter);
            }
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