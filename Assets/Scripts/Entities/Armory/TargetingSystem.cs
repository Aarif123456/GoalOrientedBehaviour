using UnityEngine;

namespace GameBrains.AI {
    public sealed class TargetingSystem {
        public TargetingSystem(Agent agent){
            Agent = agent;
            Target = null;
        }

        public Agent Agent { get; }

        public Agent Target { get; set; }

        public bool IsTargetPresent => Target != null;

        public bool IsTargetWithinFieldOfView => Agent.SensoryMemory.IsOpponentWithinFieldOfView(Target);

        public bool IsTargetShootable => Agent.SensoryMemory.IsOpponentShootable(Target);

        public Vector3 LastRecordedPosition => Agent.SensoryMemory.GetLastRecordedPosition(Target);

        public float TimeTargetVisible => Agent.SensoryMemory.GetTimeVisible(Target);

        public float TimeTargetOutOfView => Agent.SensoryMemory.GetTimeOutOfView(Target);

        public void ClearTarget(){
            Target = null;
        }

        public void Update(){
            if (Agent == null){
                return;
            }

            ClearTarget();

            var closestDistanceSoFar = float.MaxValue;
            var sensedAgents = Agent.SensoryMemory.GetListOfRecentlySensedOpponents();

            foreach (var sensedAgent in sensedAgents){
                // make sure the bot is alive and that it is not the owner
                if (!sensedAgent.IsAlive || sensedAgent == Agent){
                    continue;
                }

                var distance = (sensedAgent.Kinematic.Position - Agent.Kinematic.Position).magnitude;

                if (distance >= closestDistanceSoFar){
                    continue;
                }

                closestDistanceSoFar = distance;
                Target = sensedAgent;
            }
        }
    }
}