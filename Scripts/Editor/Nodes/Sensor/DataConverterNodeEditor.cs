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
        private GameObject _ray; //*** Eventually Photon
        [SerializeField]
        private GameObject _intensity; //*** Eventually pixelArray
        [SerializeField]
        private Vector3 _positionWorld;
        [SerializeField]
        private Quaternion _rotationWorld;
        [SerializeField]
        private GameObject _converted; //*** Eventually data array

        public override void ConstructNode()
        {
            DisplayName = "Ray Converter";

            AddVarSlot("Ray", PortDirection.Input, _ray);
            AddVarSlot("Intensity", PortDirection.Input, _intensity);
            AddVarSlot("World Position", PortDirection.Input, _positionWorld);
            AddVarSlot("World Rotation", PortDirection.Input, _rotationWorld);
            AddVarSlot("Converted", PortDirection.Output, _converted);
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