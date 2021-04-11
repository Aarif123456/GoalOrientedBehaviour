namespace GameBrains.AI
{
    public sealed class TimeSlicedAStarSearch : TimeSlicedSearch
    {
        private C5.IntervalHeap<ScoredNode> priorityQueue;
        
        public TimeSlicedAStarSearch(Node source, Node destination)
            : this(source, destination, EuclideanDistance.Calculate)
        {
        }
        
        public TimeSlicedAStarSearch(Node source, Node destination, HeuristicDelegate heuristic)
            : base(TimeSlicedSearchTypes.AStar, source)
        {
            H = heuristic;
            priorityQueue = 
                new C5.IntervalHeap<ScoredNode>(
                    new C5.DelegateComparer<ScoredNode>(
                        delegate(ScoredNode s1, ScoredNode s2)
                            { return s1.f.CompareTo(s2.f); }));
            float g = 0;
            float h = H(source, destination);
            priorityQueue.Add(new ScoredNode(source, g + h, g, null, null));
            Destination = destination;
        }
        
        public Node Destination { get; private set; }
        
        public HeuristicDelegate H { get; set; }
        
        public override SearchResults CycleOnce()
        {
            if (priorityQueue.IsEmpty)
            {
                return SearchResults.Failure;
            }
            
            ScoredNode current = priorityQueue.DeleteMin();

            if (current.node == Destination)
            {
                Solution = ExtractPath(current);
                return SearchResults.Success;
            }
            
            foreach (Edge edgeFromCurrent in current.node.outEdges)
            {
                float h = H(edgeFromCurrent.ToNode, Destination);
                float g = current.g + edgeFromCurrent.Cost;
                
                priorityQueue.Add(new ScoredNode(edgeFromCurrent.ToNode, g + h, g, edgeFromCurrent, current));
            }
            
            return SearchResults.Running;
        }
    }
}
