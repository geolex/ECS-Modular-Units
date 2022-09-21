using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct SlotComponent : IComponentData
{
    public float3 m_position;

    public float m_maxAngleLeft;
    public float m_maxAngleRight;
}
