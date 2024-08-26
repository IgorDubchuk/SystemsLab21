using SharedKernel;

namespace Domain.StationGraphEdges;

public static class CommonErrors
{
    public static Error ValidationError(IEnumerable<string> errors) => new("Common.ValidationError", "Query is invalid: " + Environment.NewLine + String.Join(Environment.NewLine, errors));
}