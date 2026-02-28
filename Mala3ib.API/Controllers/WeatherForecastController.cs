namespace Mala3ib.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet(Name = "GetWeatherForecast")]
    public IActionResult Get()
    {
        var res =  Result.Failure(new Error("Test.Com", "Error Test", StatusCodes.Status404NotFound));
        
        return res.ToProblem();
    }
}
