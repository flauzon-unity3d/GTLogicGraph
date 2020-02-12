using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    [Serializable]
    public abstract class IPortDescription
    {
        public delegate bool tDelegateSpawnEditor(IPortDescription self, PortView port);
        public delegate bool tDelegateHideEditor(IPortDescription self, PortView port);
        
        public class tHandlerDescriptor
        {
            public tDelegateSpawnEditor spawnEditor;
            public tDelegateHideEditor hideEditor;
        }
    
        private readonly string _memberName;
        private readonly string _displayName = "";
        private readonly PortDirection _portDirection;
        private readonly bool _promiscuous;
        private readonly Type _type;
        private readonly object _dataBind;

        public bool Promiscuous
        {
            get { return _promiscuous; }
        }

        public string DisplayName
        {
            get { return _displayName; }
        }

        public string MemberName
        {
            get { return _memberName; }
        }

        public object DataBind
        { 
            get { return _dataBind; }
        }
        
        public bool IsInputSlot
        {
            get { return _portDirection == PortDirection.Input; }
        }

        public bool IsOutputSlot
        {
            get { return _portDirection == PortDirection.Output; }
        }

        public PortDirection PortDirection
        {
            get { return _portDirection; }
        }

        public Type ValueType
        {
            get { return _type; }
        }

        public NodeEditor Owner { get; private set; }

        public IPortDescription(NodeEditor owner, Type type, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous = false)
        {
            Owner = owner;
            _type = type;
            _memberName = memberName;
            _displayName = displayName;
            _portDirection = portDirection;
            _promiscuous = promiscuous;
            _dataBind = dataBind;
        }

        public virtual bool IsCompatibleWithInputSlotType(Type inputType)
        {
            return (ValueType == inputType);
        }

        public bool IsCompatibleWith(IPortDescription otherPortDescription)
        {
            return otherPortDescription != null
                   && otherPortDescription.Owner != Owner
                   && otherPortDescription.IsInputSlot != IsInputSlot
                   && (((IsInputSlot
                       ? otherPortDescription.IsCompatibleWithInputSlotType(ValueType)
                       : IsCompatibleWithInputSlotType(otherPortDescription.ValueType)))
                   || otherPortDescription.Promiscuous);
        }

        public abstract void SpawnEditor(PortView port);
        public abstract void HideEditor();
    }

    [Serializable]
    [UnityEditor.InitializeOnLoad]
    public class PortDescription<T> : IPortDescription
    {
        private static Dictionary<Type, tHandlerDescriptor> _gMapFnc;

        static PortDescription()
        {           
        }

        // Promiscuous port can connect to anything
        public PortDescription(NodeEditor owner, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous=false):
            base(owner, typeof(T), memberName, displayName, dataBind, portDirection, promiscuous)
        {            
        }

        public static void RegisterHandlers(Type t, tHandlerDescriptor f)
        {
            if (_gMapFnc == null)
            {
                _gMapFnc = new Dictionary<Type, tHandlerDescriptor>();
            }

            if (!_gMapFnc.ContainsKey(t))
            {
                _gMapFnc.Add(t, f);
            }
        }

        public static void UnRegisterHandlers(Type t)
        {
            if (_gMapFnc == null)
            {
                return;
            }

            if (_gMapFnc.ContainsKey(t))
            {
                _gMapFnc.Remove(t);
            }
        }

        public override void SpawnEditor(PortView port)
        {
            tHandlerDescriptor f;
            if (_gMapFnc != null && _gMapFnc.TryGetValue(typeof(T), out f))
            {
                if (DataBind is FieldType)
                {
                    FieldType ft = (FieldType)DataBind;
                    ft.SetPortView(port);
                }

                f.spawnEditor.DynamicInvoke(this, port);
            }
        }

        public override void HideEditor()
        {
            tHandlerDescriptor f;
            if (_gMapFnc != null && _gMapFnc.TryGetValue(typeof(T), out f))
            {
                if (DataBind is FieldType)
                {
                    FieldType ft = (FieldType)DataBind;
                    f.hideEditor.DynamicInvoke(this, ft.GetPortView());
                }
            }
        }
    }

     
    //*** TODO
    [Serializable]
    [UnityEditor.InitializeOnLoad]
    public class DataArrayPortDescription : PortDescription<float>
    {
        public DataArrayPortDescription(NodeEditor owner, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous = false)
            : base(owner, memberName, displayName, dataBind, portDirection, promiscuous)
        {
        }
    }

    //*** TODO
    [Serializable]
    [UnityEditor.InitializeOnLoad]
    public class PhotonPortDescription : PortDescription<float>
    {
        public PhotonPortDescription(NodeEditor owner, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous = false)
            : base(owner, memberName, displayName, dataBind, portDirection, promiscuous)
        {
        }
    }

    //*** TODO
    [Serializable]
    [UnityEditor.InitializeOnLoad]
    public class PixelArrayPortDescription : PortDescription<float>
    {
        public PixelArrayPortDescription(NodeEditor owner, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous = false)
            : base(owner, memberName, displayName, dataBind, portDirection, promiscuous)
        {
        }
    }
    
    [Serializable]
    [UnityEditor.InitializeOnLoad]
    public class TexturePortDescription : PortDescription<Texture>
    {
        public TexturePortDescription(NodeEditor owner, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous = false)
            : base(owner, memberName, displayName, dataBind, portDirection, promiscuous)
        {
        }
    }

    [Serializable]
    [UnityEditor.InitializeOnLoad]
    public class ComputeBufferPortDescription : PortDescription<ComputeBuffer>
    {
        public ComputeBufferPortDescription(NodeEditor owner, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous = false)
            : base(owner, memberName, displayName, dataBind, portDirection, promiscuous)
        {
        }
    }

    [Serializable]
    [UnityEditor.InitializeOnLoad]
    public class BoolPortDescription : PortDescription<FieldBool>
    {
        static BoolPortDescription()
        {
            tHandlerDescriptor desc = new tHandlerDescriptor();
            desc.hideEditor = HandleHideEditor;
            desc.spawnEditor = HandleSpawnEditor;
            PortDescription<FieldBool>.RegisterHandlers(typeof(FieldBool), desc);
        }

        public BoolPortDescription(NodeEditor owner, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous = false)
            : base(owner, memberName, displayName, dataBind, portDirection, promiscuous)
        {
        }

        public static bool HandleSpawnEditor(IPortDescription self, PortView port)
        {
            if (self.PortDirection == PortDirection.Input)
            {
                Toggle t = new Toggle();
                t.userData = self.DataBind;
                port.Add(t);

                FieldBool b = (FieldBool)self.DataBind;
                t.RegisterValueChangedCallback<bool>(b.OnValueChanged);
                t.value = b.Data;
            }
            return true;
        }

        public static bool HandleHideEditor(IPortDescription self, PortView port)
        {
            foreach (VisualElement v in port.Children())
            {
                if (v is Toggle)
                {
                    v.SetEnabled(false);
                }
            }
            return true;
        }
    }

    [Serializable]
    [UnityEditor.InitializeOnLoad]
    public class FloatPortDescription : PortDescription<FieldFloat>
    {
        static FloatPortDescription()
        {
            tHandlerDescriptor desc = new tHandlerDescriptor();
            desc.hideEditor = HandleHideEditor;
            desc.spawnEditor = HandleSpawnEditor;

            PortDescription<FieldFloat>.RegisterHandlers(typeof(FieldFloat), desc);
        }

        public FloatPortDescription(NodeEditor owner, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous = false)
            : base(owner, memberName, displayName, dataBind, portDirection, promiscuous)
        {
        }

        public static bool HandleSpawnEditor(IPortDescription self, PortView port)
        {
            if (self.PortDirection == PortDirection.Input)
            {
                TextField[] fields = new TextField[1];
                for (int a = 0; a < fields.Length; ++a)
                {
                    fields[a] = new TextField();
                    fields[a].userData = self.DataBind;
                    port.Add(fields[a]);
                }

                FieldFloat f = (FieldFloat)self.DataBind;
                fields[0].RegisterValueChangedCallback<string>(f.OnValueChanged);
                fields[0].value = Convert.ToString(f.Data);
            }
            return true;
        }

        public static bool HandleHideEditor(IPortDescription self, PortView port)
        {
            foreach (VisualElement v in port.Children())
            {
                if (v is TextField)
                {
                    v.SetEnabled(false);
                }
            }
            return true;
        }
    }

    [Serializable]
    [UnityEditor.InitializeOnLoad]
    public class Vector2PortDescription : PortDescription<Vector2>
    {
        static Vector2PortDescription()
        {
            tHandlerDescriptor desc = new tHandlerDescriptor();
            desc.hideEditor = HandleHideEditor;
            desc.spawnEditor = HandleSpawnEditor;

            PortDescription<Vector2>.RegisterHandlers(typeof(Vector2), desc);
        }

        public Vector2PortDescription(NodeEditor owner, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous = false)
            : base(owner, memberName, displayName, dataBind, portDirection, promiscuous)
        {
        }

        public static bool HandleSpawnEditor(IPortDescription self, PortView port)
        {
            if (self.PortDirection == PortDirection.Input)
            {
                TextField[] fields = new TextField[2];
                for (int a = 0; a < fields.Length; ++a)
                {
                    fields[a] = new TextField();
                    fields[a].userData = self.DataBind;
                    port.Add(fields[a]);
                }

                FieldVector2 v2 = (FieldVector2)self.DataBind;
                fields[0].RegisterValueChangedCallback<string>(v2.OnValueChangedX);
                fields[1].RegisterValueChangedCallback<string>(v2.OnValueChangedY);

                fields[0].value = Convert.ToString(v2.Data.x);
                fields[1].value = Convert.ToString(v2.Data.y);
            }
            return true;
        }

        public static bool HandleHideEditor(IPortDescription self, PortView port)
        {
            foreach (VisualElement v in port.Children())
            {
                if (v is TextField)
                {
                    v.SetEnabled(false);
                }
            }
            return true;
        }
    }

    [Serializable]
    [UnityEditor.InitializeOnLoad]
    public class FieldVector3PortDescription : PortDescription<FieldVector3>
    {
        static FieldVector3PortDescription()
        {
            tHandlerDescriptor desc = new tHandlerDescriptor();
            desc.hideEditor = HandleHideEditor;
            desc.spawnEditor = HandleSpawnEditor;

            PortDescription<FieldVector3>.RegisterHandlers(typeof(FieldVector3), desc);
        }

        public FieldVector3PortDescription(NodeEditor owner, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous = false)
            : base(owner, memberName, displayName, dataBind, portDirection, promiscuous)
        {
        }

        public static bool HandleSpawnEditor(IPortDescription self, PortView port)
        {
            if (self.PortDirection == PortDirection.Input)
            {
                TextField[] fields = new TextField[3];
                for (int a = 0; a < fields.Length; ++a)
                {
                    fields[a] = new TextField();
                    fields[a].userData = self.DataBind;
                    port.Add(fields[a]);
                }

                FieldVector3 v3 = (FieldVector3)self.DataBind;
                fields[0].RegisterValueChangedCallback<string>(v3.OnValueChangedX);
                fields[1].RegisterValueChangedCallback<string>(v3.OnValueChangedY);
                fields[2].RegisterValueChangedCallback<string>(v3.OnValueChangedZ);

                fields[0].value = Convert.ToString(v3.Data.x);
                fields[1].value = Convert.ToString(v3.Data.y);
                fields[2].value = Convert.ToString(v3.Data.z);
            }
            return true;
        }

        public static bool HandleHideEditor(IPortDescription self, PortView port)
        {
            foreach (VisualElement v in port.Children())
            {
                if (v is TextField)
                {
                    v.SetEnabled(false);
                }
            }
            return true;
        }
    }

    [Serializable]
    [UnityEditor.InitializeOnLoad]
    public class FieldVector4PortDescription : PortDescription<FieldVector4>
    {
        static FieldVector4PortDescription()
        {
            tHandlerDescriptor desc = new tHandlerDescriptor();
            desc.hideEditor = HandleHideEditor;
            desc.spawnEditor = HandleSpawnEditor;

            PortDescription<FieldVector4>.RegisterHandlers(typeof(FieldVector4), desc);
        }

        public FieldVector4PortDescription(NodeEditor owner, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous = false)
            : base(owner, memberName, displayName, dataBind, portDirection, promiscuous)
        {                        
        }

        public static bool HandleSpawnEditor(IPortDescription self, PortView port)
        {
            if (self.PortDirection == PortDirection.Input)
            {
                TextField[] fields = new TextField[4];
                for (int a = 0; a < fields.Length; ++a)
                {
                    fields[a] = new TextField();
                    fields[a].userData = self.DataBind;
                    port.Add(fields[a]);
                }

                FieldVector4 v4 = (FieldVector4)self.DataBind;
                fields[0].RegisterValueChangedCallback<string>(v4.OnValueChangedX);
                fields[1].RegisterValueChangedCallback<string>(v4.OnValueChangedY);
                fields[2].RegisterValueChangedCallback<string>(v4.OnValueChangedZ);
                fields[3].RegisterValueChangedCallback<string>(v4.OnValueChangedW);

                fields[0].value = Convert.ToString(v4.Data.x);
                fields[1].value = Convert.ToString(v4.Data.y);
                fields[2].value = Convert.ToString(v4.Data.z);
                fields[3].value = Convert.ToString(v4.Data.w);
            }
            return true;
        }

        public static bool HandleHideEditor(IPortDescription self, PortView port)
        {
            foreach (VisualElement v in port.Children())
            {
                if (v is TextField)
                {
                    v.SetEnabled(false);
                }
            }
            return true;
        }
    }
   

    [Serializable]
    [UnityEditor.InitializeOnLoad]
    public class GameObjectPortDescription : PortDescription<GameObject>
    {
        public GameObjectPortDescription(NodeEditor owner, string memberName, string displayName, object dataBind, PortDirection portDirection, bool promiscuous = false)
            : base(owner, memberName, displayName, dataBind, portDirection, promiscuous)
        {
        }
    }

    public enum PortDirection
    {
        Input,
        Output
    }
}