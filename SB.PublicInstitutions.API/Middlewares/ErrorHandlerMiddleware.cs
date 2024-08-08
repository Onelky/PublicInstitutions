using SB.PublicInstitutions.Domain.Exceptions;
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
                NotFoundException e => (int)HttpStatusCode.BadRequest,
                DuplicatedInstitutionName e => (int)HttpStatusCode.BadRequest,
                BadRequestException e => (int)HttpStatusCode.BadRequest,
                AuthenticationException e => (int)HttpStatusCode.BadRequest,
                Unauthorized e => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var result = JsonSerializer.Serialize(new { message = error?.Message ?? "Internal Server Error" });
            await response.WriteAsync(result);
        }
    }
}