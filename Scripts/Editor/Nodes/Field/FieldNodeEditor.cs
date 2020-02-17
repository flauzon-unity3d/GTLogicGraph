using System;
using UnityEngine;

namespace SensorFoundation.SensorGraph
{
    [Title("Type", "Field Bool")]
    public class FieldBoolNodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldBool _in;
        [SerializeField]        
        private FieldBool _out;

        public override void ConstructNode()
        {
            DisplayName = "Bool";

            _in = new FieldBool(this);
            _out = new FieldBool(this);

            AddVarSlot("Set", PortDirection.Input, _in);
            AddVarSlot("Data", PortDirection.Output, _out);
        }
    }
    
    [Title("Type", "Field Float")]    
    public class FieldFloatNodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldFloat _in;
        [SerializeField]
        private FieldFloat _out;

        public override void ConstructNode()
        {
            DisplayName = "Float";

            _in = new FieldFloat(this);
            _out = new FieldFloat(this);

            AddVarSlot("Set", PortDirection.Input, _in);
            AddVarSlot("Data", PortDirection.Output, _out);
        }
    }

    [Title("Type", "Field Vector2")]
    public class FieldVector2NodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldVector2 _in;
        [SerializeField]
        private FieldVector2 _out;

        public override void ConstructNode()
        {
            DisplayName = "Vector2";

            _in = new FieldVector2(this);
            _out = new FieldVector2(this);

            AddVarSlot("Set", PortDirection.Input, _in);
            AddVarSlot("Data", PortDirection.Output, _out);
        }
    }

    [Title("Type", "Field Vector3")]
    public class FieldVector3NodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldVector3 _in;
        [SerializeField]
        private FieldVector3 _out;

        public override void ConstructNode()
        {
            DisplayName = "Vector3";

            _in = new FieldVector3(this);
            _out = new FieldVector3(this);

            AddVarSlot("Set", PortDirection.Input, _in);
            AddVarSlot("Data", PortDirection.Output, _out);
        }
    }


    [Title("Type", "Field Vector4")]
    public class FieldVector4NodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldVector4 _in;
        [SerializeField]
        private FieldVector4 _out;

        public override void ConstructNode()
        {
            DisplayName = "Vector4";

            _in = new FieldVector4(this);
            _out = new FieldVector4(this);

            AddVarSlot("Set", PortDirection.Input, _in);
            AddVarSlot("Data", PortDirection.Output, _out);
        }
    }

    [Title("Type", "Field Quaternion")]
    public class FieldQuaternionNodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldQuaternion _in;
        [SerializeField]
        private FieldQuaternion _out;

        public override void ConstructNode()
        {
            DisplayName = "Quaternion";

            _in = new FieldQuaternion(this);
            _out = new FieldQuaternion(this);

            AddVarSlot("Set", PortDirection.Input, _in);
            AddVarSlot("Data", PortDirection.Output, _out);
        }
    }
}