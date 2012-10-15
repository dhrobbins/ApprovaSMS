using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Raven.Client.Document;
using ApprovaFlow.Data;
using ApprovaFlow.Users;
using ApprovaFlow.SMSCommunication;
using ApprovaFlow.Workflow;
using Newtonsoft.Json;
using ApprovaFlow.Filters;

namespace ApprovaSMS
{
    public partial class ProcessInboundSMS : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string to = Request["To"].ToString();
            string body = Request["Body"].ToString();
            string from = Request["From"].ToString();

            var docStore = Application["DocumentStore"] as DocumentStore;
            var repository = new RavenDBRepository(docStore);
            var accountController = new AccountController();
            var users = accountController.GetActiveUsers(repository);

            var keywordProcessor = new KeywordProcessor(docStore);
            var message = keywordProcessor.SetActiveUsersForMatch(users)
                                            .MatchReplyToOriginalMessage(body, from);

            using (var session = docStore.OpenSession())
            {
                var step = session.Load<Step>(message.SubjectMatterId);
                step.Answer = message.UserAnswer;
                step.AnswerDate = DateTime.Now;
                string json = JsonConvert.SerializeObject(step);

                var workflowController = new WorkflowController();
                workflowController.ProcessStep(json, repository,
                                                   Application["Registry"] as RavenFilterRegistry<Step>,
                                                   message.UserId);
            }
        }
    }
}