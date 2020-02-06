using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    /// <summary>
    /// Describes how to draw a node, paired with GenericNodeview
    /// </summary>
    [Serializable]
    public abstract class NodeEditor
    {
        [NonSerialized] private List<PortDescription> _portDescriptions = new List<PortDescription>();

        [SerializeField] private string _displayName;        
        [SerializeField] private Vector3 _position;
        [SerializeField] private bool _expanded = true;
        [SerializeField] private string _nodeGuid;
        
        [SerializeField]
        private List<SerializedNode> _serializedDetailNodes = new List<SerializedNode>();

        [SerializeField]
        private List<SerializedEdge> _serializedDetailEdges = new List<SerializedEdge>();


        public LogicGraphView Owner { get; set; }
        public LogicDetailGraphView DetailView { get; set; }
        public SerializedNode SerializedNode { get; set; }

        public List<SerializedEdge> SerializedDetailEdges
        {
            get { return _serializedDetailEdges; }
        }

        public List<SerializedNode> SerializedDetailNodes
        {
            get { return _serializedDetailNodes; }
        }


        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public bool Expanded
        {
            get { return _expanded; }
            set { _expanded = value; }
        }

        public string NodeGuid
        {
            get { return _nodeGuid; }
        }

        public string DisplayName
        {
            set
            {
                _displayName = value;
            }
            get 
            {
                if (String.IsNullOrEmpty(_displayName))
                {
                    return NodeType();
                }
                return _displayName;
            }
        }

        public NodeEditor()
        {
            _nodeGuid = System.Guid.NewGuid().ToString();
            ConstructNode();
        }

        public abstract void ConstructNode();
                       
        public string NodeType()
        {
            var attrs = GetType().GetCustomAttributes(typeof(NodeEditorType), false) as NodeEditorType[];
            if (attrs != null && attrs.Length > 0)
            {
                return attrs[0].NodeType.Name;
            }
            else
            {
                Debug.LogWarning(this.GetType() + " requires a NodeType attribute");
                return "";
            }
        }

        public void GetInputSlots<T>(List<T> foundSlots) where T : PortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isInputSlot && slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void GetOutputSlots<T>(List<T> foundSlots) where T : PortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isOutputSlot && slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void GetSlots<T>(List<T> foundSlots) where T : PortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void SetDirty()
        {
            SerializedNode.JSON = JsonUtility.ToJson(this);
        }

        public void AddSlot(PortDescription portDescription)
        {
            if (!(portDescription is PortDescription))
                throw new ArgumentException(string.Format(
                    "Trying to add slot {0} to Material node {1}, but it is not a {2}", portDescription, this,
                    typeof(PortDescription)));

            _portDescriptions.Add(portDescription);
        }

        public T FindSlot<T>(string memberName) where T : PortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.MemberName == memberName && slot is T)
                    return (T) slot;
            }

            return default(T);
        }

        public T FindInputSlot<T>(string memberName) where T : PortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isInputSlot && slot.MemberName == memberName && slot is T)
                    return (T) slot;
            }

            return default(T);
        }

        public T FindOutputSlot<T>(string memberName) where T : PortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isOutputSlot && slot.MemberName == memberName && slot is T)
                    return (T) slot;
            }

            return default(T);
        }

        private void AddNodeFromLoad(SerializedNode serializedNode)
        {
            NodeDetailEditor nodeEditor = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeDetailEditor)))
                    {
                        var attrs = type.GetCustomAttributes(typeof(NodeDetailEditorType), false) as NodeDetailEditorType[];
                        if (attrs != null && attrs.Length > 0)
                        {
                            if (attrs[0].NodeType.Name == serializedNode.NodeType)
                            {
                                nodeEditor = (NodeDetailEditor)Activator.CreateInstance(type);
                            }
                        }
                    }
                }
            }

            if (nodeEditor != null)
            {                
                JsonUtility.FromJsonOverwrite(serializedNode.JSON, nodeEditor);
                nodeEditor.SerializedNode = serializedNode;
                nodeEditor.Owner = Owner;
                nodeEditor.DetailView = DetailView;
                var nodeView = new LogicDetailNodeView { userData = nodeEditor };
                DetailView.AddElement(nodeView);
                nodeView.Initialize(nodeEditor, Owner.EditorView.EdgeDetailConnectorListener);
                nodeView.MarkDirtyRepaint();
            }
            else
            {
                Debug.LogWarning("No NodeEditor found for " + serializedNode);
            }
        }

        private void AddEdgeFromLoad(SerializedEdge serializedEdge)
        {
            LogicDetailNodeView sourceNodeView = DetailView.nodes.ToList().OfType<LogicDetailNodeView>()
                .FirstOrDefault(x => x.NodeEditor.NodeGuid == serializedEdge.SourceNodeGuid);
            if (sourceNodeView != null)
            {
                PortDetailView sourceAnchor = sourceNodeView.outputContainer.Children().OfType<PortDetailView>()
                    .FirstOrDefault(x => x.PortDescription.MemberName == serializedEdge.SourceMemberName);

                LogicDetailNodeView targetNodeView = DetailView.nodes.ToList().OfType<LogicDetailNodeView>()
                    .FirstOrDefault(x => x.NodeEditor.NodeGuid == serializedEdge.TargetNodeGuid);
                PortDetailView targetAnchor = targetNodeView.inputContainer.Children().OfType<PortDetailView>()
                    .FirstOrDefault(x => x.PortDescription.MemberName == serializedEdge.TargetMemberName);

                var edgeView = new Edge
                {
                    userData = serializedEdge,
                    output = sourceAnchor,
                    input = targetAnchor
                };
                edgeView.output.Connect(edgeView);
                edgeView.input.Connect(edgeView);
                DetailView.AddElement(edgeView);
            }
        }

        public void BuildDetailView(LogicDetailGraphView detailView)
        {
            for (int i = 0; i < _serializedDetailNodes.Count; ++i)
            {
                AddNodeFromLoad(_serializedDetailNodes[i]);
            }

            for (int i = 0; i < _serializedDetailEdges.Count; ++i)
            {
                AddEdgeFromLoad(_serializedDetailEdges[i]);
            }
        }
    }
}