using System.ComponentModel;
using UnityEngine;

namespace Common {
    public sealed class Parameters : MonoBehaviour {
        private static Parameters _instance;

        public bool friendlyFire = true;

        public float hitFlashTime = 0.2f;
        public float agentGoalAppraisalUpdateFrequency = 0.5f;

        public float agentVisionUpdateFrequency = 4;
        public float agentTargetingUpdateFrequency = 1;
        public float agentWeaponSelectionFrequency = 0.5f;
        public float agentMemorySpan = 5;
        public float agentFieldOfView = 135;

        public float agentReactionTime = 0.2f;
        public float agentAimPersistenceTime = 1;
        public float agentAimAccuracy;
        public int agentMaximumHealth = 100;
        public float agentHealthGoalTweaker = 1.0f;
        public float agentShotgunGoalTweaker = 1.0f;
        public float agentRailgunGoalTweaker = 1.0f;
        public float agentRocketLauncherGoalTweaker = 1.0f;
        public float agentAggroGoalTweaker = 1.0f;

        public int maximumSearchCyclesPerUpdateStep = 1000;

        public int defaultHealthGiven = 50;
        public float healthRespawnDelay = 10;
        public float weaponRespawnDelay = 15;

        public float blasterFiringFrequency = 3;
        public float blasterMaximumSpeed = 25;
        public int blasterDefaultRounds;
        public int blasterMaximumRoundsCarried;
        public float blasterIdealRange = 250;
        public float blasterSoundRange = 1000;
        public bool useBlaster;

        public int maximumActiveBolts = 5;
        public float boltMaximumSpeed = 25;
        public float boltMass = 1;
        public float boltMaximumForce = 6000.0f;
        public float boltScale = 2;
        public float boltDamage = 1;

        public float rocketLauncherFiringFrequency = 1.5f;
        public int rocketLauncherDefaultRounds = 5;
        public int rocketLauncherMaximumRoundsCarried = 15;
        public float rocketLauncherIdealRange = 750;
        public float rocketLauncherSoundRange = 2000;

        public int maximumActiveRockets = 2;
        public bool rocketIsHeatSeeking;
        public float rocketBlastRadius = 20;
        public float rocketMaximumSpeed = 10;
        public float rocketMass = 1;
        public float rocketMaximumForce = 600.0f;
        public float rocketScale = 1;
        public float rocketDamage = 10;
        public float rocketExplosionDecayRate = 40.0f;

        public float railgunFiringFrequency = 1;
        public int railgunDefaultRounds = 5;
        public int railgunMaximumRoundsCarried = 15;
        public float railgunIdealRange = 2000;
        public float railgunSoundRange = 2000;

        public int maximumActiveSlugs = 5;
        public float slugMaximumSpeed = 200;
        public float slugMass = 0.1f;
        public float slugMaximumForce = 1000000.0f;
        public float slugScale = 1;
        public float slugPersistence = 0.2f;
        public float slugDamage = 10;

        public float shotgunFiringFrequency = 1;
        public int shotgunDefaultRounds = 15;
        public int shotgunMaximumRoundsCarried = 30;
        public int shotgunBallsInShell = 10;
        public float shotgunSpread = 0.15f;
        public float shotgunIdealRange = 250;
        public float shotgunSoundRange = 3000;

        public int maximumActivePellets = 10;
        public float pelletMaximumSpeed = 50;
        public float pelletMass = 0.1f;
        public float pelletMaximumForce = 6000.0f;
        public float pelletScale = 1;
        public float pelletPersistence = 1f;
        public float pelletDamage = 1;

        public float soundTriggerLifetime = 2;
        public bool soundTriggerVisible;

        public static Parameters Instance =>
            _instance ? _instance : _instance = GameObject.Find("Game").GetComponent<Parameters>();

        /// <summary>
        ///     Gets or sets how long a flash is displayed when the agent is hit.
        /// </summary>
        [Category("Agents")]
        [Description("Is friendly fire enabled?")]
        public bool FriendlyFire => friendlyFire;

