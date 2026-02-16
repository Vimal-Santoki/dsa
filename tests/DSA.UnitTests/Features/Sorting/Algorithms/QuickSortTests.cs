using System;
using System.Collections.Generic;
using System.Text;
using DSA.Api.Features.Sorting.Algorithms;
using DSA.UnitTests.Features.Sorting.Data;
using Microsoft.Extensions.Logging.Abstractions;

namespace DSA.UnitTests.Features.Sorting.Algorithms
{
    public class QuickSortTests
    {
        [Theory]
        [ClassData(typeof(QuickSortTestData))]
        public void Sort_Should_Handle_Various_Inputs(int[] input, int[] expected)
        {
            var sorter = new QuickSort();
            sorter.Sort(input);
            Assert.Equal(expected, input);
        }
    }
}
