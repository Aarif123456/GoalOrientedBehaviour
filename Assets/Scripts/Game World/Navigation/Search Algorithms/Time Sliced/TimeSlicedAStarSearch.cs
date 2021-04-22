using C5;
using GameWorld.Navigation.Heuristics;

namespace GameWorld.Navigation.Graph {
    public sealed class TimeSlicedAStarSearch : TimeSlicedSearch {
        private readonly IntervalHeap<ScoredNode> priorityQueue;

        public TimeSlicedAStarSearch(Node source, Node destination)
            : this(source, destination, EuclideanDistance.Calculate){
        }

        public TimeSlicedAStarSearch(Node source, Node destination, HeuristicDelegate heuristic)
            : base(TimeSlicedSearchTypes.AStar, source){
            H = heuristic;
            priorityQueue =
                new IntervalHeap<ScoredNode>(
                    new DelegateComparer<ScoredNode>(
                        delegate(ScoredNode s1, ScoredNode s2) { return s1.f.CompareTo(s2.f); }));
            const float g = 0;
            var h = H(source, destination);
            priorityQueue.Add(new ScoredNode(source, g + h, g, null, null));
            Destination = destination;
        }

        public Node Destination { get; }

        public HeuristicDelegate H { get; set; }

        public override SearchResults CycleOnce(){
            if (priorityQueue.IsEmpty) return SearchResults.Failure;

            var current = priorityQueue.DeleteMin();

            if (current.node == Destination){
                Solution = ExtractPath(current);
                return SearchResults.Success;
            }

            foreach (var edgeFromCurrent in current.node.outEdges){
                var h = H(edgeFromCurrent.ToNode, Destination);
                var g = current.g + edgeFromCurrent.Cost;

                priorityQueue.Add(new ScoredNode(edgeFromCurrent.ToNode, g + h, g, edgeFromCurrent, current));
            }

            return SearchResults.Running;
        }
    }
}