﻿using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    public const string EXCEPTION_MSG = "This is my exception";

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return GetWeatherForecast(5);
    }

    [AllowAnonymous]
    [HttpGet(nameof(PublicGet))]
    public IEnumerable<WeatherForecast> PublicGet()
    {
        return GetWeatherForecast(10);
    }

    [HttpGet(nameof(ParametersGet))]
    public IEnumerable<WeatherForecast> ParametersGet(int num)
    {
        return GetWeatherForecast(num);
    }

    [HttpGet(nameof(HiddenParametersGet))]
    public ActionResult<IEnumerable<WeatherForecast>> HiddenParametersGet()
    {
        if (HttpContext.Request.Query.TryGetValue("num", out var value) && int.TryParse(value.First(), out var num))
            return Ok(GetWeatherForecast(num));
        else
            return BadRequest();
    }

    [Authorize("ValidateClaims")]
    [HttpGet(nameof(PolicyGet))]
    public IEnumerable<WeatherForecast> PolicyGet()
    {
        return GetWeatherForecast(10);
    }

    [HttpGet(nameof(GetException))]
    public IActionResult GetException()
    {
        throw new NotImplementedException(EXCEPTION_MSG);
    }

    [HttpPost]
    public WeatherForecast Create(WeatherForecast model)
    {
        return model;
    }
    
    [HttpDelete]
    public ActionResult<bool> Delete(int id)
    {
        return id > 0;
    }

    [HttpPatch]
    public ActionResult<WeatherForecast> Patch(int id, WeatherForecast model)
    {
        if (id <= 0)
            return NotFound();

        var newModel = GetWeatherForecast(1).First();
        newModel.Summary = model.Summary;

        return newModel;
    }

    private IEnumerable<WeatherForecast> GetWeatherForecast(int max)
    {
        return Enumerable.Range(1, max).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
