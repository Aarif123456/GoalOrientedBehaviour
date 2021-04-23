using Random = UnityEngine.Random;

namespace Entities {
    public struct Biases {
        const float LOW_RANGE_OF_BIAS = 0.5f;
        const float HIGH_RANGE_OF_BIAS = 1.5f;

        public float HealthBias { get; }
        public float ExploreBias { get; }
        public float AttackBias { get; }
        public float EvadeBias { get; }
        public float ShotgunBias { get; }
        public float RailgunBias { get; }
        public float RocketLauncherBias { get; }

        private static float CreateRandomVal(){
            return Random.Range(LOW_RANGE_OF_BIAS, HIGH_RANGE_OF_BIAS);
        }
        /* these biases could be loaded in from a script on a per 
            bot basis but for now we'll just give them some random values
        */
        public Biases(float healthBias = -1, float exploreBias = -1,
                      float attackBias = -1, float evadeBias = -1,
                      float shotgunBias = -1, float railgunBias = -1,
                      float rocketLauncherBias = -1
                      ) {
            HealthBias = healthBias<0 ? CreateRandomVal() : healthBias;
            ExploreBias = exploreBias<0 ? CreateRandomVal() : exploreBias;
            AttackBias = attackBias<0 ? CreateRandomVal() : attackBias;
            EvadeBias = evadeBias<0 ? CreateRandomVal() : evadeBias;
            ShotgunBias = shotgunBias<0 ? CreateRandomVal() : shotgunBias;
            RailgunBias = railgunBias<0 ? CreateRandomVal() : railgunBias;
            RocketLauncherBias = rocketLauncherBias<0 ? CreateRandomVal() : rocketLauncherBias;

        }
    }
}