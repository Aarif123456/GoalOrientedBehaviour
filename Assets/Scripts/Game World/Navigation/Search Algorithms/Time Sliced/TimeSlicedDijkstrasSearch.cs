using System;
using C5;

namespace GameWorld.Navigation.Graph {
    public sealed class TimeSlicedDijkstrasSearch : TimeSlicedSearch {
        private readonly IntervalHeap<ScoredNode> _priorityQueue;

        public TimeSlicedDijkstrasSearch(Node source, Predicate<Node> isGoal)
            : base(TimeSlicedSearchTypes.Dijkstra, source){
            IsGoal = isGoal;

            _priorityQueue =
                new IntervalHeap<ScoredNode>(
                    new DelegateComparer<ScoredNode>(
                        (s1, s2) => s1.f.CompareTo(s2.f)));
            const float g = 0;
            const float h = 0;
            _priorityQueue.Add(new ScoredNode(source, g + h, g, null, null));
        }

        private Predicate<Node> IsGoal { get; }

        public override SearchResults CycleOnce(){
            if (_priorityQueue.IsEmpty) return SearchResults.Failure;

            var current = _priorityQueue.DeleteMin();

            if (IsGoal(current.node)){
                Solution = ExtractPath(current);
                return SearchResults.Success;
            }

            foreach (var edgeFromCurrent in current.node.outEdges){
                const float h = 0;
                var g = current.g + edgeFromCurrent.Cost;

                _priorityQueue.Add(new ScoredNode(edgeFromCurrent.ToNode, g + h, g, edgeFromCurrent, current));
            }

            return SearchResults.Running;
        }
    }
}