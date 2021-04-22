using System;
using System.Collections.Generic;
using GameBrains.AI;
using GameBrains.Cameras;
using UnityEngine;

/* Used for entity manager and agent */
/* Used to select view*/

// Add to the component menu.
[AddComponentMenu("Scripts/GUI/Console Controller")]
public class ConsoleController : WindowManager {
    public Vector2 positionOffset = new Vector2(0, 50);
    public int minimumWindowWidth = 150;
    public int minimumColumnWidth = 120;

    private readonly string windowTitle = "Console Controller";

    private List<Agent> agents;
    private Dictionary<string, SelectableCamera> cameras;
    private string[] camerasNames;
    private int curAgent;
    private int curCamera = -1;
    private int height;
    private int width;
    private Rect windowRectangle;

    private float x;
    private float y;

    /********************************************************************************************************************/
    // If this behaviour is enabled, Start is called once
    // after all Awake calls and before all any Update calls.
    public void Awake(){
    }

    public new void Start(){
        base.Start(); // initializes the window id
        /* Get list of all Agents*/
        agents = EntityManager.FindAll<Agent>();
        GetViews();
    }

    public void Update(){
    }


    /********************************************************************************************************************/
    // If this behaviour is enabled, OnGUI is called for rendering and handling GUI events.
    // It might be called several times per frame (one call per event).
    public void OnGUI(){
        if (width != Screen.width || height != Screen.height){
            x = Screen.width * 0.02f + positionOffset.x;
            y = Screen.height * 0.02f + positionOffset.y;
            width = Screen.width;
            height = Screen.height;
            windowRectangle = new Rect(x, y, minimumWindowWidth, 0); // GUILayout will determine height
        }

        windowRectangle =
            GUILayout.Window(
                windowId,
                windowRectangle,
                WindowFunction,
                windowTitle,
                GUILayout.MinWidth(minimumWindowWidth));
    }

    /********************************************************************************************************************/
    /* Initialization helpers */
    /* Get camera script to allows users to toggle view  */
    private void GetViews(){
        cameras = new Dictionary<string, SelectableCamera>();
        var cameraObjects = GameObject.FindGameObjectsWithTag("MainCamera");
        var curCameraName = "";
        foreach (var cameraObject in cameraObjects){
            var attachedCameras = cameraObject.GetComponents<SelectableCamera>();
            foreach (var camera in attachedCameras){
                cameras.Add(camera.CameraName, camera);
                /* Make sure we only have one active camera */
                if (string.IsNullOrEmpty(curCameraName) && camera.IsActive()){
                    curCameraName = camera.CameraName;
                }

                camera.Deactivate();
            }
        }

        /* Store camera names in array*/
        camerasNames = new string[cameras.Count];
        cameras.Keys.CopyTo(camerasNames, 0);

        /* Activate current camera and set the correct index*/
        if (!string.IsNullOrEmpty(curCameraName)){
            cameras[curCameraName].Activate();
            curCamera = Array.IndexOf(camerasNames, curCameraName);
        }
    }

    /********************************************************************************************************************/
    /* Define the GUI of the console controller  */

    // It requires the id of the window it's currently making GUI for. 
    private void WindowFunction(int windowID){
        if (agents.Count == 0){
            return;
        }

        // Layout dummy label to keep the title showing
        // even when no items are selected.
        GUILayout.Label("", GUIStyle.none);

        /* Let users select which agent they want to view */
        AgentSelection();
        var agent = agents[curAgent];

        /* Show info about the current agent */
        /* TODO: allow user to toggle what they want to see */
        ShowAgentStats(agent);
        ViewSelection(agent);


        // Make the windows be draggable.
        GUI.DragWindow();
    }

    /* Create button that allow users to see sift through the characters */
    /*NOTE: if you do not have many agents a toolbar is more convenient  */
    private void AgentSelection(){
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Back", GUILayout.ExpandWidth(true))){
            curAgent--;
            if (curAgent < 0){
                curAgent = (curAgent + agents.Count) % agents.Count;
            }
        }

        if (GUILayout.Button("Next", GUILayout.ExpandWidth(true))){
            curAgent++;
            curAgent %= agents.Count;
        }

        GUILayout.EndHorizontal();
    }

    /* Show info about the agent */
    private void ShowAgentStats(Agent agent){
        GUILayout.BeginVertical(GUILayout.MinWidth(minimumColumnWidth));
        var style = new GUIStyle();
        style.normal.textColor = agent.color;
        GUILayout.Label("Name: " + agent.shortName, style);
        GUILayout.Label("Score: " + agent.Score);
        GUILayout.Label("Health: " + agent.Health);
        var agentPos = agent.gameObject.transform.position;
        GUILayout.Label("Position: " + agentPos);
        GUILayout.EndVertical();
    }

    /* Little bit ugly cause Unity forces us to use arrays for the selection grid */
    private SelectableCamera GetCamera(int index){
        return cameras[camerasNames[index]];
    }

    private void SetCurrentView(SelectableCamera oldCamera, SelectableCamera newCamera, Transform target){
        oldCamera.Deactivate();
        newCamera.Activate();
        /* If the camera needs a target then we pass it in */
        if (newCamera is TargetedCamera targetedCamera){
            targetedCamera.target = target;
        }
    }

    private void ViewSelection(Agent agent){
        if (curCamera < 0){
            return;
        }

        /* Set current camera*/
        GUILayout.BeginVertical();
        var oldCamera = GetCamera(curCamera);
        curCamera = GUILayout.SelectionGrid(curCamera, camerasNames, 2);
        var newCamera = GetCamera(curCamera);
        SetCurrentView(oldCamera, newCamera, agent.gameObject.transform);
        GUILayout.EndVertical();
    }
}