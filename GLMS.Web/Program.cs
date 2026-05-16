using GLMS.Web.Data;
using GLMS.Web.Factories;
using GLMS.Web.Observers;
using GLMS.Web.Observers.Contracts;
using GLMS.Web.Repositories;
using GLMS.Web.Repositories.Contracts;
using GLMS.Web.Services;
using GLMS.Web.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register SQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

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

// Configure login redirect
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/accessDenied";
});

// Register repositories for dependency injection
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

// Register factory resolver
builder.Services.AddSingleton<ContractFactoryResolver>();


// Register currency service with typed HttpClient
builder.Services.AddHttpClient<ICurrencyService, ExchangeRateService>();

// Register observers
builder.Services.AddScoped<ServiceRequestBlocker>();
builder.Services.AddScoped<AuditLogger>();
builder.Services.AddScoped<ContractSubject>(provider =>
{
    var subject = new ContractSubject();

    // Attach all observers at construction time
    subject.Attach(provider.GetRequiredService<ServiceRequestBlocker>());
    subject.Attach(provider.GetRequiredService<AuditLogger>());

    return subject;
});

// Register file service
builder.Services.AddScoped<IFileService, FileService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session support for TempData
builder.Services.AddSession();
builder.Services.Configure<CookieTempDataProviderOptions>(options =>
{
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");d

// Seed roles and default users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.SeedRolesAndUsersAsync(services);
}

app.Run();
