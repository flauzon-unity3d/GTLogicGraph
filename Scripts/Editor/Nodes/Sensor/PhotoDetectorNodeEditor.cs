using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Photon", "Photo Detector")]
    [NodeEditorType(typeof(PhotoDetectorLogicNode))]
    public class PhotoDetectorNodeEditor : NodeEditor
    {
        [Serializable]
        public class Parameters
        {
            [SerializeField]
            public float[] sensitivityCurveWavelength;
        };

        [SerializeField]
        private Parameters _params;
       
        public override void ConstructNode()
        {
            DisplayName = "Photo Detector";

            AddSlot(new PhotonPortDescription(this, "Photon", "Ray", PortDirection.Input));
            AddSlot(new TransformPortDescription(this, "Transform", "World", PortDirection.Input));            
            AddSlot(new PhotonPortDescription(this, "Photon", "Ray", PortDirection.Output));
            AddSlot(new PixelArrayPortDescription(this, "PixelArray", "Intensity", PortDirection.Output));            
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