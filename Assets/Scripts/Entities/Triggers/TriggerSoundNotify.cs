using Common;
using GameWorld;
using UnityEngine;

namespace Entities.Triggers {
    public sealed class TriggerSoundNotify : TriggerTimeLimited {
        /// <summary>
        ///     Gets the agent that has made the sound.
        /// </summary>
        public Agent NoiseMakingAgent { get; set; }

        public override void Awake(){
            base.Awake();

            EntityType = EntityTypes.SoundNotifier;
            Lifetime = Parameters.Instance.SoundTriggerLifetime;
        }

        public void OnTriggerEnter(Collider triggeringCollider){
            TriggeringAgent = triggeringCollider.GetComponent<Agent>();

            if (TriggeringAgent != null){
                EventManager.Instance.Enqueue(
                    Events.WeaponSound,
                    new WeaponSoundEventPayload(TriggeringAgent, NoiseMakingAgent));
            }
        }
    }
}