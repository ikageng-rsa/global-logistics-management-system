using GLMS.Api.Data;
using GLMS.Api.Factories;
using GLMS.Api.Observers;
using GLMS.Api.Repositories;
using GLMS.Api.Repositories.Contracts;
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

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
