using System;

namespace GeoTetra.GTLogicGraph.Slots
{
    [Serializable]
    public class DataArrayPortDescription : PortDescription
    {
        public override PortValueType ValueType
        {
            get { return PortValueType.DataArray; }
        }

        public DataArrayPortDescription(NodeEditor owner, string memberName, string displayName, PortDirection portDirection) 
            : base(owner, memberName, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.DataArray;
        }
    }
}
