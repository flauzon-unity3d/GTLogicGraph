using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Photo Detector", "CCD")]
    public class CCDNodeDetailEditor : NodeDetailEditor
    {
        [SerializeField]
        private FieldFloat _in;
        [SerializeField]
        private FieldFloat _out;

        public override void ConstructNode()
        {
            DisplayName = "CCD";

            _in = new FieldFloat(this);
            _out = new FieldFloat(this);

            AddVarSlot("In", PortDirection.Input, _in);
            AddVarSlot("Out", PortDirection.Output, _out);
        }
    }
}