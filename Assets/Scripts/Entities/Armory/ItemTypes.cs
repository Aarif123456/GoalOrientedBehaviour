using System.ComponentModel;

namespace GameBrains.AI {
    public enum ItemTypes {
        Blaster,

        Health,

        Railgun,

        [Description("Rocket launcher")] RocketLauncher,

        Shotgun
    }
}