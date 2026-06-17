using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;

namespace Univision.RememberRestful.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class WeatherForecastController : ControllerBase
  {
    private static readonly string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
      _logger = logger;
    }

    [HttpGet]
    public List<board> Get()
    {
      BoardRepository br = new BoardRepository();
      int totalCount = 0;
      var list = br.BoardList(1, 1, 1, 1, 10, "", "", out totalCount);

      //var rng = new Random();
      return list;
    }
  }
}
