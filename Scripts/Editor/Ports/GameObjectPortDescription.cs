using System;

namespace GeoTetra.GTLogicGraph.Slots
{
    [Serializable]
    public class GameObjectPortDescription : PortDescription
    {
        public override PortValueType ValueType
        { 
            get 
            { 
                return PortValueType.GameObject; 
            } 
        }

        public GameObjectPortDescription(NodeEditor owner, string memberName, string displayName, PortDirection portDirection) 
            : base(owner, memberName, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.GameObject;
        }
    }
}
