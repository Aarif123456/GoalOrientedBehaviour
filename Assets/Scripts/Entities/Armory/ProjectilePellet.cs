using Common;
using GameWorld;
using UnityEngine;

namespace Entities.Armory {
    public sealed class ProjectilePellet : Projectile {
        public void OnTriggerEnter(Collider hitCollider){
            var hitEntity = hitCollider.GetComponent<Entity>();

            if (hitEntity == null) return;
            var hitPoint = hitCollider.ClosestPointOnBounds(transform.position);

            switch (hitEntity.EntityType){
                case EntityTypes.Wall:
                    ProcessImpact(hitEntity, hitPoint);
                    break;
                case EntityTypes.Agent:{
                    var hitAgent = hitEntity as Agent;

                    if (hitAgent == null || hitAgent == Shooter ||
                        !Parameters.Instance.FriendlyFire && Shooter.color == hitAgent.color) return;
                    ProcessImpact(hitEntity, hitPoint);
                    EventManager.Instance.Enqueue(
                        Events.DAMAGE_INFLICTED,
                        new DamageInflictedEventPayload(Shooter, hitAgent, hitPoint, DamageInflicted));
                    break;
                }
            }
        }

        public void Spawn(Weapon weapon, Agent shooter, Vector3 targetPosition){
            var heightOffset = Vector3.up * 0.3f; // raise up to shoot over low walls
            Spawn(weapon, shooter, shooter.Kinematic.Position + heightOffset, targetPosition,
                Parameters.Instance.PelletDamage);

            name = "PELLET_" + id;
            EntityType = EntityTypes.Pellet;
            IsAiControlled = true;
            Kinematic.MaximumSpeed = Parameters.Instance.PelletMaximumSpeed;

            var directionToTarget = (targetPosition - (shooter.Kinematic.Position + heightOffset)).normalized;

            var transform1 = transform;
            transform1.position =
                shooter.Kinematic.Position + heightOffset + directionToTarget * shooter.Kinematic.Radius;
            transform1.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            Kinematic.SetVelocity(directionToTarget * Parameters.Instance.PelletMaximumSpeed);
        }
    }
}