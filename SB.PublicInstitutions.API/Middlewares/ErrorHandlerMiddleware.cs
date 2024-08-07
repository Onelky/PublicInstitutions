using System.Net;
using System.Security.Authentication;
using System.Text.Json;

public class ErrorHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = error switch
            {
                KeyNotFoundException e => (int)HttpStatusCode.NotFound,
                AuthenticationException e => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException e => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var result = JsonSerializer.Serialize(new { message = error?.Message ?? "Internal Server Error" });
            await response.WriteAsync(result);
        }
    }
}