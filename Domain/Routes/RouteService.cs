using SharedKernel;

namespace Domain.Routes
{
    public sealed class RouteService : IRouteService
    {
        private readonly IRouteRepository _routeRepository;

        public RouteService(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }

        public async Task<Result<Route>> GetSafeRouteAsync(Guid node1Id, Guid node2Id, CancellationToken cancellationToken)
        {
            var routesFromNode1ToNode2 = await _routeRepository.GetRoutesFromNode1toNode2(node1Id, node2Id, cancellationToken);
            if (routesFromNode1ToNode2 == null || !routesFromNode1ToNode2.Any())
                return Result.Failure<Route>(RouteErrors.NoRouteFromNode1ToNode2);

            var freeRoutesFromNode1ToNode2 = routesFromNode1ToNode2.Where(r => r.IsFree());
            if (freeRoutesFromNode1ToNode2 == null || !freeRoutesFromNode1ToNode2.Any())
                return Result.Failure<Route>(RouteErrors.NoFreeRouteFromNode1ToNode2);

            var bestSafeRoute = BestRoute(freeRoutesFromNode1ToNode2);

            return bestSafeRoute;
        }

        private static Route BestRoute(IEnumerable<Route> routes)
        {
            return routes.MinBy(r => r.Nodes.Count)!;
        }

    }
}
