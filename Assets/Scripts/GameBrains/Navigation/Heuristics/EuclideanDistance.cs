namespace GameBrains.AI
{
    using UnityEngine;
    
    public static class EuclideanDistance
    {
        public static float Calculate(Node n, Node goal)
        {
            return Vector3.Distance(n.Position, goal.Position);
        }
    }
}