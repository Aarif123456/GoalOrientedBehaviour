using Entities.Steering;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public sealed class AdjustRange : Goal {
        private readonly Face _look;
        private readonly Seek _seek;
        private readonly Agent _targetAgent;

        public AdjustRange(Agent agent, Agent targetAgent, float idealRange = 5, float satisfactionRadius = 1)
            : base(agent, GoalTypes.AdjustRange){
            _targetAgent = targetAgent;
            _seek = new Seek(agent.Kinematic);
            _look = new Face(Agent.Kinematic, targetAgent.Kinematic.Position);
            IdealRange = idealRange;
            SatisfactionRadius = satisfactionRadius;
        }

        public float IdealRange { get; set; }
        public float SatisfactionRadius { get; set; }

        public override void Activate(){
            Status = StatusTypes.Active;
            Agent.SteeringBehaviours.Add(_seek);
            Agent.SteeringBehaviours.Add(_look);
        }

        public override StatusTypes Process(){
            ActivateIfInactive();

            if (IsStuck())
                Status = StatusTypes.Failed;
            else{
                var distanceFromTarget = Vector3.Distance(Agent.Kinematic.Position, _targetAgent.Kinematic.Position);

                if (Mathf.Abs(distanceFromTarget - IdealRange) <= SatisfactionRadius)
                    Status = StatusTypes.Completed;
                else{
                    var idealPosition = _targetAgent.Kinematic.Position;

                    if (Mathf.Approximately(distanceFromTarget, 0)){
                        Vector2 aDirection = Random.onUnitSphere;
                        idealPosition += new Vector3(aDirection.x, Agent.Kinematic.Position.y, aDirection.y) *
                                         IdealRange;
                    }
                    else{
                        idealPosition += (Agent.Kinematic.Position - _targetAgent.Kinematic.Position).normalized *
                                         IdealRange;
                    }

                    _seek.OtherKinematic.Position = idealPosition;
                }
            }

            return Status;
        }

        public override void Terminate(){
            Agent.SteeringBehaviours.Remove(_seek);
            Agent.SteeringBehaviours.Remove(_look);
        }

        private static bool IsStuck(){
            return false;
        }
    }
}