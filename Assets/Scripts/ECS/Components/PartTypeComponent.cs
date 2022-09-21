using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public enum PartSize { Minuscule, Small, Average, Large, Extreme };
public enum PartType { Armor, Engine, Expansion, Hull, Traction, Turret, Weapon };

public struct PartTypeComponent : IComponentData
{
    public PartSize m_size;
    public PartType m_type;
}
