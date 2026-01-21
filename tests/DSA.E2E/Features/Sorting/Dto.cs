using System;
using System.Collections.Generic;
using System.Text;

namespace DSA.E2E.Features.Sorting
{
    public class dto
    {
        public class E2ESortResult
        {
            public int[] SortedData { get; set; }
            public int Iterations { get; set; }
            public string Algorithm { get; set; }
        }
    }
}
