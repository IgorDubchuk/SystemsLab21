using Domain.StationGraphNodes;

namespace Infrastructure.Repositories.StationGraphNodes
{
    public sealed class OneTimeStationGraphNodeRepository : IStationGraphNodeRepository
    {
        private List<StationGraphNode> StationGraphNodes = new();

        public Task<bool> CheckExistanceAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = StationGraphNodes.Any(n => n.Id == id);
            return Task.FromResult(result);
        }

        public Task<IEnumerable<StationGraphNode>> GetAllNodesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((IEnumerable<StationGraphNode>)StationGraphNodes);
        }

        public Task SaveStationGraphNodes(IEnumerable<StationGraphNode> stationGraphNodes, CancellationToken cancellationToken)
        {
            StationGraphNodes.AddRange(stationGraphNodes);
            return Task.CompletedTask;
        }
    }
}
