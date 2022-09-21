using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using System.IO;
using UnityEditor;
using System;
using System.Xml.Serialization;
using System.Xml;

public class DesignerScript : MonoBehaviour
{
	[SerializeField] private GameObject m_hullPrefab;
	
	[SerializeField] private Slot m_selectedSlot;           // Currently active Slot
	[SerializeField] private GameObject m_hull;             // Root of the designer
	
	private RaycastHit m_raycastHit;
	
	[SerializeField] private List<Part> m_parts;
	
	EntityArchetype customUnit;
	
	[Header("UI Handler")]
	[SerializeField] Designer_UI m_UIHandler;
	
	public string m_name;
	
	
	
	
	// Start is called before the first frame update
	void Start()
	{
		// Should be created when Unit is read from JSon
		//customUnit = World.Active.EntityManager.CreateArchetype(typeof(LocalToWorld), typeof(Translation), typeof(RenderMesh), typeof(VelocityComponent), typeof(ArmorComponent));
		
		m_hull = Instantiate(m_hullPrefab, Vector3.zero, Quaternion.identity);
		
		if (m_UIHandler)
		{
			m_UIHandler.m_designerScript = this;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out m_raycastHit);
			
			if (m_raycastHit.collider != null)
				{
				if (m_raycastHit.collider.tag == "slotTag")
				{
					m_selectedSlot = m_raycastHit.collider.gameObject.GetComponent<Slot>();
					
					List<Part> compatibleParts = GetCompatibleParts(m_selectedSlot.m_type, m_selectedSlot.m_size);
					
					if (m_UIHandler)
					{
					m_UIHandler.SetList(compatibleParts);
					}
				}
				else
				{
					Debug.Log(m_raycastHit.collider.name);
				}
			}
		}
	}
	
	#region PART LIST ACCESSORS
	private List<Part> GetCompatibleParts(PartType _type, PartSize _size)
	{
	List<Part> compatibleParts = new List<Part>();
	
	m_parts.ForEach(part => { if (part.m_type == _type && part.m_size == _size) compatibleParts.Add(part); });
	
	return compatibleParts;
	}
	
	private void PrintAllCompatibleParts(PartType _type, PartSize _size)
	{
	GetCompatibleParts(_type, _size).ForEach(part => { Debug.Log(part.name); });
	}
	#endregion
	
	public void AddComponent(Part _part)
	{
	m_selectedSlot.SetPart(_part);
	}
	
	
	
	/*
	public void BakeUnit(string _name)
	{
	string newUnitPath = Application.dataPath + "/Units/Custom/" + _name + ".json";
	
	if (!File.Exists(newUnitPath))
	{
	StreamWriter fileOutput = new StreamWriter(newUnitPath);
	
	fileOutput.Write(UnitToJSon(_name));
	
	MergeMeshes_Recursive(m_hull.GetComponent<Part>(), _name);
	//MergeData_Recursive(m_hull.GetComponent<Part>(), fileOutput, _name) ;
	
	fileOutput.Close();
	}
	}
	*/
	
	
	/*
	private void MergeData_Recursive(Part _base, StreamWriter _output, string _name, int _index = 0)
	{
	#region Components
	
	_output.WriteLine(_name + "_" + _base.name + "_" + _index + "{");
	
	List<Part> tractions = _base.GetChildsRecursively<Traction>();
	List<Part> armors = _base.GetChildsRecursively<Armor>();
	List<Part> weapons = _base.GetChildsRecursively<Weapon>();
	
	// TRACTION
	float maxSpeed = 0f;
	float maxSlope = 0f;
	tractions.ForEach(traction => 
	{
	maxSpeed = Mathf.Min(maxSpeed, ((Traction)traction).m_maxSpeed);
	maxSlope = Mathf.Min(maxSlope, ((Traction)traction).m_maxSlope);
	});
	
	_output.WriteLine("Velocity{ maxSpeed = " + maxSpeed + " }");
	_output.WriteLine("Navigation{ maxSlope = " + maxSlope + " }");
	
	
	// ARMOR
	int totalArmor = 0;
	int totalLife = 0;
	armors.ForEach(armor =>
	{
	totalArmor += ((Armor)armor).m_armor;
	totalLife += ((Armor)armor).m_life;
	});
	
	_output.WriteLine("Resistance{ armor = " + totalArmor + ", life = " + totalLife + " }");
	#endregion
	
	#region subEntities
	_output.WriteLine("Childs{");
	
	
	List<Part> turrets = _base.GetChildsRecursively<Turret>();
	
	turrets.ForEach(turret =>
	{
	MergeData_Recursive(turret, _output, _name, (_index * 10) + 1);
	});
	
	_output.WriteLine("}");
	#endregion
	
	_output.WriteLine("}");
	}
	
	
	
	
	
	
	
	
	
	
	public string UnitToJSon(string _name)
	{
	string jsonValue = "";
	
	jsonValue += m_hull.GetComponent<Part>().ToJSon();
	
	return jsonValue;
	}
	
	public void JSonToUnit(string _json)
	{
	
	}
	
	
	
	
	
	#region MESH TOOLS
	private void MergeMeshes_Recursive(Part _base, string _name, int _index = 0) { 
	List<MeshFilter> linkedMeshes = new List<MeshFilter>();
	Mesh resultMesh = new Mesh();
	
	// Add mesh of _base model
	linkedMeshes.Add(_base.GetComponent<MeshFilter>());
	
	// Add meshes of all independent (!= turret) parts recursively
	_base.GetChilds<Part>().ForEach(part =>
	{
	if (part.m_type == PartType.Turret)
	{
	MergeMeshes_Recursive(part, _name, (_index * 10) + 1);
	}
	else
	{
	linkedMeshes.AddRange(part.GetLinkedMeshes_Recursive());
	}
	});
	
	CombineInstance[] combineInstances = new CombineInstance[linkedMeshes.Count];
	
	for(int i=0; i < combineInstances.Length; i++)
	{
	combineInstances[i].mesh = linkedMeshes[i].sharedMesh;
	combineInstances[i].transform = linkedMeshes[i].transform.localToWorldMatrix;
	}
	
	resultMesh.CombineMeshes(combineInstances);
	
	AssetDatabase.CreateAsset(resultMesh, "Assets/Meshes/Procedural/" + _name + "_" + _base.name + "_" + _index + ".asset");
	AssetDatabase.SaveAssets();
	}
	
	
	private Mesh MergeMeshes(Part _base)
	{
List<MeshFilter> linkedMeshes = new List<MeshFilter>();
Mesh resultMesh = new Mesh();

// Add mesh of _base model
linkedMeshes.Add(_base.GetComponent<MeshFilter>());

// Add meshes of all independent (!= turret) parts recursively
_base.GetChilds<Part>().ForEach(part =>
{
if (part.m_type != PartType.Turret)
{
linkedMeshes.AddRange(part.GetLinkedMeshes_Recursive());
}
});

CombineInstance[] combineInstances = new CombineInstance[linkedMeshes.Count];

for (int i = 0; i < combineInstances.Length; i++)
{
combineInstances[i].mesh = linkedMeshes[i].sharedMesh;
combineInstances[i].transform = linkedMeshes[i].transform.localToWorldMatrix;
}

resultMesh.CombineMeshes(combineInstances);

return resultMesh;
}
#endregion
*/




