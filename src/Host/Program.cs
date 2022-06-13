using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddApiServices();

builder.Services.AddControllers()
    .AddApplicationPart(Assembly.Load(new AssemblyName(nameof(Api))));

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOpenApi();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
