using Domain.StationGraphEdges;
using Domain.StationGraphs;
using SharedKernel;

namespace Infrastructure.Repositories.StationGraphs
{
    public sealed class OneTimeStationGraphRepository : IStationGraphRepository
    {
        private StationGraph? StationGraph;

        public Task<StationGraph?> GetStationGraphAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(StationGraph);
        }

        public Task<Result<StationGraphEdge?>> GetEdgeByNodesAsync(Guid node1Id, Guid node2Id, CancellationToken cancellationToken)
        {
            if (StationGraph == null)
                return Task.FromResult(Result.Failure<StationGraphEdge?>(OneTimeStationGraphRepositoryErrors.NoStationGraph));
            return Task.FromResult(Result.Success(StationGraph.Edges.FirstOrDefault(edge =>
                edge.Node1.Id == node1Id && edge.Node2.Id == node2Id
                ||
                edge.Node1.Id == node2Id && edge.Node2.Id == node1Id)));
        }

        public Task SaveStationGraph(StationGraph stationGraph, CancellationToken cancellationToken)
        {
            StationGraph = stationGraph;
            return Task.CompletedTask;
        }
    }
}
