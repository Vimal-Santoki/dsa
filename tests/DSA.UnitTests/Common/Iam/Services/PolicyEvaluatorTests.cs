using DSA.Api.Common.Iam.Services;
using DSA.Api.Common.Iam.Models;

namespace DSA.UnitTests.Common.Iam.Services
{
    public class PolicyEvaluatorTests
    {
        private readonly PolicyEvaluator _sut;

        public PolicyEvaluatorTests()
        {
            _sut = new PolicyEvaluator();
        }

        [Fact]
        public void Evaluate_ShouldReturnFalse_WhenPolicyIsNull()
        {
            // Act
            var result = _sut.Evaluate(null!, "action", "resource");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Evaluate_ShouldReturnFalse_WhenPolicyStatementsAreNull()
        {
            // Arrange
            var policy = new PolicyDocument("1", null!);

            // Act
            var result = _sut.Evaluate(policy, "action", "resource");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Evaluate_ShouldReturnTrue_WhenActionAndResourceMatchExact()
        {
            // Arrange
            var policy = new PolicyDocument("1", new List<Statements>
            {
                new(Effect.Allow, ["Sorting:List"], ["Sorting"])
            });

            // Act
            var result = _sut.Evaluate(policy, "Sorting:List", "Sorting");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Evaluate_ShouldReturnTrue_WhenActionAndResourceMatchCaseInsensitive()
        {
            // Arrange
            var policy = new PolicyDocument("1", new List<Statements>
            {
                new(Effect.Allow, ["Sorting:List"], ["Sorting"])
            });

            // Act
            var result = _sut.Evaluate(policy, "sorting:list", "sorting");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Evaluate_ShouldReturnTrue_WhenActionIsWildcard()
        {
            // Arrange
            var policy = new PolicyDocument("1", new List<Statements>
            {
                new(Effect.Allow, ["*"], ["Sorting"])
            });

            // Act
            var result = _sut.Evaluate(policy, "Sorting:Anything", "Sorting");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Evaluate_ShouldReturnTrue_WhenResourceIsWildcard()
        {
            // Arrange
            var policy = new PolicyDocument("1", new List<Statements>
            {
                new(Effect.Allow, ["Sorting:List"], ["*"])
            });

            // Act
            var result = _sut.Evaluate(policy, "Sorting:List", "AnyResource");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Evaluate_ShouldReturnFalse_WhenExplicitDenyExists()
        {
            // Arrange
            var policy = new PolicyDocument("1", new List<Statements>
            {
                new(Effect.Allow, ["Sorting:Execute"], ["*"]),
                new(Effect.Deny, ["Sorting:Execute"], ["BubbleSort"])
            });

            // Act
            var result = _sut.Evaluate(policy, "Sorting:Execute", "BubbleSort");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Evaluate_ShouldReturnTrue_WhenDenyDoesNotMatch()
        {
            // Arrange
            var policy = new PolicyDocument("1", new List<Statements>
            {
                new(Effect.Allow, ["Sorting:Execute"], ["*"]),
                new(Effect.Deny, ["Sorting:Execute"], ["BubbleSort"])
            });

            // Act
            var result = _sut.Evaluate(policy, "Sorting:Execute", "QuickSort");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Evaluate_ShouldReturnFalse_WhenNoStatementMatches()
        {
            // Arrange
            var policy = new PolicyDocument("1", new List<Statements>
            {
                new(Effect.Allow, ["Sorting:List"], ["Sorting"])
            });

            // Act
            var result = _sut.Evaluate(policy, "Sorting:Execute", "Sorting");

            // Assert
            Assert.False(result);
        }
    }
}
