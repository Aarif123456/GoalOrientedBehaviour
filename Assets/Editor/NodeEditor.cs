using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor {
    private Node node;

    public override void OnInspectorGUI(){
        // This is to handle when we are moving a node and UnityEditor.MoveTool has set an undo target using the gameobject name which we change when moving!
        // Without this we get a "Stored Snapshot does not match same set of objects ..."

        //Undo.SetSnapshotTarget(Selection.transforms, "Move " + ((Selection.transforms.Length != 1) ? string.Empty : Selection.activeGameObject.name));
        // Undo.CreateSnapshot();

        //NOTE: Replace above obsolete methods. Also, need to check if this is still an issue. Following line may be unnecessary now.
        Undo.RecordObjects(Selection.transforms,
            "Move " + (Selection.transforms.Length != 1 ? string.Empty : Selection.activeGameObject.name));

        node = target as Node;

        if (ReferenceEquals(node, null)){
            return;
        }

        if (!ReferenceEquals(node.Graph, null) &&
            node.Graph.IsLocked){
            GUILayout.Label("The node is locked because its graph is");
            GUILayout.Label("locked. Unlock the graph to edit.");
        }
        else if (!ReferenceEquals(node.NodeCollection, null) &&
                 node.NodeCollection.IsLocked){
            GUILayout.Label("The node is locked because its collection");
            GUILayout.Label("is locked. Unlock the collection to edit.");
        }
        else if (node.IsLocked){
            GUILayout.Label("The node is locked. Unlock to edit.");
            node.locked = EditorGUILayout.Toggle("\tLocked", node.locked);
        }
        else{
            DrawDefaultInspector();
        }

        if (!node.IsLocked &&
            node.GetComponent<Renderer>().sharedMaterial.color != node.color){
            node.GetComponent<Renderer>().sharedMaterial.color = node.color;
        }

        if (!ReferenceEquals(node.Graph, null)){
            if (GUILayout.Button("Edit graph")){
                Selection.activeGameObject = node.Graph.gameObject;
            }

            if (!ReferenceEquals(node.Graph.nodeCollectionObject, null) &&
                GUILayout.Button("Edit node collection")){
                Selection.activeGameObject = node.Graph.nodeCollectionObject;
            }
        }


        if (node.IsLocked){
            return;
        }

        if (GUILayout.Button("Drop selected node to surface")){
            node.DropToSurface();
        }

        if (GUILayout.Button("Connect a new node with selected node")){
            Selection.activeGameObject = node.AddConnectedNode(false, SceneView.lastActiveSceneView.camera);
        }

        if (GUILayout.Button("Connect from selected node to a new node")){
            Selection.activeGameObject = node.AddConnectedNode(true, SceneView.lastActiveSceneView.camera);
        }

        if (GUILayout.Button("Connect selected nodes")){
            Node.ConnectNodes(Array.ConvertAll(Selection.GetFiltered(typeof(Node), SelectionMode.TopLevel),
                element => (Node) element));
        }

        if (GUILayout.Button("Remove all connections of selected node")){
            node.RemoveAllConnections();
        }

        if (GUILayout.Button("Disconnect selected nodes from each other")){
            Node.DisconnectNodes(Array.ConvertAll(Selection.GetFiltered(typeof(Node), SelectionMode.TopLevel),
                element => (Node) element));
        }

        if (GUILayout.Button("Cycle connection between two selected nodes")){
            var filtered = Selection.GetFiltered(typeof(Node), SelectionMode.TopLevel);

            if (filtered.Length == 2){
                var fromNode = (Node) filtered[0];
                var toNode = (Node) filtered[1];
                Node.CycleConnection(fromNode, toNode);
            }
        }

        if (!GUILayout.Button("Raycast connections for each selected node")){
            return;
        }

        node.RemoveAllConnections();
        node.RaycastNeighbours(node.NodeCollection.Nodes, false, true);
    }
}