using System;
using System.Collections.Generic;
using Entities;
using GameWorld.Cameras;
using UnityEngine;

// Add to the component menu.
namespace GameWorld.GUI {
    [AddComponentMenu("Scripts/GUI/Console Controller")]
    public class ConsoleController : WindowManager {
        private const string _WINDOW_TITLE = "Console Controller";
        public Vector2 positionOffset = new Vector2(0, 50);

        public int minimumWindowWidth = 150;
        public int minimumColumnWidth = 120;

        private List<Agent> _agents;
        private Dictionary<string, SelectableCamera> _cameras;
        private string[] _camerasNames;
        private int _curAgent;
        private int _curCamera = -1;

        private MessageManager _messageManager;

        // The variable to control where the scroll-view 'looks' into its child elements.
        private Vector2 _scrollPosition;
        private Rect _windowRectangle;

        public void Awake(){
            _windowRectangle = new Rect(positionOffset.x, positionOffset.y, minimumWindowWidth, 0);
        }

        /********************************************************************************************************************/
        // If this behaviour is enabled, Start is called once
        // after all Awake calls and before all any Update calls.
        public override void Start(){
            base.Start(); // initializes the window id
            /* Get list of all Agents*/
            _agents = EntityManager.FindAll<Agent>();
            GetViews();
            _messageManager = MessageManager.Instance;
        }

        /********************************************************************************************************************/
        // If this behaviour is enabled, OnGUI is called for rendering and handling GUI events.
        // It might be called several times per frame (one call per event).
        public void OnGUI(){
            _windowRectangle =
                GUILayout.Window(
                    windowId,
                    _windowRectangle,
                    WindowFunction,
                    _WINDOW_TITLE,
                    GUILayout.MinWidth(minimumWindowWidth));
        }

        /********************************************************************************************************************/
        /* Initialization helpers */
        /* Get camera script to allows users to toggle view  */
        private void GetViews(){
            _cameras = new Dictionary<string, SelectableCamera>();
            var cameraObjects = GameObject.FindGameObjectsWithTag("MainCamera");
            var curCameraName = "";
            foreach (var cameraObject in cameraObjects){
                var attachedCameras = cameraObject.GetComponents<SelectableCamera>();
                foreach (var selectableCamera in attachedCameras){
                    _cameras.Add(selectableCamera.CameraName, selectableCamera);
                    /* Make sure we only have one active camera */
                    if (string.IsNullOrEmpty(curCameraName) && selectableCamera.IsActive())
                        curCameraName = selectableCamera.CameraName;

                    selectableCamera.Deactivate();
                }
            }

            /* Store camera names in array*/
            _camerasNames = new string[_cameras.Count];
            _cameras.Keys.CopyTo(_camerasNames, 0);

            /* Activate current camera and set the correct index*/
            if (string.IsNullOrEmpty(curCameraName)) return;

            _cameras[curCameraName].Activate();
            _curCamera = Array.IndexOf(_camerasNames, curCameraName);
        }

        /********************************************************************************************************************/
        /* Define the GUI of the console controller  */

        // It requires the id of the window it's currently making GUI for. 
        private void WindowFunction(int windowID){
            if (_agents.Count == 0) return;

            // Layout dummy label to keep the title showing
            // even when no items are selected.
            GUILayout.Label("", GUIStyle.none);

            /* Let users select which agent they want to view */
            AgentSelection();
            var agent = _agents[_curAgent];

            /* Show info about the current agent */
            /* TODO: allow user to toggle what they want to see */
            ShowAgentStats(agent);
            ViewSelection(agent);
            ShowThoughts(agent);
            ControlAgent(agent);
            // Make the windows be draggable.
            UnityEngine.GUI.DragWindow();
        }

        /* Create button that allow users to see sift through the characters */
        /*NOTE: if you do not have many agents a toolbar is more convenient  */
        private void AgentSelection(){
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Back", GUILayout.ExpandWidth(true))){
                _curAgent--;
                if (_curAgent < 0) _curAgent = (_curAgent + _agents.Count) % _agents.Count;
            }

            if (GUILayout.Button("Next", GUILayout.ExpandWidth(true))){
                _curAgent++;
                _curAgent %= _agents.Count;
            }

            GUILayout.EndHorizontal();
        }

        /* Show info about the agent */
        private void ShowAgentStats(Agent agent){
            GUILayout.BeginVertical(GUILayout.MinWidth(minimumColumnWidth));
            var style = new GUIStyle{normal ={textColor = agent.color}};
            GUILayout.Label("Name: " + agent.shortName, style);
            GUILayout.Label("Score: " + agent.Score);
            GUILayout.Label("Health: " + agent.Health);
            var agentPos = agent.Kinematic.Position;
            GUILayout.Label("Position: " + agentPos);
            GUILayout.EndVertical();
        }

        /* Little bit ugly cause Unity forces us to use arrays for the selection grid */
        private SelectableCamera GetCamera(int index){
            return _cameras[_camerasNames[index]];
        }

        private static void SetCurrentView(SelectableCamera oldCamera, SelectableCamera newCamera, Transform target){
            oldCamera.Deactivate();
            newCamera.Activate();
            /* If the camera needs a target then we pass it in */
            if (newCamera is TargetedCamera targetedCamera) targetedCamera.target = target;
        }

        /* Used to select view */
        private void ViewSelection(Component agent){
            if (_curCamera < 0) return;

            /* Set current camera*/
            GUILayout.BeginVertical();
            var oldCamera = GetCamera(_curCamera);
            _curCamera = GUILayout.SelectionGrid(_curCamera, _camerasNames, 2);
            var newCamera = GetCamera(_curCamera);
            SetCurrentView(oldCamera, newCamera, agent.gameObject.transform);
            GUILayout.EndVertical();
        }

        /* Show the agents thought process */
        private void ShowThoughts(Agent agent){
            var agentBrain = agent.Brain;
            var headMessage = agentBrain.GetHeadMessage();
            var agentMessages = _messageManager.GetMessages(agent);
            /* Get top message */
            var headStyle = new GUIStyle{normal ={textColor = headMessage.Color}};
            GUILayout.Label(headMessage.Text, headStyle);
            _scrollPosition = GUILayout.BeginScrollView(
                _scrollPosition,
                GUILayout.ExpandWidth(true),
                GUILayout.MinHeight(50)
            );
            foreach (var message in agentMessages){
                var style = new GUIStyle{normal ={textColor = message.Color}};
                GUILayout.Label(message.Text, style);
            }

            GUILayout.EndScrollView();
        }

        /* Allow user to set agent goal */
        private static void ControlAgent(Agent agent){
            const int maxButtonInRow = 2;
            var curButtons = 0;
            GUILayout.BeginVertical();
            agent.IsAiControlled = GUILayout.Toggle(agent.IsAiControlled, "Is AI controlled");
            if (!agent.IsAiControlled){
                GUILayout.BeginHorizontal();
                foreach (var evaluator in agent.Brain.evaluators){
                    if (GUILayout.Button(evaluator.GoalName, GUILayout.ExpandWidth(true))) evaluator.SetGoal(agent);
                    curButtons++;
                    if (curButtons < maxButtonInRow) continue;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    curButtons = 0;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}