using Core.Errors;
using Core.Results;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebApi.Extensions;

public static class ResultExtensions
{
    public static IStatusCodeActionResult ToStatusCodeActionResult(this Result result)
    {
        return CreateStatusCodeResultFromError(result.Error);
    }

    public static IStatusCodeActionResult ToStatusCodeActionResult<TModel>(this Result<TModel> result)
    {
        if (result.Error is not null)
            return CreateStatusCodeResultFromError(result.Error);

        return new OkObjectResult(result.Model);
    }

    internal static IStatusCodeActionResult CreateStatusCodeResultFromError(Error? error) => error switch
    {
        UnauthorizedError => CreateStatusCodeResult(error, HttpStatusCode.Unauthorized),
        BadRequestError or GenericError => CreateStatusCodeResult(error, HttpStatusCode.BadRequest),
        ConflictError => CreateStatusCodeResult(error, HttpStatusCode.Conflict),
        NotFoundError => CreateStatusCodeResult(error, HttpStatusCode.NotFound),
        _ => new OkResult()
    };

    internal static IStatusCodeActionResult CreateStatusCodeResult(object value, HttpStatusCode statusCode) => new ObjectResult(value)
    {
        StatusCode = (int)statusCode
    };
}
