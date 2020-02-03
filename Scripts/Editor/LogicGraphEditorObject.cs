﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
	public class LogicGraphEditorObject : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField]
		private LogicGraphData _logicGraphData;
		
		public event Action Deserialized;
		
		public LogicGraphData LogicGraphData
		{
			get { return _logicGraphData; }
		}

		public void Initialize(LogicGraphData logicGraphData)
		{
			_logicGraphData = logicGraphData;
			if (_logicGraphData == null)
			{
				_logicGraphData = new LogicGraphData();
			}
		}
		
		public void RegisterCompleteObjectUndo(string name)
		{
#if UNITY_EDITOR
			UnityEditor.Undo.RegisterCompleteObjectUndo(this, name);
#endif
		}
		
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (Deserialized != null) Deserialized();
		}
	}
}
