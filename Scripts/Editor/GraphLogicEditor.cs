using System;
using System.Collections;
using System.Collections.Generic;
using GeoTetra.GTLogicGraph;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

[CustomEditor(typeof(LogicGraphInstance))]
public class GraphLogicEditor : Editor
{
    private SerializedProperty _logicGraphObjectProperty;
    private SerializedProperty _inputsProperty;
    private SerializedProperty _outputsProperty;


	public static string UXMLResourceToPackage(string resourcePath)
    {
        return "Assets/Editor Default Resources/" + resourcePath + ".uxml"; //***
    }

    public static StyleSheet LoadStyleSheet(string text)
    {
        return AssetDatabase.LoadAssetAtPath<StyleSheet>(text);
    }

    public static VisualTreeAsset LoadUXML(string text)
    {
        return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(text);
    }

    public static Texture2D LoadImage(string text)
    {
        return AssetDatabase.LoadAssetAtPath<Texture2D>(text);
    }
	
	public static void AddStyleSheetPath(VisualElement visualElement, string path)
	{
		var sheet = LoadStyleSheet(path);
		if (sheet != null)
			visualElement.styleSheets.Add(sheet);
	}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        LogicGraphInstance logicGraphInstance = (LogicGraphInstance) target;

        _logicGraphObjectProperty = serializedObject.FindProperty("_logicGraphObject");
        _inputsProperty = serializedObject.FindProperty("_inputs");
        _outputsProperty = serializedObject.FindProperty("_outputs");

        EditorGUILayout.PropertyField(_logicGraphObjectProperty);
        if (GUILayout.Button("Refresh"))
        {
            logicGraphInstance.OnEnable();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Inputs");

        for (int i = 0; i < _inputsProperty.arraySize; ++i)
        {
            SerializedProperty displayName = _inputsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("DisplayName");       
            
            EditorGUI.indentLevel = 2;
            EditorGUILayout.LabelField(displayName.stringValue);

            if (logicGraphInstance.Inputs[i].InputType == typeof(float))
            {
                SerializedProperty floatValue =
                    _inputsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("FloatValueX");
                EditorGUILayout.PropertyField(floatValue);
            }
            else
            {
                SerializedProperty componentValue = _inputsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("ComponentValue");    
                EditorGUILayout.ObjectField(componentValue, typeof(Transform));
            }
            EditorGUILayout.Space();

        }
        
        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Outputs");
        
        for (int i = 0; i < _outputsProperty.arraySize; ++i)
        {
            SerializedProperty displayName = _outputsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("DisplayName");
            EditorGUI.indentLevel = 2;
            EditorGUILayout.LabelField(displayName.stringValue);
            
            if (logicGraphInstance.Outputs[i].OutputType == typeof(Single))
            {
                SerializedProperty eventProperty = _outputsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("_updatedFloat");
                EditorGUILayout.PropertyField(eventProperty);
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}