#region MESH MERGER

private Mesh MergeMeshes(Part _base)
{

List<MeshFilter> linkedMeshes = new List<MeshFilter>();
Mesh resultMesh = new Mesh();

// Add mesh of _base model
linkedMeshes.Add(_base.GetComponent<MeshFilter>());

// Add meshes of all independent (!= turret) parts recursively
_base.GetChildsRecursively<Part>().ForEach(part =>
{
linkedMeshes.Add(part.GetComponent<MeshFilter>());
});

CombineInstance[] combineInstances = new CombineInstance[linkedMeshes.Count];

for (int i = 0; i < combineInstances.Length; i++)
{
combineInstances[i].mesh = linkedMeshes[i].sharedMesh;
combineInstances[i].transform = linkedMeshes[i].transform.localToWorldMatrix;
}

resultMesh.CombineMeshes(combineInstances);

return resultMesh;
}
#endregion




#region DATA MERGERS

// Merge Health of All Objects
int MergeMaxHealth(Part _base)
{
// The Health of a Part is the combined Health of it's linked Parts
HashSet<Type> typeMask = new HashSet<Type>();
typeMask.Add(typeof(Turret));
typeMask.Add(typeof(Weapon));

int maxHealth = 0;

foreach (Part part in _base.GetChildsRecursively<Part>())
{
maxHealth += part.m_health;
}

return maxHealth;
}

