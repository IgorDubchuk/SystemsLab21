using Domain.Routes;

namespace Application.Dto
{
    public sealed record SafeRouteDto(
        IEnumerable<KeyValuePair<int, string>> Nodes)
    {
        public static SafeRouteDto GetFromRouteEntity(Route route)
        {
            return new SafeRouteDto(route.Nodes.Select(n => new KeyValuePair<int, string>(n.OrderNumber, n.Node.Name)));
        }
    }
}