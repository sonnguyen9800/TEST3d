using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SceneHelper
{
    private static string _scenePath = "Assets/_Test/SimpleMovement.unity";
    [MenuItem("Test/Open Test Scene %&o")] // Shortcut: Ctrl + Alt + O (Windows) / Cmd + Option + O (Mac)
    public static void OpenScene()
    {
        if (!string.IsNullOrEmpty(_scenePath))
        {
            EditorSceneManager.OpenScene(_scenePath);
            Debug.Log($"Opened scene: {_scenePath}");
        }
        else
        {
            Debug.LogWarning("No last opened scene found.");
        }
    }
}
