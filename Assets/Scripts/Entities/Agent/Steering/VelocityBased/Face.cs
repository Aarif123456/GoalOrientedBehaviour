using UnityEngine;
using Utility.Math;

namespace Entities.Steering {
    public class Face : Align {
        public Face(Kinematic agentKinematic, Vector3 targetPosition, float timeToTarget = 0.25f, float satisfactionRadius = 5)
            : this(agentKinematic, new Kinematic{Position = targetPosition}, timeToTarget, satisfactionRadius){
        }

        public Face(Kinematic agentKinematic, Kinematic targetKinematic, float timeToTarget = 0.25f, float satisfactionRadius = 5)
            : base(agentKinematic, targetKinematic, timeToTarget, satisfactionRadius){
        }

        public override Steering Steer(){
            var direction = OtherKinematic.Position - AgentKinematic.Position;

            if (Mathf.Approximately(direction.magnitude, 0f)) return new Steering{Type = Steering.Types.Velocities};

            // TODO: dangerous side-effect
            OtherKinematic.Rotation = Math.GetLookAtVector(AgentKinematic, OtherKinematic);

            return base.Steer();
        }
    }
}