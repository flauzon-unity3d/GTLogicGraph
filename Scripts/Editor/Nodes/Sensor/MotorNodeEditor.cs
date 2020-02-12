using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Actuator", "Motor")]
    [NodeEditorType(typeof(MotorLogicNode))]
    public class MotorNodeEditor : NodeEditor
    {
        [SerializeField]
        private float _power;
        [SerializeField]
        private float _desiredRPM;
        [SerializeField]
        private Vector3 _positionWorld = new Vector3();
        [SerializeField]
        private Quaternion _rotationWorld = new Quaternion();
        [NonSerialized]
        private Quaternion _outRotationWorld = new Quaternion();

        public override void ConstructNode()
        {
            DisplayName = "Motor";

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