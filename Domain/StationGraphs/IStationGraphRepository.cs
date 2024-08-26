using Domain.StationGraphEdges;
using SharedKernel;

namespace Domain.StationGraphs
{
    public interface IStationGraphRepository
    {
        Task<StationGraph?> GetStationGraphAsync(CancellationToken cancellationToken);
        Task<Result<StationGraphEdge?>> GetEdgeByNodesAsync(Guid node1Id, Guid node2Id, CancellationToken cancellationToken);
        Task SaveStationGraph(StationGraph stationGraph, CancellationToken cancellationToken);
    }
}
