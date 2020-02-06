using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Control", "PID Controller")]
    [NodeDetailEditorType(typeof(PIDControllerLogicNodeDetail))]
    public class PIDControllerNodeDetailEditor : NodeDetailEditor
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
        private double _gainP;
        [SerializeField]
        private double _gainI;
        [SerializeField]
        private double _gainD;

        [LabelDetailControl("Gain P")]
        public LabelDetailData PValue
        {
            get { return new LabelDetailData(_gainP.ToString()); }
            set
            {
                _gainP = Convert.ToDouble(value.data);
                SetDirty();
            }
        }

        [LabelDetailControl("Gain I")]
        public LabelDetailData IValue
        {
            get { return new LabelDetailData(_gainI.ToString()); }
            set
            {
                _gainI = Convert.ToDouble(value.data);
                SetDirty();
            }
        }

        [LabelDetailControl("Gain D")]
        public LabelDetailData DValue
        {
            get { return new LabelDetailData(_gainD.ToString()); }
            set
            {
                _gainD = Convert.ToDouble(value.data);
                SetDirty();
            }
        }

        public override void ConstructNode()
        {
            DisplayName = "PID Controller";

            AddSlot(new TriggerDetailPortDescription(this, "Trigger", "In", PortDetailDirection.Input));
            AddSlot(new TriggerDetailPortDescription(this, "Trigger", "Out", PortDetailDirection.Output));
        }
    }

    public class PIDControllerLogicNodeDetail : LogicNode
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