//namespace Thot.GameAI
//{
//    using System.Collections.Generic;
//
//    public abstract class Search
//    {
//        protected Search(SearchTypes searchType, Node source)
//        {
//            SearchType = searchType;
//            Source = source;        
//        }
//        
//        public SearchTypes SearchType { get; private set; }
//        
//        public Node Source { get; private set; }
//        
//        public List<Edge> Solution { get; protected set; }
//        
//        public abstract SearchResults CycleOnce();
//            
//        public List<Edge> ExtractPath(ScoredNode current)
//        {
//            List<Edge> path = new List<Edge>();
//            
//            while (current.edgeFromParent != null)
//            {
//                path.Add(current.edgeFromParent);
//                current = current.parentScoredNode;
//            }
//            
//            path.Reverse();
//            return path;
//        }
//    }
//}