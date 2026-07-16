using System.Text.Json.Serialization;
using EcoMealApp.Components;
using EcoMealApp.Data;
using EcoMealApp.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using EcoMealApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using EcoMealApp.Handlers;

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

// Register the custom delegating handler
builder.Services.AddTransient<CookieDelegatingHandler>();

// 3. Application Services & Repositories Registration
builder.Services.AddScoped<IBusinessService, BusinessService>();
builder.Services.AddScoped<IBusinessTypeService, BusinessTypeService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPackageTypeService, PackageTypeService>();
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

// --- NEW: Register your custom Auth Service ---
builder.Services.AddScoped<IAuthService, AuthService>(); 

// 4. API Controllers & JSON Formatting Configurations
builder.Services.AddControllersWithViews().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// 5. HTTP Client Configuration 
// 5a. Configure the named client with the Cookie Handler attached
builder.Services.AddHttpClient("EcoMealApi", client =>
    {
        client.BaseAddress = new Uri("http://localhost:5029/");
    })
    .AddHttpMessageHandler<CookieDelegatingHandler>();

// 5b. Resolve the default HttpClient injection to use the "EcoMealApi" client configuration.
// This allows you to keep using '@inject HttpClient Http' in your pages without making changes!
builder.Services.AddScoped(sp => 
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("EcoMealApi");
});

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
builder.Services.AddAntiforgery(); 

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

app.UseAuthentication(); 
app.UseAuthorization();  
app.UseAntiforgery();    

// --- ENDPOINT MAPPINGS ---
app.MapControllers(); 
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();