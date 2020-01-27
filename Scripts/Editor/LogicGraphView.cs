using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
	public class LogicGraphView : GraphView
	{		
		public LogicGraphEditorObject LogicGraphEditorObject { get; private set; }
		
		public LogicGraphView()
		{
            GraphLogicEditor.AddStyleSheetPath(this, "Styles/LogicGraphView");
			Debug.Log("LogicGraphView Constructor");
		}
		
		public LogicGraphView(LogicGraphEditorObject logicGraphEditorObject) : this()
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