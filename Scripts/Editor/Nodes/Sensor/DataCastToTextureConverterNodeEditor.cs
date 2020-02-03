using GeoTetra.GTLogicGraph.Slots;
using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Data", "Data Cast To Texture")]
    [NodeEditorType(typeof(DataCastToTextureLogicNode))]
    public class DataCastToTextureNodeEditor : NodeEditor
    {
        [SerializeField]
        private float _value;
       
        public override void ConstructNode()
        {
            DisplayName = "Data Cast To Texture";
            
            AddSlot(new DataArrayPortDescription(this, "DataArray", "From", PortDirection.Input));
            AddSlot(new TexturePortDescription(this, "Texture", "To", PortDirection.Output));
        }
    }

    public class DataCastToTextureLogicNode : LogicNode
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