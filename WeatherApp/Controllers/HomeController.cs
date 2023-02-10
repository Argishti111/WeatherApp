using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models.WeatherModels;
using WeatherApp.Services.Weather;

namespace Admin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        
        [HttpGet]
        [Route("/")]
        public IActionResult Home()
        {
            string workingDirectory = Environment.CurrentDirectory;
            Stream stream = new FileStream(workingDirectory + "\\wwwroot\\index.html", FileMode.Open);

            //"C:\\Users\\Argisht\\Documents\\Repos\\deptApp-Backend\\Admin\\Client\\htmlpage.html"

            if (stream == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(stream, "text/html"); // returns a FileStreamResult;
        }

        [HttpGet]
        [Route("WeatherOfDay")]
        public async Task<IActionResult> GetWeater(DateTime day)
        {
            var result = _weatherService.GetWeater(day);
            return Ok(result);
        }

        [HttpGet]
        [Route("UpcomingWeather")]
        public async Task<IActionResult> GetUpcomingWeater()
        {
            var result = _weatherService.GetUpcomingWeater();
            return Ok(result);
        }

        [HttpPost]
        [Route("SetOrUpdateWeather")]
        public async Task<IActionResult> SetOrUpdateWeather([FromBody] List<WeatherModel> weatherModels)
        {
            var result = _weatherService.SetOrUpdateWeather(weatherModels);
            return Ok(result);
        }

        [HttpPost]
        [Route("SetWeatherForWeek")]
        public async Task<IActionResult> SetWeather([FromBody] List<WeatherModel> weatherModels)
        {
            var result = _weatherService.SetWeather(weatherModels);
            return Ok(result);
        }

    }
}