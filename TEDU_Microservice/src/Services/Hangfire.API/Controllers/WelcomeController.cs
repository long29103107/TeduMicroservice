using Contracts.ScheduledJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace Hangfire.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class WelcomeController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IScheduledJobService _jobSevice;

    public WelcomeController(ILogger logger, IScheduledJobService jobSevice)
    {
        _logger = logger;
        _jobSevice = jobSevice;
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Welcome()
    {
        var jobId = _jobSevice.Enqueue(() => ResponseWelcome("Welcome to Hangfire API"));
        return Ok($"Job ID: {jobId} - Enqueue Job");
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult DelayedWelcome()
    {
        var seconds = 5;
        var jobId = _jobSevice.Schedule(() => ResponseWelcome("Welcome to Hangfire API"),TimeSpan.FromSeconds(seconds));
        return Ok($"Job ID: {jobId} - Delayed Job");
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult WelcomeAt()
    {
        var enqueueAt = DateTimeOffset.UtcNow.AddSeconds(10);
        var jobId = _jobSevice.Schedule(() => ResponseWelcome("Welcome to Hangfire API"), enqueueAt);
        return Ok($"Job ID: {jobId} - Schedule Job");
    }


    [HttpPost]
    [Route("[action]")]
    public IActionResult ConfirmWelcome()
    {
        const int timeInSeconds = 5;
        var parentJobId = _jobSevice.Schedule(() => ResponseWelcome("Welcome to Hangfire API"),TimeSpan.FromMilliseconds(timeInSeconds));

        var jobId = _jobSevice.ContinueQueueWith(parentJobId, () => ResponseWelcome("Welcome message is sent"));
        return Ok($"Job ID: {jobId} - Confirm welcome will be sent in {timeInSeconds} seconds");
    }

    [NonAction]
    public void ResponseWelcome(string text)
    {
        _logger.Information(text);
    }
}
