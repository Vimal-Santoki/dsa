using System.Threading.Tasks;
using DSA.Api.Features.Sorting.Algorithms;
using DSA.UnitTests.Features.Sorting.Data;
using Microsoft.Extensions.Logging.Abstractions;

namespace DSA.UnitTests.Features.Sorting.Algorithms
{
    public class BubbleSortTests
    {
        NullLogger<BubbleSort> logger;

        public BubbleSortTests()
        {
            logger = NullLogger<BubbleSort>.Instance;
        }
        
        // [Fact] indicates this is a single test case
        [Fact]
        public async Task Sort_Should_Sort_Unsorted_Array()
        {
            // Arrange
            var sorter = new BubbleSort(logger);
            int[] input = [5, 1, 4, 2, 8];
            int[] expected = [1, 2, 4, 5, 8];

            // Act
            var iterations = sorter.Sort(input);

            // Assert
            Assert.Equal(expected, input);
            Assert.True(iterations > 0);
        }

        [Theory]
        [ClassData(typeof(BubbleSortTestData))]
        public void Sort_Should_Handle_Various_Inputs(int[] input, int[] expected) {
            var sorter = new BubbleSort(logger);
            var iterations = sorter.Sort(input);
            Assert.Equal(expected, input);
        }
    }
}