float MergeMaxSpeed(Part _base)
{
// The Max Speed of a Part is the minimum MaxSpeed of it's linked Parts
HashSet<Type> typeMask = new HashSet<Type>();
typeMask.Add(typeof(Turret));
typeMask.Add(typeof(Weapon));

float maxSpeed = float.MaxValue;

foreach (Traction part in _base.GetChildsRecursively<Traction>())
{
maxSpeed = Mathf.Min(maxSpeed, ((Traction)part).m_maxSpeed);
}

return maxSpeed;
}

float MergeAcceleration(Part _base)
{

// The Max Speed of a Part is the minimum MaxSpeed of it's linked Parts
HashSet<Type> typeMask = new HashSet<Type>();
typeMask.Add(typeof(Turret));
typeMask.Add(typeof(Weapon));

float maxSpeed = float.MaxValue;

foreach (Traction part in _base.GetChildsRecursively<Traction>())
{
maxSpeed = Mathf.Min(maxSpeed, part.m_maxSpeed);
}

return maxSpeed;
}

	float MergeWeight(Part _base)
	{
		float weight = 0;

		foreach (Part part in _base.GetAllChildsRecursively<Part>())
		{
			weight += part.m_weight;
		}

		return weight;
	}

int MergeCost(Part _base)
{
int cost = 0;

foreach (Part part in _base.GetAllChildsRecursively<Part>())
{
cost += part.m_cost;
}

return cost;
}
#endregion



















#region SCRIPTABLE OBJECTS SAVE

	public Storage_Unit SaveUnitToScriptableObject()
	{
		Storage_Unit newUnit = ScriptableObject.CreateInstance<Storage_Unit>();
		Part unit = m_hull.GetComponent<Part>();
		
		newUnit.name = m_name;
		
		newUnit.m_name = m_name;
		newUnit.m_hullMesh = MergeMeshes(unit);
		
		
		newUnit.m_maxHealth = MergeMaxHealth(unit);
		newUnit.m_maxSpeed = MergeMaxSpeed(unit);
		newUnit.m_acceleration = MergeAcceleration(unit);
		newUnit.m_cost = MergeCost(unit);
		newUnit.m_weight = MergeWeight(unit);
		

		foreach(Turret turret in unit.GetChildsRecursively<Turret>())
		{
			newUnit.m_turrets.Add(SaveTurretToScriptableObject(turret));
		}
		
		foreach(Weapon weapon in unit.GetChildsRecursively<Weapon>())
		{
			newUnit.m_weapons.Add(SaveWeaponToScriptableObject(weapon));
		}
		
		
		//AssetDatabase.CreateAsset(newUnit, "Assets/Units/Custom/");
		//AssetDatabase.SaveAssets();
		
		return newUnit;
	}
	
	
	Storage_Turret SaveTurretToScriptableObject(Turret _turret)
	{
		Storage_Turret newTurret = new Storage_Turret();
		
		newTurret.name = m_name + "_Turret";
		
		newTurret.m_transform = _turret.transform;
		newTurret.m_turretMesh = _turret.gameObject.GetComponent<MeshFilter>().mesh;
		
		newTurret.m_maxRotation = _turret.m_maxRotation;
		newTurret.m_rotationSpeed = _turret.m_rotationSpeed;
		
		newTurret.m_maxHealth = MergeMaxHealth(_turret);
		
		
		foreach (Turret turret in _turret.GetChildsRecursively<Turret>())
		{
			newTurret.m_turrets.Add(SaveTurretToScriptableObject(turret));
		}
		
		foreach (Weapon weapon in _turret.GetChildsRecursively<Weapon>())
		{
			Debug.Log(weapon);
			Debug.Log(newTurret);
			
			newTurret.m_weapons.Add(SaveWeaponToScriptableObject(weapon));
		}
		
		return newTurret;
	}
	
	
	
	Storage_Weapon SaveWeaponToScriptableObject(Weapon _weapon)
	{
		Storage_Weapon newWeapon = new Storage_Weapon();
		
		newWeapon.m_weaponMesh = _weapon.GetComponent<MeshFilter>().mesh;
		newWeapon.m_transform = _weapon.transform;
		
		newWeapon.m_damage = _weapon.m_damage;
		newWeapon.m_fireRate = _weapon.m_fireRate;
		newWeapon.m_penetration = _weapon.m_penetration;
		newWeapon.m_range = _weapon.m_range;
		
		return newWeapon;
	}

