//namespace Thot.GameAI
//{
//    using System;
//    using System.Collections.Generic;
//
//    public sealed class DijkstrasSearch : Search
//    {
//        private C5.IntervalHeap<ScoredNode> priorityQueue;
//        
//        public DijkstrasSearch(Node source, Predicate<Node> isGoal)
//            : base(SearchTypes.Dijkstra, source)
//        {
//            IsGoal = isGoal;
//            
//            priorityQueue = 
//                new C5.IntervalHeap<ScoredNode>(
//                    new C5.DelegateComparer<ScoredNode>(
//                        delegate(ScoredNode s1, ScoredNode s2)
//                            { return s1.f.CompareTo(s2.f); }));
//            float g = 0;
//            float h = 0;
//            priorityQueue.Add(new ScoredNode(source, g + h, g, null, null));
//        }
//        
//        public Predicate<Node> IsGoal { get; set; }
//        
//        public SearchResults DoSearch()
//        {
//            SearchResults searchResult;
//            while ((searchResult = CycleOnce()) == SearchResults.Running);
//            
//            return searchResult;
//        }
//        
//        public override SearchResults CycleOnce()
//        {
//            if (priorityQueue.IsEmpty)
//            {
//                return SearchResults.Failure;
//            }
//            
//            ScoredNode current = priorityQueue.DeleteMin();
//            
//            if (IsGoal(current.node))
//            {
//                Solution = ExtractPath(current);
//                return SearchResults.Success;
//            }
//            
//            foreach (Edge edgeFromCurrent in current.node.outEdges)
//            {
//                float h = 0;
//                float g = current.g + edgeFromCurrent.Cost;
//                
//                priorityQueue.Add(new ScoredNode(edgeFromCurrent.ToNode, g + h, g, edgeFromCurrent, current));
//            }
//            
//            return SearchResults.Running;
//        }
//    }
//}