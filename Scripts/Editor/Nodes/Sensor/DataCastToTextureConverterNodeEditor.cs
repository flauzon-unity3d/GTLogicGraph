using System;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Data", "Data Cast To Texture")]
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
}