using Controllers;
using Core;
using Managers;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts
{
    public class CurrencyDebugWindow : EditorWindow
    {
        private int _amountToAdd = 100;
        private int _amountToSubtract = 50;
        private CurrencyController _currencyController;

        [MenuItem(ProjectConstants.Global.PROJECT_NAME + "/Currency Debug Window")]
        public static void ShowWindow()
        {
            GetWindow<CurrencyDebugWindow>("Currency Debug");
        }

        private void OnEnable()
        {
            var uiManager = FindFirstObjectByType<UIManager>();
            
            if (uiManager != null)
            {
                _currencyController = uiManager.CurrencyController;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Currency Debug Window", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (_currencyController == null)
            {
                EditorGUILayout.HelpBox("Currency Controller not found. Make sure there's a UIManager in the scene.", MessageType.Warning);
                
                if (GUILayout.Button("Find UIManager"))
                {
                    var uiManager = FindFirstObjectByType<UIManager>();
                    
                    if (uiManager != null)
                    {
                        _currencyController = uiManager.CurrencyController;
                    }
                }
                
                return;
            }

            EditorGUILayout.LabelField("Current Balance", _currencyController.GetCurrency().ToString());
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Add Currency", EditorStyles.boldLabel);
            _amountToAdd = EditorGUILayout.IntField("Amount to Add", _amountToAdd);
            
            if (GUILayout.Button("Add Currency"))
            {
                _currencyController.AddCurrency(_amountToAdd);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Subtract Currency", EditorStyles.boldLabel);
            _amountToSubtract = EditorGUILayout.IntField("Amount to Subtract", _amountToSubtract);
            
            if (GUILayout.Button("Subtract Currency"))
            {
                _currencyController.SubtractCurrency(_amountToSubtract);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Add 1000"))
            {
                _currencyController.AddCurrency(1000);
            }
            if (GUILayout.Button("Subtract 1000"))
            {
                _currencyController.SubtractCurrency(1000);
            }
            if (GUILayout.Button("Set to 0"))
            {
                var current = _currencyController.GetCurrency();
                _currencyController.SubtractCurrency(current);
            }
        }
    }
} 