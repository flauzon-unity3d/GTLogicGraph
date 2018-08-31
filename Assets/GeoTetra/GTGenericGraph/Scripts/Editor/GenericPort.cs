﻿using System;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
    public sealed class GenericPort : Port
    {
        GenericPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
            AddStyleSheetPath("Styles/GenericPort");
        }

        PortDescription _portDescription;

        public static Port Create(PortDescription portDescription, IEdgeConnectorListener connectorListener)
        {
            var port = new GenericPort(Orientation.Horizontal, 
                portDescription.isInputSlot ? Direction.Input : Direction.Output,
                portDescription.isInputSlot ? Capacity.Single : Capacity.Multi,
                null)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.PortDescription = portDescription;
            port.portName = portDescription.DisplayName;
            port.visualClass = portDescription.ValueType.ToString();
            return port;
        }

        public PortDescription PortDescription
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
