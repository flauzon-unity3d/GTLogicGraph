using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    /// <summary>
    /// Describes how to draw a node, paired with GenericNodeview
    /// </summary>
    [Serializable]
    public abstract class NodeDetailEditor
    {
        [NonSerialized] private List<PortDetailDescription> _portDescriptions = new List<PortDetailDescription>();

        [SerializeField] private string _displayName;
        
        [SerializeField] private Vector3 _position;

        [SerializeField] private bool _expanded = true;

        [SerializeField] private string _nodeGuid;

        public LogicGraphView Owner { get; set; }
        public LogicDetailGraphView DetailView { get; set; }
        public SerializedNode SerializedNode { get; set; }

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

        public void GetInputSlots<T>(List<T> foundSlots) where T : PortDetailDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isInputSlot && slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void GetOutputSlots<T>(List<T> foundSlots) where T : PortDetailDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isOutputSlot && slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void GetSlots<T>(List<T> foundSlots) where T : PortDetailDescription
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

        public void AddSlot(PortDetailDescription portDescription)
        {
            if (!(portDescription is PortDetailDescription))
                throw new ArgumentException(string.Format(
                    "Trying to add slot {0} to Material node {1}, but it is not a {2}", portDescription, this,
                    typeof(PortDetailDescription)));

            _portDescriptions.Add(portDescription);
        }

        public T FindSlot<T>(string memberName) where T : PortDetailDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.MemberName == memberName && slot is T)
                    return (T) slot;
            }

            return default(T);
        }

        public T FindInputSlot<T>(string memberName) where T : PortDetailDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isInputSlot && slot.MemberName == memberName && slot is T)
                    return (T) slot;
            }

            return default(T);
        }

        public T FindOutputSlot<T>(string memberName) where T : PortDetailDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isOutputSlot && slot.MemberName == memberName && slot is T)
                    return (T) slot;
            }

            return default(T);
        }
    }
}