using System;

namespace GeoTetra.GTLogicGraph.Slots
{
    [Serializable]
    public class TexturePortDescription : PortDescription
    {
        public override PortValueType ValueType
        {
            get { return PortValueType.Texture; }
        }

        public TexturePortDescription(NodeEditor owner, string memberName, string displayName, PortDirection portDirection) 
            : base(owner, memberName, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.Texture;
        }
    }
}
