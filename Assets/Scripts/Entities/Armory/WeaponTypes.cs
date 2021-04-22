using System.ComponentModel;

namespace GameBrains.AI {
    public enum WeaponTypes {
        Blaster,

        Railgun,

        [Description("Rocket launcher")] RocketLauncher,

        Shotgun
    }
}