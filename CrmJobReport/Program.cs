using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CrmJobReport.Helpers;
using CrmJobReport.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CrmJobReport
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings();
            GenerateEmailData(settings.Organizations, out string bodyMsg, out string errors);
            SendMail(bodyMsg, errors, settings.MailSettings);
        }

        private static void GenerateEmailData(List<Organization> organizations, out string bodyMsg, out string errors)
        {
            var tableRows = string.Empty;
            errors = string.Empty;

            foreach (var organization in organizations){
                try{
                    tableRows += GenerateTableRows(organization);
                }catch (Exception ex){
                    errors += ErrorLog.Write(organization.Url, ex);
                }
            }

            bodyMsg = $@"<h1>Jobs Status report for {DateTime.Now.AddDays(-1):dd.MM.yyyy}</h1>";
            if (tableRows != string.Empty){
                bodyMsg += $@"<table border=1 style=""width:100%"">
                                <tr>
                                <th align=""left"">Organization</th>
                                <th align=""left"">Job Name</th> 
                                <th align=""left"">Failed</th>
                                <th align=""left"">Waiting (Failed)</th>
                                <th align=""left"">Waiting For Resources</th>
                                </tr>
                                {tableRows}
                            </table>";
            }else{
                bodyMsg += "<p>Today there were no failed system jobs.</p>";
            }

            if (errors != string.Empty){
                bodyMsg += "<p style=\"color:red;\"><b>There were some errors in execution please check error log in attachment for more informations!</b></p>";
            }
        }

        public static string GenerateTableRows(Organization organization)
        {
            var oUrl = organization.Url;
            var username = organization.Username;
            var password = organization.Password;

            var jobs = new List<JobReport>();
            var tableRows = string.Empty;
            var service = CRM.GetService(oUrl, username, password);
            Regex r = new Regex(@"(?<=https:\/\/).*?(?=\/)", RegexOptions.IgnoreCase);
            var orgName = r.Match(oUrl).Groups[0].Value;

            QueryExpression pagequery = new QueryExpression();
            pagequery.EntityName = "asyncoperation";
            pagequery.ColumnSet = new ColumnSet("name", "statuscode");
            pagequery.Criteria.AddCondition(new ConditionExpression("modifiedon", ConditionOperator.Yesterday));
            pagequery.Criteria.AddCondition(new ConditionExpression("statuscode", ConditionOperator.In, "31", "10", "0"));
            pagequery.Criteria.AddCondition(new ConditionExpression("name", ConditionOperator.NotNull));
            pagequery.Criteria.AddCondition(new ConditionExpression("workflowactivationid", ConditionOperator.NotNull));
            pagequery.Criteria.AddCondition(new ConditionExpression("errorcode", ConditionOperator.NotNull));

            // Assign the pageinfo properties to the query expression.
            pagequery.PageInfo = new PagingInfo();
            pagequery.PageInfo.Count = 200;
            pagequery.PageInfo.PageNumber = 1;
            pagequery.PageInfo.PagingCookie = null;

            while (true)
            {
                // Retrieve the page.
                EntityCollection results = service.RetrieveMultiple(pagequery);

                foreach (var job in results.Entities)
                {
                    var jobFromList = jobs.FirstOrDefault(x => x.Name == (string)job["name"]);
                    if (jobFromList != null)
                    {
                        IncrementStatusValue(job, jobFromList);
                    }
                    else
                    {
                        var newJob = new JobReport { Name = (string)job["name"] };
                        var workflowId = CRM.GetWorkflowGuidByWorkflowName(service, (string)job["name"]);
                        newJob.Organization = orgName;
                        newJob.WorkflowUrl = "https://" + orgName.Replace("api.", "") + $"/sfa/workflow/areas.aspx?oId=%7b{workflowId}%7d&oType=4703&security=851973&tabSet=areaAsyncOperations";
                        IncrementStatusValue(job, newJob);
                        jobs.Add(newJob);
                    }
                }

                // Check for more records, if it returns true.
                if (results.MoreRecords)
                {
                    pagequery.PageInfo.PageNumber++;
                    pagequery.PageInfo.PagingCookie = results.PagingCookie;
                }
                else
                {
                    break;
                }
            }
            for (var i = 0; i < jobs.Count; i++)
            {
                if (i == 0)
                {
                    //tableRows += $"  <tr>\r\n    <td rowspan=\"{jobs.Count}\">{jobs[i].Organization}</td>\r\n    <td><a href=\"{jobs[i].WorkflowUrl}\">{jobs[i].Name}</a></td>\r\n    <td>{jobs[i].Failed}</td>\r\n    <td>{jobs[i].Waiting}</td>\r\n    <td>{jobs[i].WaitingForResources}</td>\r\n  </tr>";
                    tableRows += $"<tr><td rowspan=\"{jobs.Count}\">{jobs[i].Organization}</td><td><a href=\"{jobs[i].WorkflowUrl}\">{jobs[i].Name}</a></td><td>{jobs[i].Failed}</td><td>{jobs[i].Waiting}</td><td>{jobs[i].WaitingForResources}</td></tr>";
                }
                else
                {
                    tableRows += $"<tr><td><a href=\"{jobs[i].WorkflowUrl}\">{jobs[i].Name}</a></td><td>{jobs[i].Failed}</td><td>{jobs[i].Waiting}</td><td>{jobs[i].WaitingForResources}</td></tr>";
                }
            }
            return tableRows;
        }

        public static void IncrementStatusValue(Entity job, JobReport jobReport)
        {
            switch (((OptionSetValue)job["statuscode"]).Value)
            {
                // Failed Status
                case 31:
                    jobReport.Failed++;
                    break;
                // Waiting Status
                case 10:
                    jobReport.Waiting++;
                    break;
                // Waiting For Resources Status
                case 0:
                    jobReport.WaitingForResources++;
                    break;
                default:
                    jobReport.Failed++;
                    break;
            }
        }

        public static void SendMail(string bodyMsg, string errors, MailSettings settings)
        {
            var fromAddress = new MailAddress(settings.Email, settings.SenderName);
            var toAddress = new MailAddress(settings.ToEmail, settings.ToName);
            string fromPassword = settings.Password;
            string subject = $"CRM job report - {DateTime.Now:dd.MM.yyyy}";
            string body = bodyMsg;

            var smtp = new SmtpClient
            {
                Host = settings.SmtpHost,
                Port = settings.Port,
                EnableSsl = settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                if (errors != string.Empty)
                {
                    message.Attachments.Add(Attachment.CreateAttachmentFromString(errors, $"error_log_{DateTime.Now:yyyyMMdd}.txt"));
                }
                smtp.Send(message);
            }
        }
    }
}
