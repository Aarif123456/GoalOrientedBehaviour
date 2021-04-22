using UnityEngine;

namespace GameBrains.AI {
    public class Face : Align {
        public Face(Kinematic agentKinematic, Vector3 targetPosition)
            : this(agentKinematic, targetPosition, 0.25f, 5){
        }

        public Face(Kinematic agentKinematic, Kinematic targetKinematic)
            : this(agentKinematic, targetKinematic, 0.25f, 5){
        }

        public Face(Kinematic agentKinematic, Vector3 targetPosition, float timeToTarget, float satisfactionRadius)
            : this(agentKinematic, new Kinematic{Position = targetPosition}, timeToTarget, satisfactionRadius){
        }

        public Face(Kinematic agentKinematic, Kinematic targetKinematic, float timeToTarget, float satisfactionRadius)
            : base(agentKinematic, targetKinematic, timeToTarget, satisfactionRadius){
        }

        public override Steering Steer(){
            var direction = OtherKinematic.Position - AgentKinematic.Position;

            if (Mathf.Approximately(direction.magnitude, 0f)){
                return new Steering{Type = Steering.Types.Velocities};
            }

            // TODO: dangerous side-effect
            OtherKinematic.Rotation = Math.GetLookAtVector(AgentKinematic, OtherKinematic);

            return base.Steer();
        }
    }
}