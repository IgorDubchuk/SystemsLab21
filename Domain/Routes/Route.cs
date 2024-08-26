using Domain.RouteNodes;
using Domain.StationGraphs;
using SharedKernel;

namespace Domain.Routes
{
    public sealed class Route
    {
        private Route(Guid id, IEnumerable<RouteNode> nodes, bool isOccupied)
        {
            Id = id;
            Nodes = nodes.ToList();
            IsOccupied = isOccupied;
        }

        public Guid Id { get; }
        public List<RouteNode> Nodes { get; }
        public bool IsOccupied { get; private set; }

        public static async Task<Result<Route>> Create(
            IEnumerable<RouteNode> nodes, 
            IStationGraphRepository stationGraphRepository, 
            CancellationToken cancellationToken)
        {
            var invariantsCheckingResult = await CheckInvariants(nodes, stationGraphRepository, cancellationToken);
            if (invariantsCheckingResult.IsFailure)
                return Result.Failure<Route>(invariantsCheckingResult.Error);

            var entity = new Route(Guid.NewGuid(), nodes, false);

            return Result.Success(entity);
        }

        public Result SetOccupied()
        {
            this.IsOccupied = true;
            foreach (var routNode in Nodes)
            {
                var nodeOccupationResult = routNode.Node.SetOccupied();
                if (nodeOccupationResult.IsFailure)
                    return RouteErrors.CanNotOccupy(nodeOccupationResult.Error.Description);
            }
            return Result.Success();
        }

        public bool IsFree()
        {
            return !IsOccupied && !Nodes.Any(n => n.Node.IsOccupied);
        }

        private static async Task<Result> CheckInvariants(
            IEnumerable<RouteNode> routeNodes,
            IStationGraphRepository stationGraphRepository,
            CancellationToken cancellationToken)
        {
            if (routeNodes == null || routeNodes.Count() < 2)
                return RouteErrors.LessThatTwoNodes;

            if (ContainsDoubleNodes(routeNodes))
                return RouteErrors.ContainsDuplicateNodes;

            if (!RouteNodesAreCorrectlyNumerated(routeNodes))
                return RouteErrors.IncorrectNodesNumeration;

            var complianceWithStationGraphCheckResult = await IsCompliantWithStationGraph(routeNodes, stationGraphRepository, cancellationToken);
            if (complianceWithStationGraphCheckResult.IsFailure)
                return complianceWithStationGraphCheckResult.Error;

            return Result.Success();
        }

        private static bool ContainsDoubleNodes(IEnumerable<RouteNode> nodes)
        {
            return nodes.Select(n => n.Id).Distinct().Count() != nodes.Count();
        }

        private static async Task<Result> IsCompliantWithStationGraph(
            IEnumerable<RouteNode> routeNodes, 
            IStationGraphRepository stationGraphRepository, 
            CancellationToken cancellationToken)
        {
            var stationGraph = await stationGraphRepository.GetStationGraphAsync(cancellationToken);
                if (stationGraph == null) return RouteErrors.NoStationGraph;
            List<RouteNode> orderedRouteNodes = [.. routeNodes.OrderBy(n => n.OrderNumber)];
            RouteNode node1;
            RouteNode node2;
            for (int i = 1; i < orderedRouteNodes.Count(); i++)
            {
                node1 = orderedRouteNodes[i - 1];
                node2 = orderedRouteNodes[i];
                if (!stationGraph.CheckNodesConnection(node1.Node.Id, node2.Node.Id).Value)
                    return RouteErrors.NotConnectedNodes;
            }
            return Result.Success();
        }

        private static bool RouteNodesAreCorrectlyNumerated(IEnumerable<RouteNode> nodes)
        {
            if (nodes.Min(n => n.OrderNumber) == 1
                &&
                nodes.Max(n => n.OrderNumber) == nodes.Count()
                &&
                nodes.Select(n => n.OrderNumber).Distinct().Count() == nodes.Count())
                return true;
            return false;
        }
    }
}
