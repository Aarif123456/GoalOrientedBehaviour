using Entities.Steering;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public class Strafe : Goal {
        private readonly Face look;
        private readonly float satisfactionRadius;
        private readonly Seek seek;
        private bool clockwise;
        private Vector3 strafeTarget;

        public Strafe(Agent agent, Agent targetAgent)
            : base(agent, GoalTypes.Strafe){
            clockwise = Random.Range(0, 2) == 0;
            satisfactionRadius = 0.1f;
            seek = new Seek(agent.Kinematic);
            seek.SatisfactionRadius = satisfactionRadius;
            //look = new Face(Agent.Kinematic, targetAgent.Kinematic);
            look = new Face(Agent.Kinematic, targetAgent.Kinematic.Position);
        }

        public override void Activate(){
            Status = StatusTypes.Active;

            Agent.SteeringBehaviours.Add(seek);
            Agent.SteeringBehaviours.Add(look);
            Agent.IsStrafing = true;

            if (clockwise){
                if (Agent.CanStepRight(0, out strafeTarget)){
                    seek.OtherKinematic.Position = strafeTarget;
                }
                else{
                    Status = StatusTypes.Inactive;
                    clockwise = !clockwise;
                }
            }
            else{
                if (Agent.CanStepLeft(0, out strafeTarget)){
                    seek.OtherKinematic.Position = strafeTarget;
                }
                else{
                    Status = StatusTypes.Inactive;
                    clockwise = !clockwise;
                }
            }
        }

        public override StatusTypes Process(){
            // if status is inactive, call Activate()
            ActivateIfInactive();

            // if target goes out of view terminate
            if (!Agent.TargetingSystem.IsTargetWithinFieldOfView){
                Status = StatusTypes.Completed;
            }

            // else if agent reaches the target position set status to inactive so
            // the goal is reactivated on the next update-step
            else if (Agent.IsAtPosition(strafeTarget, satisfactionRadius)){
                Status = StatusTypes.Inactive;
            }

            return Status;
        }

        public override void Terminate(){
            Agent.SteeringBehaviours.Remove(seek);
            Agent.SteeringBehaviours.Remove(look);
            Agent.IsStrafing = false;
        }
    }
}