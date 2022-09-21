using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slot : MonoBehaviour
{
    // Slot Characteristics
    public PartType m_type;
    public PartSize m_size;

    // Lateral Movement
    [SerializeField] private float m_clockwise;
    [SerializeField] private float m_counterClockwise;

    // Vertical Movement
    [SerializeField] private float m_up;
    [SerializeField] private float m_down;

    // Attached Part
    [SerializeField] private Part m_part;

    public T GetPart<T>() where T : Part
    {
        if(m_part is T)
        {
            return (T)m_part;
        }
        else
        {
	        return null;
        }
    }

    public void SetPart<T>(T _part) where T : Part
    {
        if(m_part != null)
        {
            //throw new System.Exception("Slot already filled");
            Destroy(m_part.gameObject);
        }

        if (_part.m_type == m_type && _part.m_size == m_size)
        {
            m_part = Instantiate(_part, transform);
        }
        else
        {
            throw new System.Exception("Part not compatible with slot");
        } 
    }



    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, new Vector3(0.2f, 0.2f, 0.2f));
    }
    
}
