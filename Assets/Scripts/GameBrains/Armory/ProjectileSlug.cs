namespace GameBrains.AI
{
    using UnityEngine;
    
    public sealed class ProjectileSlug : Projectile
    {
        public void Spawn(Weapon weapon, Agent shooter, Vector3 targetPosition)
        {
            Vector3 heightOffset = Vector3.up * 0.3f; // raise up to shoot over low walls
            Spawn(weapon, shooter, shooter.Kinematic.Position + heightOffset, targetPosition, Parameters.Instance.SlugDamage);    
            
            name = "PELLET_" + id;
            EntityType = EntityTypes.Pellet;
            IsAiControlled = true;
            Kinematic.MaximumSpeed = Parameters.Instance.SlugMaximumSpeed;
            
            Vector3 directionToTarget = (targetPosition - (shooter.Kinematic.Position + heightOffset)).normalized;
            
            transform.position = shooter.Kinematic.Position + heightOffset + directionToTarget * shooter.Kinematic.Radius;    
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            
            Kinematic.SetVelocity(directionToTarget * Parameters.Instance.SlugMaximumSpeed);
        }
        
        public void OnTriggerEnter(Collider hitCollider)
        {
            Entity hitEntity = hitCollider.GetComponent<Entity>();
            
            if (hitEntity != null)
            {
                Vector3 hitPoint = hitCollider.ClosestPointOnBounds(transform.position);
                
                if (hitEntity.EntityType == EntityTypes.Wall)
                {
                    ProcessImpact(hitEntity, hitPoint);
                }
                else if (hitEntity.EntityType == EntityTypes.Agent)
                {
                    Agent hitAgent = hitEntity as Agent;
                    
                    if (hitAgent != null && hitAgent != Shooter && (Parameters.Instance.FriendlyFire || Shooter.color != hitAgent.color))
                    {
                        //ProcessImpact(hitEntity, hitPoint); // high speed slugs penetrate multiple targets
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
