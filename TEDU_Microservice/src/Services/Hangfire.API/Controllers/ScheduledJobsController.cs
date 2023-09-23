using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.ScheduledJob;

using Hangfire.API.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.API.Controllers;

[Route("api/scheduled-jobs")]
[ApiController]
public class ScheduledJobsController : ControllerBase
{
    private readonly IBackgroundJobService _jobService;

    public ScheduledJobsController(IBackgroundJobService backgroundJobService)
    {
        _jobService = backgroundJobService;
    }

    [HttpPost]
    [Route("send-email-reminder-checkout-order")]
    public async Task<IActionResult> SendReminderCheckoutOrderEmail([FromBody] ReminderCheckoutOrderDto model)
    {
        await Task.Delay(10000); // simulate some delay fro 10 seconds 
        var jobId = _jobService.SendEmailContent(model.email, model.subject, model.emailContent,model.enqueueAt);

        return Ok(jobId);

    }

    [HttpDelete]
    [Route("delete/jobId/{id}")]
    public IActionResult DeleteJobId([Required] string id)
    {
        var result = _jobService.ScheduledJobService.Delete(id);

        return Ok(result);

    }
}
