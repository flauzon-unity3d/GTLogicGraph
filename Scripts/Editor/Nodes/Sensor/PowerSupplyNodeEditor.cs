using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Power", "Power Supply")]
    [NodeEditorType(typeof(PowerSupplyLogicNode))]
    public class PowerSupplyNodeEditor : NodeEditor
    {
        [SerializeField]
        private float _value;
       
        public override void ConstructNode()
        {
            DisplayName = "Power Supply";
            
            AddSlot(new FloatPortDescription(this, "float", "Voltage", PortDirection.Input));
            AddSlot(new FloatPortDescription(this, "float", "MaxCurrent", PortDirection.Input));
            AddSlot(new TriggerPortDescription(this, "Trigger", "Power", PortDirection.Output));            
        }
    }

    public class PowerSupplyLogicNode : LogicNode
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