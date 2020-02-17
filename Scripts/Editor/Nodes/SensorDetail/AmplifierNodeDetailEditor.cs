using System;
using UnityEngine;

namespace SensorFoundation.SensorGraph
{
    [Title("Signal", "Amplifier")]
    public class AmplifierNodeDetailEditor : NodeDetailEditor
    {
        [SerializeField]
        private FieldFloat _in;
        [SerializeField]
        private FieldFloat _out;
       
        public override void ConstructNode()
        {
            DisplayName = "Amplifier";

            _in = new FieldFloat(this);
            _out = new FieldFloat(this);

            AddVarSlot("In", PortDirection.Input, _in);
            AddVarSlot("Out", PortDirection.Output, _out);
        }
    }
}