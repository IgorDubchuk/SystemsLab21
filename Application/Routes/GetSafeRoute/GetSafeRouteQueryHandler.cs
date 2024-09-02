using Domain.Routes;
using SharedKernel;
using MediatR;
using Application.Dto;
using FluentValidation;
using Domain.StationGraphEdges;
using Domain.StationGraphNodes;
using Domain.StationGraphs;
using Domain.RouteNodes;

namespace Application.Routes.GetSafeRoute;

public sealed record GetSafeRouteQuery(
    IEnumerable<StationGraphEdgeDto> StationGraph,
    IEnumerable<RouteDto> Routes,
    RerquestedRouteDto CheckRoute
    ) : IRequest<Result<GetSafeRouteQueryResult>>;

internal sealed class GetSafeRouteQueryHandler : IRequestHandler<GetSafeRouteQuery, Result<GetSafeRouteQueryResult>>
{
    private readonly IRouteService _routeService;
    private readonly IStationGraphNodeRepository _stationGraphNodeRepository;
    private readonly IStationGraphRepository _stationGraphRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IValidator<GetSafeRouteQuery> _validator;

    public GetSafeRouteQueryHandler(
        IRouteService routeService,
        IStationGraphNodeRepository stationGraphNodeRepository,
        IStationGraphRepository stationGraphRepository,
        IRouteRepository routeRepository,
        IValidator<GetSafeRouteQuery> validator)
    {
        _routeService = routeService;
        _stationGraphNodeRepository = stationGraphNodeRepository;
        _stationGraphRepository = stationGraphRepository;
        _routeRepository = routeRepository;
        _validator = validator;
    }


    public async Task<Result<GetSafeRouteQueryResult>> Handle(GetSafeRouteQuery query, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(query);
        if (!validationResult.IsValid)
            return Result.Failure<GetSafeRouteQueryResult>(CommonErrors.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));

        #region Creating station graph nodes from query and saving to repository
        var stationGraphNodesCreationResults = CreateStationGraphNodesFromQuery(query);

        if (stationGraphNodesCreationResults.Any(r => r.IsFailure))
        {
            var errors = stationGraphNodesCreationResults.Where(r => r.IsFailure).Select(r => r.Error.Description);
            return Result.Failure<GetSafeRouteQueryResult>(GetSafeRouteErrors.CreatingStationGraphNodesError(errors));
        }

        var createdStationGraphNodes = stationGraphNodesCreationResults.Select(r => r.Value).ToList();

        await _stationGraphNodeRepository.SaveStationGraphNodes(createdStationGraphNodes, cancellationToken);
        #endregion

        #region Creating station graph from query and saving to repository
        var stationGraphEdgesCreationResults = await CreateStationGraphEdgesFromQuery(
            query,
            createdStationGraphNodes,
            _stationGraphNodeRepository,
            _stationGraphRepository,
            cancellationToken);

        if (stationGraphEdgesCreationResults.Any(r => r.IsFailure))
        {
            var errors = stationGraphEdgesCreationResults.Where(r => r.IsFailure).Select(r => r.Error.Description);
            return Result.Failure<GetSafeRouteQueryResult>(GetSafeRouteErrors.CreatingStationGraphEdgesError(errors));
        }

        var createdStationGraphEdges = stationGraphEdgesCreationResults.Select(r => r.Value);

        var stationGraphCreationResult = StationGraph.Create(createdStationGraphEdges);

        if (stationGraphCreationResult.IsFailure)
            return Result.Failure<GetSafeRouteQueryResult>(stationGraphCreationResult.Error);

        await _stationGraphRepository.SaveStationGraph(stationGraphCreationResult.Value, cancellationToken);
        #endregion

        #region Creating routes from query and saving to repository
        var routesCreatingResult = await CreateRoutesFromQuery(query, createdStationGraphNodes, cancellationToken);

        if (routesCreatingResult.IsFailure)
            return Result.Failure<GetSafeRouteQueryResult>(routesCreatingResult.Error);

        await _routeRepository.SaveRoutes(routesCreatingResult.Value, cancellationToken);
        #endregion


        var startNode = GetStartNodeFromQuery(query, createdStationGraphNodes);
        if (startNode == null)
            return Result.Failure<GetSafeRouteQueryResult>(GetSafeRouteErrors.RequestedRouteNodeDoesNotExist);
        
        var endNode = GetEndNodeFromQuery(query, createdStationGraphNodes);
        if (endNode == null)
            return Result.Failure<GetSafeRouteQueryResult>(GetSafeRouteErrors.RequestedRouteNodeDoesNotExist);

