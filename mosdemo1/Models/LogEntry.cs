namespace mosdemo1.Models
{
        public class LogEntry
        {
            public string timestamp { get; set; }
            public string correlationId { get; set; }
            public string entityName { get; set; }
            public string action { get; set; }
            public string status { get; set; }
            public string queueName { get; set; }
        }
    }


