using GLMS.Api.Data;
using GLMS.Api.Factories;
using GLMS.Api.Observers;
using GLMS.Api.Repositories;
using GLMS.Api.Repositories.Contracts;
using GLMS.Api.Services;
using GLMS.Api.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Register repositories
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

// Register factory resolver
builder.Services.AddSingleton<ContractFactoryResolver>();

// Register observers as concrete types
builder.Services.AddScoped<ServiceRequestBlocker>();
builder.Services.AddScoped<AuditLogger>();

// Build ContractSubject and attach observers
builder.Services.AddScoped<ContractSubject>(provider =>
{
    var subject = new ContractSubject();

    subject.Attach(provider.GetRequiredService<ServiceRequestBlocker>());
    subject.Attach(provider.GetRequiredService<AuditLogger>());

    return subject;
});

// Register currency service with typed HttpClient
builder.Services.AddHttpClient<ICurrencyService, ExchangeRateService>();

// Register file service
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Seed roles and default users
using (var scope = app.Services.CreateScope())
{
    await SeedData.SeedRolesAndUsersAsync(scope.ServiceProvider);
}

app.Run();
