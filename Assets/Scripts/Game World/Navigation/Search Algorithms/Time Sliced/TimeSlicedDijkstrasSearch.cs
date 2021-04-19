namespace GameBrains.AI
{
    using System;

    public sealed class TimeSlicedDijkstrasSearch : TimeSlicedSearch
    {
        private C5.IntervalHeap<ScoredNode> priorityQueue;
        
        public TimeSlicedDijkstrasSearch(Node source, Predicate<Node> isGoal)
            : base(TimeSlicedSearchTypes.Dijkstra, source)
        {
            IsGoal = isGoal;
            
            priorityQueue = 
                new C5.IntervalHeap<ScoredNode>(
                    new C5.DelegateComparer<ScoredNode>(
                        delegate(ScoredNode s1, ScoredNode s2)
                            { return s1.f.CompareTo(s2.f); }));
            float g = 0;
            float h = 0;
            priorityQueue.Add(new ScoredNode(source, g + h, g, null, null));
        }
        
        public Predicate<Node> IsGoal { get; set; }
        
        public override SearchResults CycleOnce()
        {
            if (priorityQueue.IsEmpty)
            {
                return SearchResults.Failure;
            }
            
            ScoredNode current = priorityQueue.DeleteMin();
            
            if (IsGoal(current.node))
            {
                Solution = ExtractPath(current);
                return SearchResults.Success;
            }
            
            foreach (Edge edgeFromCurrent in current.node.outEdges)
            {
                float h = 0;
                float g = current.g + edgeFromCurrent.Cost;
                
                priorityQueue.Add(new ScoredNode(edgeFromCurrent.ToNode, g + h, g, edgeFromCurrent, current));
            }
            
            return SearchResults.Running;
        }
    }
}