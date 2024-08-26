using Domain.Routes;

namespace Application.Dto
{
    public sealed record RouteDto(
        IEnumerable<KeyValuePair<int, string>> Nodes,
        bool Occupied);
}