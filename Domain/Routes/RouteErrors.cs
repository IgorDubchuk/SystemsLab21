using SharedKernel;

namespace Domain.Routes;

public static class RouteErrors
{
    public static readonly Error LessThatTwoNodes = new("Route.LessThatTwoNodes", "A route should have at least two nodes");
    public static readonly Error NotConnectedNodes = new("Route.NotConnectedNodes", "All route nodes should be consequently connected to each other (have correspondent edges in the station graph)");
    public static readonly Error ContainsDuplicateNodes = new("Route.ContainsDuplicateNodes", "A route should not have duplicate route nodes");
    public static readonly Error NoStationGraph = new("Route.NoStationGraph", "Can not check the route for compliance with station graph: no station graph found");
    public static readonly Error IncorrectNodesNumeration = new("Route.IncorrectNodesNumeration", "Incorrect node numeration: numbers should start from 1, be unique and have increment 1");
    public static readonly Error NoRouteFromNode1ToNode2 = new("Route.NoRouteFromNode1ToNode2", "There is no route from node 1 to node 2");
    public static readonly Error NoFreeRouteFromNode1ToNode2 = new("Route.NoFreeRouteFromNode1ToNode2", "There is no free route from node 1 to node 2");
    public static Error CanNotOccupy(string reason) => new("Route.CanNotOccupy", $"A route can not be occupied: {reason}");
}