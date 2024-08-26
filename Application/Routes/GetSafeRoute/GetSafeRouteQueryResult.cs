using Application.Dto;

namespace Application.Routes.GetSafeRoute
{
    public sealed record GetSafeRouteQueryResult(
        bool Success,
        SafeRouteDto? SafeRoute);
}
