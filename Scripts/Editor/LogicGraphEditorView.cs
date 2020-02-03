using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace GeoTetra.GTLogicGraph
{
    public class LogicGraphEditorView : VisualElement
    {
        private LogicGraphEditorObject _logicGraphEditorObject;
        private LogicGraphView _graphView;
        private LogicDetailGraphView _detailGraphView;
        private EditorWindow _editorWindow;
        private EdgeConnectorListener _edgeConnectorListener;
        private SearchWindowProvider _searchWindowProvider;
        private bool _reloadGraph;

        public Action saveRequested { get; set; }

        public Action showInProjectRequested { get; set; }

        public LogicGraphView LogicGraphView
        {
            get { return _graphView; }
        }

        public LogicGraphEditorView(EditorWindow editorWindow, LogicGraphEditorObject logicGraphEditorObject)
        {
            _editorWindow = editorWindow;

            _logicGraphEditorObject = logicGraphEditorObject;
            _logicGraphEditorObject.Deserialized += LogicGraphEditorDataOnDeserialized;

            GraphLogicEditor.AddStyleSheetPath(this, "Styles/LogicGraphEditorView");

            var toolbar = new IMGUIContainer(() =>
            {                
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                
                GUILayout.Label(Path.GetFileName(_editorWindow.name));
                GUILayout.Space(32.0f);
                
                if (GUILayout.Button("Save Asset", EditorStyles.toolbarButton))
                {
                    if (saveRequested != null)
                        saveRequested();
                }

                GUILayout.Space(6);
                if (GUILayout.Button("Show In Project", EditorStyles.toolbarButton))
                {
                    if (showInProjectRequested != null)
                        showInProjectRequested();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            });
            Add(toolbar);

            var clientView = new VisualElement {name = "clientView"};
            clientView.style.flexGrow = 1;

            var mainGraph = new VisualElement {name = "mainGraph"};
            {
                _graphView = new LogicGraphView(_logicGraphEditorObject)
                {
                    name = "GraphView",
                    viewDataKey = "LogicGraphView"
                };
                
                _graphView.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
                _graphView.AddManipulator(new ContentDragger());
                _graphView.AddManipulator(new SelectionDragger());
                _graphView.AddManipulator(new RectangleSelector());
                _graphView.AddManipulator(new ClickSelector());
                _graphView.AddManipulator(new EdgeManipulator());
                _graphView.AddManipulator(new FreehandSelector());
                _graphView.RegisterCallback<KeyDownEvent>(KeyDown);

                //*** Test... _graphView.Insert(0, new GridBackground());
                _graphView.focusable = true;
                
                _graphView.graphViewChanged = GraphViewChanged;

                // Add the minimap.
                var miniMap = new MiniMap();
                miniMap.SetPosition(new Rect(0, 372, 200, 176));
                _graphView.Add(miniMap);

                var blackBoard = new Blackboard();
                blackBoard.SetPosition(new Rect(0, 0, 64, 64));
                _graphView.Add(blackBoard);

                _graphView.StretchToParentSize();
                mainGraph.Add(_graphView);
            }

            var detailGraph = new VisualElement {name = "detailGraph"};
            {
                _detailGraphView = new LogicDetailGraphView(_logicGraphEditorObject)
                {
                    name = "DetailGraphView",
                    viewDataKey = "LogicDetailGraphView"
                };
                
                _detailGraphView.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
                _detailGraphView.AddManipulator(new ContentDragger());
                _detailGraphView.AddManipulator(new SelectionDragger());
                _detailGraphView.AddManipulator(new RectangleSelector());
                _detailGraphView.AddManipulator(new ClickSelector());
                _detailGraphView.AddManipulator(new EdgeManipulator());
                _detailGraphView.AddManipulator(new FreehandSelector());
                _detailGraphView.RegisterCallback<KeyDownEvent>(KeyDown);

                _detailGraphView.focusable = true;
                
                _detailGraphView.graphViewChanged = GraphViewChanged;

                _detailGraphView.style.backgroundColor = new Color(0.05f, 0.05f, 0.05f);

                var miniMap = new MiniMap();
                miniMap.SetPosition(new Rect(0, 0, 200, 176));
                _detailGraphView.Add(miniMap);


                _detailGraphView.StretchToParentSize();
                detailGraph.Add(_detailGraphView);
            }


            _searchWindowProvider = ScriptableObject.CreateInstance<SearchWindowProvider>();
            _searchWindowProvider.Initialize(editorWindow, this, _graphView);

            _edgeConnectorListener = new EdgeConnectorListener(this, _searchWindowProvider);

            _graphView.nodeCreationRequest = (c) =>
            {
                _searchWindowProvider.ConnectedPortView = null;
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), _searchWindowProvider);
            };

            LoadElements();

            // Main Graph Style            
            mainGraph.style.width = new Length(100, LengthUnit.Percent);
            mainGraph.style.height = new Length(70, LengthUnit.Percent);
            clientView.Add(mainGraph);
            
            // Detail Graph Style
            detailGraph.style.width = new Length(100, LengthUnit.Percent);
            detailGraph.style.height = new Length(30, LengthUnit.Percent);
            detailGraph.style.borderTopWidth = 2;
            detailGraph.style.borderTopColor = Color.black;
            detailGraph.style.backgroundColor = Color.red;
            clientView.Add(detailGraph);
            
            clientView.StretchToParentSize();
            _editorWindow.rootVisualElement.Add(clientView);            
        }

        private void LoadElements()
        {
            for (int i = 0; i < _logicGraphEditorObject.LogicGraphData.SerializedNodes.Count; ++i)
            {
                AddNodeFromload(_logicGraphEditorObject.LogicGraphData.SerializedNodes[i]);
            }
            
            for (int i = 0; i < _logicGraphEditorObject.LogicGraphData.SerializedInputNodes.Count; ++i)
            {
                AddNodeFromload(_logicGraphEditorObject.LogicGraphData.SerializedInputNodes[i]);
            }
            
            for (int i = 0; i < _logicGraphEditorObject.LogicGraphData.SerializedOutputNodes.Count; ++i)
            {
                AddNodeFromload(_logicGraphEditorObject.LogicGraphData.SerializedOutputNodes[i]);
            }

            for (int i = 0; i < _logicGraphEditorObject.LogicGraphData.SerializedEdges.Count; ++i)
            {
                AddEdgeFromLoad(_logicGraphEditorObject.LogicGraphData.SerializedEdges[i]);
            }
        }

        public void HandleGraphChanges()
        {
            if (_reloadGraph)
            {
                _reloadGraph = false;
                
                foreach (var nodeView in _graphView.nodes.ToList())
                {
                    _graphView.RemoveElement(nodeView);
                }

                foreach (var edge in _graphView.edges.ToList())
                {
                    _graphView.RemoveElement(edge);
                }
                
                LoadElements();
            }
        }
        
        private void LogicGraphEditorDataOnDeserialized()
        {
            // Comes after GraphData was undone, so reload graph
            _reloadGraph = true;
        }

        private GraphViewChange GraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                _logicGraphEditorObject.RegisterCompleteObjectUndo("Graph Element Moved.");
                foreach (var element in graphViewChange.movedElements)
                {
                    NodeEditor nodeEditor = element.userData as NodeEditor;
                    nodeEditor.Position = element.GetPosition().position;
                    nodeEditor.SerializedNode.JSON = JsonUtility.ToJson(nodeEditor);
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                _logicGraphEditorObject.RegisterCompleteObjectUndo("Deleted Graph Elements.");
                
                foreach (var nodeView in graphViewChange.elementsToRemove.OfType<LogicNodeView>())
                {
                    _logicGraphEditorObject.LogicGraphData.SerializedNodes.Remove(nodeView.NodeEditor.SerializedNode);
                    _logicGraphEditorObject.LogicGraphData.SerializedInputNodes.Remove(nodeView.NodeEditor.SerializedNode);
                    _logicGraphEditorObject.LogicGraphData.SerializedOutputNodes.Remove(nodeView.NodeEditor.SerializedNode);
                }

                foreach (var edge in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    _logicGraphEditorObject.LogicGraphData.SerializedEdges.Remove(edge.userData as SerializedEdge);
                }
            }

            return graphViewChange;
        }


        private void KeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Space && !evt.shiftKey && !evt.altKey && !evt.ctrlKey && !evt.commandKey)
            {
            }
            else if (evt.keyCode == KeyCode.F1)
            {
            }
        }

        public void AddNode(NodeEditor nodeEditor)
        {
            _logicGraphEditorObject.RegisterCompleteObjectUndo("Add Node " + nodeEditor.NodeType());

            SerializedNode serializedNode = new SerializedNode
            {
                NodeType = nodeEditor.NodeType(),
                JSON = JsonUtility.ToJson(nodeEditor)
            };

            nodeEditor.SerializedNode = serializedNode;
            if (nodeEditor is IInputNode)
            {
                _logicGraphEditorObject.LogicGraphData.SerializedInputNodes.Add(serializedNode);
            }
            else if (nodeEditor is IOutputNode)
            {
                _logicGraphEditorObject.LogicGraphData.SerializedOutputNodes.Add(serializedNode);
            }
            else
            {
                _logicGraphEditorObject.LogicGraphData.SerializedNodes.Add(serializedNode);
            }

            nodeEditor.Owner = _graphView;
            nodeEditor.DetailView = _detailGraphView;
            var nodeView = new LogicNodeView {userData = nodeEditor};
            _graphView.AddElement(nodeView);
            nodeView.Initialize(nodeEditor, _edgeConnectorListener);
            nodeView.MarkDirtyRepaint();
        }

        private void AddNodeFromload(SerializedNode serializedNode)
        {
            NodeEditor nodeEditor = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeEditor)))
                    {
                        var attrs = type.GetCustomAttributes(typeof(NodeEditorType), false) as NodeEditorType[];
                        if (attrs != null && attrs.Length > 0)
                        {
                            if (attrs[0].NodeType.Name == serializedNode.NodeType)
                            {
                                nodeEditor = (NodeEditor) Activator.CreateInstance(type);
                            }
                        }
                    }
                }
            }

            if (nodeEditor != null)
            {
                JsonUtility.FromJsonOverwrite(serializedNode.JSON, nodeEditor);
                nodeEditor.SerializedNode = serializedNode;
                nodeEditor.Owner = _graphView;
                nodeEditor.DetailView = _detailGraphView;
                var nodeView = new LogicNodeView {userData = nodeEditor};                
                _graphView.AddElement(nodeView);
                nodeView.Initialize(nodeEditor, _edgeConnectorListener);
                nodeView.MarkDirtyRepaint();
            }
            else
            {
                Debug.LogWarning("No NodeEditor found for " + serializedNode);
            }
        }

        public void AddEdge(Edge edgeView)
        {
            PortDescription leftPortDescription;
            PortDescription rightPortDescription;
            GetSlots(edgeView, out leftPortDescription, out rightPortDescription);

            _logicGraphEditorObject.RegisterCompleteObjectUndo("Connect Edge");
            SerializedEdge serializedEdge = new SerializedEdge
            {
                SourceNodeGuid = leftPortDescription.Owner.NodeGuid,
                SourceMemberName = leftPortDescription.MemberName,
                TargetNodeGuid = rightPortDescription.Owner.NodeGuid,
                TargetMemberName = rightPortDescription.MemberName
            };

            _logicGraphEditorObject.LogicGraphData.SerializedEdges.Add(serializedEdge);

            edgeView.userData = serializedEdge;
            edgeView.output.Connect(edgeView);
            edgeView.input.Connect(edgeView);
            _graphView.AddElement(edgeView);
        }

        private void AddEdgeFromLoad(SerializedEdge serializedEdge)
        {
            LogicNodeView sourceNodeView = _graphView.nodes.ToList().OfType<LogicNodeView>()
                .FirstOrDefault(x => x.NodeEditor.NodeGuid == serializedEdge.SourceNodeGuid);
            if (sourceNodeView != null)
            {
                PortView sourceAnchor = sourceNodeView.outputContainer.Children().OfType<PortView>()
                    .FirstOrDefault(x => x.PortDescription.MemberName == serializedEdge.SourceMemberName);

                LogicNodeView targetNodeView = _graphView.nodes.ToList().OfType<LogicNodeView>()
                    .FirstOrDefault(x => x.NodeEditor.NodeGuid == serializedEdge.TargetNodeGuid);
                PortView targetAnchor = targetNodeView.inputContainer.Children().OfType<PortView>()
                    .FirstOrDefault(x => x.PortDescription.MemberName == serializedEdge.TargetMemberName);

                var edgeView = new Edge
                {
                    userData = serializedEdge,
                    output = sourceAnchor,
                    input = targetAnchor
                };
                edgeView.output.Connect(edgeView);
                edgeView.input.Connect(edgeView);
                _graphView.AddElement(edgeView);
            }
        }


        private void GetSlots(Edge edge, out PortDescription leftPortDescription, out PortDescription rightPortDescription)
        {
            leftPortDescription = (edge.output as PortView).PortDescription;
            rightPortDescription = (edge.input as PortView).PortDescription;
        }
    }
}