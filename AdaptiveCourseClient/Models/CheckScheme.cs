using AdaptiveCourseClient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveCourseClient.Models
{
    public class CheckScheme
    {
        public Dictionary<string, List<string>> OrientedGraph { get; set; }

        public int Id { get; set; }

        public CheckScheme(Dictionary<string, List<string>> orientedGraph, int id)
        {
            OrientedGraph = orientedGraph;
            Id = id;
        }
    }
}
