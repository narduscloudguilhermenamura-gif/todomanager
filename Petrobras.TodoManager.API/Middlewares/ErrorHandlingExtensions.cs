using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Petrobras.TodoManager.API.Middlewares;

public static class ErrorHandlingExtensions
{
    public static IApplicationBuilder UseStandardErrorHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(handlerApp =>
        {
            handlerApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                var isBusinessException = exception is ArgumentException;

                context.Response.StatusCode = isBusinessException
                    ? StatusCodes.Status400BadRequest
                    : StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = BuildProblemDetails(
                    context,
                    context.Response.StatusCode,
                    null,
                    isBusinessException
                        ? exception?.Message ?? "Falha na validação da regra de negócio."
                        : "Falha interna no servidor.");

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            });
        });

        app.UseStatusCodePages(async statusCodeContext =>
        {
            var response = statusCodeContext.HttpContext.Response;
            if (response.HasStarted || !string.IsNullOrWhiteSpace(response.ContentType))
            {
                return;
            }

            var message = response.StatusCode switch
            {
                StatusCodes.Status404NotFound => "Recurso não encontrado.",
                _ => "Falha na requisição."
            };

            response.ContentType = "application/json";
            var payload = BuildProblemDetails(
                statusCodeContext.HttpContext,
                response.StatusCode,
                null,
                message);
            await response.WriteAsync(JsonSerializer.Serialize(payload));
        });

        return app;
    }

    private static ProblemDetails BuildProblemDetails(HttpContext context, int statusCode, string? title, string detail)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title ?? ReasonPhrases.GetReasonPhrase(statusCode),
            Detail = detail,
            Instance = context.Request.Path.Value ?? string.Empty
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;
        return problem;
    }
}
