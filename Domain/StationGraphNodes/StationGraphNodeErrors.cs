using SharedKernel;

namespace Domain.StationGraphNodes;

public static class StationGraphNodeErrors
{
    public static readonly Error AllreadyOccupied = new("StationGraphNode.AllreadyOccupied", "Station graph node is allready occupied");
    public static readonly Error NameIsNullOrEmpty = new("StationGraphNode.NameIsNullOrEmpty", "Station graph node name should contain at least one character");
}