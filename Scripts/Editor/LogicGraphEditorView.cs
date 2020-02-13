using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
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
        private EdgeDetailConnectorListener _edgeDetailConnectorListener;
        private SearchWindowProvider _searchWindowProvider;
        private SearchDetailWindowProvider _searchDetailWindowProvider;
        private bool _reloadGraph;
        private IMGUIContainer _toolbarDetail;
        
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
            public string Type;

            public tPropertyParameter(Blackboard _bb, string _name, string _type)
            {
                Bb = _bb;
                Name = _name;
                Type = _type;
            }
        }

        void OnMainAddProperty(Blackboard bb)
        {
            GenericMenu menu = new GenericMenu();
            
            menu.AddItem(EditorGUIUtility.TrTextContent("FieldBool"), false, OnMainAddPropertyParameter, new tPropertyParameter(bb, "FieldBool", "FieldBool"));
            menu.AddItem(EditorGUIUtility.TrTextContent("FieldFloat"), false, OnMainAddPropertyParameter, new tPropertyParameter(bb, "FieldFloat", "FieldFloat"));
            menu.AddItem(EditorGUIUtility.TrTextContent("FieldVector2"), false, OnMainAddPropertyParameter, new tPropertyParameter(bb, "FieldVector2", "FieldVector2"));
            menu.AddItem(EditorGUIUtility.TrTextContent("FieldVector3"), false, OnMainAddPropertyParameter, new tPropertyParameter(bb, "FieldVector3", "FieldVector3"));
            menu.AddItem(EditorGUIUtility.TrTextContent("FieldVector4"), false, OnMainAddPropertyParameter, new tPropertyParameter(bb, "FieldVector4", "FieldVector4"));

            //menu.AddItem(EditorGUIUtility.TrTextContent("Parameter"), false, OnMainAddPropertyCategory);
            /*
            if (!(controller.model.subgraph is VisualEffectSubgraphOperator))
            {
                menu.AddItem(EditorGUIUtility.TrTextContent("Category"), false, OnAddCategory);
                menu.AddSeparator(string.Empty);
            }

            foreach (var parameter in VFXLibrary.GetParameters())
            {
                VFXParameter model = parameter.model as VFXParameter;

                var type = model.type;
                if (type == typeof(GPUEvent))
                    continue;

                menu.AddItem(EditorGUIUtility.TextContent(type.UserFriendlyName()), false, OnAddParameter, parameter);
            }
            */

            menu.ShowAsContext();
        }

        void OnMainAddPropertyParameter(object parameter)
        {
            tPropertyParameter p = (tPropertyParameter)parameter;
            p.Bb.Add(new BlackboardField(null, p.Name, p.Type));
            /*var selectedCategory = m_View.selection.OfType<VFXBlackboardCategory>().FirstOrDefault();
            VFXParameter newParam = m_Controller.AddVFXParameter(Vector2.zero, (VFXModelDescriptorParameters)parameter);
            if (selectedCategory != null && newParam != null)
                newParam.category = selectedCategory.title;*/
        }


        void OnMainAddPropertyCategory()
        {
            /*string newCategoryName = EditorGUIUtility.TrTextContent("new category").text;
            int cpt = 1;
            while (controller.graph.UIInfos.categories.Any(t => t.name == newCategoryName))
            {
                newCategoryName = string.Format(EditorGUIUtility.TrTextContent("new category {0}").text, cpt++);
            }

            controller.graph.UIInfos.categories.Add(new VFXUI.CategoryInfo() { name = newCategoryName });
            controller.graph.Invalidate(VFXModel.InvalidationCause.kUIChanged);*/
        }

        void OnMainEditPropertyName(Blackboard bb, VisualElement element, string value)
        {
            if (element is BlackboardField)
            {
                (element as BlackboardField).name = value;
            }
        }

        void OnPropertyDragUpdatedEvent(DragUpdatedEvent e)
        {
            Debug.Log("DROP");
           /* var selection = DragAndDrop.GetGenericData("DragSelection") as List<ISelectable>;

            if (selection == null)
            {
                SetDragIndicatorVisible(false);
                return;
            }

            if (selection.Any(t => !(t is VFXBlackboardCategory)))
            {
                SetDragIndicatorVisible(false);
                return;
            }

            Vector2 localPosition = e.localMousePosition;

            m_InsertIndex = InsertionIndex(localPosition);

            if (m_InsertIndex != -1)
            {
                float indicatorY = 0;

                if (m_InsertIndex == childCount)
                {
                    if (childCount > 0)
                    {
                        VisualElement lastChild = this[childCount - 1];

                        indicatorY = lastChild.ChangeCoordinatesTo(this, new Vector2(0, lastChild.layout.height + lastChild.resolvedStyle.marginBottom)).y;
                    }
                    else
                    {
                        indicatorY = this.contentRect.height;
                    }
                }
                else
                {
                    VisualElement childAtInsertIndex = this[m_InsertIndex];

                    indicatorY = childAtInsertIndex.ChangeCoordinatesTo(this, new Vector2(0, -childAtInsertIndex.resolvedStyle.marginTop)).y;
                }

                SetDragIndicatorVisible(true);

                m_DragIndicator.style.top =  indicatorY - m_DragIndicator.resolvedStyle.height * 0.5f;

                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            }
            else
            {
                SetDragIndicatorVisible(false);
            }
            e.StopPropagation();*/
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

                var blackBoard = new Blackboard();
                blackBoard.SetPosition(new Rect(4, 40, 300, 100));
                blackBoard.title = "Properties";
                blackBoard.subTitle = String.Empty;
                blackBoard.addItemRequested = OnMainAddProperty;
                blackBoard.editTextRequested = OnMainEditPropertyName;
                blackBoard.RegisterCallback<DragUpdatedEvent>(OnPropertyDragUpdatedEvent);
                _graphView.Add(blackBoard);

                _graphView.StretchToParentSize();
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
                    _logicGraphEditorObject.LogicGraphData.SerializedInputNodes.Remove(nodeView.NodeEditor.SerializedNode);
                    _logicGraphEditorObject.LogicGraphData.SerializedOutputNodes.Remove(nodeView.NodeEditor.SerializedNode);
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
            IPortDescription leftPortDescription;
            IPortDescription rightPortDescription;
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
                SourceMemberName = leftPortDescription.MemberName,
                TargetNodeGuid = rightPortDescription.Owner.NodeGuid,
                TargetMemberName = rightPortDescription.MemberName
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