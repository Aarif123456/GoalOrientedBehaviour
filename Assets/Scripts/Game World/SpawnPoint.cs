using System.Linq;
using Entities;
using UnityEngine;

namespace GameWorld {
    public class SpawnPoint : Entity {
        public Color color;
        public float radius = 1;

        public bool IsAvailable {
            get {
                var colliders = Physics.OverlapSphere(transform.position, radius);

                return colliders.All(collider => ReferenceEquals(collider.GetComponent<Agent>(), null));
            }
        }
    }
}