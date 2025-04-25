using Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Editor.Scripts
{
    public abstract class BootstrapSettings
    {
        private const string MENU_NAME = "MgGame/Always Start From Bootstrapper";
        private const string PREF_KEY = "AlwaysStartFromBootstrapper";
        private const string LAST_SCENE_SAVED_KEY = "LastSceneSaved";
        private const string BOOTSTRAPPER_SCENE_PATH = "Assets/Scenes/Bootstrapper.unity";
        
        [MenuItem(MENU_NAME)]
        private static void ToggleStartFromBootstrapper()
        {
            var currentValue = EditorPrefs.GetBool(PREF_KEY, false);
            var newValue = !currentValue;
        
            EditorPrefs.SetBool(PREF_KEY, newValue);
            Menu.SetChecked(MENU_NAME, newValue);
        
            MgLogger.Log($"Always start from bootstrapper: {newValue}");
        }

        [MenuItem(MENU_NAME, true)]
        private static bool ValidateToggleStartFromBootstrapper()
        {
            Menu.SetChecked(MENU_NAME, EditorPrefs.GetBool(PREF_KEY, false));
            return true;
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!EditorPrefs.GetBool(PREF_KEY, false)) return;

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    HandleEnterPlayMode();
                    break;
                
                case PlayModeStateChange.EnteredEditMode:
                    HandleExitPlayMode();
                    break;
            }
        }
        
        private static void HandleEnterPlayMode()
        {
            var currentScene = SceneManager.GetActiveScene().path;

            if (currentScene == BOOTSTRAPPER_SCENE_PATH) return;
            
            EditorPrefs.SetString(LAST_SCENE_SAVED_KEY, currentScene);
            
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(BOOTSTRAPPER_SCENE_PATH);
                
            EditorApplication.isPlaying = false;
            EditorApplication.delayCall += () => EditorApplication.isPlaying = true;
        }
        
        private static void HandleExitPlayMode()
        {
            var originalScenePath = EditorPrefs.GetString(LAST_SCENE_SAVED_KEY, string.Empty);
            
            if (!string.IsNullOrEmpty(originalScenePath) && originalScenePath != BOOTSTRAPPER_SCENE_PATH)
            {
                EditorApplication.delayCall += () =>
                {
                    if (System.IO.File.Exists(originalScenePath))
                    {
                        EditorSceneManager.OpenScene(originalScenePath);
                        MgLogger.Log($"Returned to scene: {originalScenePath}");
                    }
                    
                    EditorPrefs.DeleteKey(LAST_SCENE_SAVED_KEY);
                };
            }
        }
    }
}
