using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Photo Detector", "Pixel Array")]
    [NodeDetailEditorType(typeof(PixelArrayLogicNodeDetail))]
    public class PixelArrayNodeDetailEditor : NodeDetailEditor
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
            DisplayName = "Pixel Array";

            AddSlot(new TriggerDetailPortDescription(this, "Trigger", "Out", PortDetailDirection.Input));            
        }
    }

    public class PixelArrayLogicNodeDetail : LogicNode
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