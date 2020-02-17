using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Signal", "Efficiency")]
    public class EfficiencyNodeDetailEditor : NodeDetailEditor
    {
        [SerializeField]
        private FieldFloat _in;
        [SerializeField]
        private FieldFloat _out;
       
        public override void ConstructNode()
        {
            DisplayName = "Efficiency";

            _in = new FieldFloat(this);
            _out = new FieldFloat(this);

            AddVarSlot("In", PortDirection.Input, _in);
            AddVarSlot("Out", PortDirection.Output, _out);
        }
    }
}