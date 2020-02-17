using System;
using System.Collections;
using System.Collections.Generic;
using GeoTetra.GTLogicGraph;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

//*** Add for Inspector GUI [CustomEditor(typeof(LogicGraphInstance))]
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
        return Resources.Load<StyleSheet>(text);
    }

    public static VisualTreeAsset LoadUXML(string text)
    {
        return Resources.Load<VisualTreeAsset>(text);
    }

    public static Texture2D LoadImage(string text)
    {
        return Resources.Load<Texture2D>(text);
    }
	
	public static void AddStyleSheetPath(VisualElement visualElement, string path)
	{
		var sheet = LoadStyleSheet(path);
		if (sheet != null)
			visualElement.styleSheets.Add(sheet);
        else
            Debug.LogError("Cannot add style sheet " + path);
    }

    public override void OnInspectorGUI()
    {
        //*** TODO
        serializedObject.Update();        
        //LogicGraphInstance logicGraphInstance = (LogicGraphInstance) target;        
    }
}