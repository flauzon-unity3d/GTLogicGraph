using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Data", "Data Cast To ComputeBuffer")]
    public class DataCastToComputeBufferNodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldFloat _dataArray; //*** Eventually use DataArray object
        [NonSerialized]
        private FieldFloat _computeBuffer;
       
        public override void ConstructNode()
        {
            DisplayName = "Data Cast To ComputeBuffer";

            _dataArray = new FieldFloat(this);
            _computeBuffer = new FieldFloat(this);

            AddVarSlot("From", PortDirection.Input, _dataArray);
            AddVarSlot("To", PortDirection.Output, _computeBuffer);
        }
    }
}