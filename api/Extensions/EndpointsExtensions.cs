using Microsoft.AspNetCore.Authorization;

namespace IotSmartHome.Extensions;

public static class EndpointsExtensions
{
    public static TBuilder RequireAdminAuthorization<TBuilder>(this TBuilder endpoints)
        where TBuilder : IEndpointConventionBuilder
        => endpoints.RequireAuthorization(new AuthorizeAttribute { Roles = AppConsts.RoleAdmin });
}