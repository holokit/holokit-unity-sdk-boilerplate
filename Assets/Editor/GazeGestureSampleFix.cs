#if UNITY_IOS
using HoloKit;
using HoloKit.iOS;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// The Gaze Gesture Interaction sample shipped without its interactors, so gaze + pinch never selected
// anything. This adds them to "HoloKit XR Origin", matching the Gaze Interaction sample.
public static class GazeGestureSampleFix
{
    const string Scene = "Assets/Samples/GazeGestureInteraction/Scenes/GazeGestureInteraction.unity";

    [MenuItem("HoloKit/Samples/Fix Gaze Gesture Interaction Scene")]
    public static void Apply()
    {
        var scene = EditorSceneManager.OpenScene(Scene, OpenSceneMode.Single);
        GameObject origin = GameObject.Find("HoloKit XR Origin");
        if (origin == null) throw new System.Exception($"{Scene} has no HoloKit XR Origin");
        if (origin.GetComponent<GazeRaycastInteractor>() == null) origin.AddComponent<GazeRaycastInteractor>();
        if (origin.GetComponent<GazeGestureInteractor>() == null) origin.AddComponent<GazeGestureInteractor>();
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"[GazeGestureSampleFix] interactors present on {origin.name}");
    }
}
#endif
