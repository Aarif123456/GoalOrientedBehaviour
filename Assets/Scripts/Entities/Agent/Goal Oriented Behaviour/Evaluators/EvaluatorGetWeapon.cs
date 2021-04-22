using UnityEngine;

namespace GameBrains.AI {
    public class EvaluatorGetWeapon : Evaluator {
        private readonly WeaponTypes weaponType;

        public EvaluatorGetWeapon(float characterBias, WeaponTypes weaponType)
            : base(characterBias){
            this.weaponType = weaponType;
        }

        public override float CalculateDesirability(Agent agent){
            // grab the distance to the closest instance of the weapon type
            var distance = Feature.DistanceToItem(agent, EnumUtility.WeaponTypeToItemType(weaponType));

            // if the distance feature is rated with a value of 1 it means that the
            // item is either not present on the map or too far away to be worth 
            // considering, therefore the desirability is zero
            if (distance == 1){
                return 0;
            }

            // value used to tweak the desirability
            float tweaker;
            switch (weaponType){
                case WeaponTypes.Railgun:
                    tweaker =
                        Parameters.Instance.AgentRailgunGoalTweaker;
                    break;
                case WeaponTypes.RocketLauncher:
                    tweaker =
                        Parameters.Instance.AgentRocketLauncherGoalTweaker;
                    break;
                case WeaponTypes.Shotgun:
                    tweaker =
                        Parameters.Instance.AgentShotgunGoalTweaker;
                    break;
                default:
                    tweaker = 1.0f;
                    break;
            }

            var health = Feature.Health(agent);

            var weaponStrength = Feature.IndividualWeaponStrength(agent, weaponType);

            var desirability = tweaker * health * (1 - weaponStrength) / distance;

            desirability *= characterBias;

            // ensure the value is in the range 0 to 1
            desirability = Mathf.Clamp(desirability, 0, 1);

            return desirability;
        }

        public override void SetGoal(Agent agent){
            agent.Brain.AddGoalGetItemOfType(EnumUtility.WeaponTypeToItemType(weaponType));
        }
    }
}