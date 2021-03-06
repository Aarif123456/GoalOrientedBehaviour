using Common;
using Entities.Armory;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public class EvaluatorGetWeapon : Evaluator {
        private readonly WeaponTypes _weaponType;

        public EvaluatorGetWeapon(float characterBias, WeaponTypes weaponType)
            : base(characterBias){
            _weaponType = weaponType;
            GoalName = "Get Weapon: " + EnumUtility.GetDescription(weaponType);
        }

        public override float CalculateDesirability(Agent agent){
            // grab the distance to the closest instance of the weapon type
            var distance = Feature.DistanceToItem(agent, EnumUtility.WeaponTypeToItemType(_weaponType));

            // if the distance feature is rated with a value of 1 it means that the
            // item is either not present on the map or too far away to be worth 
            // considering, therefore the desirability is zero
            if (distance == 1) return 0;

            // value used to tweak the desirability
            var tweaker = _weaponType switch{
                WeaponTypes.Railgun => Parameters.Instance.AgentRailgunGoalTweaker,
                WeaponTypes.RocketLauncher => Parameters.Instance.AgentRocketLauncherGoalTweaker,
                WeaponTypes.Shotgun => Parameters.Instance.AgentShotgunGoalTweaker,
                _ => 1.0f
            };

            var health = Feature.Health(agent);

            var weaponStrength = Feature.IndividualWeaponStrength(agent, _weaponType);

            var desirability = tweaker * health * (1 - weaponStrength) / distance;

            desirability *= characterBias;

            // ensure the value is in the range 0 to 1
            desirability = Mathf.Clamp(desirability, 0, 1);

            return desirability;
        }

        public override void SetGoal(Agent agent){
            agent.Brain.AddGoalGetItemOfType(EnumUtility.WeaponTypeToItemType(_weaponType));
        }
    }
}