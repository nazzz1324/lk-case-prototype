using Account.api;
using Account.api.Middlewares;
using Account.Application.DependencyInjection;
using Account.Application.Services;
using Account.DAL.DependencyInjection;
using Account.Domain.Settings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwagger();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection));

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplication();

builder.Services.AddControllers();

builder.Services.AddAuthenticationandAuthorization(builder);
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddDataAccessLayer(builder.Configuration);
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        
    });
}
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
