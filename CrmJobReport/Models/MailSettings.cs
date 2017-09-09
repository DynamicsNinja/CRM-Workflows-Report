using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CrmJobReport.Models
{
    public class MailSettings
    {
        [JsonProperty("senderName")]
        public string SenderName { get; set; }
        [JsonProperty("to")]
        public string ToEmail { get; set; }
        [JsonProperty("toName")]
        public string ToName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("host")]
        public string SmtpHost { get; set; }
        [JsonProperty("port")]
        public int Port { get; set; }
        [JsonProperty("enableSsl")]
        public bool EnableSsl { get; set; }
    }
}
