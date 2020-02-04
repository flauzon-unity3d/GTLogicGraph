using System;

namespace GeoTetra.GTLogicGraph
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NodeDetailEditorType : Attribute
	{
		public readonly Type NodeType;
		public NodeDetailEditorType(Type nodeType) { this.NodeType = nodeType; }
	}
}
