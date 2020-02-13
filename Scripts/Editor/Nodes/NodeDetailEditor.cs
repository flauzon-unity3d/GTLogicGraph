using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    /// <summary>
    /// Describes how to draw a node, paired with GenericNodeview
    /// </summary>
    [Serializable]
    public abstract class NodeDetailEditor : INodeEditor
    {
        [SerializeField] private string _displayName;        
        [SerializeField] private Vector3 _position;
        [SerializeField] private bool _expanded = true;

        public LogicGraphView Owner { get; set; }
        public LogicDetailGraphView DetailView { get; set; }

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

        public override void SetDirty()
        {
            SerializedNode.JSON = JsonUtility.ToJson(this);
            if (Owner != null && Owner.ContextNode != null && Owner.ContextNode is NodeEditor)
            {
                Owner.ContextNode.SetDirty();
            }
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

        public NodeDetailEditor()
        {
            _nodeGuid = System.Guid.NewGuid().ToString();
            ConstructNode();
        }

        public abstract void ConstructNode();
                       
        public string NodeType()
        {
            var attrs = GetType().GetCustomAttributes(typeof(NodeDetailEditorType), false) as NodeDetailEditorType[];
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
    }
}