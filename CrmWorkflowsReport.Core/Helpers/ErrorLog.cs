using System;
using System.IO;
using System.Text;

namespace CrmWorkflowsReport.Core.Helpers
{
    public static class ErrorLog
    {
        public static string Write(string organization, Exception exception)
        {
            string path = Environment.CurrentDirectory + @"\ErrorLogs";
            Directory.CreateDirectory(path);
            var yesterday = DateTime.Now.AddDays(-1);
            using (StreamWriter file = new StreamWriter(path + $@"\error_log_{yesterday:yyyyMMdd}.txt", true))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Time: {DateTime.Now}");
                sb.AppendLine($"Organization: {organization}");
                sb.AppendLine($"Message: {exception.Message}");
                sb.AppendLine($"Inner Exception: {exception.InnerException}");
                sb.AppendLine($"{exception.StackTrace}");
                sb.AppendLine("====================================================================================================================================================================================================");
                file.WriteLine(sb);

                return sb.ToString();
            }
        }
    }
}
