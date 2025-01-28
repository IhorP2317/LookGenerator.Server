using FluentValidation;
using LookGenerator.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LookGenerator.WebAPI.ExceptionHandlers ;

    public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(
                "Error Message: {exceptionMessage}, Time of occurrence: {time}",
                exception.Message, DateTime.UtcNow);

            (string Detail, string Title, int StatusCode) details = exception switch
            {
                CustomHttpRequestException ex =>
                    (
                        ex.Message,
                            ex.GetType().Name,
                            httpContext.Response.StatusCode = (int)ex.StatusCode
                        ),
                InternalServerException =>
                    (
                        exception.Message,
                            exception.GetType().Name,
                            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError
                        ),
                ValidationException =>
                    (
                        exception.Message,
                            exception.GetType().Name,
                            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest
                        ),
                BadRequestException =>
                    (
                        exception.Message,
                            exception.GetType().Name,
                            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest
                        ),
                NotFoundException =>
                    (
                        exception.Message,
                            exception.GetType().Name,
                            httpContext.Response.StatusCode = StatusCodes.Status404NotFound
                        ),
                UnauthorizedException =>
                    (
                        exception.Message,
                            exception.GetType().Name,
                            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized
                        ),
                AccessForbiddenException =>
                    (
                        exception.Message,
                            exception.GetType().Name,
                            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden
                        ),
                _ =>
                    (
                        // Fallback for unhandled exceptions
                        exception.Message,
                            exception.GetType().Name,
                            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError
                        )
                };

            // Build the ProblemDetails object
            var problemDetails = new ProblemDetails
            {
                Title = details.Title,
                Detail = details.Detail,
                Status = details.StatusCode,
                Instance = httpContext.Request.Path
            };

           
            problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);
            
            if (exception is ValidationException validationException)
            {
                problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);
            }

            // Write to response as JSON
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

            return true;
        }
    }