namespace GameBrains.AI
{
    using UnityEngine;
    
    public sealed class ProjectileRocket : Projectile
    {
        public void Spawn(Weapon weapon, Agent shooter, Vector3 targetPosition)
        {
            Vector3 heightOffset = Vector3.up * 0.3f; // raise up to shoot over low walls
            Spawn(weapon, shooter, shooter.Kinematic.Position + heightOffset, targetPosition, Parameters.Instance.RocketDamage);    
            
            name = "ROCKET_" + id;
            EntityType = EntityTypes.Rocket;
            IsAiControlled = true;
            Kinematic.MaximumSpeed = Parameters.Instance.RocketMaximumSpeed;
            
            Vector3 directionToTarget = (targetPosition - (shooter.Kinematic.Position + heightOffset)).normalized;
            
            transform.position = shooter.Kinematic.Position + heightOffset + directionToTarget * shooter.Kinematic.Radius;    
            transform.localScale = new Vector3(0.25f, 0.5f, 0.25f);
            transform.LookAt(targetPosition);
            transform.Rotate(Vector3.right, 90);
            
            if (Parameters.Instance.RocketIsHeatSeeking)
            {
                SteeringBehaviours.Add(new Seek(Kinematic, shooter.TargetingSystem.Target.Kinematic));
            }
            else
            {
                Kinematic.SetVelocity(directionToTarget * Parameters.Instance.RocketMaximumSpeed);
            }
        }
        
        public void OnTriggerEnter(Collider hitCollider)
        {
            Entity hitEntity = hitCollider.GetComponent<Entity>();
            
            if (hitEntity != null)
            {
                Vector3 hitPoint = hitCollider.ClosestPointOnBounds(transform.position);
                
                if (hitEntity.EntityType == EntityTypes.Wall )
                {
                    ProcessImpact(hitEntity, hitPoint);
                }
                else if (hitEntity.EntityType == EntityTypes.Agent)
                {
                    Agent hitAgent = hitEntity as Agent;
                    
                    if (hitAgent != null && hitAgent != Shooter && (Parameters.Instance.FriendlyFire || Shooter.color != hitAgent.color))
                    {
                        ProcessImpact(hitEntity, hitPoint);
                        InflictDamageOnBotsWithinBlastRadius();
                    }
                }
            }
        }
        
        protected override void Act(float deltaTime)
        {
            Vector3 targetPosition = TargetPosition;
            targetPosition.y = Kinematic.Position.y;
            if (IsAtPosition(TargetPosition, 0.1f))
            {
                ProcessImpact(null, TargetPosition); // TODO: handle multiple impacts??
                InflictDamageOnBotsWithinBlastRadius();
            }
        }
        
        private void InflictDamageOnBotsWithinBlastRadius()
        {
            foreach (Agent agent in EntityManager.FindAll<Agent>())
            {
                float distanceFromBlast = Vector3.Distance(Kinematic.Position, agent.Kinematic.Position) - agent.Kinematic.Radius;
                if (distanceFromBlast <= Parameters.Instance.RocketBlastRadius)
                {
                    float fallOffFactor = (Parameters.Instance.RocketBlastRadius - distanceFromBlast) / Parameters.Instance.RocketBlastRadius;
                    EventManager.Instance.Enqueue<DamageInflictedEventPayload>(
                        Events.DamageInflicted,
                        new DamageInflictedEventPayload(Shooter, agent, agent.GetComponent<Collider>().ClosestPointOnBounds(Kinematic.Position), DamageInflicted * fallOffFactor));
                }
            }
        }
    
        private void ProcessImpact(Entity hitEntity, Vector3 hitPoint)
        {
            ImpactPoint = hitPoint;
            HasImpacted = true;
            SetDead();
            Destroy(gameObject);
            Weapon.OnProjectileRemoved();
        }
    }
}