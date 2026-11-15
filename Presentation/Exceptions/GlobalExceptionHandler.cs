using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ValidationException =>
                (400, "Error de validacion", exception.Message),

            ArgumentException =>
                (400, "Solicitud Invalida", exception.Message),

            KeyNotFoundException =>
                (404, "Recurso no encontrado", exception.Message),

            DbUpdateException =>
                (409, "Conflicto de integridad", "No se pudo completar la operación debido a un conflicto con los datos existentes."),

            InvalidOperationException =>
                (409, "Operacion no permitida", exception.Message),

            _ =>
                (500, "Error interno", "Ocurrio un error inesperado en el servidor")
        };

        if(statusCode == 500)
        {
            _logger.LogError(
                exception,
                "Error inesperado al procesar {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
