using DSA.Api.Features.Sorting.Algorithms;
using DSA.UnitTests.Features.Sorting.Data;

namespace DSA.UnitTests.Features.Sorting.Algorithms
{
    public class BubbleSortTests
    {
        // [Fact] indicates this is a single test case
        [Fact]
        public void Sort_Should_Sort_Unsorted_Array()
        {
            // Arrange
            var sorter = new BubbleSort();
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
            var sorter = new BubbleSort();
            var iterations = sorter.Sort(input);
            Assert.Equal(expected, input);
        }
    }
}