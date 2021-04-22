using System.Collections.Generic;

namespace GameBrains.AI {
    public abstract class TimeSlicedSearch {
        protected TimeSlicedSearch(TimeSlicedSearchTypes searchType, Node source){
            SearchType = searchType;
            Source = source;
        }

        public TimeSlicedSearchTypes SearchType { get; }

        public Node Source { get; }

        public List<Edge> Solution { get; protected set; }

        public abstract SearchResults CycleOnce();

        public List<Edge> ExtractPath(ScoredNode current){
            var path = new List<Edge>();

            while (current.edgeFromParent != null){
                path.Add(current.edgeFromParent);
                current = current.parentScoredNode;
            }

            path.Reverse();
            return path;
        }
    }
}