        /// <summary>
        ///     Gets or sets how long a flash is displayed when the agent is hit.
        /// </summary>
        [Category("Agents")]
        [Description("How long a flash is displayed when the agent is hit.")]
        public float HitFlashTime => hitFlashTime;

        /// <summary>
        ///     Gets or sets the number of times a second an agent 'thinks' about changing strategy.
        /// </summary>
        /// <remarks>
        ///     A frequency of -1 will disable the feature and a frequency of zero will ensure the
        ///     feature is updated every bot update.
        /// </remarks>
        [Category("Agents")]
        [Description("The number of times a second an agent 'thinks' about changing strategy.")]
        public float AgentGoalAppraisalUpdateFrequency => agentGoalAppraisalUpdateFrequency;

        /// <summary>
        ///     Gets or sets the number of times a second an agent updates its vision.
        /// </summary>
        /// <remarks>
        ///     A frequency of -1 will disable the feature and a frequency of zero will ensure the
        ///     feature is updated every bot update.
        /// </remarks>
        [Category("Agents")]
        [Description("The number of times a second an agent updates its vision.")]
        public float AgentVisionUpdateFrequency => agentVisionUpdateFrequency;

        /// <summary>
        ///     Gets or sets the number of times a second an agent updates its target info.
        /// </summary>
        /// <remarks>
        ///     A frequency of -1 will disable the feature and a frequency of zero will ensure the
        ///     feature is updated every bot update.
        /// </remarks>
        [Category("Agents")]
        [Description("The number of times a second an agent updates its target info.")]
        public float AgentTargetingUpdateFrequency => agentTargetingUpdateFrequency;

        /// <summary>
        ///     Gets or sets the number of times a second an agent 'thinks' about weapon selection.
        /// </summary>
        /// <remarks>
        ///     A frequency of -1 will disable the feature and a frequency of zero will ensure the
        ///     feature is updated every bot update.
        /// </remarks>
        [Category("Agents")]
        [Description("The number of times a second an agent 'thinks' about weapon selection.")]
        public float AgentWeaponSelectionFrequency => agentWeaponSelectionFrequency;

        /// <summary>
        ///     Gets or sets how long (in seconds) an agent's sensory memory persists.
        /// </summary>
        [Category("Agents")]
        [Description("How long (in seconds) an agent's sensory memory persists.")]
        public float AgentMemorySpan => agentMemorySpan;

        /// <summary>
        ///     Gets or sets the agent's field of view (in degrees).
        /// </summary>
        [Category("Agents")]
        [Description("The agent's field of view (in degrees).")]
        public float AgentFieldOfView => agentFieldOfView;

        /// <summary>
        ///     Gets or sets the agent's reaction time (in seconds).
        /// </summary>
        [Category("Agents")]
        [Description("The agent's reaction time (in seconds).")]
        public float AgentReactionTime => agentReactionTime;

        /// <summary>
        ///     Gets or sets how long (in seconds) the agent will keep pointing its weapon at its target
        ///     after the target goes out of view.
        /// </summary>
        [Category("Agents")]
        [Description(
            "How long (in seconds) the agent will keep pointing its weapon at its target after the target goes out of view.")]
        public float AgentAimPersistenceTime => agentAimPersistenceTime;

        /// <summary>
        ///     Gets or sets how accurate the bots are at aiming. 0 is very accurate, (the value
        ///     represents the max deviation in range (in degrees)).
        /// </summary>
        [Category("Agents")]
        [Description(
            "How accurate the agents are at aiming. 0 is very accurate, (the value represents the max deviation in range (in degrees)).")]
        public float AgentAimAccuracy => agentAimAccuracy;

        /// <summary>
        ///     Gets or sets the maximum health of an agent.
        /// </summary>
        [Category("Agents")]
        [Description("The maximum health of an agent.")]
        public int AgentMaximumHealth => agentMaximumHealth;

