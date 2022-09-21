using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PartDatabase : MonoBehaviour
{
	[SerializeField] List<GameObject> m_partPrefabList;
	
	// Awake is called when the script instance is being loaded.
	protected void Awake()
	{
		for(int i = 0; i < m_partPrefabList.Count; ++i){
			m_partPrefabList[i].GetComponent<Part>().m_id = i;
		}
	}
	
	public GameObject GetPrefab(int m_id){
		return m_partPrefabList.Single(part => part.GetComponent<Part>().m_id == m_id);
	}
}
