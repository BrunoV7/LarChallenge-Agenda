using Agenda.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Middleware
{
    internal sealed class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService
        ) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Exceção não tratada");

            var (status, detail) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
                ConflictException => (StatusCodes.Status409Conflict, exception.Message),
                ArgumentNullException => (StatusCodes.Status500InternalServerError, "Ocorreu um erro interno no servidor."),
                ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
                DbUpdateException => (StatusCodes.Status409Conflict, "Registro duplicado."),
                _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro interno no servidor.")
            };

            httpContext.Response.StatusCode = status;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = status,
                    Type = exception.GetType().Name,
                    Title = "Ocorreu um erro",
                    Detail = detail
                }
            });
        }
    }
}