using System;
using System.Web;
using System.Data.SqlClient;
using System.Web.Hosting;
using Microsoft.Web.Administration;

namespace stats.kjonigsen.net.loggiong
{
    public class SqlLoggingModule : IHttpModule
    {
        public SqlLoggingModule()
        {
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.LogRequest += new EventHandler(context_LogRequest);
        }

        void context_LogRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            LogRequest(application.Context);
        }

        private string GetConnectionString(HttpContext httpContext)
        {
            string connectionString = string.Empty;
            try
            {
                // Configure as according to this article:
                // http://blogs.iis.net/bills/archive/2007/05/01/building-an-iis7-sql-logging-module-with-net.aspx
                ConfigurationSection section = WebConfigurationManager.GetSection("system.webServer/sqlLogging");
                connectionString = (string)section["connectionString"];
            }
            catch (Exception) { }

            return connectionString;
        }

        private void LogRequest(HttpContext httpContext)
        {
            try
            {
                string connectionString = GetConnectionString(httpContext);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText =
                        "insert into Log (Method, IPAddress, Url, UserName, UserAgent, ResponseCode, SiteName, ApplicationName, Referer) values" +
                                      "(@Method, @IPAddress, @Url, @UserName, @UserAgent, @ResponseCode, @SiteName, @ApplicationName, @Referer)";

                    cmd.Parameters.AddWithValue("@Method", httpContext.Request.HttpMethod);
                    cmd.Parameters.AddWithValue("@IPAddress", httpContext.Request.UserHostAddress);
                    cmd.Parameters.AddWithValue("@Url", httpContext.Request.Url.ToString());
                    cmd.Parameters.AddWithValue("@UserName", httpContext.Request.ServerVariables["LOGON_USER"]);
                    cmd.Parameters.AddWithValue("@UserAgent", httpContext.Request.UserAgent);
                    cmd.Parameters.AddWithValue("@ResponseCode", httpContext.Response.StatusCode + "." + httpContext.Response.SubStatusCode);
                    cmd.Parameters.AddWithValue("@SiteName", HostingEnvironment.SiteName);
                    cmd.Parameters.AddWithValue("@ApplicationName", httpContext.Request.ApplicationPath);
                    string referer = httpContext.Request.Headers["Referer"];
                    if (referer == null)
                    {
                        referer = "";
                    }
                    cmd.Parameters.AddWithValue("@Referer", referer);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    cmd.Dispose();
                }
                // logging code here
            }
            catch (Exception) { }
        }

    }
}
