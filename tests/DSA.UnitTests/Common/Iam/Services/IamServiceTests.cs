using DSA.Api.Common.Iam.Interfaces;
using DSA.Api.Common.Iam.Models;
using DSA.Api.Common.Iam.Services;
using NSubstitute;

namespace DSA.UnitTests.Common.Iam.Services
{
    public class IamServiceTests
    {
        private readonly IPermissionService _permissionService;
        private readonly IPolicyEvaluator _policyEvaluator;
        private readonly IamService _sut;

        public IamServiceTests()
        {
            _permissionService = Substitute.For<IPermissionService>();
            _policyEvaluator = Substitute.For<IPolicyEvaluator>();
            _sut = new IamService(_permissionService, _policyEvaluator);
        }

        [Fact]
        public async Task IsAuthorizedAsync_ShouldReturnFalse_WhenPermissionServiceReturnsNull()
        {
            // Arrange
            _permissionService.GetPolicyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns((PolicyDocument?)null);

            // Act
            var result = await _sut.IsAuthorizedAsync("user1", "action", "resource");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsAuthorizedAsync_ShouldCallEvaluator_WhenPolicyExists()
        {
            // Arrange
            var policy = new PolicyDocument("1", new List<Statements>());
            _permissionService.GetPolicyAsync("user1", Arg.Any<CancellationToken>())
                .Returns(policy);

            _policyEvaluator.Evaluate(policy, "action", "resource").Returns(true);

            // Act
            var result = await _sut.IsAuthorizedAsync("user1", "action", "resource");

            // Assert
            Assert.True(result);
            _policyEvaluator.Received(1).Evaluate(policy, "action", "resource");
        }

        [Fact]
        public async Task IsAuthorizedAsync_ShouldReturnFalse_WhenEvaluatorReturnsFalse()
        {
            // Arrange
            var policy = new PolicyDocument("1", new List<Statements>());
            _permissionService.GetPolicyAsync("user1", Arg.Any<CancellationToken>())
                .Returns(policy);

            _policyEvaluator.Evaluate(policy, "action", "resource").Returns(false);

            // Act
            var result = await _sut.IsAuthorizedAsync("user1", "action", "resource");

            // Assert
            Assert.False(result);
        }
    }
}
