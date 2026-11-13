using Microsoft.AspNetCore.Mvc;
using SaskaituAgentas.Api.Services;

namespace SaskaituAgentas.Api.Controllers;

internal static class ServiceResultExtensions
{
    public static ActionResult ToProblem<T>(this ControllerBase controller, ServiceResult<T> result) =>
        result.Error switch
        {
            ServiceError.NotFound => controller.Problem(result.Message, statusCode: StatusCodes.Status404NotFound),
            ServiceError.Conflict => controller.Problem(result.Message, statusCode: StatusCodes.Status409Conflict),
            ServiceError.Validation => controller.Problem(result.Message, statusCode: StatusCodes.Status400BadRequest),
            ServiceError.Unauthorized => controller.Problem(result.Message, statusCode: StatusCodes.Status401Unauthorized),
            _ => controller.Problem(result.Message),
        };
}
