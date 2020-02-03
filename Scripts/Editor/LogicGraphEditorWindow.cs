﻿using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GeoTetra.GTLogicGraph
{
    public class LogicGraphEditorWindow : EditorWindow
    {
        private LogicGraphEditorObject _logicGraphEditorObject;

        private LogicGraphEditorView _graphEditorView;
        private string _selectedGuid;

        private LogicGraphEditorView LogicGraphEditorView
        {
            get { return _graphEditorView; }
            set
            {
                if (_graphEditorView != null)
                {
                    _graphEditorView.RemoveFromHierarchy();
                }

                _graphEditorView = value;

                if (_graphEditorView != null)
                {
                    _graphEditorView.saveRequested += UpdateAsset;
                    _graphEditorView.showInProjectRequested += PingAsset;
                    this.rootVisualElement.Add(_graphEditorView);
                }
            }
        }

        public string SelectedGuid
        {
            get { return _selectedGuid; }
        }

        public void Initialize(string guid)
        {
            try
            {
                _selectedGuid = guid;
                var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
                var path = AssetDatabase.GetAssetPath(asset);
                var textGraph = File.ReadAllText(path, Encoding.UTF8);

                _logicGraphEditorObject = CreateInstance<LogicGraphEditorObject>();
                LogicGraphData logicGraphData = JsonUtility.FromJson<LogicGraphData>(textGraph);
                _logicGraphEditorObject.Initialize(logicGraphData);
                LogicGraphEditorView = new LogicGraphEditorView(this, _logicGraphEditorObject)
                {
                    viewDataKey = _logicGraphEditorObject.GetInstanceID().ToString()
                };
                LogicGraphEditorView.RegisterCallback<GeometryChangedEvent>(OnPostLayout);

                this.name = path;
                titleContent = new GUIContent("Sensor Graph");
                
                Repaint();
            }
            catch (Exception)
            {
                _graphEditorView = null;
                _logicGraphEditorObject = null;
                throw;
            }
        }

        private void OnDisable()
        {
            LogicGraphEditorView = null;
        }

        private void OnDestroy()
        {
            LogicGraphEditorView = null;
        }

        void Update()
        {
            LogicGraphEditorView.HandleGraphChanges();
        }

        public void PingAsset()
        {
            if (SelectedGuid != null)
            {
                var path = AssetDatabase.GUIDToAssetPath(SelectedGuid);
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                EditorGUIUtility.PingObject(asset);
            }
        }

        public void UpdateAsset()
        {
            if (SelectedGuid != null && _logicGraphEditorObject != null)
            {
                var path = AssetDatabase.GUIDToAssetPath(SelectedGuid);
                if (string.IsNullOrEmpty(path) && _logicGraphEditorObject.LogicGraphData != null)
                    return;

                var shaderImporter = AssetImporter.GetAtPath(path) as LogicGraphImporter;
                if (shaderImporter == null)
                    return;
                
                File.WriteAllText(path, EditorJsonUtility.ToJson(_logicGraphEditorObject.LogicGraphData, true));
                shaderImporter.SaveAndReimport();
                AssetDatabase.ImportAsset(path);
            }
        }

        void OnPostLayout(GeometryChangedEvent evt)
        {            
            LogicGraphEditorView.UnregisterCallback<GeometryChangedEvent>(OnPostLayout);
            LogicGraphEditorView.LogicGraphView.FrameAll();
        }
    }
}