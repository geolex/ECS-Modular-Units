using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] float m_panSpeed;
    [SerializeField] float m_zoomSpeed;

    [Header("Linked Camera")]
    [SerializeField] Camera m_orbitalCamera;


    float m_horizontalRotation, m_verticalRotation = 0f;
    Vector3 m_currentEulerAngle = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Camera Panning
	    if (Input.GetMouseButton(1))
        {
            if (Mathf.Abs(Input.GetAxis("Mouse X")) >= 0.1f || Mathf.Abs(Input.GetAxis("Mouse Y")) >= 0.1f)
            {
                m_horizontalRotation += Input.GetAxis("Mouse X");
                m_verticalRotation += Input.GetAxis("Mouse Y");

                m_currentEulerAngle.y = m_horizontalRotation * m_panSpeed;
                m_currentEulerAngle.x = m_verticalRotation * m_panSpeed;

                transform.eulerAngles = m_currentEulerAngle;
            }
        }
        
        // Camera Zooming
        m_orbitalCamera.transform.position += m_orbitalCamera.transform.forward * Input.mouseScrollDelta.y * Time.deltaTime * m_zoomSpeed;
    }
}
