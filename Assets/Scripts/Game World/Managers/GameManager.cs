using Entities;
using UnityEngine;

namespace GameWorld.Managers {
    public sealed class GameManager : MonoBehaviour {
        private static GameManager _instance;
        public GameObject boltPrefab;
        public GameObject rocketPrefab;
        public GameObject slugPrefab;
        public GameObject pelletPrefab;
        public GameObject soundNotifyTriggerPrefab;

        public static GameManager Instance {
            get { return _instance ??= GameObject.Find("Game").GetComponent<GameManager>(); }
        }

        public void Update(){
            var isSpawnPossible = true;

            foreach (var agent in EntityManager.FindAll<Agent>())
                // if this agent's status is 'spawning' attempt to resurrect it
                // from an unoccupied spawn point
            {
                if (agent.IsSpawning && isSpawnPossible)
                    isSpawnPossible = AttemptToAddAgent(agent);
                // if this agent's status is 'dead' add a grave at its current
                // location then change its status to 'spawning'
                else if (agent.IsDead){
                    // create a grave
                    AddGrave(agent.Kinematic.Position);

                    // change its status to spawning
                    agent.State = Entity.States.Spawning;
                }

                // if this agent is alive update it.
                else if (agent.IsAlive){
                }
            }
        }

        private static bool AttemptToAddAgent(Agent agent){
            var spawnPoints = EntityManager.FindAll<SpawnPoint>();

            foreach (var spawnPoint in spawnPoints){
                if (spawnPoint.color != agent.color || !spawnPoint.IsAvailable) continue;

                var position = spawnPoint.transform.position;
                position.y = 1.1f; // could place higher and let drop in with gravity
                agent.Spawn(position);
            }

            return false;
        }

        public void AddGrave(Vector3 position){
        }
    }
}