using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EdgeCollection))]
public class EdgeCollectionEditor : Editor {
    private EdgeCollection edgeCollection;
    private bool edgeFoldoutStatus;

    public override void OnInspectorGUI(){
        edgeCollection = target as EdgeCollection;

        if (ReferenceEquals(edgeCollection, null)){
            return;
        }

        if (!ReferenceEquals(edgeCollection.Graph, null) &&
            edgeCollection.Graph.IsLocked){
            GUILayout.Label("The edge collection is locked because its");
            GUILayout.Label("graph is locked. Unlock the graph to edit.");
        }
        else if (edgeCollection.IsLocked){
            GUILayout.Label("The edge collection is locked. Unlock to edit.");
            edgeCollection.locked = EditorGUILayout.Toggle("\tLocked", edgeCollection.locked);
        }
        else{
            DrawDefaultInspector();
        }

        if (!edgeCollection.IsLocked){
            edgeFoldoutStatus = EditorGUILayout.Foldout(edgeFoldoutStatus, "Edges");

            if (edgeFoldoutStatus){
                EditorGUILayout.LabelField("\t\tCount", edgeCollection.Edges.Length.ToString());

                for (var i = 0; i < edgeCollection.Edges.Length; i++){
                    edgeCollection.Edges[i] = (Edge) EditorGUILayout.ObjectField(
                        "\t\tNode " + i,
                        edgeCollection.Edges[i],
                        typeof(Edge),
                        true);
                }
            }
        }

        if (!ReferenceEquals(edgeCollection.Graph, null) &&
            GUILayout.Button("Edit graph")){
            Selection.activeGameObject = edgeCollection.Graph.gameObject;
        }

        if (!edgeCollection.IsLocked &&
            GUILayout.Button("Apply parameters to all edges")){
            edgeCollection.ApplyParametersToEdges();
        }

        if (edgeCollection.IsVisible){
            if (GUILayout.Button("Hide edges")){
                edgeCollection.IsVisible = false;
            }
        }
        else if (GUILayout.Button("Show edges")){
            edgeCollection.IsVisible = true;
        }
    }
}