        /// <summary>
        ///     Gets or sets the value used to tweak desirability of the get health goal.
        /// </summary>
        [Category("Agents")]
        [Description("The value used to tweak desirability of the get health goal.")]
        public float AgentHealthGoalTweaker => agentHealthGoalTweaker;

        /// <summary>
        ///     Gets or sets the value used to tweak desirability of the get shotgun goal.
        /// </summary>
        [Category("Agents")]
        [Description("The value used to tweak desirability of the get shotgun goal.")]
        public float AgentShotgunGoalTweaker => agentShotgunGoalTweaker;

        /// <summary>
        ///     Gets or sets the value used to tweak desirability of the get railgun goal.
        /// </summary>
        [Category("Agents")]
        [Description("The value used to tweak desirability of the get railgun goal.")]
        public float AgentRailgunGoalTweaker => agentRailgunGoalTweaker;

        /// <summary>
        ///     Gets or sets the value used to tweak desirability of the get rocket launcher goal.
        /// </summary>
        [Category("Agents")]
        [Description("The value used to tweak desirability of the get rocket launcher goal.")]
        public float AgentRocketLauncherGoalTweaker => agentRocketLauncherGoalTweaker;

        /// <summary>
        ///     Gets or sets the value used to tweak desirability of the attack target goal.
        /// </summary>
        [Category("Agent")]
        [Description("The value used to tweak desirability of the attack target goal.")]
        public float AgentAggroGoalTweaker => agentAggroGoalTweaker;

        /// <summary>
        ///     Gets or sets the maximum number of search cycles allocated to each path planning search per update.
        /// </summary>
        [Category("Map")]
        [Description("The maximum number of search cycles allocated to each path planning search per update.")]
        public int MaximumSearchCyclesPerUpdateStep => maximumSearchCyclesPerUpdateStep;

        /// <summary>
        ///     Gets or sets how much health a giver-trigger gives..
        /// </summary>
        [Category("Map")]
        [Description("How much health a giver-trigger gives.")]
        public int DefaultHealthGiven => defaultHealthGiven;

        /// <summary>
        ///     Gets or sets how many seconds before a giver-trigger reactivates itself.
        /// </summary>
        [Category("Map")]
        [Description("How many seconds before a giver-trigger reactivates itself.")]
        public float HealthRespawnDelay => healthRespawnDelay;

        /// <summary>
        ///     Gets or sets how many seconds before a giver-trigger reactivates itself.
        /// </summary>
        [Category("Map")]
        [Description("How many seconds before a giver-trigger reactivates itself.")]
        public float WeaponRespawnDelay => weaponRespawnDelay;

        /// <summary>
        ///     Gets or sets the blaster rate of fire (shots per second).
        /// </summary>
        [Category("Blaster")]
        [Description("The blaster rate of fire (shots per second).")]
        public float BlasterFiringFrequency => blasterFiringFrequency;

        /// <summary>
        ///     Gets or sets the maximum speed of blaster (bolt) projectile.
        ///     TODO: this seems to duplicate <see cref="BoltMaximumSpeed" />.
        /// </summary>
        [Category("Blaster")]
        [Description("The maximum speed of blaster (bolt) projectile.")]
        public float BlasterMaximumSpeed => blasterMaximumSpeed;

        /// <summary>
        ///     Gets or sets the initial number of blaster rounds carried. Not used, a blaster always
        ///     has ammo.
        /// </summary>
        [Category("Blaster")]
        [Description("The initial number of blaster rounds carried. Not used, a blaster always has ammo.")]
        public int BlasterDefaultRounds => blasterDefaultRounds;

        /// <summary>
        ///     Gets or sets the maximum number of rounds for the blaster. Not used, a blaster always
        ///     has ammo.
        /// </summary>
        [Category("Blaster")]
        [Description("The maximum number of rounds for the blaster. Not used, a blaster always has ammo.")]
        public int BlasterMaxRoundsCarried => blasterMaximumRoundsCarried;

        /// <summary>
        ///     Gets or sets the ideal range to target for blaster.
        /// </summary>
        [Category("Blaster")]
        [Description("The ideal range to target for blaster.")]
        public float BlasterIdealRange => blasterIdealRange;

