
namespace event_log_monitor;

public class LogEntry
{
    public string Level { get; set; } = "";
    public DateTime TimeGenerated { get; set; }
    public string Source { get; set; } = "";
    public string Message { get; set; } = "";
}