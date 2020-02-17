using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Signal", "Band Pass Filter")]
    public class BandPassFilterNodeDetailEditor : NodeDetailEditor
    {
        [SerializeField]
        private FieldFloat _in;
        [SerializeField]
        private FieldFloat _out;

        public override void ConstructNode()
        {
            DisplayName = "Band Pass Filter";

            _in = new FieldFloat(this);
            _out = new FieldFloat(this);

            AddVarSlot("In", PortDirection.Input, _in);
            AddVarSlot("Out", PortDirection.Output, _out);
        }
    }
}