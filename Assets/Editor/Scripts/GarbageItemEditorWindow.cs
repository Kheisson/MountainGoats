using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Data;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts
{
    public class GarbageItemEditorWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private readonly List<GarbageItemData> _garbageItems = new();
        private string _searchFilter = "";
        private bool _showNewItemForm;
        private GarbageItemData _newItem;

        [MenuItem(ProjectConstants.Global.PROJECT_NAME + "/Garbage Item Manager")]
        public static void ShowWindow()
        {
            GetWindow<GarbageItemEditorWindow>("Garbage Item Manager");
        }

        private void OnEnable()
        {
            RefreshItems();
        }

        private void OnGUI()
        {
            GUILayout.Label("Garbage Item Manager", EditorStyles.boldLabel);

            DrawSearchBar();

            if (!_showNewItemForm && GUILayout.Button("Add New Item"))
            {
                _showNewItemForm = true;
                _newItem = CreateInstance<GarbageItemData>();
            }

            if (_showNewItemForm)
            {
                DrawNewItemForm();
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
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawNewItemForm()
        {
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Label("New Item", EditorStyles.boldLabel);

            _newItem.name = EditorGUILayout.TextField("Name", _newItem.name);
            var serializedObject = new SerializedObject(_newItem);
            var itemNameProperty = serializedObject.FindProperty("itemName");
            var spriteProperty = serializedObject.FindProperty("itemSprite");
            var prefabProperty = serializedObject.FindProperty("itemPrefab");
            var descriptionProperty = serializedObject.FindProperty("description");
            var minWeightProperty = serializedObject.FindProperty("minWeight");
            var maxWeightProperty = serializedObject.FindProperty("maxWeight");
            var baseValueProperty = serializedObject.FindProperty("baseValue");
            var weightMultiplierProperty = serializedObject.FindProperty("weightMultiplier");

            EditorGUILayout.PropertyField(itemNameProperty);
            EditorGUILayout.PropertyField(spriteProperty);
            EditorGUILayout.PropertyField(prefabProperty);
            EditorGUILayout.PropertyField(descriptionProperty);
            EditorGUILayout.PropertyField(minWeightProperty);
            EditorGUILayout.PropertyField(maxWeightProperty);
            EditorGUILayout.PropertyField(baseValueProperty);
            EditorGUILayout.PropertyField(weightMultiplierProperty);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Save"))
            {
                SaveNewItem();
            }
            
            if (GUILayout.Button("Cancel"))
            {
                _showNewItemForm = false;
                _newItem = null;
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
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
            EditorGUILayout.LabelField(item.Description, EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField($"Weight Range: {item.MinWeight} - {item.MaxWeight}");
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Edit"))
            {
                Selection.activeObject = item;
            }
            
            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("Delete Item",
                        $"Are you sure you want to delete {item.ItemName}?",
                        "Yes", "No"))
                {
                    DeleteItem(item);
                }
            }
            
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

        private void SaveNewItem()
        {
            if (string.IsNullOrEmpty(_newItem.name))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a name for the item.", "OK");
                return;
            }

            var path = "Assets/Resources/GarbageItems";
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var assetPath = $"{path}/{_newItem.name}.asset";
            AssetDatabase.CreateAsset(_newItem, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _showNewItemForm = false;
            _newItem = null;
            RefreshItems();
        }

        private void DeleteItem(GarbageItemData item)
        {
            var path = AssetDatabase.GetAssetPath(item);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RefreshItems();
        }
    }
} 