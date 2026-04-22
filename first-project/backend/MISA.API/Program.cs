using System.Text.Json;
using MISA.BL.Base;
using MISA.BL.Service;
using MISA.DL.Base;
using MISA.DL.Repository;
using MISA.DL.Context;
using MISA.API.Exception;
using Dapper;
using MISA.Common.Extension;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

// Register Dapper TypeHandlers
SqlMapper.AddTypeHandler(new GuidTypeHandler());
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped(typeof(IBaseBl<>), typeof(BaseBl<>));
builder.Services.AddScoped(typeof(IBaseDl<>), typeof(BaseDl<>));
builder.Services.AddScoped<DbContext>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

# region Theme for console log
var theme = new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
{
    [ConsoleThemeStyle.Text] = "\x1b[37m", // trắng
    [ConsoleThemeStyle.SecondaryText] = "\x1b[30m", // xám
    [ConsoleThemeStyle.TertiaryText] = "\x1b[90m",

    [ConsoleThemeStyle.Name] = "\x1b[36m", // cyan
    [ConsoleThemeStyle.String] = "\x1b[32m", // xanh lá

    [ConsoleThemeStyle.LevelInformation] = "\x1b[32m",
    [ConsoleThemeStyle.LevelWarning] = "\x1b[33m",
    [ConsoleThemeStyle.LevelError] = "\x1b[31m",
    [ConsoleThemeStyle.LevelFatal] = "\x1b[41m", // nền đỏ
    [ConsoleThemeStyle.LevelDebug] = "\x1b[45m",
});
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(theme: theme,
        outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message}{NewLine}{Exception}")
    .CreateLogger();
#endregion

#region CORS config

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });

    options.AddPolicy("AllowAllDev", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

#endregion

builder.Host.UseSerilog();

var app = builder.Build();

app.UseCors("AllowSpecificOrigins");

app.UseRouting();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();


// app.UseHttpsRedirection();


app.Run();