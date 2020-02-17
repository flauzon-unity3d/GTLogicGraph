using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Photo Detector", "Pixel Array")]
    public class PixelArrayNodeDetailEditor : NodeDetailEditor
    {
        [SerializeField]
        private FieldFloat _in;
        [SerializeField]
        private FieldFloat _out;  
       
        public override void ConstructNode()
        {
            DisplayName = "Pixel Array";

            _in = new FieldFloat(this);
            _out = new FieldFloat(this);

            AddVarSlot("In", PortDirection.Input, _in);
            AddVarSlot("Out", PortDirection.Output, _out);
        }
    }
}