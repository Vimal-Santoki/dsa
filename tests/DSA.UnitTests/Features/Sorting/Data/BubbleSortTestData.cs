using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DSA.UnitTests.Features.Sorting.Data
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via xUnit reflection")]
    internal sealed class BubbleSortTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // standard data
            yield return new object[] {
                new int[] { 5, 3, 8, 1, 2 }, // input
                new int[] { 1, 2, 3, 5, 8 }  // expected output
            };

            // mix of positive and negative numbers
            yield return new object[] { new int[] { -1, -3, -2, 0 }, new int[] { -3, -2, -1, 0 } };

            // single element
            yield return new object[] { new int[] { 10 }, new int[] { 10 } };

            // empty array
            yield return new object[] { Array.Empty<int>(), Array.Empty<int>() };

            // repetitive elements
            yield return new object[] { new int[] { 2, 2, 2 }, new int[] { 2, 2, 2 } };

            // already sorted
            yield return new object[] { new int[] { 2, 3, 4 }, new int[] { 2, 3, 4 } };

            // reverse sorted
            yield return new object[] { new int[] { 3, 2, 1 }, new int[] { 1, 2, 3 } };

            // large dataset
            var largeInputCount = 10000;
            var largeExpected = Enumerable.Range(1, largeInputCount).ToArray();
            var largeInput = largeExpected.Shuffle().ToArray();
            yield return new object[] { largeInput, largeExpected };

        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
