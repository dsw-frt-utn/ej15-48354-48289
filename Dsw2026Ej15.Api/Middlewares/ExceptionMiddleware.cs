using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Ej15.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ValidationException ex) // Captura validaciones de negocio
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new { Error = ex.Message });
        }
        catch (Exception ex) // Captura cualquier otro error
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/problem+json";
            
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Error interno del servidor",
                Detail = ex.Message
            };
            
            await httpContext.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}