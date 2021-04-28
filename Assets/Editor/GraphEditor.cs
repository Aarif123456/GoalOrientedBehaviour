using GameWorld.Navigation.Graph;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Graph))]
public class GraphEditor : Editor {
    private bool edgeFoldoutStatus;
    private Graph graph;
    private bool nodeFoldoutStatus;

    public override void OnInspectorGUI(){
        graph = target as Graph;
        if (ReferenceEquals(graph, null)) return;

        if (graph.IsLocked){
            GUILayout.Label("The graph is locked. Unlock to edit.");
            graph.locked = EditorGUILayout.Toggle("\tLocked", graph.locked);
        }
        else
            DrawDefaultInspector();

        if (!graph.IsLocked &&
            ReferenceEquals(graph.nodeCollectionObject, null) &&
            ReferenceEquals(graph.edgeCollectionObject, null) &&
            GUILayout.Button("Create new node and edge collections")){
            CreateNewNodeCollection();
            CreateNewEdgeCollection();
        }

        if (!graph.IsLocked &&
            ReferenceEquals(graph.nodeCollectionObject, null) &&
            !ReferenceEquals(graph.edgeCollectionObject, null) &&
            GUILayout.Button("Create a new node collection"))
            CreateNewNodeCollection();

        if (!graph.IsLocked &&
            !ReferenceEquals(graph.nodeCollectionObject, null) &&
            GUILayout.Button("Edit node collection"))
            Selection.activeGameObject = graph.nodeCollectionObject;

        if (!graph.IsLocked &&
            ReferenceEquals(graph.edgeCollection, null) &&
            !ReferenceEquals(graph.nodeCollectionObject, null) &&
            GUILayout.Button("Create a new edge collection"))
            CreateNewEdgeCollection();

        if (!graph.IsLocked &&
            !ReferenceEquals(graph.edgeCollectionObject, null) &&
            GUILayout.Button("Edit edge collection"))
            Selection.activeGameObject = graph.edgeCollectionObject;

        if (graph.IsVisible){
            if (!GUILayout.Button("Hide graph")) return;
            if (!ReferenceEquals(graph.nodeCollection, null)) graph.nodeCollection.IsVisible = false;

            if (!ReferenceEquals(graph.edgeCollection, null)) graph.edgeCollection.IsVisible = false;
        }
        else if (GUILayout.Button("Show graph")){
            if (!ReferenceEquals(graph.nodeCollection, null)) graph.nodeCollection.IsVisible = true;

            if (!ReferenceEquals(graph.edgeCollection, null)) graph.edgeCollection.IsVisible = true;
        }
    }

    private void CreateNewNodeCollection(){
        if (ReferenceEquals(graph, null)) return;
        graph.nodeCollectionObject = CreateNewCollection("Nodes", graph.transform);
        if (ReferenceEquals(graph.nodeCollectionObject, null)) return;
        graph.nodeCollection = graph.nodeCollectionObject.AddComponent<NodeCollection>();
        graph.nodeCollection.Graph = graph;
    }

    private void CreateNewEdgeCollection(){
        if (ReferenceEquals(graph, null)) return;

        graph.edgeCollectionObject = CreateNewCollection("Edges", graph.transform);

        if (ReferenceEquals(graph.edgeCollectionObject, null)) return;

        graph.edgeCollection = graph.edgeCollectionObject.AddComponent<EdgeCollection>();
        graph.edgeCollection.Graph = graph;
    }

    private static GameObject CreateNewCollection(string defaultName, Transform parent){
        var s = defaultName;
        var nameSuffix = 1;

        while (GameObject.Find(s) != null){
            s = defaultName + nameSuffix++;
        }

        var collectionObject = new GameObject(s){
            transform ={position = Vector3.zero}
        };

        collectionObject.transform.parent = parent;
        return collectionObject;
    }
}