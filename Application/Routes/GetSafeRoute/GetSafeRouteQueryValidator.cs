using FluentValidation;

namespace Application.Routes.GetSafeRoute;

public sealed class GetSafeRouteQueryValidator : AbstractValidator<GetSafeRouteQuery>
{
    public GetSafeRouteQueryValidator()
    {
        RuleFor(q => q.StationGraph).NotEmpty().WithMessage("Station graph is mandatory and should contain at least one node");
        RuleFor(q => q.Routes).NotEmpty().WithMessage("Request should contain at least one route");
        RuleFor(q => q.CheckRoute)
            .NotEmpty().WithMessage("Rerquested route is mandatory")
            .Must(rr => rr.StartNodeName != null && rr.StartNodeName != "").WithMessage("Requested route start node name is mandatory")
            .Must(rr => rr.EndNodeName != null && rr.EndNodeName != "").WithMessage("Requested route end node name is mandatory")
            .Must(rr => rr.StartNodeName != rr.EndNodeName).WithMessage("Requested route begin and end node should differ");
        RuleFor(q => q).Must(q =>
        {
            var distinctNodeNamesInStationGraph = GetDistinctNodeNamesInStationGraph(q);            
            var distinctNodeNamesInRoutes = GetDistinctNodeNamesInRoutes(q);
            return !distinctNodeNamesInRoutes.Except(distinctNodeNamesInStationGraph).Any();
        }).WithMessage("All route nodes should exist in station graph");

    }

    private static IEnumerable<string> GetDistinctNodeNamesInRoutes(GetSafeRouteQuery query)
    {
        List<string> nodeNamesInRoutes = new();
        foreach (var routeDto in query.Routes)
        {
            nodeNamesInRoutes.AddRange(routeDto.Nodes.Select(r => r.Value));
        }
        return nodeNamesInRoutes.Distinct();
    }

    private static IEnumerable<string> GetDistinctNodeNamesInStationGraph(GetSafeRouteQuery query)
    {
        var nodeNamesInStationGraph = query.StationGraph.Select(sgn => sgn.Node1Name).ToList();
        nodeNamesInStationGraph.AddRange(query.StationGraph.Select(sgn => sgn.Node2Name));
        return nodeNamesInStationGraph.Distinct();
    }
}
