using Common;
using Entities.Steering;
using GameWorld;
using UnityEngine;

namespace Entities.Armory {
    public sealed class ProjectileRocket : Projectile {
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
                    InflictDamageOnBotsWithinBlastRadius();
                    break;
                }
            }
        }

        public void Spawn(Weapon weapon, Agent shooter, Vector3 targetPosition){
            var heightOffset = Vector3.up * 0.3f; // raise up to shoot over low walls
            Spawn(weapon, shooter, shooter.Kinematic.Position + heightOffset, targetPosition,
                Parameters.Instance.RocketDamage);

            name = "ROCKET_" + id;
            EntityType = EntityTypes.Rocket;
            IsAiControlled = true;
            Kinematic.MaximumSpeed = Parameters.Instance.RocketMaximumSpeed;

            var directionToTarget = (targetPosition - (shooter.Kinematic.Position + heightOffset)).normalized;

            var transform1 = transform;
            transform1.position =
                shooter.Kinematic.Position + heightOffset + directionToTarget * shooter.Kinematic.Radius;
            transform1.localScale = new Vector3(0.25f, 0.5f, 0.25f);
            transform.LookAt(targetPosition);
            transform.Rotate(Vector3.right, 90);

            if (Parameters.Instance.RocketIsHeatSeeking)
                SteeringBehaviours.Add(new Seek(Kinematic, shooter.TargetingSystem.Target.Kinematic));
            else
                Kinematic.SetVelocity(directionToTarget * Parameters.Instance.RocketMaximumSpeed);
        }

        protected override void Act(float deltaTime){
            var targetPosition = TargetPosition;
            targetPosition.y = Kinematic.Position.y;
            if (!IsAtPosition(TargetPosition, 0.1f)) return;
            ProcessImpact(null, TargetPosition); // TODO: handle multiple impacts??
            InflictDamageOnBotsWithinBlastRadius();
        }

        private void InflictDamageOnBotsWithinBlastRadius(){
            foreach (var agent in EntityManager.FindAll<Agent>()){
                var distanceFromBlast = Vector3.Distance(Kinematic.Position, agent.Kinematic.Position) -
                                        agent.Kinematic.Radius;
                if (!(distanceFromBlast <= Parameters.Instance.RocketBlastRadius)) continue;
                var fallOffFactor = (Parameters.Instance.RocketBlastRadius - distanceFromBlast) /
                                    Parameters.Instance.RocketBlastRadius;
                EventManager.Instance.Enqueue(
                    Events.DAMAGE_INFLICTED,
                    new DamageInflictedEventPayload(Shooter, agent,
                        agent.GetComponent<Collider>().ClosestPointOnBounds(Kinematic.Position),
                        DamageInflicted * fallOffFactor));
            }
        }
    }
}