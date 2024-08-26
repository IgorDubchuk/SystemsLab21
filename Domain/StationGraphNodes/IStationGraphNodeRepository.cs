namespace Domain.StationGraphNodes
{
    public interface IStationGraphNodeRepository
    {
        Task<IEnumerable<StationGraphNode>> GetAllNodesAsync(CancellationToken cancellationToken);
        Task<bool> CheckExistanceAsync(Guid id, CancellationToken cancellationToken);
        Task SaveStationGraphNodes(IEnumerable<StationGraphNode> stationGraphNodes, CancellationToken cancellationToken);
    }
}
