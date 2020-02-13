using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

namespace GeoTetra.GTLogicGraph
{
    /// <summary>
    /// Actual visual nodes which gets added to the graph UI.
    /// </summary>
    public class LogicDetailNodeView : Node
    {
        VisualElement _controlsDivider;
        VisualElement _controlItems;
        VisualElement _portInputContainer;
        IEdgeConnectorListener _connectorListener;
        
        public NodeDetailEditor NodeEditor { get; private set; }

        public void Initialize(NodeDetailEditor nodeEditor, IEdgeConnectorListener connectorListener)
        {
            GraphLogicEditor.AddStyleSheetPath(this, "Styles/LogicDetailNodeView");

            _connectorListener = connectorListener;
            NodeEditor = nodeEditor;
            title = NodeEditor.DisplayName;

            var contents = this.Q("contents");

            var controlsContainer = new VisualElement {name = "controls"};
            {
                _controlsDivider = new VisualElement {name = "divider"};
                _controlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(_controlsDivider);
                _controlItems = new VisualElement {name = "items"};
                controlsContainer.Add(_controlItems);
            }
            contents.Add(controlsContainer);

            List<IPortDescription> foundSlots = new List<IPortDescription>();
            nodeEditor.GetSlots(foundSlots);
            AddSlots(foundSlots);

            SetPosition(new Rect(nodeEditor.Position.x, nodeEditor.Position.y, 0, 0));
            base.expanded = nodeEditor.Expanded;
            RefreshExpandedState();
        }
               
        private void AddSlots(IEnumerable<IPortDescription> slots)
        {
            foreach (var slot in slots)
            {
                var port = PortView.Create(slot, _connectorListener);
                if (slot.IsOutputSlot)
                    outputContainer.Add(port);
                else
                    inputContainer.Add(port);
            }
        }

        public override void OnSelected()
        {           
        }

        public override void OnUnselected()
        {
        }

        public override bool expanded
        {
            get { return base.expanded; }
            set
            {
                if (base.expanded != value)
                    base.expanded = value;

                NodeEditor.Expanded = value;
                RefreshExpandedState(); //This should not be needed. GraphView needs to improve the extension api here
            }
        }
    }
}