using Microsoft.AspNetCore.Mvc;
using Quartz;
using ShopApp.Domain.Models;

namespace ShopAppV2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuartzTestController : ControllerBase
{
    private readonly ISchedulerFactory _schedulerFactory;

    public QuartzTestController(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    // GET: api/CheckCleanUpJob
    [HttpGet("CheckCleanUpJob")]
    public async Task<Response> CheckCleanUpJob()
    {
        try
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey("CleanUploadsJob", "DEFAULT");
            if (await scheduler.CheckExists(jobKey)) return ShopApp.Domain.Models.Response.Success("Ok");
            return ShopApp.Domain.Models.Response.Fail("Job doesn't exist!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ShopApp.Domain.Models.Response.Fail(e.Message);
        }
    }

    // GET: api/TriggerCleanUpJob
    [HttpGet("TriggerCleanUpJob")]
    public async Task<Response> TriggerCleanUpJob()
    {
        try
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey("CleanUploadsJob", "DEFAULT");
            if (!await scheduler.CheckExists(jobKey)) return ShopApp.Domain.Models.Response.Fail("Job doesn't exist!");
            await scheduler.TriggerJob(new JobKey("CleanUploadsJob"));
            return ShopApp.Domain.Models.Response.Success("Ok");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ShopApp.Domain.Models.Response.Fail(e.Message);
        }
    }
}