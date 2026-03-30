using CustomerManagementApi.Application.ResponseModel;
using CustomerManagementApi.Application.ValueObjects;
using CustomerManagementApi.Domain.Exceptions;
using MongoDB.Driver;

namespace CustomerManagementApi.Api.Middlewares;

/// <summary>
/// Middleware que trata exceções não capturadas na aplicação
/// </summary>
[ExcludeFromCodeCoverage]
public class ExceptionMiddleware(RequestDelegate next)
{
    const string DEFAULT_EXCEPTION = "Ocorreu um erro inesperado";
    const string SERVICE_UNAVAILABLE = "O serviço está temporariamente indisponível, tente novamente mais tarde";
    const string CANCELED_EXCEPTION = "A solicitacao foi cancelada";
    const string CONFLICT_EXCEPTION = "Não foi possível processar a solicitação devido a um conflito com o estado atual do recurso";

    readonly RequestDelegate _next = next;

    /// <summary>
    /// Executa o middleware
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException domainException)
        {
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new ErrorModel(nameof(DomainException), domainException.Message));
            context.Items.Add(nameof(Exception), domainException);
        }
        catch (ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new ErrorModel(validationException.Notifications));
            context.Items.Add(nameof(Exception), validationException);
        }
        catch (KeyNotFoundException keyNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new ErrorModel("NotFound", keyNotFoundException.Message));
            context.Items.Add(nameof(Exception), keyNotFoundException);
        }
        catch (Exception exception)
        {
            var (statusCode, errorMessage) = exception switch
            {
                MongoWriteException => (StatusCodes.Status409Conflict, CONFLICT_EXCEPTION),
                OperationCanceledException => (StatusCodes.Status499ClientClosedRequest, CANCELED_EXCEPTION),
                HttpRequestException httpRequestException when httpRequestException.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable => (StatusCodes.Status503ServiceUnavailable, SERVICE_UNAVAILABLE),
                MongoConnectionException => (StatusCodes.Status503ServiceUnavailable, SERVICE_UNAVAILABLE),
                _ => (StatusCodes.Status500InternalServerError, DEFAULT_EXCEPTION)
            };

            await HandlingExceptionAsync(context, statusCode, errorMessage);
            context.Items.Add(nameof(Exception), exception);
        }
    }

    private static Task HandlingExceptionAsync(HttpContext context,
        int statusCodes,
        string error)
    {
        context.Response.StatusCode = statusCodes;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(new ErrorModel("Internal server error", error));
    }
}