        /// <summary>
        ///     Gets or sets the distance blaster sound is heard.
        /// </summary>
        [Category("Blaster")]
        [Description("The distance blaster sound is heard.")]
        public float BlasterSoundRange => blasterSoundRange;

        /// <summary>
        ///     Gets or sets whether to use blaster.
        /// </summary>
        [Category("Blaster")]
        [Description("Whether to allow use of blaster.")]
        public bool UseBlaster => useBlaster;

        [Category("Bolt")]
        [Description("Maximum number of active bolts allowed at one time.")]
        public int MaximumActiveBolts => maximumActiveBolts;

        /// <summary>
        ///     Gets or sets the maximum speed of blaster (bolt) projectile.
        ///     TODO: this seems to duplicate <see cref="BlasterMaximumSpeed" />.
        /// </summary>
        [Category("Bolt")]
        [Description("The maximum speed of blaster (bolt) projectile.")]
        public float BoltMaximumSpeed => boltMaximumSpeed;

        /// <summary>
        ///     Gets or sets the mass of bolt projectile.
        /// </summary>
        [Category("Bolt")]
        [Description("The mass of bolt projectile.")]
        public float BoltMass => boltMass;

        /// <summary>
        ///     Gets or sets the maximum steering force for bolt projectile.
        /// </summary>
        [Category("Bolt")]
        [Description("The maximum steering force for bolt projectile.")]
        public float BoltMaximumForce => boltMaximumForce;

        /// <summary>
        ///     Gets or sets the scale of bolt projectile.
        /// </summary>
        [Category("Bolt")]
        [Description("The scale of bolt projectile.")]
        public float BoltScale => boltScale;

        /// <summary>
        ///     Gets or sets the damage inflicted by a bolt.
        /// </summary>
        [Category("Bolt")]
        [Description("The damage inflicted by a bolt.")]
        public float BoltDamage => boltDamage;

        /// <summary>
        ///     Gets or sets the rocket launcher rate of fire (shots per second).
        /// </summary>
        [Category("Rocket Launcher")]
        [Description("The rocket launcher rate of fire (shots per second).")]
        public float RocketLauncherFiringFrequency => rocketLauncherFiringFrequency;

        /// <summary>
        ///     Gets or sets the initial number of rounds carried.
        /// </summary>
        [Category("Rocket Launcher")]
        [Description("The initial number of rounds carried.")]
        public int RocketLauncherDefaultRounds => rocketLauncherDefaultRounds;

        /// <summary>
        ///     Gets or sets the maximum number of rounds carriable.
        /// </summary>
        [Category("Rocket Launcher")]
        [Description("The maximum number of rounds carriable.")]
        public int RocketLauncherMaximumRoundsCarried => rocketLauncherMaximumRoundsCarried;

        /// <summary>
        ///     Gets or sets the ideal range to target for rocket launcher.
        /// </summary>
        [Category("Rocket Launcher")]
        [Description("The ideal range to target for rocket launcher.")]
        public float RocketLauncherIdealRange => rocketLauncherIdealRange;

        /// <summary>
        ///     Gets or sets the distance rocket launcher sound is heard.
        /// </summary>
        [Category("Rocket Launcher")]
        [Description("The distance rocket launcher sound is heard.")]
        public float RocketLauncherSoundRange => rocketLauncherSoundRange;

        [Category("Rocket")]
        [Description("Maximum number of active rockets allowed at one time.")]
        public int MaximumActiveRockets => maximumActiveRockets;

        [Category("Rocket")]
        [Description("Whether rockets are heat seeking.")]
        public bool RocketIsHeatSeeking => rocketIsHeatSeeking;

        /// <summary>
        ///     Gets or sets the blast radius of exploding rocket projectile.
        /// </summary>
        [Category("Rocket")]
        [Description("The blast radius of exploding rocket projectile.")]
        public float RocketBlastRadius => rocketBlastRadius;

