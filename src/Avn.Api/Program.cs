using Avn.Api.Extentions;
using Avn.Api.Middlewares;
using Avn.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


#region Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddApplicationDbContext(configuration);

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    //opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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

builder.Services.RegisterApplicatioinServices();
#endregion

#region Application
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
#endregion

