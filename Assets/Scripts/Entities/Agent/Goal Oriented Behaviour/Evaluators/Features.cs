using System;
using Entities.Armory;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public sealed class Feature {
        public static float Health(Agent agent){
            return agent.Health / (float) agent.MaximumHealth;
        }

        public static float DistanceToItem(Agent agent, ItemTypes itemType){
            // determine the distance to the closest instance of the item type
            var distanceToItem = agent.PathPlanner.GetCostToClosestItem(itemType);

            // if the previous method returns a negative value then there is no item of
            // the specified type present in the game world at this time.
            if (distanceToItem < 0) return 1;

            // these values represent cutoffs. Any distance over maxDistance results in
            // a value of 1, and value below minDistance results in a value of 0
            const float MAX_DISTANCE = 500.0f;
            const float MIN_DISTANCE = 50.0f;
            distanceToItem = Mathf.Clamp(distanceToItem, MIN_DISTANCE, MAX_DISTANCE);
            return distanceToItem / MAX_DISTANCE;
        }

        public static float IndividualWeaponStrength(Agent agent, WeaponTypes weaponType){
            // grab a pointer to the gun (if the bot owns an instance)
            var weapon = agent.WeaponSystem.GetWeaponFromInventory(weaponType);

            if (weapon != null) return weapon.RoundsRemaining / GetMaxRoundsBotCanCarryForWeapon(weaponType);

            return 0.0f;
        }

        public static float TotalWeaponStrength(Agent agent){
            var maxRoundsForShotgun = GetMaxRoundsBotCanCarryForWeapon(WeaponTypes.Shotgun);
            var maxRoundsForRailgun = GetMaxRoundsBotCanCarryForWeapon(WeaponTypes.Railgun);
            var maxRoundsForRocketLauncher = GetMaxRoundsBotCanCarryForWeapon(WeaponTypes.RocketLauncher);
            var totalRoundsCarryable = maxRoundsForShotgun + maxRoundsForRailgun + maxRoundsForRocketLauncher;

            float numSlugs = agent.WeaponSystem.GetRoundsRemaining(WeaponTypes.Railgun);
            float numCartridges = agent.WeaponSystem.GetRoundsRemaining(WeaponTypes.Shotgun);
            float numRockets = agent.WeaponSystem.GetRoundsRemaining(WeaponTypes.RocketLauncher);

            // the value of the tweaker (must be in the range 0-1) indicates how much
            // desirability value is returned even if a bot has not picked up any weapons.
            // (it basically adds in an amount for a bot's persistent weapon -- the blaster)
            const float TWEAKER = 0.1f;

            return TWEAKER + (1.0f - TWEAKER) * (numSlugs + numCartridges + numRockets) / totalRoundsCarryable;
        }

        private static float GetMaxRoundsBotCanCarryForWeapon(WeaponTypes weaponType){
            switch (weaponType){
                case WeaponTypes.Railgun: return Parameters.Instance.RailgunMaximumRoundsCarried;

                case WeaponTypes.RocketLauncher: return Parameters.Instance.RocketLauncherMaximumRoundsCarried;

                case WeaponTypes.Shotgun: return Parameters.Instance.ShotgunMaximumRoundsCarried;

                default: throw new Exception("Trying to calculate  of unknown weapon.");
            }
        }
    }
}