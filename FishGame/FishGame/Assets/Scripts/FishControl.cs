using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FishControl : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent m_navmeshAgent;
    [HideInInspector] public Animator m_animator;
    public List<Vector3> m_listTarget;

    public AudioSource m_audioSource;
    public AudioClip m_clipWin;
    public AudioClip m_clipFail;

    private void Awake()
    {
        m_navmeshAgent = GetComponent<NavMeshAgent>();
        m_animator = transform.GetChild(0).GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        ResetFishAI();
        GameControl.Instance.m_bubbleFxFishPink.SetActive(true);
        GameControl.Instance.m_bubbleFxFishPink.transform.position = transform.position + new Vector3(0.5f, 1.5f, 0.5f);
    }

    private void Update()
    {
        if (!GameControl.Instance.m_isComplete && !GameControl.Instance.m_isFail)
        {
            if (m_listTarget.Count > 0 && UiManager.Instance.m_panelPlay.activeSelf)
            {
                if (Vector3.Distance(transform.position, PutSandControl.Instance.m_fishEndControl.transform.position) < 1.5f)
                {
                    if (m_navmeshAgent.remainingDistance <= 1.5f)
                    {
                        m_audioSource.clip = m_clipWin;
                        m_audioSource.Play();
                        //m_effect.gameObject.SetActive(false);
                        transform.LookAt(m_listTarget[0]);
                        GameControl.Instance.m_isComplete = true;
                        m_animator.SetTrigger("Happy");
                        m_animator.Play("Happy");
                        StartCoroutine(PutSandControl.Instance.m_fishEndControl.WaitTimeToEat(transform.position));
                        StartCoroutine(ProcessComplete(PutSandControl.Instance.m_fishEndControl.transform.position));
                    }
                    
                }
                else
                {
                    if (m_navmeshAgent.isOnNavMesh && !m_navmeshAgent.isStopped && m_navmeshAgent.remainingDistance <= m_navmeshAgent.stoppingDistance)
                    {
                        //m_navmeshAgent.isStopped = true;
                        GameControl.Instance.LookAtTarget(gameObject, PutSandControl.Instance.m_fishEndControl.transform.position, 200f);
                        //m_animator.Play("Idle");
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Navigate();
            }
        }
        else
        {
            if (m_navmeshAgent.isOnNavMesh && !m_navmeshAgent.isStopped)
            {
                //m_effect.gameObject.SetActive(false);
                m_navmeshAgent.isStopped = true;
            }
        }
        
    }

    public void Navigate()
    {
        if (m_navmeshAgent.isActiveAndEnabled && m_navmeshAgent.isOnNavMesh)
        {
            NavMeshPath path = new NavMeshPath();
            //foreach (var item in m_listTarget)
            {
                if (m_navmeshAgent.CalculatePath(m_listTarget[0], path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    //m_effect.gameObject.SetActive(true);
                    m_animator.Play("Swim");
                    m_animator.SetFloat("Speed",2);
                    m_navmeshAgent.isStopped = false;
                    m_navmeshAgent.SetPath(path);
                    //m_navmeshAgent.SetDestination(item.position);
                    GameControl.Instance.m_bubbleFxFishPink.SetActive(false);
                    //break;
                }
                else
                {
                    //m_navmeshAgent.SetDestination(m_listTarget[0]);
                }
            }
        }
    }

    public void ResetFishAI()
    {
        //m_effect.transform.parent.gameObject.SetActive(true);
        //m_effect.gameObject.SetActive(false);
        m_navmeshAgent.enabled = true;
    }

    float m_speed;
    public void MoveFollowMouse(Vector3 target)
    {
        m_navmeshAgent.enabled = false;
        float distance = Vector3.Distance(transform.position, target);
        if (distance > 1f)
        {
            GameControl.Instance.LookAtTarget(gameObject, target, 20f);
            if (!Physics.Raycast(transform.position, transform.forward, 0.5f, LayerMask.GetMask("Sand")))
            {
                m_speed = GetSpeedByDistance(distance);
                m_animator.SetFloat("Speed", m_speed);
            }
            else
            {
                if (m_speed >0f)
                {
                    m_speed -= Time.deltaTime * 10f;
                }
                else
                {
                    m_speed = 0f;
                }
            }

            transform.position += transform.forward * m_speed * Time.deltaTime;

            //if (m_navmeshAgent.isOnNavMesh)
            //{
            //    m_navmeshAgent.Move(movement);
            //}
        }
    }

    private float GetSpeedByDistance(float distance)
    {
        return distance * 2f;
    }

    public void OnDeath(float time = 0f, bool isActive = true)
    {
        GameControl.Instance.m_bubbleFxFishPink.SetActive(false);
        GameControl.Instance.m_isFail = true;
        StartCoroutine(WaitToDeath(time, isActive));
    }

    IEnumerator WaitToDeath(float time = 0f, bool isActive = true)
    {
        //m_audioSource.clip = m_clipFail;
        //m_audioSource.Play();
        yield return Yielders.Get(time);
        if (PlayerOptions.GetVibrate() == 1)
        {
            Vibration.Vibrate(60);
        }
        if (isActive)
        {
            m_animator.Play("Death");
            m_animator.SetTrigger("Death");
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    private IEnumerator ProcessComplete(Vector3 _point)
    {
        GameControl.Instance.LookAtTarget(gameObject, _point, 200f);
        yield return Yielders.Get(0.5f);
        GameControl.Instance.LookAtTarget(gameObject, _point, 200f);
        m_animator.ResetTrigger("Happy");
        m_animator.Play("Swim");
        SimplePool.Spawn("Splash", transform.position, Quaternion.identity);
        while (Vector3.Distance(transform.position, _point) > 0.5f)
        {
            transform.position += transform.forward.normalized * Time.deltaTime;
            yield return null;
        }
        float speedRot = 10f;
        float t = 1.0f;
        bool check = true;
        while (gameObject.activeSelf)
        {
            if (check)
            {
                if (t > 0)
                {
                    t -= Time.deltaTime;
                }
                else
                {
                    check = false;
                    StartCoroutine(PutSandControl.Instance.m_fishEndControl.SwimAround(_point));
                }
            }
            
            if (speedRot < 100f)
            {
                speedRot += Time.deltaTime * speedRot;
            }
            transform.RotateAround(_point, Vector3.up, 200f * Time.deltaTime);
            Vector3 _lookDirection = _point - transform.position;
            _lookDirection = Vector3.Cross(_lookDirection.normalized, Vector3.up);
            Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, _rot, speedRot * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator WaitTimeToNavigate()
    {
        yield return Yielders.EndOfFrame;
        yield return Yielders.EndOfFrame;
        m_navmeshAgent.enabled = true;
        Navigate();
    }

    public void FindLove()
    {
        StartCoroutine(WaitTimeToNavigate());
    }
}
