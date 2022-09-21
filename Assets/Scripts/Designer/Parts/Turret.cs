using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Part
{
    public float m_maxRotation;
    public float m_rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
	public string ToXML(int _indent, string _indenter = "   ") {
		string xmlValue = "";
		
		for(int i = 0; i < _indent; ++i){ xmlValue += "   "; }
		xmlValue += "<" + name + "> \n";
		
		// Serialises Part
		xmlValue += base.ToXML(_indent, _indenter);
		
		// Serialises Turret
		for(int i = 0; i < _indent; ++i){ xmlValue += _indenter; }
		xmlValue += "   <Turret";
		xmlValue += " RotationValue=" + m_maxRotation;
		xmlValue += " RotationSpeed=" + m_rotationSpeed;
		xmlValue += "/> \n";
		
		
		// Serialises all attached Turrets
		for(int i = 0; i < _indent; ++i){ xmlValue += _indenter; }
		xmlValue += _indenter + "<TurretList> \n";
			
		foreach(Turret turret in GetChildsRecursively<Turret>())
		{
			xmlValue += turret.ToXML(_indent + 2, _indenter);
		}
			
		for(int i = 0; i < _indent; ++i){ xmlValue += _indenter; }
		xmlValue += _indenter + "</TurretList> \n";
		
		
		
		// Serialises all attached Weapons
		for(int i = 0; i < _indent; ++i){ xmlValue += _indenter; }
		xmlValue += _indenter + "<WeaponList> \n";
			
		foreach(Weapon weapon in GetChildsRecursively<Weapon>())
		{
			xmlValue += weapon.ToXML(_indent + 2, _indenter);
		}
		
		for(int i = 0; i < _indent; ++i){ xmlValue += _indenter; }
		xmlValue += _indenter + "</WeaponList> \n";
		
		
		
		for(int i = 0; i < _indent; ++i){ xmlValue += "   "; }
		xmlValue += "</" + name + "> \n";
		
		return xmlValue;
	}
}
