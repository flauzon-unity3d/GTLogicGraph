using System;

namespace GeoTetra.GTLogicGraph.Slots
{
    [Serializable]
    public class TriggerPortDescription : PortDescription
    {
        public override PortValueType ValueType
        {
            get { return PortValueType.Trigger; }
        }

        public TriggerPortDescription(NodeEditor owner, string memberName, string displayName, PortDirection portDirection) 
            : base(owner, memberName, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            // A Trigger accepts any types
            return true;
        }
    }
}
