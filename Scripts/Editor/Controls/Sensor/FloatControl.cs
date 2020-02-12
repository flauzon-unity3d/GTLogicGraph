using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    [Serializable]
    public struct FloatData
    {
        public float data;

        public FloatData(float inData)
        {
            data = inData;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class FloatControlAttribute : Attribute, INodeControlAttribute
    {
        string _label;

        public FloatControlAttribute(string label = null)
        {
            _label = label;
        }

        public VisualElement InstantiateControl(NodeEditor nodeEditor, PropertyInfo propertyInfo)
        {
            return new FloatControlView(_label, nodeEditor, propertyInfo);
        }
    }

    public class FloatControlView : VisualElement
    {
        private NodeEditor _nodeEditor;
        private PropertyInfo _propertyInfo;
        private TextField _label;

        public FloatControlView(string label, NodeEditor nodeEditor, PropertyInfo propertyInfo)
        {
            _nodeEditor = nodeEditor;
            _propertyInfo = propertyInfo;
            GraphLogicEditor.AddStyleSheetPath(this, "Styles/Controls/FloatControlView");

            if (propertyInfo.PropertyType != typeof(FloatData))
                throw new ArgumentException("Property must be a Label.", "propertyInfo");

            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);

            var value = (FloatData)_propertyInfo.GetValue(_nodeEditor, null);
            var panel = new VisualElement { name = "floatPanel" };
            if (!string.IsNullOrEmpty(label))
                panel.Add(new Label(label));
            
            _label = new TextField();            
            _label.RegisterValueChangedCallback<string>(OnChangeField);
            panel.Add(_label);
            Add(panel);
        }
        
        void OnChangeField(ChangeEvent<string> e)
        {            
            _nodeEditor.Owner.LogicGraphEditorObject.RegisterCompleteObjectUndo("Field Change");
            var value = (FloatData)_propertyInfo.GetValue(_nodeEditor, null);
            value.data = (float)Convert.ToDouble(e.newValue); 
            _propertyInfo.SetValue(_nodeEditor, value, null);
            this.MarkDirtyRepaint();
        }
    }
}
