using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
	[CustomEditor(typeof(LogicGraphImporter))]
	public class LogicGraphImporterEditor : ScriptedImporterEditor
	{
		public override void OnInspectorGUI()
		{
//			if (GUILayout.Button("Open Generic Graph Editor"))
//			{
//				AssetImporter importer = target as AssetImporter;
//				ShowGraphEditWindow(importer.assetPath);
//			}

			DrawDefaultInspector();
			base.ApplyRevertGUI();
		}

        
        public static LogicGraphObject CreateNewAsset(string path)
        {
            return CreateNew<LogicGraphObject>(path);  
        }

        public static T CreateNew<T>(string path) where T : LogicGraphObject
        {
            var templateData = Resources.Load("Templates/Template.SensorGraph");
            File.WriteAllText(path, EditorJsonUtility.ToJson(templateData, true));

            AssetDatabase.ImportAsset(path);

            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        
        internal class DoCreateSensorGraph : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var obj = CreateNewAsset(pathName);

                AssetDatabase.ImportAsset(pathName);
                var guid = AssetDatabase.AssetPathToGUID(pathName);
                ShowGraphEditWindow(pathName);

                ProjectWindowUtil.ShowCreatedAsset(obj);
            }
        }

        [MenuItem("Assets/Create/Sensor/Sensor Graph", false, 306)]
        public static void CreateVisualEffectAsset()
        {
            var action = ScriptableObject.CreateInstance<DoCreateSensorGraph>();            
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, "New_Sensor.SensorGraph", null, null);
        }

		private static bool ShowGraphEditWindow(string path)
		{
			var guid = AssetDatabase.AssetPathToGUID(path);
			var extension = Path.GetExtension(path);
			if (extension != ".SensorGraph" && extension != ".SensorGraph")
				return false;

			var foundWindow = false;
			foreach (var w in Resources.FindObjectsOfTypeAll<LogicGraphEditorWindow>())
			{
				if (w.SelectedGuid == guid)
				{
					foundWindow = true;
					w.Focus();
				}
			}

			if (!foundWindow)
			{
				var window = CreateInstance<LogicGraphEditorWindow>();
				window.Show();
				window.Initialize(guid);
			}

			return true;
		}

		[OnOpenAsset(0)]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			var path = AssetDatabase.GetAssetPath(instanceID);
			return ShowGraphEditWindow(path);
		}
	}
}