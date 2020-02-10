using System;

namespace GeoTetra.GTLogicGraph
{
    public abstract class PortDetailDescription
    {
        private readonly string _memberName;
        private readonly string _displayName = "";
        private readonly PortDetailDirection _portDirection;

        public NodeDetailEditor Owner { get; private set; }
        
        public PortDetailDescription(NodeDetailEditor owner, string memberName, string displayName, PortDetailDirection portDirection)
        {
            Owner = owner;
            _memberName = memberName;
            _displayName = displayName;
            _portDirection = portDirection;
        }

        public string DisplayName
        {
            get { return _displayName; }
        }

        public string MemberName
        {
            get { return _memberName; }
        }

        public bool isInputSlot
        {
            get { return _portDirection == PortDetailDirection.Input; }
        }

        public bool isOutputSlot
        {
            get { return _portDirection == PortDetailDirection.Output; }
        }

        public PortDetailDirection PortDirection
        {
            get { return _portDirection; }
        }

        public abstract PortDetailValueType ValueType { get; }

        public abstract bool IsCompatibleWithInputSlotType(PortDetailValueType inputType);

        public bool IsCompatibleWith(PortDetailDescription otherPortDescription)
        {
            return otherPortDescription != null
                   && otherPortDescription.Owner != Owner
                   && otherPortDescription.isInputSlot != isInputSlot
                   && (((isInputSlot
                       ? otherPortDescription.IsCompatibleWithInputSlotType(ValueType)
                       : IsCompatibleWithInputSlotType(otherPortDescription.ValueType)))
                   || otherPortDescription.ValueType == PortDetailValueType.Trigger); // Trigger is always compatible
        }
    }

    public enum PortDetailDirection
    {
        Input,
        Output
    }
    
    [Serializable]
    public enum PortDetailValueType
    {
        Boolean,
        GameObject,
        Transform,
        Photon,
        PixelArray,
        DataArray,
        Trigger
    }
}