using Entities.Steering;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public sealed class AdjustRange : Goal {
        private readonly Face look;
        private readonly Seek seek;
        private readonly Agent targetAgent;

        public AdjustRange(Agent agent, Agent targetAgent)
            : this(agent, targetAgent, 5, 1){
        }

        public AdjustRange(Agent agent, Agent targetAgent, float idealRange, float satisfactionRadius)
            : base(agent, GoalTypes.AdjustRange){
            this.targetAgent = targetAgent;
            seek = new Seek(agent.Kinematic);
            look = new Face(Agent.Kinematic, targetAgent.Kinematic.Position);
            IdealRange = idealRange;
            SatisfactionRadius = satisfactionRadius;
        }

        public float IdealRange { get; set; }
        public float SatisfactionRadius { get; set; }

        public override void Activate(){
            Status = StatusTypes.Active;
            Agent.SteeringBehaviours.Add(seek);
            Agent.SteeringBehaviours.Add(look);
        }

        public override StatusTypes Process(){
            ActivateIfInactive();

            if (IsStuck())
                Status = StatusTypes.Failed;
            else{
                var distanceFromTarget = Vector3.Distance(Agent.Kinematic.Position, targetAgent.Kinematic.Position);

                if (Mathf.Abs(distanceFromTarget - IdealRange) <= SatisfactionRadius)
                    Status = StatusTypes.Completed;
                else{
                    var idealPosition = targetAgent.Kinematic.Position;

                    if (Mathf.Approximately(distanceFromTarget, 0)){
                        Vector2 aDirection = Random.onUnitSphere;
                        idealPosition += new Vector3(aDirection.x, Agent.Kinematic.Position.y, aDirection.y) *
                                         IdealRange;
                    }
                    else{
                        idealPosition += (Agent.Kinematic.Position - targetAgent.Kinematic.Position).normalized *
                                         IdealRange;
                    }

                    seek.OtherKinematic.Position = idealPosition;
                }
            }

            return Status;
        }

        public override void Terminate(){
            Agent.SteeringBehaviours.Remove(seek);
            Agent.SteeringBehaviours.Remove(look);
        }

        private bool IsStuck(){
            return false;
        }
    }
}