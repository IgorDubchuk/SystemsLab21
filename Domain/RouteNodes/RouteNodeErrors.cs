using SharedKernel;

namespace Domain.RouteNodes;

public static class RouteNodeErrors
{
    public static readonly Error UnexistedNode = new("RouteNode.UnexistedNode", "A route node can point only to an existing station graph node");
}