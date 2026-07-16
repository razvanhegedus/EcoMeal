namespace EcoMealApp.Handlers;

public class CookieDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;

        if (context != null)
        {
            // Must match the name defined in your Program.cs cookie configuration
            var authCookieName = "EcoMealAuthCookie"; 
                
            if (context.Request.Cookies.TryGetValue(authCookieName, out var cookieValue))
            {
                request.Headers.Add("Cookie", $"{authCookieName}={cookieValue}");
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}