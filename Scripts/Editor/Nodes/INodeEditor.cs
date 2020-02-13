using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    /// <summary>
    /// Describes how to draw a node, paired with GenericNodeview
    /// </summary>
    [Serializable]
    public abstract class INodeEditor
    {
        [NonSerialized] 
        protected List<IPortDescription> _portDescriptions = new List<IPortDescription>();
        [SerializeField] 
        protected string _nodeGuid;

        public string NodeGuid
        {
            get { return _nodeGuid; }
        }

        public SerializedNode SerializedNode { get; set; }
                       
        public virtual void SetDirty()
        {
            SerializedNode.JSON = JsonUtility.ToJson(this);
        }
        
        public void GetInputSlots<T>(List<T> foundSlots) where T : IPortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.IsInputSlot && slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void GetOutputSlots<T>(List<T> foundSlots) where T : IPortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.IsOutputSlot && slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void GetSlots<T>(List<T> foundSlots) where T : IPortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void AddVarSlot<T>(string name, PortDirection portDirection, T refData)
        {
            if (refData == null)
            {                
                Debug.LogError("[AddVarSlot] refData is null.");
                return;
            }

            AddSlot(new PortDescription<T>(this, typeof(T).Name, name, refData, portDirection));
        }

        public void AddSlot(IPortDescription portDescription)
        {
            if (!(portDescription is IPortDescription))
            {
                throw new ArgumentException(string.Format("Trying to add slot {0} to Material node {1}, but it is not a {2}", portDescription, this,
                    typeof(IPortDescription)));
            }
            _portDescriptions.Add(portDescription);
        }

        public T FindSlot<T>(string memberName) where T : IPortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.MemberName == memberName && slot is T)
                {
                    return (T)slot;
                }
            }

            return default(T);
        }

        public T FindInputSlot<T>(string memberName) where T : IPortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.IsInputSlot && slot.MemberName == memberName && slot is T)
                {
                    return (T)slot;
                }
            }

            return default(T);
        }

        public T FindOutputSlot<T>(string memberName) where T : IPortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.IsOutputSlot && slot.MemberName == memberName && slot is T)
                    return (T) slot;
            }

            return default(T);
        }
    }
}