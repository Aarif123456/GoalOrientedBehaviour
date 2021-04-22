using System.Collections.Generic;
using UnityEngine;

namespace GameBrains.AI {
    public class Path {
        public Path(Vector3 source, List<Edge> edges, Vector3 destination){
            Source = source;
            Edges = edges;
            Destination = destination;
        }

        public Vector3 Source { get; set; }
        public List<Edge> Edges { get; set; }
        public Vector3 Destination { get; set; }
    }
}