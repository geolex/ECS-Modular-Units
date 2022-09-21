using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct ArmorComponent : IComponentData
{
    public int m_armorValue;
    public int m_life;
}
