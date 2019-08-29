using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericSearch.Domains;

namespace GenericSearch
{
    class Program
    {
        public static Random rng;
        static void Main(string[] args)
        {
            rng = new Random();
            var search = new BreadthFirstSearch();
            var results = search.AllPaths(new PegBoard());
            float successRate = ((float)results.results.Count / (float)results.leafNodes) * 100;
        }
    }
}
