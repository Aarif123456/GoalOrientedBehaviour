using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Edge))]
public class EdgeEditor : Editor
{    
    private Edge edge;
    
    public override void OnInspectorGUI()
    {
        edge = target as Edge;
        
        if (edge != null)
        {
            if (edge.Graph != null &&
                edge.Graph.IsLocked)
            {
                GUILayout.Label("The edge is locked because its graph is.");
                GUILayout.Label("locked. Unlock the graph to edit.");
            }
            else if (edge.EdgeCollection != null &&
                edge.EdgeCollection.IsLocked)
            {
                GUILayout.Label("The edge is locked because its collection");
                GUILayout.Label("is locked. Unlock the collection to edit.");
            }
            else if (edge.IsLocked)
            {
            GUILayout.Label("The edge is locked. Unlock to edit.");
                edge.locked = EditorGUILayout.Toggle("\tLocked", edge.locked);
            }
            else
            {
                DrawDefaultInspector();
            }
            
            if (!edge.IsLocked &&
                edge.GetComponent<Renderer>().sharedMaterial.color != edge.color)
            {
                edge.GetComponent<Renderer>().sharedMaterial.color = edge.color;
            }
            
            if (edge.Graph != null &&
                GUILayout.Button("Edit graph"))
            {
                Selection.activeGameObject = edge.Graph.gameObject;
            }
            
            if (edge.Graph != null &&
                edge.Graph.edgeCollectionObject != null &&
                GUILayout.Button("Edit edge collection"))
            {
                Selection.activeGameObject = edge.Graph.edgeCollectionObject;
            }
        }
    }
}