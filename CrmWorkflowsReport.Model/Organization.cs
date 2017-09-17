using Newtonsoft.Json;

namespace CrmWorkflowsReport.Model {
    public class Organization {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
