using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct TurretComponent : IComponentData
{
    public float m_currentAngle;
    public float m_maxAngularSpeed;

    //float m_AngularAcceleration;
}
