#if UNITY_IOS
using System.IO;
using HoloKit.iOS;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Regenerates the two recording-audio sample scenes from the Stereoscopic Rendering sample.
//   unity run-method style: -executeMethod RecordingAudioSampleScenes.Create
public static class RecordingAudioSampleScenes
{
    const string Source = "Assets/Samples/StereoscopicRendering/Scenes/StereoscopicRendering.unity";
    const string Folder = "Assets/Samples/RecordingAudio/Scenes";

    [MenuItem("HoloKit/Samples/Create Recording Audio Scenes")]
    public static void Create()
    {
        Directory.CreateDirectory(Folder);
        Build("RecordingEnvironment", RecordingAudioMode.Environment, MicrophoneProcessing.Environment, false);
        Build("RecordingGameAndMicrophone", RecordingAudioMode.GameAndMicrophone, MicrophoneProcessing.EchoCancellation, true);
        AssetDatabase.SaveAssets();
    }

    static void Build(string name, RecordingAudioMode mode, MicrophoneProcessing processing, bool gameTone)
    {
        var scene = EditorSceneManager.OpenScene(Source, OpenSceneMode.Single);
        var recorder = Object.FindFirstObjectByType<HoloKitVideoRecorder>();
        if (recorder == null) throw new System.Exception($"{Source} has no HoloKitVideoRecorder");
        var microphone = new GameObject("Microphone Input").AddComponent<HoloKitMicrophoneCapture>();

        var recorderSettings = new SerializedObject(recorder);
        recorderSettings.FindProperty("_audioMode").enumValueIndex = (int)mode;
        recorderSettings.FindProperty("_microphoneInput").objectReferenceValue = microphone;
        recorderSettings.FindProperty("_includeMicrophone").boolValue = true;
        if (gameTone)
            recorderSettings.FindProperty("_gameAudioListener").objectReferenceValue = Object.FindFirstObjectByType<AudioListener>();
        recorderSettings.ApplyModifiedPropertiesWithoutUndo();

        var canvas = Object.FindFirstObjectByType<Canvas>();
        Text label = null;
        if (canvas != null)
        {
            var labelObject = new GameObject("Microphone Level", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(canvas.transform, false);
            var rect = (RectTransform)labelObject.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -120);
            rect.sizeDelta = new Vector2(900, 80);
            label = labelObject.GetComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 40;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
        }

        var demo = new GameObject("Recording Audio Demo").AddComponent<RecordingAudioDemo>();
        var demoSettings = new SerializedObject(demo);
        demoSettings.FindProperty("m_Microphone").objectReferenceValue = microphone;
        demoSettings.FindProperty("m_Processing").enumValueIndex = (int)processing;
        demoSettings.FindProperty("m_PlayGameTone").boolValue = gameTone;
        demoSettings.FindProperty("m_LevelText").objectReferenceValue = label;
        demoSettings.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.SaveScene(scene, $"{Folder}/{name}.unity");
        Debug.Log($"[RecordingAudioSampleScenes] wrote {Folder}/{name}.unity mode={mode} processing={processing}");
    }
}
#endif
