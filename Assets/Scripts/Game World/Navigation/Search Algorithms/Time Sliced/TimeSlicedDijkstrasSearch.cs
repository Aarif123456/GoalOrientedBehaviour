using System;
using C5;

namespace GameWorld.Navigation.Graph {
    public sealed class TimeSlicedDijkstrasSearch : TimeSlicedSearch {
        private readonly IntervalHeap<ScoredNode> priorityQueue;

        public TimeSlicedDijkstrasSearch(Node source, Predicate<Node> isGoal)
            : base(TimeSlicedSearchTypes.Dijkstra, source){
            IsGoal = isGoal;

            priorityQueue =
                new IntervalHeap<ScoredNode>(
                    new DelegateComparer<ScoredNode>(
                        delegate(ScoredNode s1, ScoredNode s2) { return s1.f.CompareTo(s2.f); }));
            const float g = 0;
            const float h = 0;
            priorityQueue.Add(new ScoredNode(source, g + h, g, null, null));
        }

        public Predicate<Node> IsGoal { get; set; }

        public override SearchResults CycleOnce(){
            if (priorityQueue.IsEmpty) return SearchResults.Failure;

            var current = priorityQueue.DeleteMin();

            if (IsGoal(current.node)){
                Solution = ExtractPath(current);
                return SearchResults.Success;
            }

            foreach (var edgeFromCurrent in current.node.outEdges){
                const float h = 0;
                var g = current.g + edgeFromCurrent.Cost;

                priorityQueue.Add(new ScoredNode(edgeFromCurrent.ToNode, g + h, g, edgeFromCurrent, current));
            }

            return SearchResults.Running;
        }
    }
}