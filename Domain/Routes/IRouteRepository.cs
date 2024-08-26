
namespace Domain.Routes
{
    public interface IRouteRepository
    {
        Task<IEnumerable<Route>> GetRoutesFromNode1toNode2(Guid node1Id, Guid node2Id, CancellationToken cancellationToken);
        Task SaveRoutes(IEnumerable<Route> routes, CancellationToken cancellationToken);
    }
}