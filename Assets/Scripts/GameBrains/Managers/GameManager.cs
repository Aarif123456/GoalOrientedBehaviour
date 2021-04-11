using System.Collections.Generic;

using GameBrains.AI;

using UnityEngine;
    
public sealed class GameManager : MonoBehaviour
{
    public GameObject spawnPointPrefab;
    public GameObject boltPrefab;
    public GameObject rocketPrefab;
    public GameObject slugPrefab;
    public GameObject pelletPrefab;
    public GameObject soundNotifyTriggerPrefab;
    
    private static GameManager _instance;
    
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("Game").GetComponent<GameManager>();
            }
            
            return _instance;
        }
    }
    
    public void Update()
    {
        bool isSpawnPossible = true;
        
        foreach (Agent agent in EntityManager.FindAll<Agent>())
        {
            // if this agent's status is 'spawning' attempt to resurrect it
            // from an unoccupied spawn point
            if (agent.IsSpawning && isSpawnPossible)
            {
                isSpawnPossible = AttemptToAddAgent(agent);
            }
            // if this agent's status is 'dead' add a grave at its current
            // location then change its status to 'spawning'
            else if (agent.IsDead)
            {
                // create a grave
                AddGrave(agent.Kinematic.Position);

                // change its status to spawning
                agent.State = Entity.States.Spawning;
            }

            // if this agent is alive update it.
            else if (agent.IsAlive)
            {
                          
            }
        }
    }
    
    public bool AttemptToAddAgent(Agent agent)
    {
        List<SpawnPoint> spawnPoints = EntityManager.FindAll<SpawnPoint>();
        
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.color == agent.color && spawnPoint.IsAvailable)
            {
                Vector3 position = spawnPoint.transform.position;
                position.y = 1.1f; // could place higher and let drop in with gravity
                agent.Spawn(position);
            }
        }
        
        return false;
    }
    
    public void AddGrave(Vector3 position)
    {
    }
}
