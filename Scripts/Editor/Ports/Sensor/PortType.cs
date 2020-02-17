using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace SensorFoundation.SensorGraph
{
    [Serializable]
    public abstract class FieldType
    {
        private INodeEditor _editor;
        private PortView _portView;

        public INodeEditor Editor
        { 
            get { return _editor; }
            set { _editor = value; }
        }

        public FieldType(INodeEditor editor)
        {
            _editor = editor;
            _portView = null;
        }

        public void SetPortView(PortView portView)
        {
            _portView = portView;
        }

        public PortView GetPortView()
        {
            return _portView;
        }
    }

    [Serializable]
    public class FieldBool : FieldType
    {
        [SerializeField]
        private bool _data;
        public bool Data
        {
            set { _data = value; }
            get { return _data; }
        }

        public FieldBool(NodeEditor editor) : base(editor)
        {
        }

        public void OnValueChanged(ChangeEvent<bool> e)
        {
            _data = e.newValue;
            Editor.SetDirty();
        }
    }

    [Serializable]
    public class FieldQuaternion : FieldType
    {
        [SerializeField]
        private Quaternion _data = new Quaternion();
        public Quaternion Data
        {
            set { _data = value; }
            get { return _data; }
        }

        public FieldQuaternion(INodeEditor editor) : base(editor)
        {
        }

        public void OnValueChangedX(ChangeEvent<string> e)
        {
            _data.eulerAngles = new Vector3(Convert.ToSingle(e.newValue), _data.eulerAngles.y, _data.eulerAngles.z);
            Editor.SetDirty();
        }

        public void OnValueChangedY(ChangeEvent<string> e)
        {
            _data.eulerAngles = new Vector3(_data.eulerAngles.x, Convert.ToSingle(e.newValue), _data.eulerAngles.z);
            Editor.SetDirty();
        }

        public void OnValueChangedZ(ChangeEvent<string> e)
        {
            _data.eulerAngles = new Vector3(_data.eulerAngles.x, _data.eulerAngles.y, Convert.ToSingle(e.newValue));
            Editor.SetDirty();
        }
    }

    [Serializable]
    public class FieldFloat : FieldType
    {
        [SerializeField]
        private float _data;
        public float Data
        {
            set { _data = value; }
            get { return _data; }
        }

        public FieldFloat(INodeEditor editor) : base(editor)
        {
        }

        public void OnValueChanged(ChangeEvent<string> e)
        {
            _data = Convert.ToSingle(e.newValue);
            Editor.SetDirty();
        }
    }

    [Serializable]
    public class FieldVector2 : FieldType
    {
        [SerializeField]
        private Vector2 _data = new Vector2();
        public Vector2 Data
        {
            set { _data = value; }
            get { return _data; }
        }

        public FieldVector2(INodeEditor editor) : base(editor)
        {
        }

        public void OnValueChangedX(ChangeEvent<string> e)
        {
            _data.x = Convert.ToSingle(e.newValue);
            Editor.SetDirty();
        }

        public void OnValueChangedY(ChangeEvent<string> e)
        {
            _data.y = Convert.ToSingle(e.newValue);
            Editor.SetDirty();
        }        
    }

    [Serializable]
    public class FieldVector3 : FieldType
    {
        [SerializeField]
        private Vector3 _data = new Vector3();
        public Vector3 Data
        {
            set { _data = value; }
            get { return _data; }
        }

        public FieldVector3(INodeEditor editor) : base(editor)
        {
        }

        public void OnValueChangedX(ChangeEvent<string> e)
        {
            _data.x = Convert.ToSingle(e.newValue);
            Editor.SetDirty();
        }

        public void OnValueChangedY(ChangeEvent<string> e)
        {
            _data.y = Convert.ToSingle(e.newValue);
            Editor.SetDirty();
        }

        public void OnValueChangedZ(ChangeEvent<string> e)
        {
            _data.z = Convert.ToSingle(e.newValue);
            Editor.SetDirty();
        }
    }

    [Serializable]
    public class FieldVector4 : FieldType
    {
        [SerializeField]
        private Vector4 _data = new Vector4();
        public Vector4 Data
        {
            set { _data = value; }
            get { return _data; }
        }

        public FieldVector4(INodeEditor editor) : base(editor)
        {
        }

        public void OnValueChangedX(ChangeEvent<string> e)
        {
            _data.x = Convert.ToSingle(e.newValue);
            Editor.SetDirty();
        }

        public void OnValueChangedY(ChangeEvent<string> e)
        {
            _data.y = Convert.ToSingle(e.newValue);
            Editor.SetDirty();
        }

        public void OnValueChangedZ(ChangeEvent<string> e)
        {
            _data.z = Convert.ToSingle(e.newValue);
            Editor.SetDirty();
        }

        public void OnValueChangedW(ChangeEvent<string> e)
        {
            _data.w = Convert.ToSingle(e.newValue);
            Editor.SetDirty();
        }
    }
}