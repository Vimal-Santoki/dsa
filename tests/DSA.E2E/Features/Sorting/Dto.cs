using System;
using System.Collections.Generic;
using System.Text;

namespace DSA.E2E.Features.Sorting
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Deserialized via JSON")]
    internal sealed class E2ESortResult
    {
        public int[]? SortedData { get; set; }
        public int Iterations { get; set; }
        public string? Algorithm { get; set; }
    }
}
