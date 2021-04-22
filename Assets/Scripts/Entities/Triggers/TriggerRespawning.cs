using UnityEngine;

namespace Entities.Triggers {
    public abstract class TriggerRespawning : Trigger {
        /// <summary>
        ///     Gets or sets the number of seconds the trigger is inactive before respawning.
        /// </summary>
        public float TimeBetweenRespawns { get; protected set; }

        /// <summary>
        ///     Gets or sets the number of seconds remaining until the trigger respawns.
        /// </summary>
        public float TimeUntilRespawn { get; protected set; }

        public override void Awake(){
            base.Awake();

            TimeBetweenRespawns = 0;
            TimeUntilRespawn = 0;
        }

        public override void Update(){
            base.Update();

            TimeUntilRespawn -= Time.deltaTime;

            if (TimeUntilRespawn <= 0 && !IsActive) IsActive = true;
        }

        /// <summary>
        ///     Set the trigger inactive for <see cref="TimeBetweenRespawns" /> seconds.
        /// </summary>
        protected void Deactivate(){
            IsActive = false;
            TimeUntilRespawn = TimeBetweenRespawns;
        }
    }
}