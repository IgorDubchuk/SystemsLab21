using Domain.RouteNodes;
using Domain.Routes;
using Domain.StationGraphEdges;
using Domain.StationGraphNodes;
using Domain.StationGraphs;
using FluentAssertions;
using NSubstitute;

namespace Domain.UnitTests
{
    public class RouteServiceTests
    {
        private readonly RouteService _routeService;
        private readonly IRouteRepository _routeRepositoryMock;
        private readonly IStationGraphNodeRepository _stationGraphNodeRepositoryMock;
        private readonly IStationGraphRepository _stationGraphRepositoryMock;

        public RouteServiceTests()
        {
            _routeRepositoryMock = Substitute.For<IRouteRepository>();
            _stationGraphNodeRepositoryMock = Substitute.For<IStationGraphNodeRepository>();
            _stationGraphRepositoryMock = Substitute.For<IStationGraphRepository>();
            _routeService = new RouteService(_routeRepositoryMock);
        }

        [Fact]
        public async Task GetSafeRouteAsync_Should_ReturnError_WhenNoRootAtAll()
        {            
            //Arrange
            _routeRepositoryMock
                .GetRoutesFromNode1toNode2(Guid.NewGuid(), Guid.NewGuid(), default)
                .Returns(new List<Route>());

            //Act
            var result = await _routeService.GetSafeRouteAsync(Guid.NewGuid(), Guid.NewGuid(), default);

            //Assert
            result.Error.Should().Be(RouteErrors.NoRouteFromNode1ToNode2);
        }

        [Fact]
        public async Task GetSafeRouteAsync_Should_ReturnRoute_WhenEverythingIsOk()
        {
            //Arrange
            _stationGraphNodeRepositoryMock
                .CheckExistanceAsync(default, default)
                .ReturnsForAnyArgs(true);            

            var stationGraphNodes = (new string[] { "node0", "node1", "node2" }).Select(n => StationGraphNode.Create(n).Value).ToList();
            var stationGraphEdges = new StationGraphEdge[]
            {
                (await StationGraphEdge.Create(new StationGraphEdge.CreateRequest(stationGraphNodes[0], stationGraphNodes[1], _stationGraphNodeRepositoryMock), default)).Value,
                (await StationGraphEdge.Create(new StationGraphEdge.CreateRequest(stationGraphNodes[0], stationGraphNodes[2], _stationGraphNodeRepositoryMock), default)).Value
            };
            var stationGraph = StationGraph.Create(stationGraphEdges).Value;

            _stationGraphRepositoryMock.GetStationGraphAsync(default).Returns(stationGraph);

            var routes = await Task.WhenAll(stationGraphEdges.Select(async edge => (await Route.Create(
                [
                    (await RouteNode.Create(new RouteNode.CreateRequest(edge.Node1, 1, _stationGraphNodeRepositoryMock), default)).Value,
                    (await RouteNode.Create(new RouteNode.CreateRequest(edge.Node2, 2, _stationGraphNodeRepositoryMock), default)).Value
                ], _stationGraphRepositoryMock, default)).Value));            

            var requestedRouteStartNodeId = routes[0].Nodes[0].Id;
            var requestedRouteEndNodeId = routes[0].Nodes[1].Id;

            _routeRepositoryMock
                .GetRoutesFromNode1toNode2(requestedRouteStartNodeId, requestedRouteEndNodeId, default)
                .Returns(routes);

            //Act
            var result = await _routeService.GetSafeRouteAsync(requestedRouteStartNodeId, requestedRouteEndNodeId, default);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(routes[0].Id);
        }
    }
}