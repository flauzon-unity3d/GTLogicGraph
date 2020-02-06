using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    [Serializable]
    public struct ToggleDetailData
    {
        public bool isOn;

        public ToggleDetailData(bool on)
        {
            isOn = on;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NodeDetailToggleControlAttribute : Attribute, INodeDetailControlAttribute
    {
        string _label;

        public NodeDetailToggleControlAttribute(string label = null)
        {
            _label = label;
        }

        public VisualElement InstantiateControl(NodeDetailEditor nodeEditor, PropertyInfo propertyInfo)
        {
            return new ToggleDetailControlView(_label, nodeEditor, propertyInfo);
        }
    }

    public class ToggleDetailControlView : VisualElement
    {
        private NodeDetailEditor _nodeEditor;
        private PropertyInfo _propertyInfo;
        private Toggle _toggle;



        public ToggleDetailControlView(string label, NodeDetailEditor nodeEditor, PropertyInfo propertyInfo)
        {
            _nodeEditor = nodeEditor;
            _propertyInfo = propertyInfo;
            GraphLogicEditor.AddStyleSheetPath(this, "Styles/Controls/ToggleControlView");

            if (propertyInfo.PropertyType != typeof(ToggleData))
                throw new ArgumentException("Property must be a Toggle.", "propertyInfo");

            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);

            var value = (ToggleData)_propertyInfo.GetValue(_nodeEditor, null);
            var panel = new VisualElement { name = "togglePanel" };
            if (!string.IsNullOrEmpty(label))
                panel.Add(new Label(label));
            
            _toggle = new Toggle();
            _toggle.RegisterCallback<ChangeEvent<bool>>(OnChangeToggle);
            _toggle.value = value.isOn;

            panel.Add(_toggle);            
            Add(panel);
        }

        void OnChangeToggle(ChangeEvent<bool> e)
        {            
            _nodeEditor.Owner.LogicGraphEditorObject.RegisterCompleteObjectUndo("Toggle Change");
            var value = (ToggleData)_propertyInfo.GetValue(_nodeEditor, null);
            value.isOn = e.newValue; // !value.isOn;
            _propertyInfo.SetValue(_nodeEditor, value, null);
            this.MarkDirtyRepaint();
        }
    }
}
