using DSA.Api.Features.Sorting.Interfaces;

namespace DSA.Api.Features.Sorting.Algorithms
{
    /// <summary>
    /// Bubble Sort Implementation.
    /// Time Complexity: O(n^2) Average/Worst, O(n) Best Case (Optimized).
    /// Space Complexity: O(1) In-place.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via DI")]
    internal sealed class BubbleSort : ISortAlgorithm
    {
        public string Name => "Bubble Sort";
        public string Code => "BubbleSort";

        public int Sort(int[] array)
        {
            var swapped = false;
            var iterations = 0;

            // Outer loop: Each pass bubbles the largest element to the end.
            for (var i = 0; i < array.Length - 1; i++)
            {
                swapped = false;
                // Optimization: We subtract 'i' because the last 'i' elements are already sorted.
                for (var j = 0; j < array.Length - i - 1; j++)
                {
                    iterations++;
                    if (array[j] > array[j + 1])
                    {
                        swapped = true;
                        (array[j], array[j + 1]) = (array[j + 1], array[j]);
                    }
                }

                // Optimization: If no elements were swapped, the array is already sorted.
                if (!swapped) break;
            }
            return iterations;
        }
    }
}
