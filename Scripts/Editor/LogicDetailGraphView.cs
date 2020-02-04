using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
	public class LogicDetailGraphView : GraphView
	{		
		public LogicGraphEditorObject LogicGraphEditorObject { get; private set; }
		
		public LogicDetailGraphView()
		{
            GraphLogicEditor.AddStyleSheetPath(this, "Styles/LogicDetailGraphView");
		}
		
		public LogicDetailGraphView(LogicGraphEditorObject logicGraphEditorObject) : this()
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