        /// <summary>
        ///     Gets or sets the maximum speed of rocket projectile.
        /// </summary>
        [Category("Rocket")]
        [Description("The maximum speed of rocket projectile.")]
        public float RocketMaximumSpeed => rocketMaximumSpeed;

        /// <summary>
        ///     Gets or sets the mass of rocket projectile.
        /// </summary>
        [Category("Rocket")]
        [Description("The mass of rocket projectile.")]
        public float RocketMass => rocketMass;

        /// <summary>
        ///     Gets or sets the maximum steering force for rocket projectile.
        /// </summary>
        [Category("Rocket")]
        [Description("The maximum steering force for rocket projectile.")]
        public float RocketMaxForce => rocketMaximumForce;

        /// <summary>
        ///     Gets or sets the scale of rocket projectile.
        /// </summary>
        [Category("Rocket")]
        [Description("The scale of rocket projectile.")]
        public float RocketScale => rocketScale;

        /// <summary>
        ///     Gets or sets the damage inflicted by a rocket projectile.
        /// </summary>
        [Category("Rocket")]
        [Description("The damage inflicted by a rocket projectile.")]
        public float RocketDamage => rocketDamage;

        /// <summary>
        ///     Gets or sets how fast the explosion occurs (in radius units per sec).
        /// </summary>
        [Category("Rocket")]
        [Description("How fast the explosion occurs (in radius units per sec).")]
        public float RocketExplosionDecayRate => rocketExplosionDecayRate;

        /// <summary>
        ///     Gets or sets the railgun rate of fire (shots per second).
        /// </summary>
        [Category("Railgun")]
        [Description("The railgun rate of fire (shots per second).")]
        public float RailgunFiringFrequency => railgunFiringFrequency;

        /// <summary>
        ///     Gets or sets the initial number of rounds carried.
        /// </summary>
        [Category("Railgun")]
        [Description("The initial number of rounds carried.")]
        public int RailgunDefaultRounds => railgunDefaultRounds;

        /// <summary>
        ///     Gets or sets the maximum number of rounds carriable.
        /// </summary>
        [Category("Railgun")]
        [Description("The maximum number of rounds carriable.")]
        public int RailgunMaximumRoundsCarried => railgunMaximumRoundsCarried;

        /// <summary>
        ///     Gets or sets the ideal range to target for railgun.
        /// </summary>
        [Category("Railgun")]
        [Description("The ideal range to target for railgun.")]
        public float RailgunIdealRange => railgunIdealRange;

        /// <summary>
        ///     Gets or sets the distance railgun sound is heard.
        /// </summary>
        [Category("Railgun")]
        [Description("The distance railgun sound is heard.")]
        public float RailgunSoundRange => railgunSoundRange;

        [Category("Slug")]
        [Description("Maximum number of active slugs allowed at one time.")]
        public int MaximumActiveSlugs => maximumActiveSlugs;

        /// <summary>
        ///     Gets or sets the maximum speed of slug projectile.
        /// </summary>
        [Category("Slug")]
        [Description("The maximum speed of slug projectile.")]
        public float SlugMaximumSpeed => slugMaximumSpeed;

        /// <summary>
        ///     Gets or sets the mass of slug projectile.
        /// </summary>
        [Category("Slug")]
        [Description("The mass of slug projectile.")]
        public float SlugMass => slugMass;

        /// <summary>
        ///     Gets or sets the maximum steering force for slug projectile.
        /// </summary>
        [Category("Slug")]
        [Description("The maximum steering force for slug projectile.")]
        public float SlugMaximumForce => slugMaximumForce;

        /// <summary>
        ///     Gets or sets the scale of slug projectile.
        /// </summary>
        [Category("Slug")]
        [Description("The scale of slug projectile.")]
        public float SlugScale => slugScale;

        /// <summary>
        ///     Gets or sets the time slug (and trajectory) remain visible.
        /// </summary>
        [Category("Slug")]
        [Description("The time slug (and trajectory) remain visible.")]
        public float SlugPersistence => slugPersistence;

