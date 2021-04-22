using UnityEditor;

[CustomEditor(typeof(PathManager))]
public class PathManagerEditor : Editor {
    private PathManager pathManager;

    public override void OnInspectorGUI(){
        pathManager = target as PathManager;

        if (!ReferenceEquals(pathManager, null)){
            DrawDefaultInspector();
        }
    }
}