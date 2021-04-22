using UnityEngine;

namespace GameBrains.AI {
    public static class EuclideanDistance {
        public static float Calculate(Node n, Node goal){
            return Vector3.Distance(n.Position, goal.Position);
        }
    }
}