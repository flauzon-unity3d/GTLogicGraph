using System;

namespace GeoTetra.GTLogicGraph.Slots
{
    [Serializable]
    public class TriggerDetailPortDescription : PortDetailDescription
    {
        public override PortDetailValueType ValueType
        {
            get { return PortDetailValueType.Trigger; }
        }

        public TriggerDetailPortDescription(NodeDetailEditor owner, string memberName, string displayName, PortDetailDirection portDirection) 
            : base(owner, memberName, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortDetailValueType inputType)
        {
            // A Trigger accepts any types
            return true;
        }
    }
}
