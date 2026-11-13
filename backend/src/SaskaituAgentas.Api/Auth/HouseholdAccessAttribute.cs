using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SaskaituAgentas.Api.Auth;

/// <summary>
/// Leidžia veiksmą tik tada, kai maršruto parametras <paramref name="routeParameter"/> sutampa su
/// prisijungusio vartotojo namų ūkiu. Svetimas namų ūkis grąžina 404, kad nebūtų atskleista, jog jis egzistuoja.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HouseholdAccessAttribute(string routeParameter) : Attribute, IAuthorizationFilter
{
    public bool RequireOwner { get; init; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user.Identity?.IsAuthenticated != true) return; // [Authorize] grąžins 401

        if (!Guid.TryParse(context.RouteData.Values[routeParameter]?.ToString(), out var householdId)
            || householdId != user.GetHouseholdId())
        {
            context.Result = new NotFoundResult();
            return;
        }

        if (RequireOwner && !user.IsHouseholdOwner())
            context.Result = new ForbidResult();
    }
}
