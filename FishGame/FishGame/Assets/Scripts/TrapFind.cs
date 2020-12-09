using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrapFind : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent m_navmeshAgent;
    [HideInInspector] public Animator m_animator;

    public virtual void Awake()
    {
        m_navmeshAgent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
    }

    public virtual void Update()
    {
        if (!GameControl.Instance.m_isComplete && !GameControl.Instance.m_isFail)
        {
            if (Input.GetMouseButtonUp(0))
            {
                OnActivated();
            }
        }
    }

    public virtual void OnActivated()
    {

    }

    public void MoveFollowTarget(Vector3 target, float speed = 2f)
    {
        float distance = Vector3.Distance(transform.position, target);
        m_animator.SetTrigger("Swim");
        m_animator.Play("swim");
        if (distance > 0.5f)
        {
            LookMouse(target, distance);
            m_animator.SetFloat("Speed", speed);
            Vector3 movement = transform.forward * Time.deltaTime * speed;
            m_navmeshAgent.Move(movement);
        }
    }

    private float GetSpeedByDistance(float distance)
    {
        return distance * 2f;
    }

    public void LookMouse(Vector3 point, float distance)
    {
        Vector3 _lookDirection = point - transform.position;
        Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rot, Time.deltaTime * 10 / distance);
    }
}