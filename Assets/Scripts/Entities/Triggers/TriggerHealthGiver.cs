using Common;
using UnityEngine;

namespace Entities.Triggers {
    public sealed class TriggerHealthGiver : TriggerRespawning {
        /// <summary>
        ///     Gets the amount of health an agent receives when it runs over this trigger.
        /// </summary>
        public int HealthGiven { get; private set; }

        public override void Awake(){
            base.Awake();

            EntityType = EntityTypes.Health;
            HealthGiven = Parameters.Instance.DefaultHealthGiven;
            TimeBetweenRespawns = Parameters.Instance.HealthRespawnDelay;
        }

        public void OnTriggerEnter(Collider triggeringCollider){
            if (!IsActive) return;
            TriggeringAgent = triggeringCollider.GetComponent<Agent>();

            if (TriggeringAgent == null) return;
            TriggeringAgent.IncreaseHealth(HealthGiven);

            Deactivate();
        }
    }
}