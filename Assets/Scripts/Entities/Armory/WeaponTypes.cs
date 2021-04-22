using System.ComponentModel;

namespace Entities.Armory {
    public enum WeaponTypes {
        Blaster,

        Railgun,

        [Description("Rocket launcher")] RocketLauncher,

        Shotgun
    }
}