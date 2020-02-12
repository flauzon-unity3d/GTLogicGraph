using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Actuator", "Servo Motor")]
    [NodeEditorType(typeof(ServoMotorLogicNode))]
    public class ServoMotorNodeEditor : NodeEditor
    {
        [SerializeField]
        private float _power;
        [SerializeField]
        private float _desiredAngle;
        [SerializeField]
        private Vector3 _positionWorld;
        [SerializeField]
        private Quaternion _rotationWorld;
        [NonSerialized]
        private Quaternion _outRotationWorld;

        public override void ConstructNode()
        {
            DisplayName = "Servo Motor";

            AddVarSlot("Power", PortDirection.Input, _power);
            AddVarSlot("Desired Angle", PortDirection.Input, _desiredAngle);
            AddVarSlot("World Position", PortDirection.Input, _positionWorld);
            AddVarSlot("World Rotation", PortDirection.Input, _rotationWorld);
            AddVarSlot("World Rotation", PortDirection.Output, _outRotationWorld);
        }
    }

    public class ServoMotorLogicNode : LogicNode
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