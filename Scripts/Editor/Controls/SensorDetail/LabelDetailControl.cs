using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    [Serializable]
    public struct LabelDetailData
    {
        public string data;

        public LabelDetailData(string inData)
        {
            data = inData;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class LabelDetailControlAttribute : Attribute, INodeDetailControlAttribute
    {
        string _label;

        public LabelDetailControlAttribute(string label = null)
        {
            _label = label;
        }

        public VisualElement InstantiateControl(NodeDetailEditor nodeEditor, PropertyInfo propertyInfo)
        {
            return new LabelDetailControlView(_label, nodeEditor, propertyInfo);
        }
    }

    public class LabelDetailControlView : VisualElement
    {
        private NodeDetailEditor _nodeEditor;
        private PropertyInfo _propertyInfo;
        private TextField _label;

        public LabelDetailControlView(string label, NodeDetailEditor nodeEditor, PropertyInfo propertyInfo)
        {
            _nodeEditor = nodeEditor;
            _propertyInfo = propertyInfo;
            GraphLogicEditor.AddStyleSheetPath(this, "Styles/Controls/ToggleControlView");

            if (propertyInfo.PropertyType != typeof(LabelDetailData))
                throw new ArgumentException("Property must be a Label.", "propertyInfo");

            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);

            var value = (LabelDetailData)_propertyInfo.GetValue(_nodeEditor, null);
            var panel = new VisualElement { name = "labelPanel" };
            if (!string.IsNullOrEmpty(label))
                panel.Add(new Label(label));
            
            _label = new TextField();
            
            panel.Add(_label);
            Add(panel);
        }
        /*
        void OnChangeToggle(ChangeEvent<bool> e)
        {            
            _nodeEditor.Owner.LogicGraphEditorObject.RegisterCompleteObjectUndo("Toggle Change");
            var value = (ToggleData)_propertyInfo.GetValue(_nodeEditor, null);
            value.isOn = e.newValue; // !value.isOn;
            _propertyInfo.SetValue(_nodeEditor, value, null);
            this.MarkDirtyRepaint();
        }*/
    }
}
