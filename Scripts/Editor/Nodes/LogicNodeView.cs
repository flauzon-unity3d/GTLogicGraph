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
    public class LogicNodeView : Node
    {
        VisualElement _controlsDivider;
        VisualElement _controlItems;
        VisualElement _portInputContainer;
        IEdgeConnectorListener _connectorListener;

        public NodeEditor NodeEditor { get; private set; }

        public void Initialize(NodeEditor nodeEditor, IEdgeConnectorListener connectorListener)
        {
            GraphLogicEditor.AddStyleSheetPath(this, "Styles/LogicNodeView");

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

                foreach (var propertyInfo in
                    nodeEditor.GetType().GetProperties(BindingFlags.Instance |
                                                            BindingFlags.Public |
                                                            BindingFlags.NonPublic))
                {
                    foreach (INodeControlAttribute attribute in
                        propertyInfo.GetCustomAttributes(typeof(INodeControlAttribute), false))
                    {
                        _controlItems.Add(attribute.InstantiateControl(nodeEditor, propertyInfo));
                    }
                }
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

        private void ClearDetailNodes()
        {
            List<VisualElement> lstRemove = new List<VisualElement>();
            foreach (VisualElement ele in NodeEditor.DetailView.Children())
            {
                if (ele as Node != null || ele as Edge != null ||
                    ele as MiniMap != null ||
                    ele as Blackboard != null || ele as BlackboardField != null || ele as BlackboardRow != null || ele as BlackboardSection != null)
                {
                    lstRemove.Add(ele);
                }
            }
            lstRemove.AddRange(NodeEditor.DetailView.graphElements.ToList());

            foreach (VisualElement ele in lstRemove)
            {
                if (ele as GraphElement != null)
                {
                    NodeEditor.DetailView.RemoveElement(ele as GraphElement);
                }
                else
                {
                    NodeEditor.DetailView.Remove(ele);
                }
            }            
        }

        public override void OnSelected()
        {
            // Seek and destroy nodes and edges
            ClearDetailNodes();
            NodeEditor.Owner.ContextNode = NodeEditor;

            var bb = new Blackboard(NodeEditor.DetailView);
            bb.title = NodeEditor.DisplayName;
            bb.subTitle = string.Empty;
            bb.SetPosition(new Rect(4, 24, 300, 200));            
            NodeEditor.DetailView.Add(bb);

            // Deserialize context node
            NodeEditor.BuildDetailView(NodeEditor.DetailView);
        }

        public override void OnUnselected()
        {
            //*** Save modifs in detail view?
            NodeEditor.Owner.ContextNode = null;
            ClearDetailNodes();
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