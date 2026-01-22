namespace DSA.Api.Features.Sorting.Dto
{
    internal sealed class SortResult
    {
        public int[]? SortedData { get; set; }
        public int Iterations { get; set; }
        public string? Algorithm { get; set; }
    }
}
