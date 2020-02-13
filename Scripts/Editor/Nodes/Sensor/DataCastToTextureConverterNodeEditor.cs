using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Data", "Data Cast To Texture")]
    [NodeEditorType(typeof(DataCastToTextureLogicNode))]
    public class DataCastToTextureNodeEditor : NodeEditor
    {        
        [SerializeField]
        private FieldFloat _dataArray;
        [NonSerialized]
        private FieldFloat _texture;
       
        public override void ConstructNode()
        {
            DisplayName = "Data Cast To Texture";

            _dataArray = new FieldFloat(this);
            _texture = new FieldFloat(this);

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