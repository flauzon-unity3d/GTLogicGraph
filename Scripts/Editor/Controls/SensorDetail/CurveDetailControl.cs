using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    [Serializable]
    public struct CurveDetailData
    {
        public List<float> data;

        public CurveDetailData(List<float> inData)
        {
            data = inData;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NodeDetailCurveControlAttribute : Attribute, INodeDetailControlAttribute
    {
        string _label;

        public NodeDetailCurveControlAttribute(string label = null)
        {
            _label = label;
        }

        public VisualElement InstantiateControl(NodeDetailEditor nodeEditor, PropertyInfo propertyInfo)
        {
            return new CurveDetailControlView(_label, nodeEditor, propertyInfo);
        }
    }

    public class CurveDetailControlView : VisualElement
    {
        private NodeDetailEditor _nodeEditor;
        private PropertyInfo _propertyInfo;
        private CurveField _curve;

        public CurveDetailControlView(string label, NodeDetailEditor nodeEditor, PropertyInfo propertyInfo)
        {
            _nodeEditor = nodeEditor;
            _propertyInfo = propertyInfo;
            GraphLogicEditor.AddStyleSheetPath(this, "Styles/Controls/ToggleControlView");

            if (propertyInfo.PropertyType != typeof(CurveDetailData))
                throw new ArgumentException("Property must be a Curve Field.", "propertyInfo");

            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);

            var value = (CurveDetailData)_propertyInfo.GetValue(_nodeEditor, null);
            var panel = new VisualElement { name = "curveFieldPanel" };
            if (!string.IsNullOrEmpty(label))
                panel.Add(new Label(label));

            _curve = new CurveField();
            
          

            //***
            panel.Add(_curve);            
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
