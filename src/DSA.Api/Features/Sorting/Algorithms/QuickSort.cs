using System.Linq.Expressions;
using DSA.Api.Features.Sorting.Interfaces;

namespace DSA.Api.Features.Sorting.Algorithms
{
    internal class QuickSort : ISortAlgorithm
    {
        public string Name => "Quick Sort";

        public string Code => "QuickSort";

        public void Sort(int[] array)
        {
            var partitions = array.AsSpan();
            QuickSortRecursive(partitions);
        }

        private static void QuickSortRecursive(Span<int> span)
        {
            if (span.Length <= 1)
            {
                return;
            }
            var pivotIndex = MedianOfThree(span);
            pivotIndex = Partition(span, pivotIndex);
            QuickSortRecursive(span.Slice(0, pivotIndex));
            QuickSortRecursive(span.Slice(pivotIndex + 1));
        }

        private static int MedianOfThree(Span<int> span)
        {
            var lo = 0;                 // value i.e 5
            var mid = span.Length / 2; // value i.e 15
            var hi = span.Length - 1; // value i.e 12

            if (span[lo] > span[mid]) Swap(span, lo, mid);  //5,15,12
            if (span[lo] > span[hi]) Swap(span, lo, hi);  //5,15,12
            if (span[mid] > span[hi]) Swap(span, mid, hi); //5,12,15

            // Place pivot at the end
            Swap(span, mid, hi); //5,15,12

            return hi;
        }

        private static int Partition(Span<int> span, int pivotIndex)
        {
            var pivot = span[pivotIndex];
            var i = -1;
            for (var j = 0; j < span.Length - 1; j++)
            {
                if (span[j] <= pivot)
                {
                    i++;
                    Swap(span, i, j);
                }
            }
            Swap(span, i + 1, span.Length - 1);
            return i + 1;
        }

        private static void Swap(Span<int> span, int i, int j)
        {
            if (i != j)
            {
                (span[i], span[j]) = (span[j], span[i]);
            }
        }
    }
}
