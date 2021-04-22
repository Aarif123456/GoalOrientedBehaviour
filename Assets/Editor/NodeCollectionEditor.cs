using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeCollection))]
public class NodeCollectionEditor : Editor
{
    private NodeCollection nodeCollection;
    private bool nodeFoldoutStatus;
    
    public override void OnInspectorGUI()
    {
        nodeCollection = target as NodeCollection;

        if (ReferenceEquals(nodeCollection, null)) return;
        if (!ReferenceEquals(nodeCollection.Graph, null) &&
            nodeCollection.Graph.IsLocked)
        {
            GUILayout.Label("The node collection is locked because its");
            GUILayout.Label("graph is locked. Unlock the graph to edit.");
        }
        else if (nodeCollection.IsLocked)
        {
            GUILayout.Label("The node collection is locked. Unlock to edit.");
            nodeCollection.locked = EditorGUILayout.Toggle("\tLocked", nodeCollection.locked);
        }
        else
        {
            DrawDefaultInspector();
        }
        
        if (!nodeCollection.IsLocked)
        {
            nodeFoldoutStatus = EditorGUILayout.Foldout(nodeFoldoutStatus, "Nodes");
                        
            if (nodeFoldoutStatus)
            {    
                EditorGUILayout.LabelField("\t\tCount", nodeCollection.Nodes.Length.ToString());
                    
                for (var i = 0; i < nodeCollection.Nodes.Length; i++)
                {
                    nodeCollection.Nodes[i] = (Node) EditorGUILayout.ObjectField(
                        "\t\tNode " + i, 
                        nodeCollection.Nodes[i], 
                        typeof(Node), 
                        true);
                }
            }
        }

        if (!ReferenceEquals(nodeCollection.Graph, null) &&
            GUILayout.Button("Edit graph"))
        {
            Selection.activeGameObject = nodeCollection.Graph.gameObject;
        }
            
        if (!nodeCollection.IsLocked && 
            !ReferenceEquals(nodeCollection.Graph, null) &&
            !ReferenceEquals(nodeCollection.Graph.nodePrefab, null) &&
            GUILayout.Button("Add a node"))
        {
            Selection.activeGameObject = nodeCollection.AddNode(SceneView.lastActiveSceneView.camera);
        }
            
        if (!nodeCollection.IsLocked && 
            GUILayout.Button("Apply parameters to all nodes"))
        {
            nodeCollection.ApplyParametersToNodes();
        }
            
        if (!nodeCollection.IsLocked && 
            GUILayout.Button("Drop all nodes to surface"))
        {
            nodeCollection.DropToSurface();
        }
            
        if (!nodeCollection.IsLocked && 
            GUILayout.Button("Raycast connections for all nodes"))
        {
            nodeCollection.RaycastNodes();
        }
            
        if (nodeCollection.IsVisible)
        {
            if (GUILayout.Button("Hide nodes"))
            {
                nodeCollection.IsVisible = false;
            }
        }
        else if (GUILayout.Button("Show nodes"))
        {
            nodeCollection.IsVisible = true;
        }
    }
}