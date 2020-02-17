using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SensorFoundation.SensorGraph
{
    /// <summary>
    /// Describes how to draw a node, paired with GenericNodeview
    /// </summary>
    [Serializable]
    public abstract class NodeEditor : INodeEditor
    {
        [SerializeField] private string _displayName;        
        [SerializeField] private Vector3 _position;
        [SerializeField] private bool _expanded = true;        
        
        [SerializeField]
        private List<SerializedNode> _serializedDetailNodes = new List<SerializedNode>();

        [SerializeField]
        private List<SerializedEdge> _serializedDetailEdges = new List<SerializedEdge>();
      
        public LogicGraphView Owner { get; set; }
        public LogicDetailGraphView DetailView { get; set; }

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
            var attrs = GetType().GetCustomAttributes(typeof(TitleAttribute), false) as TitleAttribute[];
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
                return val;
            }
            else
            {
                Debug.LogWarning(this.GetType() + " requires a NodeType attribute");
                return "";
            }
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
                PortView sourceAnchor = sourceNodeView.outputContainer.Children().OfType<PortView>()
                    .FirstOrDefault(x => x.PortDescription.Guid == serializedEdge.SourceMemberPortName);

                LogicDetailNodeView targetNodeView = DetailView.nodes.ToList().OfType<LogicDetailNodeView>()
                    .FirstOrDefault(x => x.NodeEditor.NodeGuid == serializedEdge.TargetNodeGuid);
                PortView targetAnchor = targetNodeView.inputContainer.Children().OfType<PortView>()
                    .FirstOrDefault(x => x.PortDescription.Guid == serializedEdge.TargetMemberPortName);

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