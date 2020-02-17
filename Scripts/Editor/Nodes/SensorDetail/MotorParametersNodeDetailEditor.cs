using System;
using UnityEngine;

namespace SensorFoundation.SensorGraph
{
    [Title("Actuator", "Motor Parameters")]
    public class MotorParametersNodeDetailEditor : NodeDetailEditor
    {
        [SerializeField]
        private FieldFloat _in;
        [SerializeField]
        private FieldFloat _out;  

        public override void ConstructNode()
        {
            DisplayName = "Motor Parameters";

            _in = new FieldFloat(this);
            _out = new FieldFloat(this);

            AddVarSlot("In", PortDirection.Input, _in);
            AddVarSlot("Out", PortDirection.Output, _out);
        }
    }
}