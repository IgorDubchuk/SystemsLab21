using Domain.StationGraphNodes;
using Domain.StationGraphs;
using SharedKernel;

namespace Domain.StationGraphEdges
{
    public sealed class StationGraphEdge
    {
        private StationGraphEdge(Guid id, StationGraphNode node1, StationGraphNode node2)
        {
            Id = id;
            Node1 = node1;
            Node2 = node2;
        }

        public Guid Id { get; }
        public StationGraphNode Node1 { get; }
        public StationGraphNode Node2 { get; }

        public sealed record CreateRequest(StationGraphNode Node1,
            StationGraphNode Node2,
            IStationGraphRepository StationGraphRepository,
            IStationGraphNodeRepository StationGraphNodeRepository);

        public static async Task<Result<StationGraphEdge>> Create(CreateRequest request, CancellationToken cancellationToken)
        {
            var invariantsCheckengResult = await CheckInvariants(request, cancellationToken);

            if (invariantsCheckengResult.IsFailure)
                return Result.Failure<StationGraphEdge>(invariantsCheckengResult.Error);

            var entity = new StationGraphEdge(Guid.NewGuid(), request.Node1, request.Node2);

            return Result.Success(entity);
        }

        private static async Task<Result> CheckInvariants(CreateRequest request, CancellationToken cancellationToken)
        {
            if (request.Node1.Id == request.Node2.Id)
                return StationGraphEdgeErrors.DuplicateNodes;

            if (!(await request.StationGraphNodeRepository.CheckExistanceAsync(request.Node1.Id, cancellationToken))
                ||
                !(await request.StationGraphNodeRepository.CheckExistanceAsync(request.Node2.Id, cancellationToken)))
                return StationGraphEdgeErrors.UnexistingNode;

            return Result.Success();
        }
    }
}
