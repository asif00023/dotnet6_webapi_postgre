using Serilog;
using Microsoft.EntityFrameworkCore;
using matelso.api;
using matelso.dbmodels;
using matelso.viewmodels.MapperProfile;
using matelso.repository.interfaces;
using matelso.repository.repository;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console().CreateBootstrapLogger();
Log.Information("Staring up logging");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, logConfig) => logConfig
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MatelsoDataContext>(options => {
    options.UseNpgsql("WebApiDatabase");
});
//builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddControllers()
   .AddNewtonsoftJson(options =>
      options.SerializerSettings.ReferenceLoopHandling =
        Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddAutoMapper(typeof(ContactPersonProfile));

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSerilogRequestLogging();

app.Run();
