using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Power", "Power Supply")]
    [NodeEditorType(typeof(PowerSupplyLogicNode))]
    public class PowerSupplyNodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldFloat _voltage;
        [SerializeField]
        private FieldFloat _current;
        [NonSerialized]
        private FieldFloat _power;

        public override void ConstructNode()
        {
            DisplayName = "Power Supply";

            _voltage = new FieldFloat(this);
            _current = new FieldFloat(this);
            _power = new FieldFloat(this);

            AddVarSlot("Voltage", PortDirection.Input, _voltage);
            AddVarSlot("Current", PortDirection.Input, _current);
            AddVarSlot("Power", PortDirection.Output, _power);
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