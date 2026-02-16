using System;
using System.Collections.Generic;
using System.Text;

namespace DSA.IntegrationTests.Features.Sorting.Data
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1515:can be internal", Justification = "need public for xUnit")]
    public class SortingIntegrationScenario
    {
        public required string ScenarioName { get; set; } // e.g. "QuickSort - Happy Path"
        public required string AlgoCode { get; set; }     // "QuickSort"
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Test data, not a public API")]
        public required int[] Input { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Test data, not a public API")]
        public required int[] ExpectedOutput { get; set; }
        public required string ExpectedAlgoName { get; set; } // For assertion
        public required int ExpectedStatusCode { get; set; } = 200; // Default success

        // Override ToString so xUnit shows nice names in the Test Explorer!
        public override string ToString() => $"{AlgoCode}: {ScenarioName}";
    }
}
