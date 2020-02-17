using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Signal", "Loss")]
    public class LossNodeDetailEditor : NodeDetailEditor
    {
        [SerializeField]
        private FieldFloat _in;
        [SerializeField]
        private FieldFloat _out;
       
        public override void ConstructNode()
        {
            DisplayName = "Loss";

            _in = new FieldFloat(this);
            _out = new FieldFloat(this);

            AddVarSlot("In", PortDirection.Input, _in);
            AddVarSlot("Out", PortDirection.Output, _out);
        }
    }
}