using Domain.Routes;

namespace Infrastructure.Repositories.Routes
{
    public sealed class OneTimeRouteRepository : IRouteRepository
    {
        private List<Route> Routes = new();

        public Task SaveRoutes(IEnumerable<Route> routes, CancellationToken cancellationToken)
        {
            Routes.AddRange(routes);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Route>> GetRoutesFromNode1toNode2(Guid node1Id, Guid node2Id, CancellationToken cancellationToken)
        {
            var result = Routes.Where(r =>
                r.Nodes.Single(n => n.OrderNumber == 1).Node.Id == node1Id
                &&
                r.Nodes.Single(n => n.OrderNumber == r.Nodes.Count()).Node.Id == node2Id);
            return Task.FromResult(result);
        }
    }
}
