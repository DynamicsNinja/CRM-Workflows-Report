using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmWorkflowsReport.Model;
using Newtonsoft.Json;

namespace CrmWorkflowsReport.Core.Helpers
{
    public class Settings
    {
        public List<Organization> Organizations { get; set; }
        public MailSettings MailSettings { get; set; }

        public Settings()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            using (StreamReader reader = new StreamReader("organizations.json"))
            {
                string json = reader.ReadToEnd();
                Organizations = JsonConvert.DeserializeObject<List<Organization>>(json);
            }
            using (StreamReader reader = new StreamReader("mail.json"))
            {
                string json = reader.ReadToEnd();
                MailSettings = JsonConvert.DeserializeObject<MailSettings>(json);
            }
        }
    }
}
