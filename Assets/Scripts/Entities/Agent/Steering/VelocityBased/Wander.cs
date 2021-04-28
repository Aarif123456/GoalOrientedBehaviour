using UnityEngine;
using Utility.Math;

namespace Entities.Steering {
    public sealed class Wander : SteeringBehaviour {
        private float _headingAngle;

        public Wander(Kinematic agentKinematic)
            : this(agentKinematic,
                new Seek(agentKinematic, new Kinematic()),
                new Face(agentKinematic, new Kinematic()),
                100.0f,
                2.0f,
                1){
        }

        public Wander(
            Kinematic agentKinematic,
            SteeringBehaviour move,
            SteeringBehaviour look,
            float wanderCircleOffset,
            float wanderCircleRadius,
            float maximumSlideDegrees)
            : base(agentKinematic){
            Move = move;
            Look = look;
            WanderCircleOffset = wanderCircleOffset;
            WanderCircleRadius = wanderCircleRadius;
            MaximumSlideDegrees = maximumSlideDegrees;
        }

        public SteeringBehaviour Move { get; set; }

        public SteeringBehaviour Look { get; set; }

        public float WanderCircleRadius { get; set; }

        public float WanderCircleOffset { get; set; }

        public float MaximumSlideDegrees { get; set; }

        public override Steering Steer(){
            var steering = new Steering{Type = Steering.Types.Velocities};

            if (WanderCircleRadius <= 0) return steering;

            _headingAngle += MaximumSlideDegrees - 2 * Random.value * MaximumSlideDegrees;
            _headingAngle = Math.WrapAngle(_headingAngle);

            var forwardDirection = new Vector3(AgentKinematic.Velocity.x, 0, AgentKinematic.Velocity.z);

            if (forwardDirection.magnitude > 0)
                forwardDirection.Normalize();
            else{
                forwardDirection = new Vector3(-Mathf.Sin(AgentKinematic.AngularVelocity.y), 0,
                    -Mathf.Cos(AgentKinematic.AngularVelocity.y));
                forwardDirection.Normalize();
            }

            var rightDirection = new Vector3(-forwardDirection.z, 0, forwardDirection.x);
            forwardDirection *= WanderCircleOffset;
            rightDirection *= -Mathf.Sin(_headingAngle) * WanderCircleRadius;

            var direction = forwardDirection + rightDirection;
            direction.Normalize();

            var targetKinematic = new Kinematic{
                Position = AgentKinematic.Position + direction * AgentKinematic.MaximumSpeed
            };

            if (Move != null){
                Move.OtherKinematic = targetKinematic;
                steering.Linear = Move.Steer().Linear;
            }

            if (Look == null) return steering;
            Look.OtherKinematic = targetKinematic;
            steering.Angular = Look.Steer().Angular;

            return steering;
        }
    }
}