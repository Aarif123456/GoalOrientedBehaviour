using Common;
using UnityEngine;

namespace Entities.Triggers {
    public enum WeaponGiverTypes {
        Railgun,
        RocketLauncher,
        Shotgun
    }

    public sealed class TriggerWeaponGiver : TriggerRespawning {
        public WeaponGiverTypes weaponGiverType;

        public override void Awake(){
            base.Awake();

            EntityType =
                weaponGiverType switch{
                    WeaponGiverTypes.Railgun => EntityTypes.Railgun,
                    WeaponGiverTypes.RocketLauncher => EntityTypes.RocketLauncher,
                    _ => EntityTypes.Shotgun
                };

            TimeBetweenRespawns = Parameters.Instance.WeaponRespawnDelay;
        }

        public void OnTriggerEnter(Collider triggeringCollider){
            if (!IsActive) return;
            TriggeringAgent = triggeringCollider.GetComponent<Agent>();

            if (TriggeringAgent == null) return;
            TriggeringAgent.WeaponSystem.AddWeapon(EnumUtility.EntityTypeToWeaponType(EntityType));

            Deactivate();
        }
    }
}