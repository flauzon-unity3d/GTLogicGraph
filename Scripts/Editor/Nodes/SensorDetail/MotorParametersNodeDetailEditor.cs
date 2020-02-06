using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Actuator", "Motor Parameters")]
    [NodeDetailEditorType(typeof(MotorParametersLogicNodeDetail))]
    public class MotorParametersNodeDetailEditor : NodeDetailEditor
    {
        [Serializable]
        public class Parameters
        {
            [SerializeField]
            public float[] sensitivityCurveWavelength;
        };

        [SerializeField]
        private Parameters _params;

        [SerializeField]
        private double _kM;

        [LabelDetailControl("kM")]
        public LabelDetailData PValue
        {
            get { return new LabelDetailData(_kM.ToString()); }
            set
            {
                _kM = Convert.ToDouble(value.data);
                SetDirty();
            }
        }

        public override void ConstructNode()
        {
            DisplayName = "Motor Parameters";

            AddSlot(new TriggerDetailPortDescription(this, "Trigger", "In", PortDetailDirection.Input));
            AddSlot(new TriggerDetailPortDescription(this, "Trigger", "Out", PortDetailDirection.Output));
        }
    }

    public class MotorParametersLogicNodeDetail : LogicNode
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