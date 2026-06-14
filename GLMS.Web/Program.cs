using GLMS.Web.Handlers;
using GLMS.Web.Services;
using GLMS.Web.Services.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Required for JwtAuthHandler to access the current user's claims
builder.Services.AddHttpContextAccessor();

// Register the JWT handler as transient so it can be added to typed clients
builder.Services.AddTransient<JwtAuthHandler>();

// Register AuthApiService with typed HttpClient pointing to GLMS.Api
builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
});

// Register ClientApiService with typed HttpClient and JWT handler
builder.Services.AddHttpClient<IClientApiService, ClientApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
}).AddHttpMessageHandler<JwtAuthHandler>();

// Register ContractApiService with typed HttpClient and JWT handler
builder.Services.AddHttpClient<IContractApiService, ContractApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
}).AddHttpMessageHandler<JwtAuthHandler>();

// Register ServiceRequestApiService with typed HttpClient and JWT handler
builder.Services.AddHttpClient<IServiceRequestApiService, ServiceRequestApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
}).AddHttpMessageHandler<JwtAuthHandler>();

// Register UserApiService with typed HttpClient and JWT handler
builder.Services.AddHttpClient<IUserApiService, UserApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
}).AddHttpMessageHandler<JwtAuthHandler>();

// Configure login redirect
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/access-denied";
});


// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session support for TempData
builder.Services.AddSession();
builder.Services.Configure<CookieTempDataProviderOptions>(options =>
{
    options.Cookie.IsEssential = true;
});

// Register cookie authentication for session management
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/account/login";
        options.AccessDeniedPath = "/account/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
