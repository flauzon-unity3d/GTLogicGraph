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
                    foreach (INodeDetailControlAttribute attribute in
                        propertyInfo.GetCustomAttributes(typeof(INodeDetailControlAttribute), false))
                    {
                        _controlItems.Add(attribute.InstantiateControl(nodeEditor, propertyInfo));
                    }
                }
            }
            contents.Add(controlsContainer);

            List<PortDetailDescription> foundSlots = new List<PortDetailDescription>();
            nodeEditor.GetSlots(foundSlots);
            AddSlots(foundSlots);

            SetPosition(new Rect(nodeEditor.Position.x, nodeEditor.Position.y, 0, 0));
            base.expanded = nodeEditor.Expanded;
            RefreshExpandedState();
        }

        private void AddSlots(IEnumerable<PortDetailDescription> slots)
        {
            foreach (var slot in slots)
            {
                var port = PortDetailView.Create(slot, _connectorListener);
                if (slot.isOutputSlot)
                    outputContainer.Add(port);
                else
                    inputContainer.Add(port);
            }
        }

        public override void OnSelected()
        {
            /*NodeEditor.DetailView.Clear();

            var gn = new LogicNodeView();
            gn.Initialize(NodeEditor, null);
            gn.SetEnabled(true);
            gn.SetPosition(new Rect(0, 0, 100, 100));
            NodeEditor.DetailView.Add(gn);

            //*** Eventually, NodeEditor.BuildDetailView(NodeEditor.DetailView);

            var miniMap = new MiniMap();
            miniMap.SetPosition(new Rect(0, 0, 200, 176));
            NodeEditor.DetailView.Add(miniMap);


            Debug.Log("SELECTED " + NodeEditor.GetType().Name);*/
        }

        public override void OnUnselected()
        {
            /*NodeEditor.DetailView.Clear();
            Debug.Log("UNSELECTED");*/
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