#endregion



#region JSON SAVE
	
	void SaveXMLToFile(string _xml){
	}



	public string UnitToXML(){
		string unitXML = "";
		Part unit = m_hull.GetComponent<Part>();
		
		//Serialises unit base
		unitXML += "<" + m_name + "> \n";
		{
			unitXML += unit.ToXML();
			
			// Serialise Unit
			unitXML += "   <Unit";
			unitXML += " Health=" + MergeMaxHealth(unit);
			unitXML += " Speed=" + MergeMaxSpeed(unit);
			unitXML += " Acceleration=" + MergeAcceleration(unit);
			unitXML += " Cost=" + MergeCost(unit);
			unitXML += " Weight=" + MergeWeight(unit);
			unitXML += "/> \n";
			
			
			// Serialises all attached Turrets
			unitXML += "   <TurretList> \n";
			foreach(Turret turret in unit.GetChildsRecursively<Turret>())
			{
				unitXML += TurretToXML(turret, 2);
			}
			unitXML += "   </TurretList> \n";
			
			// Serialises all attached Weapons
			unitXML += "   <WeaponList> \n";
			foreach(Weapon weapon in unit.GetChildsRecursively<Weapon>())
			{
				unitXML += WeaponToXML(weapon, 2);
			}
			unitXML += "   </WeaponList> \n";
			
		}
		unitXML += "</" + m_name + "> \n";
		
		return unitXML;
	}




	string TurretToXML(Turret _turret, int _indent){
		return _turret.ToXML(_indent);
	}
	
	string WeaponToXML(Weapon _weapon, int _indent){
		return _weapon.ToXML(_indent);
	}




	public string SerializeUnit(){
		//return m_hull.GetComponent<Part>().SerializeUnit(0, "  ");
		XmlDocument document = new XmlDocument();
		document.CreateXmlDeclaration( "1.0", "UTF-8", null);
		
		document.AppendChild(m_hull.GetComponent<Part>().SerializeUnit(document));
		
		document.Save(m_name + ".txt");
		
		Destroy(m_hull);
		
		return document.ToString();
	}
	
	
	
	
	/*
	public void DeserializeUnit(){
		XmlDocument document = new XmlDocument();
		document.Load(m_name + ".txt");
		
		//XmlNode currentNode = document.FirstChild;
		
		DeserialiseNode(document.FirstChild);
		
		
		
		
	}
	
	private void DeserialiseNode(XmlNode _node, Slot _parent = null){
		
		Part part = Instantiate(m_parts[int.Parse(_node.Attributes["ID"].Value)]);
		
		if(_parent != null){
			_parent.SetPart<part.GetType()>(part); 
		}
		
		for (int i = 0; i < _node.ChildNodes.Count; ++i){
			DeserialiseNode(_node.ChildNodes[i], part.GetSlot(i));
		}
	}
	*/
	
#endregion








}
