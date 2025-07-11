using FirebaseAdmin.Auth;

public class ParseCookieMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string[] _excludedPaths = ["/session"]; // Add your paths here

    public ParseCookieMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        var requestPath = context.Request.Path.Value ?? "";
        
        if (_excludedPaths.Any(path => requestPath.StartsWith(path, StringComparison.OrdinalIgnoreCase)))
        {            
            await _next(context); // skip middleware
            return;
        }

        if (context.Request.Cookies.TryGetValue("token", out var cookieValue))
            {
                try
                {
                    var parsedUserId = await ParseCookie(cookieValue);
                    context.Items["userId"] = parsedUserId;
                }
                catch (Exception)
                {
                    // Parsing failed (e.g., invalid format, expired token, etc.)
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Expired cookie or invalid token");
                    return; // stop the pipeline
                }

            }        

        await _next(context);
    }

    private async Task<string> ParseCookie(string cookieValue)
    {
        // load credentials
        if (cookieValue == null)
        {
            throw new ArgumentNullException(nameof(cookieValue), "Cookie value cannot be null");
        }

        try
        {
            var user = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(cookieValue);

            return user.Uid;
        }
        catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.ExpiredIdToken)
        {
            throw new UnauthorizedAccessException("Token expired", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to parse cookie", ex);
        }

    }
}
