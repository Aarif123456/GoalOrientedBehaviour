using System.Collections.Generic;
using GameWorld.Navigation.Graph;
using UnityEngine;

namespace GameWorld {
    public sealed class PathManager : MonoBehaviour {
        public LayerMask pathObstaclesLayerMask;

        public Graph graph;
        public float closeEnoughDistance = 1;
        public List<PathPlanner> searchRequests;

        public Graph Graph {
            get => graph;

            set => graph = value;
        }

        public List<PathPlanner> SearchRequests {
            get => searchRequests;

            private set => searchRequests = value;
        }

        public LayerMask PathObstaclesLayerMask {
            get => pathObstaclesLayerMask;

            set => pathObstaclesLayerMask = value;
        }

        public float CloseEnoughDistance {
            get => closeEnoughDistance;

            set => closeEnoughDistance = value;
        }

        public int SearchesPerCycle { get; set; }

        public void Awake(){
            graph = GameObject.Find("Game").GetComponent<Graph>();
            pathObstaclesLayerMask = 1 << LayerMask.NameToLayer("Walls");
            SearchRequests = new List<PathPlanner>();
            SearchesPerCycle = Parameters.Instance.MaximumSearchCyclesPerUpdateStep;
            graph.IsLocked = true;
            LeastCostPathTable.Create(graph);
        }

        public void Update(){
            var cyclesRemaining = SearchesPerCycle;
            var currentSearchRequestIndex = 0;

            while (cyclesRemaining > 0 && SearchRequests.Count > 0){
                var searchResult = SearchRequests[currentSearchRequestIndex].CycleOnce();

                if (searchResult != SearchResults.Running){
                    SearchRequests.RemoveAt(currentSearchRequestIndex);
                }
                else{
                    currentSearchRequestIndex++;
                }

                if (currentSearchRequestIndex >= SearchRequests.Count){
                    currentSearchRequestIndex = 0;
                }

                cyclesRemaining--;
            }
        }

        public void AddPathPlanner(PathPlanner pathPlanner){
            // make sure the bot does not already have a current search
            if (pathPlanner != null && !SearchRequests.Contains(pathPlanner))
                // add to the list
            {
                SearchRequests.Add(pathPlanner);
            }
        }

        public void RemovePathPlanner(PathPlanner pathPlanner){
            if (pathPlanner != null && SearchRequests != null){
                SearchRequests.Remove(pathPlanner);
            }
        }
    }
}