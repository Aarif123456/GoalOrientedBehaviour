using GameWorld.Navigation.Graph;
using UnityEngine;

namespace GameWorld.Navigation.Heuristics {
    public static class EuclideanDistance {
        public static float Calculate(Node n, Node goal){
            return Vector3.Distance(n.Position, goal.Position);
        }
    }
}