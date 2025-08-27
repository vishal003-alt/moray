using Microsoft.AspNetCore.Mvc;
using mosdemo1.Models;
using System.Text.Json;
using System.Globalization;

namespace mosdemo1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetLogs(string range = "")
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "logs.json");

            if (!System.IO.File.Exists(path))
                return NotFound("logs.json not found");

            var json = System.IO.File.ReadAllText(path);
            var logs = JsonSerializer.Deserialize<List<LogEntry>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (logs == null || !logs.Any())
                return Ok(new List<LogEntry>());

            DateTime cutoff = range switch
            {
                "30m" => DateTime.Now.AddMinutes(-30),
                "24h" => DateTime.Now.AddHours(-24),
                "3d" => DateTime.Now.AddDays(-3),
                "7d" => DateTime.Now.AddDays(-7),
                "30d" => DateTime.Now.AddDays(-30),
                _ => DateTime.MinValue
            };

            var filtered = logs
                .Where(l =>
                {
                    if (DateTime.TryParse(l.timestamp, out var logTime))
                        return logTime >= cutoff;

                    if (DateTime.TryParseExact(l.timestamp,
                                               "yyyy-MM-dd HH:mm:ss",
                                               CultureInfo.InvariantCulture,
                                               DateTimeStyles.None,
                                               out logTime))
                        return logTime >= cutoff;

                    return false;
                })
                .Select(l =>
                {
                    // try to make timestamp human-friendly
                    if (DateTime.TryParse(l.timestamp, out var parsed))
                    {
                        l.timestamp = parsed.ToString("MMM dd, yyyy hh:mm tt"); // e.g. Aug 27, 2025 11:45 AM
                    }
                    return l;
                })
                .ToList();

            return Ok(filtered);
        }
    }
}
