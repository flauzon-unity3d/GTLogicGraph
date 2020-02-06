﻿using GeoTetra.GTLogicGraph.Slots;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Signal", "Band Pass Filter")]
    [NodeDetailEditorType(typeof(BandPassFilterLogicNodeDetail))]
    public class BandPassFilterNodeDetailEditor : NodeDetailEditor
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
        private List<float> _curveValue;
        
        [NodeDetailCurveControl("Wavelength curve")]
        public CurveDetailData CurveValue
        {
            get { return new CurveDetailData(_curveValue); }
            set
            {
                if (_curveValue == value.data)
                    return;
                _curveValue = value.data;
                SetDirty();
            }
        }

        public override void ConstructNode()
        {
            DisplayName = "Band Pass Filter";

            AddSlot(new TriggerDetailPortDescription(this, "Trigger", "In", PortDetailDirection.Input));
            AddSlot(new TriggerDetailPortDescription(this, "Trigger", "Out", PortDetailDirection.Output));
        }
    }

    public class BandPassFilterLogicNodeDetail : LogicNode
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