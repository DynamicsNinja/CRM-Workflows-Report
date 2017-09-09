using Newtonsoft.Json;

namespace CrmJobReport.Models {
    public class Organization {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
