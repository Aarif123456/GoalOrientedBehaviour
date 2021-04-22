using System.ComponentModel;

namespace GameBrains.AI {
    public enum EntityTypes {
        [Description("Default entity type")] DefaultEntityType = -1,

        Wall,

        Agent,

        Unused,

        Waypoint,

        Health,

        [Description("Spawn point")] SpawnPoint,

        Railgun,
        Slug,

        [Description("Rocket launcher")] RocketLauncher,
        Rocket,

        Shotgun,
        Pellet,

        Blaster,
        Bolt,

        Obstacle,

        [Description("Sliding door")] SlidingDoor,

        [Description("Door Trigger")] DoorTrigger,

        Flag,

        [Description("Sound Notifier")] SoundNotifier
    }
}