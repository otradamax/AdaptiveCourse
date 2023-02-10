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

        public static void AddEdge(string startNodeName, string finishNodeName, int negativeCount) => OrientedGraph[finishNodeName].Add(startNodeName + new string('!', negativeCount));

        public static void RemoveEdge(string startNodeName, string finishNodeName) => OrientedGraph[finishNodeName].Remove(startNodeName);

        public static void RemoveEdge(string startNodeName, string finishNodeName, int negativeCount) => OrientedGraph[finishNodeName].Remove(startNodeName + new string('!', negativeCount));

        public static void AddNegationNode(string startNodeName, string finishNodeName, int negativeCount)
        {
            RemoveEdge(startNodeName + new string('!', negativeCount), finishNodeName);
            AddEdge(startNodeName + new string('!', negativeCount + 1), finishNodeName);
        }

        public static void RemoveNegationNode(string startNodeName, string finishNodeName, int negativeCount)
        {
            RemoveEdge(startNodeName + new string('!', negativeCount), finishNodeName);
            AddEdge(startNodeName + new string('!', negativeCount - 1), finishNodeName);
        }
    }
}
