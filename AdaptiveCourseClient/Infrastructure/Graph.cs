using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveCourseClient.Infrastructure
{
    public static class Graph
    {
        public static Dictionary<string, List<string>> OrientedGraph { get; private set; } = new Dictionary<string, List<string>>();

        public static void AddNode(string nodeName) => OrientedGraph.Add(nodeName, new List<string>());

        public static void RemoveNode(string nodeName) => OrientedGraph.Remove(nodeName);

        public static void AddEdge(string startNodeName, string finishNodeName) => OrientedGraph[finishNodeName].Add(startNodeName);

        public static void RemoveEdge(string startNodeName, string finishNodeName) => OrientedGraph[finishNodeName].Remove(startNodeName);

        public static void AddNegationNode(string startNodeName, string finishNodeName)
        {
            RemoveEdge(startNodeName, finishNodeName);
            AddEdge(startNodeName + " NOT", finishNodeName);
        }

        public static void RemoveNegationNode(string startNodeName, string finishNodeName)
        {
            RemoveEdge(startNodeName, finishNodeName);
            AddEdge(startNodeName.Replace(" NOT", ""), finishNodeName);
        }
    }
}
