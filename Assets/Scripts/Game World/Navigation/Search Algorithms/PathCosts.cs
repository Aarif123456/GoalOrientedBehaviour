using System.Collections.Generic;
using C5;

namespace GameWorld.Navigation.Graph {
    public static class LeastCostPathTable {
        private static readonly HashDictionary<NodePair, NodeAndCost> nextNodeAndCostTable =
            new HashDictionary<NodePair, NodeAndCost>();

        public static float Cost(Node source, Node destination){
            return nextNodeAndCostTable[new NodePair(source, destination)].cost;
        }

        public static bool PathExists(Node source, Node destination){
            return nextNodeAndCostTable[new NodePair(source, destination)].cost != float.MaxValue;
        }

        public static Node NextNode(Node source, Node destination){
            return nextNodeAndCostTable[new NodePair(source, destination)].node;
        }

        public static List<Edge> Path(Node source, Node destination){
            List<Edge> path = null;
            var fromNode = source;

            while (fromNode != null){
                var nextNodeAndCost = nextNodeAndCostTable[new NodePair(fromNode, destination)];
                var toNode = nextNodeAndCost.node;

                if (toNode == null) continue;
                path ??= new List<Edge>();

                // TODO: make outEdges a hashList??
                path.Add(fromNode.outEdges.Find(element => element.ToNode == toNode));
                fromNode = toNode;
            }

            return path;
        }

        public static void Create(Graph graph){
            foreach (var sourceNode in graph.nodeCollection.Nodes)
            foreach (var destinationNode in graph.nodeCollection.Nodes){
                nextNodeAndCostTable[new NodePair(sourceNode, destinationNode)] = new NodeAndCost(null, float.MaxValue);
            }

            foreach (var node in graph.nodeCollection.Nodes){
                DikstrasSearch(graph.nodeCollection.Nodes, node);
            }
        }

        public static void DikstrasSearch(Node[] nodes, Node source){
            var table = new HashDictionary<Node, Entry>();

            foreach (var node in nodes){
                table.Add(node, new Entry(false, float.MaxValue, null));
            }

            var sourceEntry = table[source];
            sourceEntry.cost = 0;
            table[source] = sourceEntry;

            var priorityQueue =
                new IntervalHeap<NodeAndCost>(
                    new DelegateComparer<NodeAndCost>(
                        delegate(NodeAndCost nodeAndCost1, NodeAndCost nodeAndCost2) {
                            return nodeAndCost1.cost.CompareTo(nodeAndCost2.cost);
                        }));

            priorityQueue.Add(new NodeAndCost(source, 0));

            while (!priorityQueue.IsEmpty){
                var nodeAndCost = priorityQueue.DeleteMin();

                var currentNode = nodeAndCost.node;

                if (table[currentNode].known) continue;
                var currentNodeEntry = table[currentNode];
                currentNodeEntry.known = true;
                table[currentNode] = currentNodeEntry;

                foreach (var edge in currentNode.outEdges){
                    var toNode = edge.ToNode;
                    var toNodeCost = table[currentNode].cost + edge.Cost;

                    if (!(table[toNode].cost > toNodeCost)) continue;
                    var toNodeEntry = table[toNode];
                    toNodeEntry.cost = toNodeCost;
                    toNodeEntry.predecessor = currentNode;
                    table[toNode] = toNodeEntry;
                    priorityQueue.Add(new NodeAndCost(toNode, toNodeCost));
                }
            }

            foreach (var node in nodes){
                nextNodeAndCostTable[new NodePair(source, node)] =
                    ExtractNextNodeFromTable(table, source, node);
            }
        }

        // Walk back through the predecessors to the one after source.
        private static NodeAndCost ExtractNextNodeFromTable(HashDictionary<Node, Entry> table, Node source,
            Node destination){
            var nextNodeAndCost = new NodeAndCost(table[destination].predecessor, table[destination].cost);

            while (nextNodeAndCost.node != null &&
                   nextNodeAndCost.cost != float.MaxValue &&
                   table[nextNodeAndCost.node].predecessor != source){
                nextNodeAndCost.node = table[nextNodeAndCost.node].predecessor;
            }

            return nextNodeAndCost;
        }

        private struct Entry {
            public bool known;
            public float cost;
            public Node predecessor;

            public Entry(bool known, float cost, Node predecessor){
                this.known = known;
                this.cost = cost;
                this.predecessor = predecessor;
            }
        }

        private struct NodePair {
            public Node source;
            public Node destination;

            public NodePair(Node source, Node destination){
                this.source = source;
                this.destination = destination;
            }
        }

        private struct NodeAndCost {
            public Node node;
            public readonly float cost;

            public NodeAndCost(Node node, float cost){
                this.node = node;
                this.cost = cost;
            }
        }
    }
}