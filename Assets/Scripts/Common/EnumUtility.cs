using System;
using System.ComponentModel;
using Entities;
using Entities.Armory;
using Entities.GoalOrientedBehaviour;

namespace Common {
    /// <summary>
    ///     A helper class for dealing with Enums.
    /// </summary>
    public static class EnumUtility {
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
            return itemType switch{
                ItemTypes.Blaster => EntityTypes.Blaster,
                ItemTypes.Health => EntityTypes.Health,
                ItemTypes.Railgun => EntityTypes.Railgun,
                ItemTypes.RocketLauncher => EntityTypes.RocketLauncher,
                ItemTypes.Shotgun => EntityTypes.Shotgun,
                _ => throw new Exception("ItemTypeToEntityType: cannot determine item type.")
            };
        }

        /// <summary>
        ///     Convert <paramref name="weaponType" /> to its corresponding <see cref="EntityTypes" />.
        /// </summary>
        /// <param name="weaponType">The weapon type to convert.</param>
        /// <returns>The corresponding <see cref="EntityTypes" />.</returns>
        /// <exception cref="Exception">Cannot determine weapon type.</exception>
        public static EntityTypes WeaponTypeToEntityType(WeaponTypes weaponType){
            return weaponType switch{
                WeaponTypes.Blaster => EntityTypes.Blaster,
                WeaponTypes.Railgun => EntityTypes.Railgun,
                WeaponTypes.RocketLauncher => EntityTypes.RocketLauncher,
                WeaponTypes.Shotgun => EntityTypes.Shotgun,
                _ => throw new Exception("WeaponTypeToEntityType: cannot determine weapon type.")
            };
        }

        /// <summary>
        ///     Convert <paramref name="weaponType" /> to its corresponding <see cref="ItemTypes" />.
        /// </summary>
        /// <param name="weaponType">The weapon type to convert.</param>
        /// <returns>The corresponding <see cref="ItemTypes" />.</returns>
        /// <exception cref="Exception">Cannot determine weapon type.</exception>
        public static ItemTypes WeaponTypeToItemType(WeaponTypes weaponType){
            return weaponType switch{
                WeaponTypes.Blaster => ItemTypes.Blaster,
                WeaponTypes.Railgun => ItemTypes.Railgun,
                WeaponTypes.RocketLauncher => ItemTypes.RocketLauncher,
                WeaponTypes.Shotgun => ItemTypes.Shotgun,
                _ => throw new Exception("WeaponTypeToEntityType: cannot determine weapon type.")
            };
        }

        /// <summary>
        ///     Convert <paramref name="entityType" /> to its corresponding <see cref="WeaponTypes" />.
        /// </summary>
        /// <param name="entityType">The entity type to convert.</param>
        /// <returns>The corresponding <see cref="WeaponTypes" />.</returns>
        /// <exception cref="Exception">Cannot determine weapon type.</exception>
        public static WeaponTypes EntityTypeToWeaponType(EntityTypes entityType){
            return entityType switch{
                EntityTypes.Blaster => WeaponTypes.Blaster,
                EntityTypes.Railgun => WeaponTypes.Railgun,
                EntityTypes.RocketLauncher => WeaponTypes.RocketLauncher,
                EntityTypes.Shotgun => WeaponTypes.Shotgun,
                _ => throw new Exception("EntityTypeToWeaponType: cannot determine weapon type.")
            };
        }

        /// <summary>
        ///     Convert an <paramref name="itemType" /> to the corresponding goal type.
        /// </summary>
        /// <param name="itemType">The item type.</param>
        /// <returns>The corresponding goal type.</returns>
        /// <exception cref="Exception">Cannot determine item type.</exception>
        public static GoalTypes ItemTypeToGoalType(ItemTypes itemType){
            return itemType switch{
                ItemTypes.Health => GoalTypes.GetHealth,
                ItemTypes.Railgun => GoalTypes.GetRailgun,
                ItemTypes.RocketLauncher => GoalTypes.GetRocketLauncher,
                ItemTypes.Shotgun => GoalTypes.GetShotgun,
                _ => throw new Exception("ItemTypeToGoalType: cannot determine item type.")
            };
        }
    }
}