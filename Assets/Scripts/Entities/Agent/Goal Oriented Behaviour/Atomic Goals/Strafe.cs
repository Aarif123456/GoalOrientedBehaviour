using Entities.Steering;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public class Strafe : Goal {
        private readonly Face _look;
        private readonly float _satisfactionRadius;
        private readonly Seek _seek;
        private bool _clockwise;
        private Vector3 _strafeTarget;

        public Strafe(Agent agent, Entity targetAgent)
            : base(agent, GoalTypes.Strafe){
            _clockwise = Random.Range(0, 2) == 0;
            _satisfactionRadius = 0.1f;
            _seek = new Seek(agent.Kinematic){SatisfactionRadius = _satisfactionRadius};
            //look = new Face(Agent.Kinematic, targetAgent.Kinematic);
            _look = new Face(Agent.Kinematic, targetAgent.Kinematic.Position);
        }

        public override void Activate(){
            Status = StatusTypes.Active;

            Agent.SteeringBehaviours.Add(_seek);
            Agent.SteeringBehaviours.Add(_look);
            Agent.IsStrafing = true;

            if (_clockwise){
                if (Agent.CanStepRight(0, out _strafeTarget))
                    _seek.OtherKinematic.Position = _strafeTarget;
                else{
                    Status = StatusTypes.Inactive;
                    _clockwise = !_clockwise;
                }
            }
            else{
                if (Agent.CanStepLeft(0, out _strafeTarget))
                    _seek.OtherKinematic.Position = _strafeTarget;
                else{
                    Status = StatusTypes.Inactive;
                    _clockwise = !_clockwise;
                }
            }
        }

        public override StatusTypes Process(){
            // if status is inactive, call Activate()
            ActivateIfInactive();

            // if target goes out of view terminate
            if (!Agent.TargetingSystem.IsTargetWithinFieldOfView)
                Status = StatusTypes.Completed;

            // else if agent reaches the target position set status to inactive so
            // the goal is reactivated on the next update-step
            else if (Agent.IsAtPosition(_strafeTarget, _satisfactionRadius)) Status = StatusTypes.Inactive;

            return Status;
        }

        public override void Terminate(){
            Agent.SteeringBehaviours.Remove(_seek);
            Agent.SteeringBehaviours.Remove(_look);
            Agent.IsStrafing = false;
        }
    }
}