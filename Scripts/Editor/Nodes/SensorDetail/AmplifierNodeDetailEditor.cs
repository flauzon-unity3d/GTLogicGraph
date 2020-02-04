using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Signal", "Amplifier")]
    [NodeDetailEditorType(typeof(AmplifierLogicNodeDetail))]
    public class AmplifierNodeDetailEditor : NodeDetailEditor
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
            DisplayName = "Amplifier";

            AddSlot(new TriggerDetailPortDescription(this, "Trigger", "In", PortDetailDirection.Input));
            AddSlot(new TriggerDetailPortDescription(this, "Trigger", "Out", PortDetailDirection.Output));
        }
    }

    public class AmplifierLogicNodeDetail : LogicNode
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