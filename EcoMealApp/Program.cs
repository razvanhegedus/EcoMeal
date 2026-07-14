using System.Text.Json.Serialization;
using EcoMealApp.Components;
using EcoMealApp.Data;
using EcoMealApp.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using EcoMealApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;



var builder = WebApplication.CreateBuilder(args);

// 1. Core Blazor Component Services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- NEW: Required for Blazor to read the cookie ---
builder.Services.AddCascadingAuthenticationState(); 

// --- NEW: Required for AuthService to issue the cookie ---
builder.Services.AddHttpContextAccessor();          

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
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

// --- NEW: Register your custom Auth Service ---
builder.Services.AddScoped<IAuthService, AuthService>(); 

// 4. API Controllers & JSON Formatting Configurations
builder.Services.AddControllersWithViews().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// 5. HTTP Client Configuration (You can leave this here if you need to call external APIs, 
// but remember NOT to use it to call your own internal OrderController anymore!)
builder.Services.AddScoped(hp => new HttpClient { BaseAddress = new Uri("http://localhost:5029/")});

// 6. Cookie Authentication Configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "EcoMealAuthCookie";
        options.LoginPath = "/login";                  
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15); 
        options.SlidingExpiration = true;               
        
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
builder.Services.AddAntiforgery(); // Moved up here with other service registrations

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
app.UseStaticFiles(); 
app.UseRouting();

// THE ORDER HERE IS CRITICAL
app.UseAuthentication(); // 1. Who are you?
app.UseAuthorization();  // 2. Are you allowed in?
app.UseAntiforgery();    // 3. Is this form submission legitimate?

// --- ENDPOINT MAPPINGS ---
app.MapControllers(); // Removed .DisableAntiforgery() so your login form is secure!
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();