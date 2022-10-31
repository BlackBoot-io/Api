// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text.Json;
using Avn.Api.Extentions;
using Avn.Api.Middlewares;
using Avn.Services;
using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
{
    loggerConfiguration.Enrich.FromLogContext();
    loggerConfiguration.WriteTo.Console();

    // loggerConfiguration.WriteTo.Seq("http://localhost:5341");
    loggerConfiguration.WriteTo.File(new JsonFormatter(),"log\\AppLogs.json");
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddApplicationDbContext(configuration);

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    // opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(configuration.GetSection("AllowOrgin").Value.Split(";"))
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
builder.Services.AddApplicationAuthentication(configuration);
builder.Services.AddHttpClient();
builder.Services.RegisterApplicatioinServices();
builder.Services.AddHostedService<ApplicationInitialize>();
builder.Services.AddEasyCaching(options =>
{
    options.UseInMemory("inMemoryCache");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseStaticFiles();
app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(config =>
{
    config.MapControllers();
});
app.Run();
