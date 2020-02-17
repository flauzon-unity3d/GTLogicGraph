using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Data", "Ray Converter")]
    public class DataConverterNodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldFloat _ray; //*** Eventually Photon
        [SerializeField]
        private FieldFloat _intensity; //*** Eventually pixelArray
        [SerializeField]
        private FieldVector3 _positionWorld;
        [SerializeField]
        private FieldQuaternion _rotationWorld;
        [SerializeField]
        private FieldFloat _converted; //*** Eventually data array

        public override void ConstructNode()
        {
            DisplayName = "Ray Converter";

            _ray = new FieldFloat(this);
            _intensity = new FieldFloat(this);
            _positionWorld = new FieldVector3(this);
            _rotationWorld = new FieldQuaternion(this);
            _converted = new FieldFloat(this);

            AddVarSlot("Ray", PortDirection.Input, _ray);
            AddVarSlot("Intensity", PortDirection.Input, _intensity);
            AddVarSlot("World Position", PortDirection.Input, _positionWorld);
            AddVarSlot("World Rotation", PortDirection.Input, _rotationWorld);
            AddVarSlot("Converted", PortDirection.Output, _converted);
        }
    }
}