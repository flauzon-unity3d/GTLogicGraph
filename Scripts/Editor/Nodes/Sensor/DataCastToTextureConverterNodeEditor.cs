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
        private float _dataArray;
        [NonSerialized]
        private float _texture;
       
        public override void ConstructNode()
        {
            DisplayName = "Data Cast To Texture";

            AddVarSlot("From", PortDirection.Input, _dataArray);
            AddVarSlot("To", PortDirection.Output, _texture);
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