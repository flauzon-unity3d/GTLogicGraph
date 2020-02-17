﻿using System;
using UnityEngine;

namespace SensorFoundation.SensorGraph
{
    [Title("Actuator", "Servo Motor")]
    public class ServoMotorNodeEditor : NodeEditor
    {
        [SerializeField]
        private FieldFloat _power;
        [SerializeField]
        private FieldFloat _desiredAngle;
        [SerializeField]
        private FieldVector3 _positionWorld;
        [SerializeField]
        private FieldQuaternion _rotationWorld;
        [NonSerialized]
        private FieldQuaternion _outRotationWorld;

        public override void ConstructNode()
        {
            DisplayName = "Servo Motor";

            _power = new FieldFloat(this);
            _desiredAngle = new FieldFloat(this);
            _positionWorld = new FieldVector3(this);
            _rotationWorld = new FieldQuaternion(this);
            _outRotationWorld = new FieldQuaternion(this);

            AddVarSlot("Power", PortDirection.Input, _power);
            AddVarSlot("Desired Angle", PortDirection.Input, _desiredAngle);
            AddVarSlot("World Position", PortDirection.Input, _positionWorld);
            AddVarSlot("World Rotation", PortDirection.Input, _rotationWorld);
            AddVarSlot("World Rotation", PortDirection.Output, _outRotationWorld);
        }
    }
}