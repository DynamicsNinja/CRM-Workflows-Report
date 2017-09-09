using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace CrmJobReport.Helpers {
    public static class CRM {
        public static OrganizationServiceProxy GetService(string organizationUrl, string username, string password) {
            ClientCredentials credentials = new ClientCredentials {
                UserName = { UserName = username, Password = password }
            };

            Uri oUri = new Uri(organizationUrl);
            OrganizationServiceProxy serviceProxy = new OrganizationServiceProxy(
                oUri,
                null,
                credentials,
                null);
            serviceProxy.EnableProxyTypes();
            return serviceProxy;
        }

        public static string GetWorkflowGuidByWorkflowName(IOrganizationService service, string workflowName) {
            var fetchXml = $@"
            <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='workflow'>
                <filter type='and'>
                  <condition attribute='type' operator='eq' value='1' />
                  <condition attribute='statecode' operator='eq' value='1' />
                  <condition attribute='name' operator='eq' value='{workflowName}' />
                </filter>
                <attribute name='workflowid' />
                <attribute name='name' />
              </entity>
            </fetch>
            ";
            var workflow = service.RetrieveMultiple(new FetchExpression(fetchXml)).Entities.FirstOrDefault();
            return workflow?["workflowid"].ToString() ?? string.Empty;
        }
    }
}
