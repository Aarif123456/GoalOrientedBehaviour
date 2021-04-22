using System.ComponentModel;

namespace Entities.Armory {
    public enum ItemTypes {
        Blaster,

        Health,

        Railgun,

        [Description("Rocket launcher")] RocketLauncher,

        Shotgun
    }
}