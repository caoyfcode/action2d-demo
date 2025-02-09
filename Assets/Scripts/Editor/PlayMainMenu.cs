using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class PlayMainMenu
{
    [MenuItem("Tools/PlayMainMenu", false, 1)]
    static void StartMainScene()
    {
        if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Main"))
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
 
    [MenuItem("Tools/PlayMainMenu", true, 1)] 
    static bool ValidStartMainScene()
    {
        return !Application.isPlaying;
    }
}
