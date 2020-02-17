using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SensorFoundation.SensorGraph
{
	public class LogicGraphView : GraphView
	{
        public LogicGraphEditorView EditorView = null;
        public LogicGraphEditorObject LogicGraphEditorObject { get; private set; }
        public NodeEditor ContextNode = null; // Represents the selected node, used as a context for the detail subgraph

		
		public LogicGraphView(LogicGraphEditorView editorView)
		{
            EditorView = editorView;
            GraphLogicEditor.AddStyleSheetPath(this, "Styles/LogicGraphView");
		}
		
		public LogicGraphView(LogicGraphEditorView editorView, LogicGraphEditorObject logicGraphEditorObject) : this(editorView)
		{
			LogicGraphEditorObject = logicGraphEditorObject;
		}
		
		public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
		{
			var compatibleAnchors = new List<Port>();
			var startSlot = (startAnchor as PortView).PortDescription;
			if (startSlot == null)
				return compatibleAnchors;

			foreach (var candidateAnchor in ports.ToList())
			{
				var candidateSlot = (candidateAnchor as PortView).PortDescription;
				if (!startSlot.IsCompatibleWith(candidateSlot))
					continue;

				compatibleAnchors.Add(candidateAnchor);
			}
			return compatibleAnchors;
		}
	}
}