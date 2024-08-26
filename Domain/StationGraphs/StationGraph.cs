using Domain.StationGraphEdges;
using SharedKernel;

namespace Domain.StationGraphs
{
    public sealed class StationGraph
    {
        private StationGraph(Guid id, IEnumerable<StationGraphEdge> nodes)
        {
            Id = id;
            Edges = nodes.ToList();
        }

        public Guid Id { get; }
        public List<StationGraphEdge> Edges { get; }

        public static Result<StationGraph> Create(IEnumerable<StationGraphEdge> edges)
        {
            if (edges == null || !edges.Any())
                return Result.Failure<StationGraph>(StationGraphErrors.ZeroEdges);

            if (edges.Select(edge => edge.Id).Distinct().Count() != edges.Count())
                return Result.Failure<StationGraph>(StationGraphErrors.DuplicateEdges);

            var entity = new StationGraph(Guid.NewGuid(), edges);

            return Result.Success(entity);
        }

        public Result<bool> CheckNodesConnection(Guid node1Id, Guid node2Id)
        {
            bool connectionExists = this.Edges.Any(edge =>
                (edge.Node1.Id == node1Id && edge.Node2.Id == node2Id)
                ||
                (edge.Node1.Id == node2Id && edge.Node2.Id == node1Id));
            return Result.Success(connectionExists);
        }
    }
}
