using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    public sealed class PortDetailView : Port
    {
        PortDetailView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
            GraphLogicEditor.AddStyleSheetPath(this, "Styles/LogicDetailPort");
        }

        PortDetailDescription _portDescription;

        public static Port Create(PortDetailDescription portDescription, IEdgeConnectorListener connectorListener)
        {
            var port = new PortDetailView(Orientation.Horizontal, 
                portDescription.isInputSlot ? Direction.Input : Direction.Output,
                portDescription.isInputSlot ? Capacity.Single : Capacity.Multi,
                null)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.PortDescription = portDescription;
            return port;
        }

        public PortDetailDescription PortDescription
        {
            get { return _portDescription; }
            set
            {
                if (ReferenceEquals(value, _portDescription))
                    return;
                if (value == null)
                    throw new NullReferenceException();
                if (_portDescription != null && value.isInputSlot != _portDescription.isInputSlot)
                    throw new ArgumentException("Cannot change direction of already created port");
                _portDescription = value;
                portName = PortDescription.DisplayName;
                visualClass = PortDescription.ValueType.ToString();
            }
        }
    }
}
