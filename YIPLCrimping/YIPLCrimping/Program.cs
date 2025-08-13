using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using YIPLCrimping.BAL;
using YIPLCrimping.BAL.Service;
using YIPLCrimping.DAL.Repository;
using YIPLCrimping.Helper.YIPLCrimping.Helper;

var builder = WebApplication.CreateBuilder(args);

// Configure logger with path from appsettings.json
var logDirectory = builder.Configuration.GetValue<string>("CustomLogging:LogDirectory");
Logger.Configure(logDirectory);
// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Formatting = Formatting.Indented;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<YIPLCrimping.DAL.Models.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<YIPLCrimping.DAL.Models.CommonDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CommonDbConnection")));
builder.Services.AddScoped<SecurityService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<ShapeService>();
builder.Services.AddScoped<SecurityRepository>();
builder.Services.AddScoped<PlantService>();
builder.Services.AddScoped<PlantRepository>();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<ShapeRepository>();

builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<SupplierRepository>();
builder.Services.AddScoped<WireService>();
builder.Services.AddScoped<WireRepository>();
builder.Services.AddScoped<CrimpingStandardService>();
builder.Services.AddScoped<CrimpingStandardRepository>();

builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<DepartmentRepository>();
builder.Services.AddScoped<MachineService>();
builder.Services.AddScoped<MachineRepository>();
builder.Services.AddScoped<TemplateService>();
builder.Services.AddScoped<TemplateRepository>();
builder.Services.AddScoped<ActivityLoggerHelper>();

//builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDevOrigin", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200"   // ✅ Production frontend (optional)
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // only if using cookies or authorization headers
    });
});

var app = builder.Build();
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine("=== Swagger Error ===");
        Console.WriteLine(ex.ToString());
        throw;
    }
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowDevOrigin");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();