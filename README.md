# Kata de tests funcionales con Acheve.TestHost

Usaremos principalmente la documentación descrita en [el github de Acheve.TestHost](https://github.com/Xabaril/Acheve.TestHost?target=_blank).

## Paso 1: Creación del proyecto de Tests

### Paso 1.1: Creamos el proyecto de tests

```powershell
dotnet new xunit -f net6.0
```

### Paso 1.2: Hacemos referencia a nuestro proyecto de _Api_

### Paso 1.3: Añadimos el paquete nuget Acheve.TestHost

## Paso 2: Creacion del fichero _Startup_ para test

### Paso 2.1: Creamos el fichero _Startup_ que usaremos en nuestros tests

```csharp
public class StartupTest
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApiServices();

        services.AddControllers()
            .AddApplicationPart(Assembly.Load(new AssemblyName(nameof(Api))));
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```

### Paso 2.2: Añadimos la definición de autenticación para los tests

```csharp
services.AddAuthentication(TestServerDefaults.AuthenticationScheme)
    .AddTestServer();
```

## Paso 3: Creación de la _Fixture_

### Paso 3.1 Añadimos el paquete nuget _Microsoft.NET.Test.Sdk_

### Paso 3.2: Creamos el fichero _Fixture_ usando el WebApplicationFactory que hará referencia a nuestro _Startup_

```csharp
public class ApiFixture : WebApplicationFactory<StartupTest> { }
```

### Paso 3.3: Sobrescribimos la creación del _WebHost_

```csharp
protected override IWebHostBuilder CreateWebHostBuilder()
{
    return WebHost.CreateDefaultBuilder();
}
```

### Paso 3.4: Configuramos nuestro _WebHost_ para que use nustro Startup y use _TestServer_

```csharp
protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.UseStartup<StartupTest>()
        .UseSolutionRelativeContentRoot("src")
        .UseTestServer();
}
```

## Paso 4: Creamos una colección

### Paso 4.1: Creamos la colección para agrupar todos nuestros tests y usen la _fixture_

```csharp
[CollectionDefinition(nameof(ApiCollection))]
public class ApiCollection : ICollectionFixture<ApiFixture> { }
```

## Paso 5: Creamos fichero de test

### Paso 5.1 (opcional): Añadimos el paquete nuget _FluentAssertions_

### Paso 5.2: Añadimos la referencia a nuestra colección

```csharp
[Collection(nameof(ApiCollection))]
public class WeatherForecastTests
{
    private readonly ApiFixture _fixture;

    public WeatherForecastTests(ApiFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    [Fact]
    public async Task Get_weather_return_5_elements() { ... }
}
```

### Paso 5.3: Creamos unas _Claim_ para usar en los tests

```csharp
internal static class Claims
{
    public static readonly IEnumerable<Claim> UserWithPolicy = new[]
    {
        new Claim(
            type: ClaimTypes.NameIdentifier,
            value: "1",
            valueType: ClaimValueTypes.Integer32,
            issuer: "TestIssuer",
            originalIssuer: "OriginalTestIssuer"),
        new Claim(type: ClaimTypes.Name, value: "User")
    };

    public static readonly IEnumerable<Claim> User = new[]
    {
        new Claim(
            type: ClaimTypes.NameIdentifier,
            value: "2",
            valueType: ClaimValueTypes.Integer32,
            issuer: "TestIssuer2",
            originalIssuer: "OriginalTestIssuer2"),
        new Claim(type: ClaimTypes.Name, value: "User2")
    };
}
```

## Paso 6: Haciendo tests

### Paso 6.1: Test _Get_ sin autorización

```csharp
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
```

### Paso 6.2: Test _Get_ con autorización

```csharp
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
```

### Paso 6.3: Test _Get_ con _Policy_

```csharp
[Fact]
public async Task Get_weather_with_policy_user_return_Ok()
{
    var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.PolicyGet())
        .WithIdentity(Claims.UserWithPolicy)
        .GetAsync();

    await response.IsSuccessStatusCodeOrThrow();
    response.IsSuccessStatusCode.Should().BeTrue();
}

[Fact]
public async Task Get_policy_weather_without_policy_user_return_Forbidden()
{
    var response = await _fixture.Server.CreateHttpApiRequest<WeatherForecastController>(controller => controller.PolicyGet())
        .WithIdentity(Claims.User)
        .GetAsync();
    
    response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
}
```

### Paso 6.4: Test _Get_ con parámetros

```csharp
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
```

### Paso 6.5: Test _Get_ con parámetros extra

```csharp
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
```

### Paso 6.6: Test _Post_

```csharp
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
```

### Paso 6.7: Test _Patch_

```csharp
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
```

### Paso 6.8: Test _Delete_

```csharp
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
```

### Paso 6.9: Test _Exception_

```csharp
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
```

## Paso 7 (Opcional): Lanzando tests en WSL (Windows Subsytem for Linux)

### Paso 7.1: Configuramos WSL

[Documentación Microsoft sobre WSL](https://docs.microsoft.com/es-es/windows/wsl/install?target=_blank).

### Paso 7.2: Instalamos la distribución de linux deseada

```powershell
wsl --list --online
wsl --install -d ubuntu
```

### Paso 7.3: Instalamos el sdk de .Net en linux

[Documentación Microsoft instalación SDK .Net en Linux](https://docs.microsoft.com/es-es/dotnet/core/install/linux?target=_blank)

```bash
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-6.0
```

### Paso 7.4: Configuración en Visual Studio

Añadimos el siguiente fichero (_testenvironments.json_) en la carpeta _root_ de la solución:

```json
{
  "version": "1",
  "environments": [
    {
      "name": "Ubuntu",
      "type": "wsl",
      "wslDistribution": "Ubuntu"
    }
  ]
}
```
