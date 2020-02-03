using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Data", "Data Cast To ComputeBuffer")]
    [NodeEditorType(typeof(DataCastToComputeBufferLogicNode))]
    public class DataCastToComputeBufferNodeEditor : NodeEditor
    {
        [SerializeField]
        private float _value;
       
        public override void ConstructNode()
        {
            DisplayName = "Data Cast To ComputeBuffer";
            
            AddSlot(new DataArrayPortDescription(this, "DataArray", "From", PortDirection.Input));
            AddSlot(new ComputeBufferPortDescription(this, "ComputeBuffer", "To", PortDirection.Output));
        }
    }

    public class DataCastToComputeBufferLogicNode : LogicNode
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