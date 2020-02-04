using System;

namespace GeoTetra.GTLogicGraph.Slots
{
    [Serializable]
    public class TransformPortDescription : PortDescription
    {
        public override PortValueType ValueType
        {
            get { return PortValueType.Transform; }
        }

        public TransformPortDescription(NodeEditor owner, string memberName, string displayName, PortDirection portDirection) 
            : base(owner, memberName, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.Transform;
        }
    }
}
