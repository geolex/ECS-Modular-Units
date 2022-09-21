using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Xml;

[System.Serializable]
public class Part : MonoBehaviour
{
	public int m_id;
	
    // All the Slots in the Part
    [SerializeField] private List<Slot> m_slots;

    [SerializeField] private Mesh m_mesh;
    [SerializeField] private Material m_material;

    // Part Characteristics
    public PartType m_type;
    public PartSize m_size;

    public int m_health;
	public float m_weight;

    public int m_cost;

    public Mesh Mesh { get => m_mesh; set => m_mesh = value; }

	// Merged Values
	private Mesh m_mergedMesh = null;
	private int m_mergedHealth = 0;
	private float m_mergedWeight = 0;
	private int m_mergedCost = 0;

	public Slot GetSlot(int _index){
		return m_slots[_index];
	}



    // Returns Childs of type T (Stops at Turrets and Weapons)
    public List<T> GetChildsRecursively<T>() where T : Part
    {
        List<T> result = new List<T>();

        m_slots.ForEach(slot =>
        {
            T part = slot.GetPart<T>();

            if (part != default)
            {
                if (part is T)
                {
                    result.Add(part);
                }
                
                if(!(part is Turret) && !(part is Weapon))
                {
                    result.AddRange(part.GetChildsRecursively<T>());
                }

            }
        });

        return result;
    }

    // Return ALL Childs of type T
    public List<T> GetAllChildsRecursively<T>() where T : Part
    {
        List<T> result = new List<T>();

        m_slots.ForEach(slot =>
        {
            T part = slot.GetPart<T>();

	        if (part != null)
            {
                if(part is T)
                {
                    result.AddRange(part.GetAllChildsRecursively<T>());
                }
            }
        });

        return result;
    }

    // Returns ALL IMMEDIATE childs of type T
    public List<T> GetAllChilds<T>() where T : Part
    {
        List<T> result = new List<T>();

        m_slots.ForEach(slot =>
        {
            T part = slot.GetPart<T>();
            if (part != null)
            {
                result.Add(part);
            }
        });

        return result;
    }

	Mesh GetMergedMesh(){
		if(m_mergedMesh == null){
			m_mergedMesh = MergeMeshes();
		}
		
		return m_mergedMesh;
	}

	Mesh MergeMeshes(){
		List<MeshFilter> linkedMeshes = new List<MeshFilter>();
		Mesh resultMesh = new Mesh();

		// Add mesh of _base model
		linkedMeshes.Add(GetComponent<MeshFilter>());

		// Add meshes of all independent (!= turret) parts recursively
		GetChildsRecursively<Part>().ForEach(part =>
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





	/*
	public string SerializeUnit(int _indent, string _indenter) {
		string xmlValue = "";
		
		xmlValue += IndentLine("<" + GetType() + "> \n", _indent, _indenter);
		
		xmlValue += IndentLine("<ID value=" + m_id + "/> \n", _indent + 1, _indenter);
		
		xmlValue += IndentLine("<Childs> \n", _indent + 1, _indenter);
		foreach(Part part in GetAllChilds<Part>()) { xmlValue += part.SerializeUnit(_indent + 2, _indenter) ; }
		xmlValue += IndentLine("</Childs> \n", _indent + 1, _indenter);
		
		xmlValue += IndentLine("</" + GetType() + "> \n", _indent, _indenter);
		
		return xmlValue;
	}
	*/


	public XmlNode SerializeUnit(XmlDocument _document){
		
		XmlElement currentNode = _document.CreateElement(GetType().ToString());
		currentNode.SetAttribute("ID", m_id.ToString());
		
		foreach(Slot slot in m_slots){
			Part part = slot.GetPart<Part>();
			
			if(part == null){
				XmlElement noChild = _document.CreateElement("NONE");
				noChild.SetAttribute("ID", "-1");
				currentNode.AppendChild(noChild);
			}else{
				currentNode.AppendChild(part.SerializeUnit(_document));
			}
		}
		
		return currentNode;
	}




	public string ToXML(int _indent = 0, string _indenter = "   "){
		
		string xmlValue = "";
		
		// Serialises Transform
		string transformValue = "<Transform";
		transformValue += " Position=" + transform.position;
		transformValue += " Rotation=" + transform.rotation;
		transformValue += " Scale=" + transform.localScale;
		transformValue += "/> \n";
		xmlValue	+= IndentLine(transformValue, _indent + 1, _indenter);
		
		xmlValue += IndentLine("<Mesh=" + MergeMeshes().GetInstanceID() + "/> \n", _indent + 1, _indenter);
		
		// Serialises Prefab
		string prefabValue = "<Prefab";
		prefabValue += " Health=" + m_mergedHealth;
		prefabValue += " Weight=" + m_mergedWeight;
		prefabValue += " Cost=" + m_mergedCost;
		prefabValue += "/> \n";
		xmlValue	+= IndentLine(prefabValue, _indent + 1, _indenter);
		
		
		// Serialises all attached Turrets
		xmlValue += IndentLine("<TurretList> \n", _indent + 1, _indenter);
		foreach(Turret turret in GetChildsRecursively<Turret>()) { xmlValue += turret.ToXML(_indent + 2, _indenter) ; }
		xmlValue += IndentLine("</TurretList> \n", _indent + 1, _indenter);
		
		// Serialises all attached Weapons
		xmlValue += IndentLine("<WeaponList> \n", _indent + 1, _indenter);
		foreach(Weapon weapon in GetChildsRecursively<Weapon>()) { xmlValue += weapon.ToXML(_indent + 2, _indenter); }
		xmlValue += IndentLine("</WeaponList> \n", _indent + 1, _indenter);
		
		
		return xmlValue;
	}
	
	
	
	protected string IndentLine(string _text, int _indent, string _indenter){
		string indentedLine = "";
		
		for(int i = 0; i < _indent; ++i){ indentedLine += _indenter; }
		
		return (indentedLine + _text);
	}
}


