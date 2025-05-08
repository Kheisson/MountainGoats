using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using Managers;
using Services;
using Storage;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.Scripts
{
    public class NotebookEditorWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private readonly List<GarbageItemData> _garbageItems = new();
        private string _searchFilter = "";
        private IDataStorageService _dataStorage;
        private GameData _gameData;

        [MenuItem(ProjectConstants.Global.PROJECT_NAME + "/Notebook Editor")]
        public static void ShowWindow()
        {
            GetWindow<NotebookEditorWindow>("Notebook Editor");
        }

        private void OnEnable()
        {
            if (SceneManager.GetActiveScene().name == ProjectConstants.Scenes.GAME)
            {
                RefreshItems();
                LoadGameData();
            }
            else
            {
                EditorGUILayout.HelpBox($"Notebook Editor is only available in the {ProjectConstants.Scenes.GAME} scene.", MessageType.Warning);
            }
        }

        private void LoadGameData()
        {
            _dataStorage = ServiceLocator.Instance?.GetService<IDataStorageService>();
            
            if (_dataStorage != null)
            {
                _gameData = _dataStorage.GetGameData();
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Notebook Editor", EditorStyles.boldLabel);

            DrawSearchBar();

            if (_gameData == null)
            {
                EditorGUILayout.HelpBox("GameData not found. Make sure the game is running or the DataStorageService is available.", MessageType.Warning);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            foreach (var item in _garbageItems.Where(i => 
                         string.IsNullOrEmpty(_searchFilter) || 
                         i.ItemName.ToLower().Contains(_searchFilter.ToLower())))
            {
                DrawItemCard(item);
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal();
            _searchFilter = EditorGUILayout.TextField("Search", _searchFilter);
            
            if (GUILayout.Button("Refresh", GUILayout.Width(80)))
            {
                RefreshItems();
                LoadGameData();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawItemCard(GarbageItemData item)
        {
            EditorGUILayout.BeginVertical("Box");
            
            EditorGUILayout.BeginHorizontal();
            
            if (item.ItemSprite != null)
            {
                var spriteStyle = new GUIStyle
                {
                    normal =
                    {
                        background = AssetPreview.GetAssetPreview(item.ItemSprite),
                    },
                };

                var spriteRect = GUILayoutUtility.GetRect(50, 50, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                GUI.Box(spriteRect, GUIContent.none, spriteStyle);
            }
            
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.LabelField(item.ItemName, EditorStyles.boldLabel);
            
            var isUnlocked = _gameData.unlockedItems.Contains(item.name);
            var newUnlockState = EditorGUILayout.Toggle("Unlocked", isUnlocked);
            
            if (newUnlockState != isUnlocked)
            {
                if (newUnlockState)
                {
                    _gameData.unlockedItems.Add(item.name);
                }
                else
                {
                    _gameData.unlockedItems.Remove(item.name);
                }
                
                SaveGameData();
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        private void RefreshItems()
        {
            _garbageItems.Clear();
            var guids = AssetDatabase.FindAssets("t:GarbageItemData");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var item = AssetDatabase.LoadAssetAtPath<GarbageItemData>(path);
                
                if (item != null)
                {
                    _garbageItems.Add(item);
                }
            }
        }

        private void SaveGameData()
        {
            if (_dataStorage != null)
            {
                _dataStorage.ModifyGameDataSync(gameData =>
                {
                    gameData.unlockedItems = _gameData.unlockedItems;
                    return true;
                });

                var notebookManager = ServiceLocator.Instance.GetService<INotebookManager>();
                
                if (notebookManager != null)
                {
                    notebookManager.UpdateNotebook();
                }
            }
        }
    }
} 