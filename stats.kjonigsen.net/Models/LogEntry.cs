using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace stats.kjonigsen.net.Models
{
    public class LogEntry : Log
    {
        public string Name { get; set; }
        public string Time { get; set; }
        public string ShortReferer { get; set; }

        public static LogEntry CreateFor(Log log)
        {
            LogEntry logEntry = new LogEntry
            {
                ID = log.ID,
                ApplicationName = log.ApplicationName,
                Date = log.Date,
                Time = log.Date.ToString("HH:mm"),
                IPAddress = log.IPAddress,
                Method = log.Method,
                Referer = log.Referer,
                ShortReferer = SnipUrl(log.Referer, 50),
                ResponseCode = log.ResponseCode,
                SiteName = log.SiteName,
                Url = log.Url,
                UserAgent = log.UserAgent,
                UserName = log.UserName,
                Name = string.Empty
            };

            if (logEntry.ResponseCode == "200.0"
                || logEntry.ResponseCode == "301.0"
                || logEntry.ResponseCode == "302.0")
            {
                string name = GetNameForUrl(logEntry.Url);
                logEntry.Name = name;
            }
            return logEntry;
        }

        /*
         * utility methods
         */

        /*
         * name handling
         */

        private static Dictionary<string, string> UrlNameCache;

        private static string GetNameForUrl(string url)
        {
            if (UrlNameCache == null)
            {
                UrlNameCache = new Dictionary<string, string>();
            }

            if (false == UrlNameCache.ContainsKey(url))
            {
                string name = LookupNameForUrl(url);
                UrlNameCache[url] = name;
            }

            string result = UrlNameCache[url];
            return result;
        }

        private static string LookupNameForUrl(string url)
        {
            string result = url;

            try
            {
                // Create a request to the url
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
  
                // If the request wasn't an HTTP request (like a file), ignore it
                if (request == null) return result;
 
                // Obtain a response from the server, if there was an error, return nothing
                HttpWebResponse response = null;
                try { response = request.GetResponse() as HttpWebResponse; }
                catch (WebException) { return result; }

                // If the correct HTML header exists for HTML text, continue
                if (new List<string>(response.Headers.AllKeys).Contains("Content-Type"))
                {
                    string contentType = response.Headers["Content-Type"];

                    var validTypes =  new string[] { "text/html", "text/xml", "application/rss+xml" };

                    bool isValid = false;
                    isValid = validTypes.Where((x) => contentType.StartsWith(x)).Count() != 0;

                    if (isValid)
                    {
                        string content = GetPageContent(url, response);

                        if (false == string.IsNullOrWhiteSpace(content))
                        {
                            const string regex = @"(?<=<title.*>)([\s\S.]*?)(?=</title>)"; // retest
                            Regex rx = new Regex(regex, RegexOptions.IgnoreCase);
                            result = rx.Match(content).Value.Trim();
                        }
                    }
                }
                  
            }
            catch (Exception) { }

            return result;
        }

        private static string GetPageContent(string url, HttpWebResponse response)
        {
            string content = "";

            try
            {
                string contentEncoding = response.Headers["Content-Encoding"];
                Encoding encoding = GetPageEncoding(contentEncoding);

                WebClient wc = new WebClient();
                wc.Headers.Add("User-Agent", "stats.kjonigsen.net name-fetcher.");
                byte[] contentBytes = wc.DownloadData(url);
                content = encoding.GetString(contentBytes);
            }
            catch (Exception)
            { }

            return content;
        }

        private static Encoding GetPageEncoding(string header)
        {
            if (string.IsNullOrWhiteSpace(header))
            {
                header = "UTF8";
            }
            Encoding encoding = null;
            try
            {
                encoding = Encoding.GetEncoding(header);
            }
            catch (Exception) { }
            encoding = encoding ?? Encoding.UTF8;
            return encoding;
        }

        /*
         * URL snipping
         */

        private static string SnipUrl(string url, int length)
        {
            string result = url;

            Action<string, string> replace = (rx, sub) => result = new Regex(rx).Replace(result, sub);

            replace(@"http(s)?://", "");

            if (result.Length > length)
            {
                // strip out query parameters
                replace(@"(.*\?)[^\?]+", "$1...");
            }

            if (result.Length > length)
            {
                // strip out folders
                replace(@"(.+)/.*/([^/]*)", "$1/.../$2");
            }

            if (result.Length > length)
            {
                result = result.Substring(0, (length - 3)) + "...";
            }

            return result;
        }
    

    }
}