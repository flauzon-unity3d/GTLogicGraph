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
        private float _value;
       
        public override void ConstructNode()
        {
            DisplayName = "Servo Motor";

            AddSlot(new TriggerPortDescription(this, "Trigger", "Power", PortDirection.Input));
            AddSlot(new FloatPortDescription(this, "Float", "Angle Request", PortDirection.Input));
            AddSlot(new TransformPortDescription(this, "Transform", "World", PortDirection.Input));            
            AddSlot(new TransformPortDescription(this, "Transform", "World", PortDirection.Output));            
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