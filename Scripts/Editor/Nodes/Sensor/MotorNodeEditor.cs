using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Actuator", "Motor")]
    [NodeEditorType(typeof(MotorLogicNode))]
    public class MotorNodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldFloat _power;
        [SerializeField]
        private FieldFloat _desiredRPM;
        [SerializeField]
        private FieldVector3 _positionWorld;
        [SerializeField]
        private FieldQuaternion _rotationWorld;
        [NonSerialized]
        private FieldQuaternion _outRotationWorld;

        public override void ConstructNode()
        {
            DisplayName = "Motor";

            _power = new FieldFloat(this);
            _desiredRPM = new FieldFloat(this);
            _positionWorld = new FieldVector3(this);
            _rotationWorld = new FieldQuaternion(this);
            _outRotationWorld = new FieldQuaternion(this);

            AddVarSlot("Power", PortDirection.Input, _power);
            AddVarSlot("Desired RPM", PortDirection.Input, _desiredRPM);
            AddVarSlot("World Position", PortDirection.Input, _positionWorld);
            AddVarSlot("World Rotation", PortDirection.Input, _rotationWorld);
            AddVarSlot("World Rotation", PortDirection.Output, _outRotationWorld);
        }
    }

    public class MotorLogicNode : LogicNode
    {
        [NodePort]
        public event Action<float> Vector1Output;

        [Vector1Input]
        public void TestInput(float value)
        {
            if (Vector1Output != null) Vector1Output(value);
        }
    }
}