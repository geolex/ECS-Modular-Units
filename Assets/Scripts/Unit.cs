using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponData", menuName = "Unit Designer/Weapon Data", order = 53)]
public class Storage_Weapon : ScriptableObject
{
    #region Weapon Informations

    public Transform m_transform;

    public Mesh m_weaponMesh;

    public float m_penetration;
    public float m_range;
    public int m_damage;

	 public float m_fireRate;



    #endregion
}

[CreateAssetMenu(fileName = "New TurretData", menuName = "Unit Designer/Turret Data", order = 52)]
public class Storage_Turret : ScriptableObject
{
    #region Turret Informations

    public Transform m_transform;

    public Mesh m_turretMesh;

    public int m_maxHealth;
    public float m_maxRotation;
    public float m_rotationSpeed;

    #endregion

    #region Weapons Informations
    public List<Storage_Weapon> m_weapons;
    #endregion

    #region Turrets Informations
    public List<Storage_Turret> m_turrets;
    #endregion

}

[CreateAssetMenu(fileName = "New UnitData", menuName = "Unit Designer/Unit Data", order = 51)]
public class Storage_Unit : ScriptableObject
{
    public string m_name;   // Name of the Unit
    public int cost;        // Production cost of the Unit

    #region Hull Informations
	public Mesh m_hullMesh;

	public int m_maxHealth;
	public int m_cost;
	public float m_weight;

    public float m_maxSpeed;
    public float m_acceleration;
    #endregion

    #region Weapons Informations
    public List<Storage_Weapon> m_weapons;
    #endregion

    #region Turrets Informations
    public List<Storage_Turret> m_turrets;
    #endregion

}
