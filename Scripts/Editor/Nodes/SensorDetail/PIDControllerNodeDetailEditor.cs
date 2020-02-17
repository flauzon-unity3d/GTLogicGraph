using System;
using UnityEngine;

namespace SensorFoundation.SensorGraph
{
    [Title("Control", "PID Controller")]
    public class PIDControllerNodeDetailEditor : NodeDetailEditor
    {
        [SerializeField]
        private FieldFloat _in;
        [SerializeField]
        private FieldFloat _out; 

        public override void ConstructNode()
        {
            DisplayName = "PID Controller";

            _in = new FieldFloat(this);
            _out = new FieldFloat(this);

            AddVarSlot("In", PortDirection.Input, _in);
            AddVarSlot("Out", PortDirection.Output, _out);
        }
    }
}