using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SensorFoundation.SensorGraph
{
    public sealed class PortView : Port
    {
        PortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
            GraphLogicEditor.AddStyleSheetPath(this, "Styles/LogicPort");
        }

        IPortDescription _portDescription;

        public static Port Create(INodeEditor nodeEditor, IPortDescription portDescription, IEdgeConnectorListener connectorListener)
        {
            var port = new PortView(Orientation.Horizontal, 
                portDescription.IsInputSlot ? Direction.Input : Direction.Output,
                portDescription.IsInputSlot ? Capacity.Single : Capacity.Multi,
                portDescription.ValueType)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.userData = nodeEditor;
            portDescription.SpawnEditor(port);
            portDescription.RefreshEditor();
            
            port.PortDescription = portDescription;
            return port;
        }

        public IPortDescription PortDescription
        {
            get { return _portDescription; }
            set
            {
                if (ReferenceEquals(value, _portDescription))
                    return;
                if (value == null)
                    throw new NullReferenceException();
                if (_portDescription != null && value.IsInputSlot != _portDescription.IsInputSlot)
                    throw new ArgumentException("Cannot change direction of already created port");
                _portDescription = value;
                portName = PortDescription.DisplayName;
                visualClass = PortDescription.ValueType.Name;
            }
        }
    }
}
