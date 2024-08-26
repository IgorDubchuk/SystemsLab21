using SharedKernel;

namespace Infrastructure.Repositories.StationGraphs;

public static class OneTimeStationGraphRepositoryErrors
{
    public static readonly Error NoStationGraph = new("OneTimeStationGraphRepository.NoStationGraph", "Can not check edge existence: no station graph found");
}