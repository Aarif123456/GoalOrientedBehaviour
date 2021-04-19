namespace GameBrains.AI
{
    using System.Collections.Generic;
    
    using UnityEngine;
    
    public sealed class TargetingSystem
    {
        public TargetingSystem(Agent agent)
        {
            Agent = agent;
            Target = null;
        }
        
        public Agent Agent { get; private set; }
        
        public Agent Target { get; set; }
        
        public bool IsTargetPresent
        {
            get { return Target != null; }
        }
        
        public bool IsTargetWithinFieldOfView
        {
            get
            {
                return Agent.SensoryMemory.IsOpponentWithinFieldOfView(Target);
            }
        }
        
        public bool IsTargetShootable
        {
            get
            {
                return Agent.SensoryMemory.IsOpponentShootable(Target);
            }
        }
        
        public Vector3 LastRecordedPosition
        {
            get
            {
                return Agent.SensoryMemory.GetLastRecordedPosition(Target);
            }
        }
        
        public float TimeTargetVisible
        {
            get
            {
                return Agent.SensoryMemory.GetTimeVisible(Target);
            }
        }
        
        public float TimeTargetOutOfView
        {
            get
            {
                return Agent.SensoryMemory.GetTimeOutOfView(Target);
            }
        }
        
        public void ClearTarget()
        {
            Target = null;
        }
        
        public void Update()
        {
            if (Agent == null)
            {
                return;
            }
            
            ClearTarget();
            
            float closestDistanceSoFar = float.MaxValue;
            List<Agent> sensedAgents = Agent.SensoryMemory.GetListOfRecentlySensedOpponents();
            
            foreach (Agent sensedAgent in sensedAgents)
            {
                // make sure the bot is alive and that it is not the owner
                if (!sensedAgent.IsAlive || sensedAgent == Agent)
                {
                    continue;
                }

                float distance = (sensedAgent.Kinematic.Position - Agent.Kinematic.Position).magnitude;

                if (distance >= closestDistanceSoFar)
                {
                    continue;
                }

                closestDistanceSoFar = distance;
                Target = sensedAgent;
            }
        }
    }
}