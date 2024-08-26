using SharedKernel;

namespace Domain.StationGraphNodes
{
    public sealed class StationGraphNode
    {
        private StationGraphNode(Guid id, string name, bool isOccupied)
        {
            Id = id;
            Name = name;
            IsOccupied = isOccupied;
        }

        public Guid Id { get; }
        public string Name { get; }
        public bool IsOccupied { get; private set; }

        public static Result<StationGraphNode> Create(string name)
        {
            if (name == null || name == "")
                return Result.Failure<StationGraphNode>(StationGraphNodeErrors.NameIsNullOrEmpty);

            var entity = new StationGraphNode(Guid.NewGuid(), name, false);
            return entity;
        }

        public Result SetOccupied()
        {
            if (IsOccupied)
                return StationGraphNodeErrors.AllreadyOccupied;
            IsOccupied = true;
            return Result.Success();
        }
    }
}
