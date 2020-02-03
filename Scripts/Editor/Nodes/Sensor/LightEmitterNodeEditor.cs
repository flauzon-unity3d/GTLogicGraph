using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Photon", "Light Emitter")]
    [NodeEditorType(typeof(LightEmitterLogicNode))]
    public class LightEmitterNodeEditor : NodeEditor
    {
        [SerializeField]
        private float _value;
       
        public override void ConstructNode()
        {
            DisplayName = "Light Emitter";

            AddSlot(new TransformPortDescription(this, "Transform", "World", PortDirection.Input));
            AddSlot(new PhotonPortDescription(this, "Photon", "Ray", PortDirection.Output));
        }
    }

    public class LightEmitterLogicNode : LogicNode
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