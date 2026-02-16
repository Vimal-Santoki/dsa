namespace DSA.Api.Features.Sorting.Interfaces
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "Required for DispatchProxy generation")]
    public interface ISortAlgorithm
    {
        /// <summary>
        /// The friendly display name of the algorithm (e.g. "Bubble Sort").
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A unique code identifier for the algorithm (e.g. "BubbleSort").
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// To group algorithms by category (e.g. "Sort", "Search").
        /// </summary>
        public string Category => "Sort";

        /// <summary>
        /// Sorts the provided array in-place.
        /// </summary>
        /// <param name="array">The integer array to sort.</param>
        /// <returns>The number of iterations or swaps performed (complexity metric).</returns>
        void Sort(int[] array);
    }
}
