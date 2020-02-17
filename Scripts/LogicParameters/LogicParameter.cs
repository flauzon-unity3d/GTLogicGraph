using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Serializable]
    public class LogicParameter
    {        
        [SerializeField]
        private string _displayName;
        
        [SerializeField]
        private string _parameterGuid;

        public string ParameterGuid
        {
            get { return _parameterGuid; }
        }
        
        public string DisplayName
        {
            set { _displayName = value; }
            get { return _displayName; }
        }
    }
}