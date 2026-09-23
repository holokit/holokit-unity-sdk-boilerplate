#if UNITY_IOS
using System.IO;
using HoloKit;
using HoloKit.Editor;
using HoloKit.iOS;
using HoloKit.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR.ARFoundation;

// Gives every interactive sample the standard HoloKit Default UI Canvas prefab (render mode,
// record and 4K buttons) and a HoloKitVideoRecorder on its AR camera.
//   unity run . -- -buildTarget iOS -executeMethod SampleUICanvasUpgrade.Apply
public static class SampleUICanvasUpgrade
{
    static readonly string[] Scenes =
    {
        "Assets/Samples/StereoscopicRendering/Scenes/StereoscopicRendering.unity",
        "Assets/Samples/HandTracking/Scenes/HandTracking.unity",
        "Assets/Samples/HandGestureRecognition/Scenes/HandGestureRecognition.unity",
        "Assets/Samples/GazeInteraction/Scenes/GazeInteraction.unity",
        "Assets/Samples/GazeGestureInteraction/Scenes/GazeGestureInteraction.unity",
        "Assets/Samples/GlowingOrbs/Scenes/GlowingOrbs.unity",
        "Assets/Samples/HeadInteraction/Scenes/HeadInteraction.unity",
        "Assets/Samples/ResetWorldOrigin/Scenes/ResetWorldOrigin.unity",
        "Assets/Samples/ARBackgroundVideoEnhancement/Scenes/ARBackgroundVideoEnhancement.unity",
        "Assets/Samples/CustomARBackground/Scenes/CustomARBackground.unity",
    };

    [MenuItem("HoloKit/Samples/Apply Standard UI Canvas")]
    public static void Apply()
    {
        HoloKitCreateUtils.RebuildDefaultUICanvasPrefab();
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(HoloKitCreateUtils.DefaultUICanvasPrefabPath);
        if (prefab == null) throw new System.Exception("Default UI canvas prefab was not created");
        foreach (string path in Scenes)
        {
            if (!File.Exists(path)) continue;
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            foreach (var old in Object.FindObjectsByType<HoloKitDefaultUICanvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                Object.DestroyImmediate(old.gameObject);
            PrefabUtility.InstantiatePrefab(prefab, scene);
            if (Object.FindFirstObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            var arCamera = Object.FindFirstObjectByType<ARCameraManager>();
            bool addedRecorder = false;
            if (arCamera != null && arCamera.GetComponent<HoloKitVideoRecorder>() == null)
            {
                arCamera.gameObject.AddComponent<HoloKitVideoRecorder>();
                addedRecorder = true;
            }
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[SampleUICanvasUpgrade] {path}: standard canvas{(addedRecorder ? ", recorder added" : "")}");
        }
        // The recording samples are generated from Stereoscopic Rendering.
        RecordingAudioSampleScenes.Create();
    }
}
#endif
