using System.Threading.Tasks;
using DSA.Api.Features.Sorting.Algorithms;
using DSA.UnitTests.Features.Sorting.Data;
using Microsoft.Extensions.Logging.Abstractions;

namespace DSA.UnitTests.Features.Sorting.Algorithms
{
    public class BubbleSortTests
    {
        [Theory]
        [ClassData(typeof(BubbleSortTestData))]
        public void Sort_Should_Handle_Various_Inputs(int[] input, int[] expected)
        {
            var sorter = new BubbleSort();
            sorter.Sort(input);
            Assert.Equal(expected, input);
        }
    }
}
