using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace SensorFoundation.SensorGraph
{
    public class LogicGraphEditorView : VisualElement
    {
        private LogicGraphEditorObject _logicGraphEditorObject;
        private LogicGraphView _graphView;
        private LogicDetailGraphView _detailGraphView;
        private EditorWindow _editorWindow;
        private EdgeConnectorListener _edgeConnectorListener;
        private EdgeDetailConnectorListener _edgeDetailConnectorListener;
        private SearchWindowProvider _searchWindowProvider;
        private SearchDetailWindowProvider _searchDetailWindowProvider;
        private bool _reloadGraph;
        private IMGUIContainer _toolbarDetail;
        private Blackboard _blackBoard;
        
        public enum eAccuracyLevel
        {
            Lowest = 0,
            Low,
            Medium,
            High,
            Highest
        }

        private eAccuracyLevel _accuracyLevel = eAccuracyLevel.Lowest;

        public Action saveRequested { get; set; }

        public Action showInProjectRequested { get; set; }

        public IMGUIContainer ToolbarDetail
        { 
            get { return _toolbarDetail;  }
        }

        public EdgeConnectorListener EdgeConnectorListener
        {
            get { return _edgeConnectorListener; }
        }

        public EdgeDetailConnectorListener EdgeDetailConnectorListener
        {
            get { return _edgeDetailConnectorListener; }
        }

        public LogicGraphView LogicGraphView
        {
            get { return _graphView; }
        }

        class tPropertyParameter
        {
            public Blackboard Bb;
            public string Name;
            public Type Type;
            public string BoundObject;

            public tPropertyParameter(Blackboard _bb, string _name, Type _type, string _boundObject)
            {
                Bb = _bb;
                Name = _name;
                Type = _type;
                BoundObject = _boundObject;
            }
        }

        public static IEnumerable<Type> GetTypesOrNothing(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        }

        void OnMainAddProperty(Blackboard bb)
        {
            GenericMenu menu = new GenericMenu();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in GetTypesOrNothing(assembly))
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeEditor)))
                    {
                        var attrs = type.GetCustomAttributes(typeof(TitleAttribute), false) as TitleAttribute[];
                        if (attrs != null && attrs.Length > 0)
                        {
                            if (attrs[0].title[0] == "Type")
                            {
                                //*** Resolve boundobject name
                                menu.AddItem(EditorGUIUtility.TrTextContent(attrs[0].title[1]), false, OnMainAddPropertyParameter, new tPropertyParameter(bb, attrs[0].title[1], type, "testbound"));
                            }
                        }
                    }
                }
            }

          
            menu.ShowAsContext();
        }

        void OnMainAddPropertyParameter(object parameter)
        {
            tPropertyParameter p = (tPropertyParameter)parameter;
            BlackboardField field = new BlackboardField(null, p.Name, p.Type.Name);
            field.userData = p;
            p.Bb.Add(field);

            LogicParameter param = new LogicParameter();
            param.DisplayName = p.Name;
            AddParameter(param);
        }


        void OnMainAddPropertyCategory()
        {
        }

        void OnMainEditPropertyName(Blackboard bb, VisualElement element, string value)
        {
            if (element is BlackboardField)
            {
                (element as BlackboardField).name = value;
                (element as BlackboardField).text = value;
                tPropertyParameter p = (element as BlackboardField).userData as tPropertyParameter;
                if (p != null)
                {
                    p.Name = value;
                }
            }
        }

        private void OnPropertyDragUpdatedEvent(DragUpdatedEvent e)
        {            
            e.StopPropagation();
        }

        private void OnPropertyDragPerformEvent(DragPerformEvent e)
        {
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
                GUILayout.FlexibleSpace();

                _accuracyLevel = (eAccuracyLevel)EditorGUILayout.EnumPopup("Viewing Profile:", _accuracyLevel, GUILayout.MinWidth(220));
                //*** TODO Logic when changing selection
                

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

                GUILayout.Space(6);
                if (GUILayout.Button("Generate", EditorStyles.toolbarButton))
                {
                    //***
                }
                
                GUILayout.Space(6);                
                GUILayout.EndHorizontal();
            });
            Add(toolbar);

            var clientView = new VisualElement {name = "clientView"};
            clientView.style.flexGrow = 1;

            var mainGraph = new VisualElement {name = "mainGraph"};
            {
                _graphView = new LogicGraphView(this, _logicGraphEditorObject)
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

                _graphView.focusable = true;
                
                _graphView.graphViewChanged = GraphViewChanged;

                _blackBoard = new Blackboard();
                _blackBoard.SetPosition(new Rect(4, 40, 300, 100));
                _blackBoard.title = "Properties";
                _blackBoard.subTitle = String.Empty;
                _blackBoard.addItemRequested = OnMainAddProperty;
                _blackBoard.editTextRequested = OnMainEditPropertyName;
                _blackBoard.RegisterCallback<DragUpdatedEvent>(OnPropertyDragUpdatedEvent);
                _blackBoard.RegisterCallback<DragPerformEvent>(OnPropertyDragPerformEvent);
                _graphView.Add(_blackBoard);

                _graphView.StretchToParentSize();
                mainGraph.RegisterCallback<DragEnterEvent>(OnMainGraphDragEntryEvent);
                mainGraph.RegisterCallback<DragPerformEvent>(OnMainGraphDragPerformEvent);
                mainGraph.RegisterCallback<DragUpdatedEvent>(OnMainGraphDragUpdatedEvent);
                mainGraph.Add(_graphView);
            }

            var detailGraph = new VisualElement {name = "detailGraph"};
            {
                _detailGraphView = new LogicDetailGraphView(this, _logicGraphEditorObject, _graphView)
                {
                    name = "DetailGraphView",
                    viewDataKey = "LogicDetailGraphView"
                };
                
                _detailGraphView.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
                _detailGraphView.AddManipulator(new ContentDragger());
                _detailGraphView.AddManipulator(new CustomSelectionDragger());                
                _detailGraphView.AddManipulator(new ClickSelector());
                _detailGraphView.AddManipulator(new EdgeManipulator());
                _detailGraphView.AddManipulator(new FreehandSelector());
                _detailGraphView.RegisterCallback<KeyDownEvent>(KeyDown);

                _detailGraphView.focusable = true;
                
                _detailGraphView.graphViewChanged = GraphViewChanged;
                _detailGraphView.style.backgroundColor = new Color(0.05f, 0.05f, 0.05f);

                _detailGraphView.StretchToParentSize();
                detailGraph.Add(_detailGraphView);
            }

            var labelTitle = new IMGUIContainer(() =>
            {
                GUILayout.BeginVertical();
                GUILayout.Space(24.0f);
                
                GUILayout.BeginHorizontal(EditorStyles.label);                
                GUILayout.FlexibleSpace();
                GUIStyle style = new GUIStyle();
                style.font = EditorStyles.boldFont;
                style.normal.textColor = Color.white;
                style.fontSize = 20;

                GUILayout.Label("Sensor Graph", style, GUILayout.Height(24));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            });
            mainGraph.Add(labelTitle);

            _toolbarDetail = new IMGUIContainer(() =>
            {
                GUILayout.BeginVertical();
                GUILayout.Space(4.0f);

                GUILayout.BeginHorizontal(EditorStyles.label);
                GUILayout.FlexibleSpace();
                GUIStyle style = new GUIStyle();
                style.font = EditorStyles.boldFont;
                style.normal.textColor = Color.white;
                style.fontSize = 20;

                GUILayout.Label("Node simulation detail", style, GUILayout.Height(24));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            });
            detailGraph.Add(_toolbarDetail);


            // GraphView node creation search
            _searchWindowProvider = ScriptableObject.CreateInstance<SearchWindowProvider>();
            _searchWindowProvider.Initialize(editorWindow, this, _graphView);

            _edgeConnectorListener = new EdgeConnectorListener(this, _searchWindowProvider);

            _graphView.nodeCreationRequest = (c) =>
            {
                _searchWindowProvider.ConnectedPortView = null;
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), _searchWindowProvider);
            };

            // DetailGraphView node creation search
            _searchDetailWindowProvider = ScriptableObject.CreateInstance<SearchDetailWindowProvider>();
            _searchDetailWindowProvider.Initialize(editorWindow, this, _graphView, _detailGraphView);
            
            _edgeDetailConnectorListener = new EdgeDetailConnectorListener(this, _searchDetailWindowProvider);

             _detailGraphView.nodeCreationRequest = (c) =>
            {
                _searchDetailWindowProvider.ConnectedPortView = null;
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), _searchDetailWindowProvider);
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

        private void OnMainGraphDragUpdatedEvent(DragUpdatedEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            DragAndDrop.AcceptDrag();
            evt.StopPropagation();
        }

        private void OnMainGraphDragPerformEvent(DragPerformEvent evt)
        {
            List<ISelectable> obj = DragAndDrop.GetGenericData("DragSelection") as List<ISelectable>;            
            if (obj != null && obj.Count > 0)
            {
                BlackboardField field = (BlackboardField)obj[0];
                tPropertyParameter param = field.userData as tPropertyParameter;
                if (param != null)
                {
                    var inst = Activator.CreateInstance(param.Type);
                    if (inst is NodeEditor)
                    {
                        NodeEditor node = inst as NodeEditor;

                        // Check in Param what is bound to that node, and set to nodeeditor.
                        var windowMousePosition = evt.mousePosition;
                        var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(windowMousePosition);
                        node.Position = new Vector3(graphMousePosition.x, graphMousePosition.y, 0);
                        node.DisplayName = param.Name;
                        node.BoundObject = param.BoundObject;
                        AddNode(node);
                    }
                }
            }

            DragAndDrop.AcceptDrag();
            evt.StopPropagation();
        }

        private void OnMainGraphDragEntryEvent(DragEnterEvent evt)
        {
            DragAndDrop.AcceptDrag();
            evt.StopPropagation();
        }

        private void LoadElements()
        {
            for (int i = 0; i < _logicGraphEditorObject.LogicGraphData.SerializedParameters.Count; ++i)
            {
                AddParameterFromLoad(_logicGraphEditorObject.LogicGraphData.SerializedParameters[i]);
            }

            for (int i = 0; i < _logicGraphEditorObject.LogicGraphData.SerializedNodes.Count; ++i)
            {
                AddNodeFromload(_logicGraphEditorObject.LogicGraphData.SerializedNodes[i]);
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
                    if (nodeEditor != null)
                    {
                        nodeEditor.Position = element.GetPosition().position;
                        nodeEditor.SerializedNode.JSON = JsonUtility.ToJson(nodeEditor);
                    }

                    NodeDetailEditor nodeDetailEditor = element.userData as NodeDetailEditor;
                    if (nodeDetailEditor != null)
                    {
                        nodeDetailEditor.Position = element.GetPosition().position;
                        nodeDetailEditor.SerializedNode.JSON = JsonUtility.ToJson(nodeDetailEditor);
                        if (nodeDetailEditor.Owner.ContextNode != null)
                        {
                            nodeDetailEditor.Owner.ContextNode.SetDirty();
                        }
                    }
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                _logicGraphEditorObject.RegisterCompleteObjectUndo("Deleted Graph Elements.");
                
                foreach (var nodeView in graphViewChange.elementsToRemove.OfType<LogicNodeView>())
                {
                    _logicGraphEditorObject.LogicGraphData.SerializedNodes.Remove(nodeView.NodeEditor.SerializedNode);
                }

                foreach (var nodeView in graphViewChange.elementsToRemove.OfType<LogicDetailNodeView>())
                {
                    foreach (var parentNodeView in _graphView.nodes.ToList())
                    {
                        LogicNodeView n = parentNodeView as LogicNodeView;
                        if (n != null)
                        {
                            if (n.NodeEditor.SerializedDetailNodes.Remove(nodeView.NodeEditor.SerializedNode))
                            {
                                n.NodeEditor.SetDirty();
                            }
                        }
                    }
                }

                foreach (var edge in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    IPortDescription leftPortDescription;
                    IPortDescription rightPortDescription;
                    GetSlots(edge, out leftPortDescription, out rightPortDescription);
                                        
                    if (!_logicGraphEditorObject.LogicGraphData.SerializedEdges.Remove(edge.userData as SerializedEdge))
                    {
                        // Not found in graphView, dig into graphView nodes and remove.
                        foreach (var nodeView in _graphView.nodes.ToList())
                        {
                            LogicNodeView n = nodeView as LogicNodeView;
                            if (n != null)
                            {
                                n.NodeEditor.SerializedDetailEdges.Remove(edge.userData as SerializedEdge);
                                n.NodeEditor.SetDirty();
                            }
                        }
                    }

                    (edge.output as PortView).Disconnect(edge);
                    (edge.input as PortView).Disconnect(edge);

                    leftPortDescription.RefreshEditor();
                    rightPortDescription.RefreshEditor();
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

        public void AddDetailNode(NodeDetailEditor nodeEditor, NodeEditor contextNode)
        {
            if (contextNode == null)
            {
                return;
            }

            _logicGraphEditorObject.RegisterCompleteObjectUndo("Add Detail Node " + nodeEditor.NodeType());

            SerializedNode serializedNode = new SerializedNode
            {
                NodeType = nodeEditor.NodeType(),
                JSON = JsonUtility.ToJson(nodeEditor)
            };
            nodeEditor.SerializedNode = serializedNode;

            // Serialize within contextNode
            contextNode.SerializedDetailNodes.Add(serializedNode);

            nodeEditor.Owner = _graphView;
            nodeEditor.DetailView = _detailGraphView;
            var nodeView = new LogicDetailNodeView {userData = nodeEditor};
            _detailGraphView.AddElement(nodeView);
            nodeView.Initialize(nodeEditor, _edgeDetailConnectorListener);
            nodeView.MarkDirtyRepaint();

            contextNode.SetDirty();
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
            _logicGraphEditorObject.LogicGraphData.SerializedNodes.Add(serializedNode);

            nodeEditor.Owner = _graphView;
            nodeEditor.DetailView = _detailGraphView;
            var nodeView = new LogicNodeView { userData = nodeEditor };
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
                        var attrs = type.GetCustomAttributes(typeof(TitleAttribute), false) as TitleAttribute[];
                        if (attrs != null && attrs.Length > 0)
                        {
                            string val = "";
                            for (int a = 0; a < attrs[0].title.Length; ++a)
                            {
                                val += attrs[0].title[a];
                                if (a < attrs[0].title.Length - 1)
                                {
                                    val += "/";
                                }
                            }
                            if (val == serializedNode.NodeType)
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

        public void AddParameter(LogicParameter param)
        {
            SerializedParameter serializedParam = new SerializedParameter
            {
                Type = param.GetType().Name,
                JSON = param.DisplayName
            };
            _logicGraphEditorObject.LogicGraphData.SerializedParameters.Add(serializedParam);
        }

        public void AddParameterFromLoad(SerializedParameter param)
        {
            //***
            tPropertyParameter p = new tPropertyParameter(_blackBoard, "test", typeof(FieldBool), null);

            BlackboardField field = new BlackboardField(null, p.Name, p.Type.Name);
            field.userData = p;
            p.Bb.Add(field);
        }

        public void AddEdge(Edge edgeView)
        {
            IPortDescription leftPortDescription;
            IPortDescription rightPortDescription;
            GetSlots(edgeView, out leftPortDescription, out rightPortDescription);

            _logicGraphEditorObject.RegisterCompleteObjectUndo("Connect Edge");
            SerializedEdge serializedEdge = new SerializedEdge
            {
                SourceNodeGuid = leftPortDescription.Owner.NodeGuid,
                SourceMemberPortName = leftPortDescription.Guid,
                SourceMemberTypeName = leftPortDescription.MemberName,                
                TargetNodeGuid = rightPortDescription.Owner.NodeGuid,
                TargetMemberTypeName = rightPortDescription.MemberName,
                TargetMemberPortName = rightPortDescription.Guid
            };

            _logicGraphEditorObject.LogicGraphData.SerializedEdges.Add(serializedEdge);

            edgeView.userData = serializedEdge;
            edgeView.output.Connect(edgeView);
            edgeView.input.Connect(edgeView);

            leftPortDescription.RefreshEditor();
            rightPortDescription.RefreshEditor();

            _graphView.AddElement(edgeView);
        }

        private void AddEdgeFromLoad(SerializedEdge serializedEdge)
        {
            LogicNodeView sourceNodeView = _graphView.nodes.ToList().OfType<LogicNodeView>()
                .FirstOrDefault(x => x.NodeEditor.NodeGuid == serializedEdge.SourceNodeGuid);
            if (sourceNodeView != null)
            {
                PortView sourceAnchor = sourceNodeView.outputContainer.Children().OfType<PortView>()
                    .FirstOrDefault(x => x.PortDescription.Guid == serializedEdge.SourceMemberPortName);

                LogicNodeView targetNodeView = _graphView.nodes.ToList().OfType<LogicNodeView>()
                    .FirstOrDefault(x => x.NodeEditor.NodeGuid == serializedEdge.TargetNodeGuid);
                PortView targetAnchor = targetNodeView.inputContainer.Children().OfType<PortView>()
                    .FirstOrDefault(x => x.PortDescription.Guid == serializedEdge.TargetMemberPortName);

                var edgeView = new Edge
                {
                    userData = serializedEdge,
                    output = sourceAnchor,
                    input = targetAnchor
                };

                IPortDescription leftPortDescription;
                IPortDescription rightPortDescription;
                GetSlots(edgeView, out leftPortDescription, out rightPortDescription);

                edgeView.output.Connect(edgeView);
                edgeView.input.Connect(edgeView);

                leftPortDescription.RefreshEditor();
                rightPortDescription.RefreshEditor();

                _graphView.AddElement(edgeView);
            }
        }
        
        private void GetSlots(Edge edge, out IPortDescription leftPortDescription, out IPortDescription rightPortDescription)
        {
            leftPortDescription = (edge.output as PortView).PortDescription;
            rightPortDescription = (edge.input as PortView).PortDescription;
        }
        
        public void AddDetailEdge(Edge edgeView, NodeEditor contextNode)
        {
            IPortDescription leftPortDescription;
            IPortDescription rightPortDescription;
            GetSlots(edgeView, out leftPortDescription, out rightPortDescription);

            _logicGraphEditorObject.RegisterCompleteObjectUndo("Connect Edge");
            SerializedEdge serializedEdge = new SerializedEdge
            {
                SourceNodeGuid = leftPortDescription.Owner.NodeGuid,
                SourceMemberPortName = leftPortDescription.Guid,
                SourceMemberTypeName = leftPortDescription.MemberName,
                TargetNodeGuid = rightPortDescription.Owner.NodeGuid,
                TargetMemberPortName = rightPortDescription.Guid,
                TargetMemberTypeName = rightPortDescription.MemberName
            };
            contextNode.SerializedDetailEdges.Add(serializedEdge);

            edgeView.userData = serializedEdge;
            edgeView.output.Connect(edgeView);
            edgeView.input.Connect(edgeView);
            _detailGraphView.AddElement(edgeView);

            contextNode.SetDirty();
        }
    }
}