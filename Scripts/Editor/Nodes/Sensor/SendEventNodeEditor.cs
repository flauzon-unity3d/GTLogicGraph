using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Event", "Send Event")]
    public class SendEventNodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldFloat _trigger;
        [NonSerialized]
        private FieldFloat _receiver;

        public override void ConstructNode()
        {
            DisplayName = "Send Event";

            _trigger = new FieldFloat(this);
            _receiver = new FieldFloat(this);

            AddVarSlot("Input", PortDirection.Input, _trigger);
            AddVarSlot("Receiver", PortDirection.Output, _receiver);
        }
    }
}