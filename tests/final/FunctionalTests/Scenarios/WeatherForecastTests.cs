namespace FunctionalTests.Scenarios;

[Collection(nameof(ApiCollection))]
public class WeatherForecastTests
{
    private readonly ApiFixture _fixture;

    public WeatherForecastTests(ApiFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    [Fact]
    public async Task Get_weather_return_5_elements()
    {
        var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.Get())
            .WithIdentity(Claims.User)
            .GetAsync();

        await response.IsSuccessStatusCodeOrThrow();

        var result = await response.ReadContentAsAsync<IEnumerable<WeatherForecast>>();

        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task Get_weather_public_return_10_elements()
    {
        var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.PublicGet())
            .WithIdentity(Claims.User)
            .GetAsync();

        await response.IsSuccessStatusCodeOrThrow();

        var result = await response.ReadContentAsAsync<IEnumerable<WeatherForecast>>();

        result.Should().HaveCount(10);
    }

    [Fact]
    public async Task Get_weather_with_parameter_3_return_3_elements()
    {
        const int N = 3;
        
        var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.ParametersGet(N))
            .WithIdentity(Claims.User)
            .GetAsync();

        await response.IsSuccessStatusCodeOrThrow();

        var result = await response.ReadContentAsAsync<IEnumerable<WeatherForecast>>();

        result.Should().HaveCount(N);
    }

    [Fact]
    public async Task Post_weather_return_same_model()
    {
        var model = new WeatherForecast()
        {
            Date = DateTime.Today,
            Summary = nameof(WeatherForecast.Summary),
            TemperatureC = 22
        };

        var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.Create(model))
            .WithIdentity(Claims.User)
            .PostAsync();

        await response.IsSuccessStatusCodeOrThrow();
        
        var result = await response.ReadContentAsAsync<WeatherForecast>();

        result.Should().Be(model);
    }

    [Fact]
    public async Task Patch_weather_return_same_model()
    {
        var model = new WeatherForecast()
        {
            Summary = nameof(WeatherForecast.Summary)
        };

        var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.Patch(1, model))
            .WithIdentity(Claims.User)
            .SendAsync(HttpMethod.Patch.ToString());

        await response.IsSuccessStatusCodeOrThrow();

        var result = await response.ReadContentAsAsync<WeatherForecast>();

        result.Summary.Should().Be(model.Summary);
        result.Date.Should().NotBe(model.Date);
    }

    [Fact]
    public async Task Patch_weather_return_NotFound()
    {
        var model = new WeatherForecast()
        {
            Summary = nameof(WeatherForecast.Summary)
        };

        var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.Patch(0, model))
            .WithIdentity(Claims.User)
            .SendAsync(HttpMethod.Patch.ToString());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_weather_return_true()
    {
        var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.Delete(1))
            .WithIdentity(Claims.User)
            .SendAsync(HttpMethod.Delete.ToString());

        await response.IsSuccessStatusCodeOrThrow();
        var result = await response.ReadContentAsAsync<bool>();

        result.Should().BeTrue();
    }
}