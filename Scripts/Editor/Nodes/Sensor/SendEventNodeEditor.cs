using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Event", "Send Event")]
    [NodeEditorType(typeof(SendEventLogicNode))]
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

    public class SendEventLogicNode : LogicNode
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