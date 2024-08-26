using SharedKernel;

namespace Domain.Routes
{
    public interface IRouteService
    {
        Task<Result<Route>> GetSafeRoute(Guid node1Id, Guid node2Id, CancellationToken cancellationToken);
    }
}