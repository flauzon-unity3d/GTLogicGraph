using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Photo Detector", "CCD")]
    [NodeDetailEditorType(typeof(CCDLogicNodeDetail))]
    public class CCDNodeDetailEditor : NodeDetailEditor
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
            DisplayName = "CCD";

            AddSlot(new TriggerDetailPortDescription(this, "Trigger", "Out", PortDetailDirection.Output));
        }
    }

    public class CCDLogicNodeDetail : LogicNode
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