        var getSafeRouteResult = await _routeService.GetSafeRouteAsync(startNode.Id, endNode.Id, cancellationToken);

        if (getSafeRouteResult.IsFailure)
            if (getSafeRouteResult.Error == RouteErrors.NoFreeRouteFromNode1ToNode2)
                return Result.Success(new GetSafeRouteQueryResult(
                    Success: false,
                    SafeRoute: null));
            else
                return Result.Failure<GetSafeRouteQueryResult>(getSafeRouteResult.Error);

        return Result.Success(new GetSafeRouteQueryResult(
            Success: true,
            SafeRouteDto.GetFromRouteEntity(getSafeRouteResult.Value)));
    }

    #region Private methods
    private static StationGraphNode? GetStartNodeFromQuery(GetSafeRouteQuery query, IEnumerable<StationGraphNode> createdStationGraphNodes)
        => createdStationGraphNodes.SingleOrDefault(n => n.Name == query.CheckRoute.StartNodeName);

    private static StationGraphNode? GetEndNodeFromQuery(GetSafeRouteQuery query, IEnumerable<StationGraphNode> createdStationGraphNodes)
        => createdStationGraphNodes.SingleOrDefault(n => n.Name == query.CheckRoute.EndNodeName);

    private async Task<Result<List<Route>>> CreateRoutesFromQuery(GetSafeRouteQuery query, IEnumerable<StationGraphNode> createdStationGraphNodes, CancellationToken cancellationToken)
    {
        List<Route> createdRoutes = new();
        List <string> errors = new();
        foreach (var routeDto in query.Routes)
        {
            List<RouteNode> routeNodes = new(routeDto.Nodes.Count());
            List<string> errorsForRoute = new();
            foreach (var routeNodeDto in routeDto.Nodes)
            {
                var node = createdStationGraphNodes.Single(n => n.Name == routeNodeDto.Value);
                var routeNodeCreationResult = await RouteNode.Create(new RouteNode.CreateRequest(node, routeNodeDto.Key, _stationGraphNodeRepository), cancellationToken);
                if (routeNodeCreationResult.IsFailure)
                    errorsForRoute.Add(routeNodeCreationResult.Error.Description);
                else
                    routeNodes.Add(routeNodeCreationResult.Value);
            }
            if (errorsForRoute.Any())
                errors.AddRange(errorsForRoute);
            else
            {
                var routeCreationResult = await Route.Create(routeNodes, _stationGraphRepository, cancellationToken);
                if (routeCreationResult.IsFailure)
                    errors.Add(routeCreationResult.Error.Description);
                else
                {
                    createdRoutes.Add(routeCreationResult.Value);
                    if (routeDto.Occupied)
                        routeCreationResult.Value.SetOccupied();
                }
            }
        }
        if (errors.Any())
            return Result.Failure<List<Route>>(GetSafeRouteErrors.CreatingRoutesError(errors));
        else
            return createdRoutes;
    }

    private static async Task<List<Result<StationGraphEdge>>> CreateStationGraphEdgesFromQuery(
        GetSafeRouteQuery query,
        IEnumerable<StationGraphNode> createdNodes,
        IStationGraphNodeRepository stationGraphNodeRepository,
        IStationGraphRepository stationGraphRepository,
        CancellationToken cancellationToken)
    {
        List<Result<StationGraphEdge>> stationGraphEdgesCreationResults = new(query.StationGraph.Count());

        foreach (var edgeDto in query.StationGraph)
        {
            var node1 = createdNodes.Single(n => n.Name == edgeDto.Node1Name);
            var node2 = createdNodes.Single(n => n.Name == edgeDto.Node2Name);
            var edgeCreationResult = await StationGraphEdge.Create(new StationGraphEdge.CreateRequest(
                node1,
                node2,
                stationGraphNodeRepository), cancellationToken);
            stationGraphEdgesCreationResults.Add(edgeCreationResult);
        }

        return stationGraphEdgesCreationResults;
    }

    private static List<Result<StationGraphNode>> CreateStationGraphNodesFromQuery(GetSafeRouteQuery query)
    {
        HashSet<string> stationGraphNodesFromRequest = new(query.StationGraph.Count() * 2);
        foreach (var stationGraphEdgeDto in query.StationGraph)
        {
            stationGraphNodesFromRequest.Add(stationGraphEdgeDto.Node1Name);
            stationGraphNodesFromRequest.Add(stationGraphEdgeDto.Node2Name);
        }
        return stationGraphNodesFromRequest
            .Select(nodeName => StationGraphNode.Create(nodeName)).ToList();
    }
    #endregion
}
