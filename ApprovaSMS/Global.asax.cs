using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Raven.Client.Document;
using System.Configuration;
using ApprovaFlow.Data;
using ApprovaFlow.Filters;
using ApprovaFlow.Workflow;
namespace ApprovaSMS
{
    public class Global : System.Web.HttpApplication
    {
        private static DocumentStore documentStore;

        void Application_Start(object sender, EventArgs e)
        {
            if (documentStore == null)
            {
                if (ConfigurationManager.AppSettings["debuglocal"].ToString() == "true")
                {
                    documentStore = new DocumentStore() { Url = "http://localhost:8080" };
                    documentStore.Initialize();
                }
                else
                {
                    string connectionString = ConfigurationManager.AppSettings["RavenDBConnection"].ToString();
                    var connectionValues = connectionString.Split(';');
                    string url = connectionValues[0].Replace("Url=", string.Empty)
                                                        .Trim();
                    string apiKey = connectionValues[1].Replace("ApiKey=", string.Empty).Trim();

                    documentStore = new DocumentStore()
                    {
                        Url = url,
                        ApiKey = apiKey
                    };
                    documentStore.Initialize();
                }
            }

            Application["DocumentStore"] = documentStore;

            var repository = new RavenDBRepository(documentStore);
            var registry = new RavenFilterRegistry<Step>(repository);
            Application.Add("Registry", registry);
            Application.Add("Repository", repository);

        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
