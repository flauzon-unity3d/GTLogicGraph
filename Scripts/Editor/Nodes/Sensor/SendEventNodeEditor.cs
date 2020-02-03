using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Event", "Send Event")]
    [NodeEditorType(typeof(SendEventLogicNode))]
    public class SendEventNodeEditor : NodeEditor
    {
        [SerializeField]
        private float _value;
       
        public override void ConstructNode()
        {
            DisplayName = "Send Event";

            AddSlot(new TriggerPortDescription(this, "Trigger", "Input", PortDirection.Input));
            AddSlot(new GameObjectPortDescription(this, "GameObject", "Receiver", PortDirection.Output));            
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