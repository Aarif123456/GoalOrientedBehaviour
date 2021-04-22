using UnityEngine;

namespace GameBrains.AI {
    public sealed class ProjectileBolt : Projectile {
        public void OnTriggerEnter(Collider hitCollider){
            var hitEntity = hitCollider.GetComponent<Entity>();

            if (hitEntity != null){
                var hitPoint = hitCollider.ClosestPointOnBounds(transform.position);

                if (hitEntity.EntityType == EntityTypes.Wall){
                    ProcessImpact(hitEntity, hitPoint);
                }
                else if (hitEntity.EntityType == EntityTypes.Agent){
                    var hitAgent = hitEntity as Agent;

                    if (hitAgent != null && hitAgent != Shooter &&
                        (Parameters.Instance.FriendlyFire || Shooter.color != hitAgent.color)){
                        ProcessImpact(hitEntity, hitPoint);
                        EventManager.Instance.Enqueue(
                            Events.DamageInflicted,
                            new DamageInflictedEventPayload(Shooter, hitAgent, hitPoint, DamageInflicted));
                    }
                }
            }
        }

        public void Spawn(Weapon weapon, Agent shooter, Vector3 targetPosition){
            var heightOffset = Vector3.up * 0.3f; // raise up to shoot over low walls
            Spawn(weapon, shooter, shooter.Kinematic.Position + heightOffset, targetPosition,
                Parameters.Instance.BoltDamage);

            name = "BOLT_" + id;
            EntityType = EntityTypes.Bolt;
            IsAiControlled = true;
            Kinematic.MaximumSpeed = Parameters.Instance.BoltMaximumSpeed;

            var directionToTarget = (targetPosition - (shooter.Kinematic.Position + heightOffset)).normalized;

            transform.position =
                shooter.Kinematic.Position + heightOffset + directionToTarget * shooter.Kinematic.Radius;
            transform.localScale = new Vector3(0.25f, 1, 0.25f);
            transform.LookAt(targetPosition);
            transform.Rotate(Vector3.right, 90);

            Kinematic.SetVelocity(directionToTarget * Parameters.Instance.BoltMaximumSpeed);
        }

        private void ProcessImpact(Entity hitEntity, Vector3 hitPoint){
            ImpactPoint = hitPoint;
            HasImpacted = true;
            SetDead();
            Destroy(gameObject);
            Weapon.OnProjectileRemoved();
        }
    }
}