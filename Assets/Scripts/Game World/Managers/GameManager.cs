using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;

namespace GameWorld.Managers {
    public sealed class GameManager : MonoBehaviour {
        private static GameManager _instance;
        private static List<Agent> _agents;
        private static List<SpawnPoint> _spawnPoints;
        public GameObject boltPrefab;
        public GameObject rocketPrefab;
        public GameObject slugPrefab;
        public GameObject pelletPrefab;
        public GameObject soundNotifyTriggerPrefab;

        public static GameManager Instance {
            get { return _instance ??= GameObject.Find("Game").GetComponent<GameManager>(); }
        }

        public void Start(){
            _agents = EntityManager.FindAll<Agent>();
            _spawnPoints = EntityManager.FindAll<SpawnPoint>();
        }

        public void Update(){
            var isSpawnPossible = true;
            foreach (var agent in _agents){
                // if this agent's status is 'dead' add a grave at its current
                // location then change its status to 'spawning'
                if (agent.IsDead){
                    // create a grave
                    AddGrave(agent.Kinematic.Position);

                    // change its status to spawning
                    agent.SetSpawning();
                }

                // if this agent's status is 'spawning' attempt to resurrect it
                // from an unoccupied spawn point
                if (agent.IsSpawning && isSpawnPossible)
                    isSpawnPossible = AttemptToAddAgent(agent);
                // if this agent is alive update it.
                else if (agent.IsAlive){
                }
            }
        }

        private static bool AttemptToAddAgent(Agent agent){
            foreach (var position in from spawnPoint in _spawnPoints
                where spawnPoint.color == agent.color && spawnPoint.IsAvailable
                select spawnPoint.transform.position){
                // position.y = 1.1f; // could place higher and let drop in with gravity
                agent.Spawn(position);
                return true;
            }

            return false;
        }

        private static void AddGrave(Vector3 position){
        }
    }
}