        /// <summary>
        ///     Gets or sets the damage inflicted by a slug projectile.
        /// </summary>
        [Category("Slug")]
        [Description("The damage inflicted by a slug projectile.")]
        public float SlugDamage => slugDamage;

        /// <summary>
        ///     Gets or sets the shotgun rate of fire (shots per second).
        /// </summary>
        [Category("Shotgun")]
        [Description("The shotgun rate of fire (shots per second).")]
        public float ShotgunFiringFrequency => shotgunFiringFrequency;

        /// <summary>
        ///     Gets or sets the initial number of rounds carried.
        /// </summary>
        [Category("Shotgun")]
        [Description("The initial number of rounds carried.")]
        public int ShotgunDefaultRounds => shotgunDefaultRounds;

        /// <summary>
        ///     Gets or sets the maximum number of rounds carriable.
        /// </summary>
        [Category("Shotgun")]
        [Description("The maximum number of rounds carriable.")]
        public int ShotgunMaximumRoundsCarried => shotgunMaximumRoundsCarried;

        /// <summary>
        ///     Gets or sets the number of balls in a shotgun shell.
        /// </summary>
        [Category("Shotgun")]
        [Description("The number of balls in a shotgun shell.")]
        public int ShotgunBallsInShell => shotgunBallsInShell;

        /// <summary>
        ///     Gets or sets the spread angle (in radians) for shotgun balls.
        /// </summary>
        [Category("Shotgun")]
        [Description("The spread angle (in radians) for shotgun balls.")]
        public float ShotgunSpread => shotgunSpread;

        /// <summary>
        ///     Gets or sets the ideal range to target for shotgun.
        /// </summary>
        [Category("Shotgun")]
        [Description("The ideal range to target for shotgun.")]
        public float ShotgunIdealRange => shotgunIdealRange;

        /// <summary>
        ///     Gets or sets the distance shotgun sound is heard.
        /// </summary>
        [Category("Shotgun")]
        [Description("The distance shotgun sound is heard.")]
        public float ShotgunSoundRange => shotgunSoundRange;

        [Category("Pellets")]
        [Description("Maximum number of active pellets allowed at one time.")]
        public int MaximumActivePellets => maximumActivePellets;

        /// <summary>
        ///     Gets or sets the maximum speed of pellet projectile.
        /// </summary>
        [Category("Pellet")]
        [Description("The maximum speed of pellet projectile.")]
        public float PelletMaximumSpeed => pelletMaximumSpeed;

        /// <summary>
        ///     Gets or sets the mass of pellet projectile.
        /// </summary>
        [Category("Pellet")]
        [Description("The mass of pellet projectile.")]
        public float PelletMass => pelletMass;

        /// <summary>
        ///     Gets or sets the maximum steering force for pellet projectile.
        /// </summary>
        [Category("Pellet")]
        [Description("The maximum steering force for pellet projectile.")]
        public float PelletMaximumForce => pelletMaximumForce;

        /// <summary>
        ///     Gets or sets the scale of pellet projectile.
        /// </summary>
        [Category("Pellet")]
        [Description("The scale of pellet projectile.")]
        public float PelletScale => pelletScale;

        /// <summary>
        ///     Gets or sets the time pellets (and trajectory) remain visible.
        /// </summary>
        [Category("Pellet")]
        [Description("The time pellets (and trajectory) remain visible.")]
        public float PelletPersistence => pelletPersistence;

        /// <summary>
        ///     Gets or sets the damage inflicted by a pellet projectile.
        /// </summary>
        [Category("Pellet")]
        [Description("The damage inflicted by a pellet projectile.")]
        public float PelletDamage => pelletDamage;

        [Category("Triggers")]
        [Description("The time a sound trigger remains active.")]
        public float SoundTriggerLifetime => soundTriggerLifetime;

        [Category("Triggers")]
        [Description("Whether a sound trigger is visible (for debugging).")]
        public bool SoundTriggerVisible => soundTriggerVisible;
    }
}