using GameBrains.AI;
using UnityEngine;

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
            weaponGiverType == WeaponGiverTypes.Railgun
                ? EntityTypes.Railgun
                : weaponGiverType == WeaponGiverTypes.RocketLauncher
                    ? EntityTypes.RocketLauncher
                    : EntityTypes.Shotgun;

        TimeBetweenRespawns = Parameters.Instance.WeaponRespawnDelay;
    }

    public void OnTriggerEnter(Collider triggeringCollider){
        if (IsActive){
            TriggeringAgent = triggeringCollider.GetComponent<Agent>();

            if (TriggeringAgent != null){
                TriggeringAgent.WeaponSystem.AddWeapon(EnumUtility.EntityTypeToWeaponType(EntityType));

                Deactivate();
            }
        }
    }
}