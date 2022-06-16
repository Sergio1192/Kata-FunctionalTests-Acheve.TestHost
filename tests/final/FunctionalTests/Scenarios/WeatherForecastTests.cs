namespace FunctionalTests.Scenarios;

[Collection(nameof(ApiCollection))]
public class WeatherForecastTests
{
    private readonly ApiFixture _fixture;

    public WeatherForecastTests(ApiFixture fixture)
    {
        _fixture = fixture;
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
    public async Task Get_policy_weather_with_policy_user_return_ok()
    {
        var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.PolicyGet())
            .WithIdentity(Claims.UserWithPolicy)
            .GetAsync();

        await response.IsSuccessStatusCodeOrThrow();
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task Get_policy_weather_without_policy_user_return_forbidden()
    {
        var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.PolicyGet())
            .WithIdentity(Claims.User)
            .GetAsync();
        
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_weather_with_hidden_parameter_3_return_3_elements()
    {
        const int N = 3;

        var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.HiddenParametersGet())
            .AddQueryParameter("num", N)
            .WithIdentity(Claims.User)
            .GetAsync();
        
        await response.IsSuccessStatusCodeOrThrow();

        var result = await response.ReadContentAsAsync<IEnumerable<WeatherForecast>>();

        result.Should().HaveCount(N);
    }

    [Fact]
    public async Task Get_weather_with_hidden_parameter_is_not_number_return_ko()
    {
        var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.HiddenParametersGet())
            .AddQueryParameter("num", "a")
            .WithIdentity(Claims.User)
            .GetAsync();

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_weather_with_wrong_hidden_parameter_return_ko()
    {
        var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.HiddenParametersGet())
            .AddQueryParameter("numa", "a")
            .WithIdentity(Claims.User)
            .GetAsync();

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

    [Fact]
    public async Task Get_exception_return_specific_excepction_and_message()
    {
        var action = () => _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.GetException())
            .WithIdentity(Claims.User)
            .GetAsync();

        await action.Should()
            .ThrowAsync<NotImplementedException>()
            .WithMessage(WeatherForecastController.EXCEPTION_MSG);
    }
}