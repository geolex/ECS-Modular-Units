using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : Part
{
    public int m_damage;
    public float m_fireRate;
    public float m_penetration;
    public float m_range;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	public string ToXML(int _indent, string _indenter = "   "){
		string xmlValue = "";
		
		for(int i = 0; i < _indent; ++i){ xmlValue += "   "; }
		xmlValue += "<" + name + "> \n";
		
		// Serialises Part
		xmlValue += base.ToXML(_indent, _indenter);
		
		for(int i = 0; i < _indent; ++i){ xmlValue += _indenter; }
		xmlValue += "   <Weapon";
		xmlValue += " Damage=" + m_damage;
		xmlValue += " FireRate=" + m_fireRate;
		xmlValue += " Penetration=" + m_penetration;
		xmlValue += " Range=" + m_range;
		xmlValue += "/> \n";
		
		for(int i = 0; i < _indent; ++i){ xmlValue += "   "; }
		xmlValue += "</" + name + "> \n";
		
		return xmlValue;
	}
}
