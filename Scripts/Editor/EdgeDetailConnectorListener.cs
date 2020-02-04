using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace GeoTetra.GTLogicGraph
{
    public class EdgeDetailConnectorListener : IEdgeConnectorListener
    {
        private readonly LogicGraphEditorView _logicGraphEditorView;
        private readonly SearchDetailWindowProvider _searchWindowProvider;

        public EdgeDetailConnectorListener(LogicGraphEditorView logicGraphEditorView, SearchDetailWindowProvider searchWindowProvider)
        {
            _logicGraphEditorView = logicGraphEditorView;
            _searchWindowProvider = searchWindowProvider;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            var draggedPort = (edge.output != null ? edge.output.edgeConnector.edgeDragHelper.draggedPort : null) ??
                              (edge.input != null ? edge.input.edgeConnector.edgeDragHelper.draggedPort : null);
            _searchWindowProvider.ConnectedPortView = (PortDetailView) draggedPort;
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                _searchWindowProvider);
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            _logicGraphEditorView.AddDetailEdge(edge);
        }
    }
}