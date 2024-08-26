using SharedKernel;

namespace Domain.StationGraphs;

public static class StationGraphErrors
{
    public static readonly Error ZeroEdges = new("StationGraph.ZeroEdges", "Station graph should contain at least one edge");
    public static readonly Error DuplicateEdges = new("StationGraph.DuplicatedEdges", "Station graph should not contain duplicate edges");
}