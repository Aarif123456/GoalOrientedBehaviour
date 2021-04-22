using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Graph))]
public class GraphEditor : Editor {
    private bool edgeFoldoutStatus;
    private Graph graph;
    private bool nodeFoldoutStatus;

    public override void OnInspectorGUI(){
        graph = target as Graph;

        if (graph != null){
            if (graph.IsLocked){
                GUILayout.Label("The graph is locked. Unlock to edit.");
                graph.locked = EditorGUILayout.Toggle("\tLocked", graph.locked);
            }
            else{
                DrawDefaultInspector();
            }

            if (!graph.IsLocked &&
                graph.nodeCollectionObject == null &&
                graph.edgeCollectionObject == null &&
                GUILayout.Button("Create new node and edge collections")){
                CreateNewNodeCollection();
                CreateNewEdgeCollection();
            }

            if (!graph.IsLocked &&
                graph.nodeCollectionObject == null &&
                graph.edgeCollectionObject != null &&
                GUILayout.Button("Create a new node collection")){
                CreateNewNodeCollection();
            }

            if (!graph.IsLocked &&
                graph.nodeCollectionObject != null &&
                GUILayout.Button("Edit node collection")){
                Selection.activeGameObject = graph.nodeCollectionObject;
            }

            if (!graph.IsLocked &&
                graph.edgeCollectionObject == null &&
                graph.nodeCollectionObject != null &&
                GUILayout.Button("Create a new edge collection")){
                CreateNewEdgeCollection();
            }

            if (!graph.IsLocked &&
                graph.edgeCollectionObject != null &&
                GUILayout.Button("Edit edge collection")){
                Selection.activeGameObject = graph.edgeCollectionObject;
            }

            if (graph.IsVisible){
                if (GUILayout.Button("Hide graph")){
                    if (graph.nodeCollection != null){
                        graph.nodeCollection.IsVisible = false;
                    }

                    if (graph.edgeCollection != null){
                        graph.edgeCollection.IsVisible = false;
                    }
                }
            }
            else if (GUILayout.Button("Show graph")){
                if (graph.nodeCollection != null){
                    graph.nodeCollection.IsVisible = true;
                }

                if (graph.edgeCollection != null){
                    graph.edgeCollection.IsVisible = true;
                }
            }
        }
    }

    private void CreateNewNodeCollection(){
        if (graph != null){
            graph.nodeCollectionObject = CreateNewCollection("Nodes", graph.transform);

            if (graph.nodeCollectionObject != null){
                graph.nodeCollection = graph.nodeCollectionObject.AddComponent<NodeCollection>();
                graph.nodeCollection.Graph = graph;
            }
        }
    }

    private void CreateNewEdgeCollection(){
        if (graph != null){
            graph.edgeCollectionObject = CreateNewCollection("Edges", graph.transform);

            if (graph.edgeCollectionObject != null){
                graph.edgeCollection = graph.edgeCollectionObject.AddComponent<EdgeCollection>();
                graph.edgeCollection.Graph = graph;
            }
        }
    }

    private GameObject CreateNewCollection(string defaultName, Transform parent){
        var name = defaultName;
        var nameSuffix = 1;

        while (GameObject.Find(name) != null){
            name = defaultName + nameSuffix++;
        }

        var collectionObject = new GameObject(name){
            transform ={position = Vector3.zero}
        };

        collectionObject.transform.parent = parent;
        return collectionObject;
    }
}