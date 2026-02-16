using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DSA.Api.Features.Sorting.Api;
using DSA.Api.Features.Sorting.Dto;
using DSA.Api.Features.Sorting.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Polly;
using Polly.Registry;

namespace DSA.UnitTests.Features.Sorting.Api
{
    public class SortingEndpointsTests
    {
        private readonly ISortAlgorithm _mockAlgo;
        private readonly List<ISortAlgorithm> _serviceList;
        private readonly int[] _defaultInput;
        private readonly ResiliencePipelineProvider<string> _mockResilienceProvider;

        public SortingEndpointsTests()
        {
            _mockAlgo = Substitute.For<ISortAlgorithm>();
            _mockAlgo.Name.Returns("Mock Sort");
            _mockAlgo.Code.Returns("MockSort");
            _mockAlgo.Sort(Arg.Any<int[]>());

            _serviceList = new List<ISortAlgorithm> { _mockAlgo,};

            _mockResilienceProvider = Substitute.For<ResiliencePipelineProvider<string>>();
            _mockResilienceProvider.GetPipeline(Arg.Any<string>()).Returns(ResiliencePipeline.Empty);
            _defaultInput = [5, 4, 3];
        }
        [Fact]
        public async Task RunSortAlgorithm_Should_Call_Sort_On_Matching_Algorithm()
        {
            var result = await SortingEndpoints.RunSortAlgorithm("MockSort", _defaultInput, _serviceList, _mockResilienceProvider, NullLoggerFactory.Instance);

            var okResult = Assert.IsType<Ok<SortResult>>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.Equal("Mock Sort", okResult.Value?.Algorithm);
            _mockAlgo.Received(1).Sort(_defaultInput);
        }
        [Fact]
        public async Task RunSortAlgorithm_Should_Return_NotFound_For_Invalid_Code()
        {
            var result = await SortingEndpoints.RunSortAlgorithm("InvalidCode", _defaultInput, _serviceList, _mockResilienceProvider, NullLoggerFactory.Instance);
            Assert.IsType<NotFound<string>>(result.Result);
            _mockAlgo.DidNotReceive().Sort(Arg.Any<int[]>());
        }

        [Fact]
        public async Task RunSortAlgorithm_Should_Return_BadRequest_On_Exception()
        {
            _mockAlgo.When(x => x.Sort(Arg.Any<int[]>())).Do(_ => { throw new ArgumentException("Invalid input"); });
            var result = await SortingEndpoints.RunSortAlgorithm("MockSort", _defaultInput, _serviceList, _mockResilienceProvider, NullLoggerFactory.Instance);

            var badRequestResult = Assert.IsType<BadRequest<string>>(result.Result);
            Assert.NotNull(badRequestResult.Value);
            Assert.Equal("Invalid input", badRequestResult.Value);
        }

        [Fact]
        public void GetAlgorithms_Should_Return_Algorithm_Info_List()
        {
            var result = SortingEndpoints.GetAlgorithms(_serviceList);
            var okResult = Assert.IsType<Ok<IEnumerable<AlgorithmInfo>>>(result);
            var algorithms = okResult.Value;
            Assert.NotNull(algorithms);
            Assert.Single(algorithms);
            var algo = algorithms.First();
            Assert.Equal("MockSort", algo.Code);
            Assert.Equal("Mock Sort", algo.DisplayName);
        }

        [Fact]
        public void GetAlgorithms_Should_Return_Empty_List_When_No_Algorithms()
        {
            var result = SortingEndpoints.GetAlgorithms(new List<ISortAlgorithm>());
            var okResult = Assert.IsType<Ok<IEnumerable<AlgorithmInfo>>>(result);
            var algorithms = okResult.Value;
            Assert.NotNull(algorithms);
            Assert.Empty(algorithms);
        }

        [Fact]
        public async Task RunSortAlgorithm_Should_Handle_Empty_Input_Array()
        {
            var emptyInput = Array.Empty<int>();
            var result = await SortingEndpoints.RunSortAlgorithm("MockSort", emptyInput, _serviceList, _mockResilienceProvider, NullLoggerFactory.Instance);
            var okResult = Assert.IsType<Ok<SortResult>>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.Equal("Mock Sort", okResult.Value?.Algorithm);
            _mockAlgo.Received(1).Sort(emptyInput);

        }

        [Fact]
        public async Task RunSortAlgorithm_Should_Handle_Large_Input_Array()
        {
            var largeInput = Enumerable.Range(1000, 1000).Reverse().ToArray();
            var result = await SortingEndpoints.RunSortAlgorithm("MockSort", largeInput, _serviceList, _mockResilienceProvider, NullLoggerFactory.Instance);
            var okResult = Assert.IsType<Ok<SortResult>>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.Equal("Mock Sort", okResult.Value?.Algorithm);
            _mockAlgo.Received(1).Sort(largeInput);
        }

        [Fact]
        public async Task RunSortAlgorithm_Should_Be_Case_Insensitive_For_Algorithm_Code()
        {
            var result = await SortingEndpoints.RunSortAlgorithm("mOcKsOrT", _defaultInput, _serviceList, _mockResilienceProvider, NullLoggerFactory.Instance);
            var okResult = Assert.IsType<Ok<SortResult>>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.Equal("Mock Sort", okResult.Value?.Algorithm);
            _mockAlgo.Received(1).Sort(_defaultInput);
        }

        [Fact]
        public void GetAlgorithms_Should_Handle_Multiple_Algorithms()
        {
            var anotherMockAlgo = Substitute.For<ISortAlgorithm>();
            anotherMockAlgo.Name.Returns("Another Mock Sort");
            anotherMockAlgo.Code.Returns("AnotherMockSort");
            var serviceList = new List<ISortAlgorithm> { _mockAlgo, anotherMockAlgo };
            var result = SortingEndpoints.GetAlgorithms(serviceList);
            var okResult = Assert.IsType<Ok<IEnumerable<AlgorithmInfo>>>(result);
            var algorithms = okResult.Value;
            Assert.NotNull(algorithms);
            Assert.Equal(2, algorithms.Count());
            
            Assert.Contains( algorithms, a=> a.Code == "MockSort" && a.DisplayName == "Mock Sort");
            Assert.Contains(algorithms ,a=> a.Code == "AnotherMockSort" && a.DisplayName == "Another Mock Sort");
        }

        [Fact]
        public async Task RunSortAlgorithm_Should_Handle_Null_Input_Array()
        {
            int[]? nullInput = null;
            var result =await SortingEndpoints.RunSortAlgorithm("MockSort", nullInput!, _serviceList, _mockResilienceProvider, NullLoggerFactory.Instance);
            var badRequestResult = Assert.IsType<BadRequest<string>>(result.Result);
            Assert.NotNull(badRequestResult.Value);
            _mockAlgo.DidNotReceive().Sort(Arg.Any<int[]>());
        }
    }
}
