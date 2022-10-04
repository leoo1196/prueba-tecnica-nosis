using Core.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApi.Middleware;

public static class ExceptionHandler
{
    private static readonly JsonSerializerSettings _serializerSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public static WebApplication ConfigureExceptionHandler(this WebApplication application)
    {
        application.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (exceptionHandlerFeature is not null)
                {
                    var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();

                    loggerFactory
                        .CreateLogger("GlobalExceptionHandler")
                        .LogError(exceptionHandlerFeature.Error, "An exception has occurred");
                }

                context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(
                    JsonConvert.SerializeObject(new GenericError("Ocurrió un error"), _serializerSettings)
                );
            });
        });

        return application;
    }
}
