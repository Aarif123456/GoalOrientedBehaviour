using UnityEngine;

public sealed class Graph : MonoBehaviour {
    public bool locked;
    public float maximumDistanceBetweenNodes = 15 * Mathf.Sqrt(2); // TODO: this should be calculated from the graph
    public GameObject nodePrefab;
    public GameObject edgePrefab;
    public GameObject nodeCollectionObject;
    public GameObject edgeCollectionObject;
    public NodeCollection nodeCollection;
    public EdgeCollection edgeCollection;

    public bool IsLocked {
        get => locked;

        set => locked = value;
    }

    // TODO: this should be calculated from the graph
    public float MaximumDistanceBetweenNodes {
        get => maximumDistanceBetweenNodes;

        set => maximumDistanceBetweenNodes = value;
    }

    public bool IsVisible {
        get =>
            (nodeCollection == null || nodeCollection.IsVisible) &&
            (edgeCollection == null || edgeCollection.IsVisible);

        set {
            if (nodeCollection != null){
                nodeCollection.IsVisible = value;
            }

            if (edgeCollection != null){
                edgeCollection.IsVisible = value;
            }
        }
    }
}