using System.Collections.Generic;
using GameWorld.Navigation.Graph;
using UnityEngine;

namespace GameWorld {
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