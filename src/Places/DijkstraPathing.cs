using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Rooms;

namespace Wanderer.Rooms
{
    public class DijkstraPathing
    {
        public Map Map { get; }
        public Node Start { get; set;}
        public Node End { get; set;}

        public Node NearestToStart {get;set;}

        List<Node> nodesSeen = new List<Node>();

        public class Node 
        {
            public IRoom Room{get;set;}
            public int? MinCostToStart { get; set; }
            public bool Visited { get; internal set; }
            public Node NearestToStart { get; internal set; }

            public Node(IRoom place)
            {
                this.Room = place;
            }
        }

        public DijkstraPathing(Map map,IRoom start, IRoom end)
        {
            Map = map;
            Start = new Node(start);
            End = new Node(end);

            nodesSeen.Add(Start);
            nodesSeen.Add(End);
        }

        /// <summary>
        /// Returns the path from <see cref="Start"/> to <see cref="End"/> or null
        /// if there is no route found
        /// </summary>
        /// <returns></returns>
        public List<Node> GetShortestPathDijkstra()
        {
            DijkstraSearch();
            var shortestPath = new List<Node>();
            shortestPath.Add(End);
            BuildShortestPath(shortestPath, End);

            if (!shortestPath.Contains(Start))
                return null;

            shortestPath.Reverse();
            return shortestPath;
        }

        private void BuildShortestPath(List<Node> list, Node node)
        {
            if (node.NearestToStart == null)
                return;
            list.Add(node.NearestToStart);
            BuildShortestPath(list, node.NearestToStart);
        }



        private void DijkstraSearch()
        {
            Start.MinCostToStart = 0;
            var prioQueue = new List<Node>();
            prioQueue.Add(Start);
            do {
                prioQueue = prioQueue.OrderBy(x => x.MinCostToStart).ToList();
                var node = prioQueue.First();
                prioQueue.Remove(node);
                foreach (var cnn in Map.GetAdjacentRooms(node.Room,true))
                {
                    
                    var childNode = nodesSeen.FirstOrDefault(n=>n.Room == cnn.Value) ?? new Node(cnn.Value);
                    if (childNode.Visited)
                        continue;
                    if (childNode.MinCostToStart == null ||
                        node.MinCostToStart + 1 < childNode.MinCostToStart)
                    {
                        childNode.MinCostToStart = node.MinCostToStart + 1;
                        childNode.NearestToStart = node;
                        if (!prioQueue.Any(n=>n.Room == childNode.Room))
                            prioQueue.Add(childNode);
                    }
                }
                node.Visited = true;
                nodesSeen.Add(node);
                
                if (node.Room == End.Room)
                    return;
            } while (prioQueue.Any());
        }
    }
}