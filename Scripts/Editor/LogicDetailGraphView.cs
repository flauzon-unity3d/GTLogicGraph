using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
	public class LogicDetailGraphView : GraphView
	{
        public LogicGraphEditorView EditorView = null;
        public LogicGraphEditorObject LogicGraphEditorObject { get; private set; }
        public LogicGraphView ParentGraphView = null;
		
		public LogicDetailGraphView(LogicGraphEditorView editorView, LogicGraphView parentView)
		{
            EditorView = editorView;
            ParentGraphView = parentView;
            GraphLogicEditor.AddStyleSheetPath(this, "Styles/LogicDetailGraphView");
		}
		
		public LogicDetailGraphView(LogicGraphEditorView editorView, LogicGraphEditorObject logicGraphEditorObject, LogicGraphView parentView) : this(editorView, parentView)
		{
			LogicGraphEditorObject = logicGraphEditorObject;
		}
		
		public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
		{
			var compatibleAnchors = new List<Port>();
			var startSlot = (startAnchor as PortDetailView).PortDescription;
			if (startSlot == null)
				return compatibleAnchors;

			foreach (var candidateAnchor in ports.ToList())
			{
				var candidateSlot = (candidateAnchor as PortDetailView).PortDescription;
				if (!startSlot.IsCompatibleWith(candidateSlot))
					continue;

				compatibleAnchors.Add(candidateAnchor);
			}
			return compatibleAnchors;
		}
	}
}