using System;
using System.ComponentModel;

namespace GameBrains.AI {
    /// <summary>
    ///     A helper class for dealing with Enums.
    /// </summary>
    public class EnumUtility {
        /// <summary>
        ///     Get the string representation of the enum value.
        /// </summary>
        /// <param name="value">
        ///     The enum value.
        /// </param>
        /// <returns>
        ///     The get description.
        /// </returns>
        public static string GetDescription(Enum value){
            var fi = value.GetType().GetField(value.ToString());
            var attributes =
                (DescriptionAttribute[]) fi.GetCustomAttributes(
                    typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        ///     Convert <paramref name="itemType" /> to its corresponding <see cref="EntityTypes" />.
        /// </summary>
        /// <param name="itemType">Item type to convert.</param>
        /// <returns>The corresponding <see cref="EntityTypes" />.</returns>
        /// <exception cref="Exception">Cannot determine item type.</exception>
        public static EntityTypes ItemTypeToEntityType(ItemTypes itemType){
            switch (itemType){
                case ItemTypes.Blaster:
                    return EntityTypes.Blaster;

                case ItemTypes.Health:
                    return EntityTypes.Health;

                case ItemTypes.Railgun:
                    return EntityTypes.Railgun;

                case ItemTypes.RocketLauncher:
                    return EntityTypes.RocketLauncher;

                case ItemTypes.Shotgun:
                    return EntityTypes.Shotgun;

                default:
                    throw new Exception(
                        "ItemTypeToEntityType: cannot determine item type.");
            }
        }

        /// <summary>
        ///     Convert <paramref name="weaponType" /> to its corresponding <see cref="EntityTypes" />.
        /// </summary>
        /// <param name="weaponType">The weapon type to convert.</param>
        /// <returns>The corresponding <see cref="EntityTypes" />.</returns>
        /// <exception cref="Exception">Cannot determine weapon type.</exception>
        public static EntityTypes WeaponTypeToEntityType(WeaponTypes weaponType){
            switch (weaponType){
                case WeaponTypes.Blaster:
                    return EntityTypes.Blaster;

                case WeaponTypes.Railgun:
                    return EntityTypes.Railgun;

                case WeaponTypes.RocketLauncher:
                    return EntityTypes.RocketLauncher;

                case WeaponTypes.Shotgun:
                    return EntityTypes.Shotgun;

                default:
                    throw new Exception(
                        "WeaponTypeToEntityType: cannot determine weapon type.");
            }
        }

        /// <summary>
        ///     Convert <paramref name="weaponType" /> to its corresponding <see cref="ItemTypes" />.
        /// </summary>
        /// <param name="weaponType">The weapon type to convert.</param>
        /// <returns>The corresponding <see cref="ItemTypes" />.</returns>
        /// <exception cref="Exception">Cannot determine weapon type.</exception>
        public static ItemTypes WeaponTypeToItemType(WeaponTypes weaponType){
            switch (weaponType){
                case WeaponTypes.Blaster:
                    return ItemTypes.Blaster;

                case WeaponTypes.Railgun:
                    return ItemTypes.Railgun;

                case WeaponTypes.RocketLauncher:
                    return ItemTypes.RocketLauncher;

                case WeaponTypes.Shotgun:
                    return ItemTypes.Shotgun;

                default:
                    throw new Exception(
                        "WeaponTypeToEntityType: cannot determine weapon type.");
            }
        }

        /// <summary>
        ///     Convert <paramref name="entityType" /> to its corresponding <see cref="WeaponTypes" />.
        /// </summary>
        /// <param name="entityType">The entity type to convert.</param>
        /// <returns>The corresponding <see cref="WeaponTypes" />.</returns>
        /// <exception cref="Exception">Cannot determine weapon type.</exception>
        public static WeaponTypes EntityTypeToWeaponType(EntityTypes entityType){
            switch (entityType){
                case EntityTypes.Blaster:
                    return WeaponTypes.Blaster;

                case EntityTypes.Railgun:
                    return WeaponTypes.Railgun;

                case EntityTypes.RocketLauncher:
                    return WeaponTypes.RocketLauncher;

                case EntityTypes.Shotgun:
                    return WeaponTypes.Shotgun;

                default:
                    throw new Exception(
                        "EntityTypeToWeaponType: cannot determine weapon type.");
            }
        }

        /// <summary>
        ///     Convert an <paramref name="itemType" /> to the corresponding goal type.
        /// </summary>
        /// <param name="itemType">The item type.</param>
        /// <returns>The corresponding goal type.</returns>
        /// <exception cref="Exception">Cannot determine item type.</exception>
        public static GoalTypes ItemTypeToGoalType(ItemTypes itemType){
            switch (itemType){
                case ItemTypes.Health:
                    return GoalTypes.GetHealth;

                case ItemTypes.Railgun:
                    return GoalTypes.GetRailgun;

                case ItemTypes.RocketLauncher:
                    return GoalTypes.GetRocketLauncher;

                case ItemTypes.Shotgun:
                    return GoalTypes.GetShotgun;

                default:
                    throw new Exception(
                        "ItemTypeToGoalType: cannot determine item type.");
            }
        }
    }
}