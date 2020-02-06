using System.Reflection;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
	public interface INodeDetailControlAttribute
	{
		VisualElement InstantiateControl(NodeDetailEditor nodeEditor, PropertyInfo propertyInfo);
	}
}
