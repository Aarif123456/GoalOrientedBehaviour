namespace GameBrains.AI
{
    using UnityEngine;
    
    public sealed class ProjectileBolt : Projectile
    {
        public void Spawn(Weapon weapon, Agent shooter, Vector3 targetPosition)
        {
            Vector3 heightOffset = Vector3.up * 0.3f; // raise up to shoot over low walls
            Spawn(weapon, shooter, shooter.Kinematic.Position + heightOffset, targetPosition, Parameters.Instance.BoltDamage);    
            
            name = "BOLT_" + id;
            EntityType = EntityTypes.Bolt;
            IsAiControlled = true;
            Kinematic.MaximumSpeed = Parameters.Instance.BoltMaximumSpeed;
            
            Vector3 directionToTarget = (targetPosition - (shooter.Kinematic.Position + heightOffset)).normalized;
            
            transform.position = shooter.Kinematic.Position + heightOffset + directionToTarget * shooter.Kinematic.Radius;    
            transform.localScale = new Vector3(0.25f, 1, 0.25f);
            transform.LookAt(targetPosition);
            transform.Rotate(Vector3.right, 90);
            
            Kinematic.SetVelocity(directionToTarget * Parameters.Instance.BoltMaximumSpeed);
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
                        EventManager.Instance.Enqueue<DamageInflictedEventPayload>(
                            Events.DamageInflicted,
                            new DamageInflictedEventPayload(Shooter, hitAgent, hitPoint, DamageInflicted));
                    }
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