using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Photon", "Photo Detector")]
    [NodeEditorType(typeof(PhotoDetectorLogicNode))]
    public class PhotoDetectorNodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldFloat _ray;
        [SerializeField]
        private FieldVector3 _positionWorld;
        [SerializeField]
        private FieldQuaternion _rotationWorld;
        [NonSerialized]
        private FieldFloat _outRay;
        [NonSerialized]
        private FieldFloat _outIntensity;

        public override void ConstructNode()
        {
            DisplayName = "Photo Detector";

            _ray = new FieldFloat(this);
            _positionWorld = new FieldVector3(this);
            _rotationWorld = new FieldQuaternion(this);
            _outRay = new FieldFloat(this);
            _outIntensity = new FieldFloat(this);

            AddVarSlot("Ray", PortDirection.Input, _ray);
            AddVarSlot("World Position", PortDirection.Input, _positionWorld);
            AddVarSlot("World Rotation", PortDirection.Input, _rotationWorld);
            AddVarSlot("Ray", PortDirection.Output, _outRay);
            AddVarSlot("Intensity", PortDirection.Output, _outIntensity);
        }
    }

    public class PhotoDetectorLogicNode : LogicNode
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