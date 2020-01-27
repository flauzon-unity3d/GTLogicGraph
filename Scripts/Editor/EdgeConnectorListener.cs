﻿using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace GeoTetra.GTLogicGraph
{
    public class EdgeConnectorListener : IEdgeConnectorListener
    {
        private readonly LogicGraphEditorView _logicGraphEditorView;
        private readonly SearchWindowProvider _searchWindowProvider;

        public EdgeConnectorListener(LogicGraphEditorView logicGraphEditorView, SearchWindowProvider searchWindowProvider)
        {
            _logicGraphEditorView = logicGraphEditorView;
            _searchWindowProvider = searchWindowProvider;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            var draggedPort = (edge.output != null ? edge.output.edgeConnector.edgeDragHelper.draggedPort : null) ??
                              (edge.input != null ? edge.input.edgeConnector.edgeDragHelper.draggedPort : null);
            _searchWindowProvider.ConnectedPortView = (PortView) draggedPort;
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                _searchWindowProvider);
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            _logicGraphEditorView.AddEdge(edge);
        }
    }
}