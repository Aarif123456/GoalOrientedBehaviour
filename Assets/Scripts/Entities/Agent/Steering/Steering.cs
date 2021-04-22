using UnityEngine;

namespace Entities.Steering {
    public class Steering {
        public enum Types {
            Velocities,
            Accelerations,
            Forces
        }

        public Types Type { get; set; }

        public Vector3 Linear { get; set; }

        public Vector3 Angular { get; set; }
    }
}