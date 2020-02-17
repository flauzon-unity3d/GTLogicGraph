using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SensorFoundation.SensorGraph
{
	[Serializable]
	public class LogicGraphData 
	{
        [SerializeField]
        private List<SerializedParameter> _serializedParameters = new List<SerializedParameter>();

		[SerializeField]
		private List<SerializedNode> _serializedNodes = new List<SerializedNode>();
		
		[SerializeField]
		private List<SerializedEdge> _serializedEdges = new List<SerializedEdge>();
		
		public List<SerializedParameter> SerializedParameters
        {
            get { return _serializedParameters; }
        }
        
        public List<SerializedEdge> SerializedEdges
		{
			get { return _serializedEdges; }
		}

		public List<SerializedNode> SerializedNodes
		{
			get { return _serializedNodes; }
		}
	}

    [Serializable]
    public class SerializedParameter
    {
        public string Type;
        public string JSON;
    }

	[Serializable]
	public class SerializedNode
	{
		public string NodeType;
		public string JSON;
	}
	
	[Serializable]
	public class SerializedEdge
	{
		public string SourceNodeGuid;
		public string SourceMemberPortName;
        public string SourceMemberTypeName;
		public string TargetNodeGuid;
		public string TargetMemberPortName;
        public string TargetMemberTypeName;
	}
}