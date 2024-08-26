namespace Application.Dto
{
    public sealed record RerquestedRouteDto(
        string StartNodeName,
        string EndNodeName);
}