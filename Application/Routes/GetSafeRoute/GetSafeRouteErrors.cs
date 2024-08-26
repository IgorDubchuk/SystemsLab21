using SharedKernel;

namespace Domain.StationGraphEdges;

public static class GetSafeRouteErrors
{
    public static Error CreatingStationGraphNodesError(IEnumerable<string> errors) => new("GetSafeRoute.CreatingStationGraphNodesError", "Unable to create station graph routes form query: "+ Environment.NewLine + String.Join(Environment.NewLine, errors));
    public static Error CreatingStationGraphEdgesError(IEnumerable<string> errors) => new("GetSafeRoute.CreatingStationGraphEdgesError", "Unable to create station graph edges form query: " + Environment.NewLine + String.Join(Environment.NewLine, errors));
    public static Error CreatingRoutesError(IEnumerable<string> errors) => new("GetSafeRoute.CreatingRoutesError", "Unable to create routes form query: " + Environment.NewLine + String.Join(Environment.NewLine, errors));
    public static readonly Error RequestedRouteNodeDoesNotExist = new("GetSafeRoute.RequestedRouteNodeDoesNotExist", "Requested route node does not exist in station graph");
}