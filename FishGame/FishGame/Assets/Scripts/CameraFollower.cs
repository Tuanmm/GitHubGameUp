using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollower : MonoBehaviour
{
	public static CameraFollower Instance { get; set; }

	public float m_followSpeed = 10;
	public float m_lookSpeed = 50;

	public Transform m_objectToFollow;
	private Vector3 m_startPos;
	public float m_offsetZ;

    private void Awake()
    {
        if (Instance == null)
        {
			Instance = this;
        }
		m_startPos = transform.position;

	}

	public void ResetCamPos()
	{
		transform.position = m_startPos;
	}

	public void MoveToTarget()
	{
        if (m_objectToFollow.transform.position.z - transform.position.z > m_offsetZ)
        {

        }
        else
        {
			transform.position -= Vector3.forward * m_followSpeed * Time.deltaTime;
		}
    }
}