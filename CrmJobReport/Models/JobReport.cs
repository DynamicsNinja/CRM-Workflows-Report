namespace CrmJobReport.Models {
    public class JobReport {
        public string Name { get; set; }
        public string Organization { get; set; }
        public string WorkflowUrl { get; set; }
        public int Failed { get; set; }
        public int Waiting { get; set; }
        public int WaitingForResources { get; set; }
    }
}
