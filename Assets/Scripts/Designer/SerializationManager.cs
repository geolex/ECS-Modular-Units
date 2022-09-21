using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class SerializationManager
{
	static SerializationManager m_instance = null;
	public static SerializationManager Instance()
	{
		if(m_instance == null){
			m_instance = new SerializationManager();
		}
		
		return m_instance;
	}
	
	PartDatabase m_partDatabase = null;
	
	public SerializationManager()
	{
		m_partDatabase = GameObject.FindObjectOfType<PartDatabase>();
	}
	
	
	
	
	
	
	public void SerializeUnit(Part _unit){
		
	}
	
	
	
	
	public void DeserializeUnit(string _path){
		XmlDocument document = new XmlDocument();
		document.Load(_path);
		
		DeserialiseNode(document.FirstChild);
	}
	
	
	
	private void DeserialiseNode(XmlNode _node, Slot _parent = null){
		
		int id = int.Parse(_node.Attributes["ID"].Value);
		
		if(id == -1){
			return;
		}

		GameObject prefab = Object.Instantiate(m_partDatabase.GetPrefab(id));
		Part part = prefab.GetComponent<Part>();
	
		Debug.Log("Slot : " + _parent + " - Part : " + part);
	
		if(_parent != null){
			_parent.SetPart<Part>(part); 
		}
	
		for (int i = 0; i < _node.ChildNodes.Count; ++i){
			DeserialiseNode(_node.ChildNodes[i], part.GetSlot(i));
		}
		
	}
	
}
