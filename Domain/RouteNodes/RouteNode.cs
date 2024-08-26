using Domain.StationGraphNodes;
using SharedKernel;

namespace Domain.RouteNodes
{
    public sealed class RouteNode
    {
        private RouteNode(Guid id, StationGraphNode node, int orderPosition)
        {
            Id = id;
            Node = node;
            OrderNumber = orderPosition;
        }

        public Guid Id { get; }
        public StationGraphNode Node { get; }
        public int OrderNumber { get; }

        public sealed record CreateRequest(StationGraphNode Node, int OrderPosition, IStationGraphNodeRepository StationGraphNodeRepository);

        public static async Task<Result<RouteNode>> Create(CreateRequest request, CancellationToken cancellationToken)
        {
            if (!await request.StationGraphNodeRepository.CheckExistanceAsync(request.Node.Id, cancellationToken))
                return Result.Failure<RouteNode>(RouteNodeErrors.UnexistedNode);

            var entity = new RouteNode(Guid.NewGuid(), request.Node, request.OrderPosition);

            return Result.Success(entity);
        }
    }
}
