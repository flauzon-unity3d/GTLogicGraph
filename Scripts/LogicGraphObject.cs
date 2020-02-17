using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.AccessControl;
using System.Threading;
using UnityEngine;

namespace SensorFoundation.SensorGraph
{
    public class LogicGraphObject : ScriptableObject
    {
        [SerializeField] 
        private LogicGraphData _logicGraphData;
        
        public void Initialize(LogicGraphData logicGraphData)
        {
            _logicGraphData = logicGraphData;
        }

        private LogicNode FindNodeByGuid(string guid, params List<LogicNode>[] lists)
        {
            foreach (var list in lists)
            {
                LogicNode node = list.Find(n => n.NodeGuid == guid);
                if (node != null) return node;
            }

            return null;
        }

        private LogicParameter CreateLogicParameterFromSerializedParameter(SerializedParameter serializedParam)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(LogicParameter)))
                    {
                        if (type.Name == serializedParam.Type)
                        {
                            return JsonUtility.FromJson(serializedParam.JSON, type) as LogicParameter;
                        }
                    }
                }
            }

            return null;
        }

        private LogicNode CreateLogicNodeFromSerializedNode(SerializedNode serializedNode)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(LogicNode)))
                    {
                        if (type.Name == serializedNode.NodeType)
                        {
                            return JsonUtility.FromJson(serializedNode.JSON, type) as LogicNode;
                        }
                    }
                }
            }

            return null;
        }
    }
}