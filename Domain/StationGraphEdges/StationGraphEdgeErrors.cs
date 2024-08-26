using SharedKernel;

namespace Domain.StationGraphEdges;

public static class StationGraphEdgeErrors
{
    public static readonly Error DuplicateNodes = new("StationGraphEdge.DuplicateNodes", "Station graph edge can not include two equal nodes");
    public static readonly Error UnexistingNode = new("StationGraphEdge.UnexistingNode", "Station graph edge can tie only existing nodes");
}