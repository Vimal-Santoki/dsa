using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components.Forms;

namespace DSA.IntegrationTests.Features.Sorting.Data
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via xUnit")]
    internal sealed class SortingTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // Scenario 1: Bubble Sort Integration Test
            yield return new object[] {
                new SortingIntegrationScenario {
                    ScenarioName = "Standard Test",
                    AlgoCode = "BubbleSort",
                    ExpectedAlgoName = "Bubble Sort",
                    Input = [5,6,3,4,9,2,7,3,1],
                    ExpectedOutput = [1,2,3,3,4,5,6,7,9],
                    ExpectedStatusCode = 200
                }
            };

            // Scenario 2: Quick Sort Integration Test
            yield return new object[] {
                new SortingIntegrationScenario {
                    ScenarioName = "Standard Test",
                    AlgoCode = "QuickSort",
                    ExpectedAlgoName = "Quick Sort",
                    Input = [5,6,3,4,9,2,7,3,1],
                    ExpectedOutput = [1,2,3,3,4,5,6,7,9],
                    ExpectedStatusCode = 200
                }
            };


        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
