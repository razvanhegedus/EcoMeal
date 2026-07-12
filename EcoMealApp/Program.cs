using System.Text.Json.Serialization;
using EcoMealApp.Components;
using EcoMealApp.Data;
using EcoMealApp.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using EcoMealApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
Console.WriteLine("=== PASTE THIS IN SQL === " + BCrypt.Net.BCrypt.HashPassword("password123"));
var builder = WebApplication.CreateBuilder(args);

// 1. Core Blazor Component Services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. Database Context
builder.Services.AddDbContext<EcoMealDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("EcoMealDb")));

// 3. Application Services & Repositories Registration
builder.Services.AddScoped<IBusinessService, BusinessService>();
builder.Services.AddScoped<IBusinessTypeService, BusinessTypeService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPackageTypeService, PackageTypeService>();
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

// 4. API Controllers & JSON Formatting Configurations
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// 5. HTTP Client Configuration
builder.Services.AddScoped(hp => new HttpClient { BaseAddress = new Uri("http://localhost:5029/")});

// 6. Anti-Forgery & Security Services
builder.Services.AddAntiforgery();

// 7. Cookie Authentication Configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "EcoMealAuthCookie";
        options.LoginPath = "/login";                  // Redirect location for unauthorized browser requests
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15); // Session length match with AuthController
        options.SlidingExpiration = true;               // Renew cookie lifetime automatically on user activity
        
        // Intercept redirects targeting API routes to prevent raw HTML responses
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
            
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// --- MIDDLEWARE PIPELINE EXECUTION ORDER (Strictly Enforced) ---
app.UseStaticFiles(); // Ensures static elements load without auth challenges

app.UseRouting();

app.UseAntiforgery();    // Must execute after UseRouting
app.UseAuthentication(); // Must execute before UseAuthorization
app.UseAuthorization();  // Evaluates policies before mapping handlers

// --- ENDPOINT MAPPINGS ---
app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();