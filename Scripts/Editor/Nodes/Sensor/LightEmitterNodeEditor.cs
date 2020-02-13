using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Photon", "Light Emitter")]
    [NodeEditorType(typeof(LightEmitterLogicNode))]
    public class LightEmitterNodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldVector3 _WorldPosition;
        [SerializeField]
        private FieldQuaternion _WorldRotation;
        [SerializeField]        
        private FieldFloat _XChannels;
        [SerializeField]
        private FieldFloat _YChannels;
        [SerializeField]
        private FieldBool _BatchMode;
        [SerializeField]
        private FieldFloat _Rate;
        [NonSerialized]
        private FieldFloat _Ray;

        public override void ConstructNode()
        {
            DisplayName = "Light Emitter";

            _WorldPosition = new FieldVector3(this);
            _WorldRotation = new FieldQuaternion(this);
            _XChannels = new FieldFloat(this);
            _YChannels = new FieldFloat(this);
            _BatchMode = new FieldBool(this);
            _Rate = new FieldFloat(this);
            _Ray = new FieldFloat(this);

            AddVarSlot("World Position", PortDirection.Input, _WorldPosition);
            AddVarSlot("World Rotation", PortDirection.Input, _WorldRotation);
            AddVarSlot("Batch Mode", PortDirection.Input, _BatchMode);
            AddVarSlot("X Channels", PortDirection.Input, _XChannels);
            AddVarSlot("Y Channels", PortDirection.Input, _YChannels);
            AddVarSlot("Rate", PortDirection.Input, _Rate);
            AddVarSlot("Ray", PortDirection.Output, _Ray);
        }
    }

    public class LightEmitterLogicNode : LogicNode
    {       
    }
}