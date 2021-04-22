using UnityEngine;

// Add to the component menu.
[AddComponentMenu("Scripts/GUI/Window Manager")]

// Behaviours that use windows should derive from this to be able to get unique window ids.
public class WindowManager : MonoBehaviour {
    // The next available window id.
    public static int nextWindowId;

    // The id for the behaviour's main (or only) window.
    public int windowId;

    public virtual void Start(){
        windowId = nextWindowId++;
    }
}