using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Data", "Ray Converter")]
    [NodeEditorType(typeof(DataConverterLogicNode))]
    public class DataConverterNodeEditor : NodeEditor
    {
        [SerializeField]
        private float _value;
       
        public override void ConstructNode()
        {
            DisplayName = "Ray Converter";
            
            AddSlot(new PhotonPortDescription(this, "Photon", "Ray", PortDirection.Input));
            AddSlot(new PixelArrayPortDescription(this, "PixelArray", "Intensity", PortDirection.Input));
            AddSlot(new TransformPortDescription(this, "Transform", "World", PortDirection.Input));            
            AddSlot(new DataArrayPortDescription(this, "DataArray", "Converted", PortDirection.Output));
        }
    }

    public class DataConverterLogicNode : LogicNode
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