using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalFind : TrapFind
{
    public List<GameObject> m_listTarget;
    public GameObject m_currentTarget;
    private bool m_checkPlay;

    [HideInInspector] public Vector3 m_firstPoint;
    [HideInInspector] public Vector3 m_endPoint;
    public float m_lineDistance;

    private Vector3 m_tempTarget;

    public bool m_checkDie;

    private BoxCollider m_collider;

    public LineDirection m_lineDirection;

    public AudioClip m_clipAttack;
    public AudioSource m_audioSource;

    public override void Awake()
    {
        base.Awake();
        if (m_animator == null)
        {
            m_animator = transform.GetChild(0).GetComponent<Animator>();
        }
        m_collider = GetComponent<BoxCollider>();
        m_audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        m_checkActive = true;
        m_navmeshAgent.enabled = true;
        m_listTarget = new List<GameObject>();
        if (GameControl.Instance)
        {
            foreach (var item in GameControl.Instance.m_curentLevelControl.m_listAnimalFind)
            {
                m_listTarget.Add(item.gameObject);
            }
            foreach (var item in GameControl.Instance.m_curentLevelControl.m_listHuman)
            {
                m_listTarget.Add(item.gameObject);
            }
            m_listTarget.Add(PutSandControl.Instance.m_fishEndControl.gameObject);
            m_listTarget.Add(PutSandControl.Instance.m_fishControl.gameObject);
        }
        m_currentTarget = null;
        m_checkPlay = false;
        m_tempTarget = m_endPoint;
        m_checkDie = false;
        m_collider.enabled = true;
        m_animator.SetFloat("Speed", 1);
    }

    public override void OnActivated()
    {
        if (m_currentTarget == null && !m_checkDie)
        {
            StartCoroutine(Activated());
        }
    }

    IEnumerator Activated()
    {
        yield return Yielders.Get(0.1f);
        float distance = 1000f;
        bool check = false;
        for (int i = m_listTarget.Count - 1; i >= 0; i--)
        {
            if (m_listTarget[i] != gameObject)
            {
                NavMeshPath path = new NavMeshPath();
                Vector3 tempPos = m_listTarget[i].transform.position;
                if (m_navmeshAgent.isOnNavMesh && m_navmeshAgent.CalculatePath(tempPos, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    m_navmeshAgent.isStopped = false;
                    float d = CalculatePathDistance(path);
                    if (d < distance )
                    {
                        if (m_listTarget[i].tag.Equals("Human") && !m_listTarget[i].GetComponent<HumanControl>().m_checkSwim)
                        {

                        }
                        else
                        {
                            distance = d;
                            m_currentTarget = m_listTarget[i];
                            m_tempTarget = m_listTarget[i].transform.position;
                            m_checkPlay = true;
                            m_animator.SetFloat("Speed", 2);
                            m_navmeshAgent.SetPath(path);
                        }
                        
                    }
                    check = true;
                }
            }
            if (check && i == m_listTarget.Count - 2)
            {
                break;
            }
        }
    }

    bool m_checkActive;

    public override void Update() 
    {
        if (/*!GameControl.Instance.m_checkComplete*/ m_checkActive && !m_checkDie)
        {
            if (GameControl.Instance.m_isComplete)
            {
                m_checkActive = false;
                if (m_navmeshAgent.isOnNavMesh && m_navmeshAgent.isActiveAndEnabled)
                {
                    m_navmeshAgent.isStopped = true;
                }

            }
            if (m_checkPlay)
            {
                if (m_currentTarget != null)
                {

                    if (Vector3.Distance(m_tempTarget, transform.position) < 0.5f)
                    {
                        m_tempTarget = m_currentTarget.transform.position;
                        if (m_navmeshAgent.isOnNavMesh)
                        {
                            m_navmeshAgent.SetDestination(m_tempTarget);
                        }
                    }

                    float _distance = 10f;
                    if (m_navmeshAgent.isActiveAndEnabled && m_navmeshAgent.isOnNavMesh)
                    {
                        //_distance = m_navmeshAgent.remainingDistance;
                        _distance = Vector3.Distance(transform.position, m_currentTarget.transform.position);
                    }
                    switch (m_currentTarget.tag)
                    {
                        case "Player":
                            {
                                if (_distance <= 1.5f)
                                {
                                    NavMeshPath path = new NavMeshPath();
                                    m_tempTarget = m_currentTarget.transform.position;
                                    if (m_navmeshAgent.isOnNavMesh && m_navmeshAgent.CalculatePath(m_tempTarget, path) && path.status == NavMeshPathStatus.PathComplete)
                                    {
                                        if (CalculatePathDistance(path) < 0.8f)
                                        {
                                            OnAttack();
                                            UiManager.Instance.OpenPanel("Fail", 0.8f);
                                            if (m_currentTarget == PutSandControl.Instance.m_fishControl.gameObject)
                                            {
                                                PutSandControl.Instance.m_fishControl.OnDeath(0.1f);
                                                GameControl.Instance.SpawnSmoke(PutSandControl.Instance.m_fishControl.gameObject, 0.1f);
                                            }
                                            else
                                            {
                                                PutSandControl.Instance.m_fishEndControl.OnDeath(0.1f);
                                                GameControl.Instance.SpawnSmoke(PutSandControl.Instance.m_fishEndControl.gameObject, 0.1f);
                                            }
                                        }
                                        else
                                        {
                                            m_navmeshAgent.SetPath(path);
                                        }
                                    }       
                                }
                            }
                            break;
                        case "Enemy":
                            {
                                if (_distance <= 1.5f)
                                {
                                    NavMeshPath path = new NavMeshPath();
                                    m_tempTarget = m_currentTarget.transform.position;
                                    if (m_navmeshAgent.isOnNavMesh && m_navmeshAgent.CalculatePath(m_tempTarget, path) && path.status == NavMeshPathStatus.PathComplete)
                                    {
                                        if (CalculatePathDistance(path) < 1.0f)
                                        {
                                            OnAttack();
                                            GameControl.Instance.LookAtTarget(gameObject, m_currentTarget.transform.position, 50f);
                                            StartCoroutine(Test((transform.position + m_currentTarget.transform.position)/2f));
                                        }
                                        else
                                        {
                                            m_navmeshAgent.SetPath(path);
                                        }
                                    }

                                }
                            }
                            break;
                        case "Human":
                            {
                                if (_distance <= 2.0f)
                                {
                                    NavMeshPath path = new NavMeshPath();
                                    m_tempTarget = m_currentTarget.transform.position;
                                    if (m_navmeshAgent.isOnNavMesh && m_navmeshAgent.CalculatePath(m_tempTarget, path) && path.status == NavMeshPathStatus.PathComplete)
                                    {
                                        if (CalculatePathDistance(path) < 1.0f)
                                        {
                                            OnAttack();
                                            m_currentTarget.GetComponent<HumanControl>().OnDeath(transform.position);
                                        }
                                        else
                                        {
                                            m_navmeshAgent.SetPath(path);
                                        }
                                    }
 
                                }
                            }
                            break;
                    }
                }
            }
            else
            { 
                base.Update();
                MoveLoop();

                //foreach (var item in GameControl.Instance.m_curentLevelControl.m_listHuman)
                //{
                //    if (item.m_checkSwim && m_navmeshAgent.isOnNavMesh  && m_navmeshAgent.SetDestination(item.transform.position))
                //    {
                //        m_checkPlay = true;
                //        break;
                //    }
                //}
            }
        }
    }

    public void MoveLoop()
    {
        if (m_navmeshAgent.isOnNavMesh)
        {
            if (Vector3.Distance(transform.position, m_tempTarget) > 0.5f)
            {
                MoveFollowTarget(m_tempTarget, 1f);
                //OnActivated();
            }
            else
            {
                if (m_tempTarget == m_firstPoint)
                {
                    m_tempTarget = m_endPoint;
                }
                else
                {
                    m_tempTarget = m_firstPoint;
                }
            }
        }

    }

    private float CalculatePathDistance(NavMeshPath path)
    {
        float distance = .0f;
        for (var i = 0; i < path.corners.Length - 1; i++)
        {
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return distance;
    }

    public void OnDeath(float time = 0f, bool isActive = true)
    {
        StartCoroutine(WaitToDeath(time, isActive));
    }

    private IEnumerator WaitToDeath(float time, bool isActive = true)
    {
        yield return Yielders.Get(time);
        if (!isActive)
        {
            gameObject.SetActive(false);
        }
        else
        {
            m_navmeshAgent.enabled = false;
            m_checkDie = true;
            m_animator.SetTrigger("Death");
            m_animator.Play("death");
            m_collider.enabled = false;
            StartCoroutine(DeathProcess());
        }

    }

    public void OnAttack()
    {
        GameControl.Instance.LookAtTarget(gameObject, m_currentTarget.transform.position, 50f);
        m_checkActive = false;
        m_navmeshAgent.enabled = false;
        m_animator.SetTrigger("Bang");
        m_animator.Play("bang");
    }

    IEnumerator DeathProcess()
    {
        yield return Yielders.Get(0.5f);
        float time = 0.3f;
        while (time >0)
        {
            time -= Time.deltaTime;
            transform.position += Vector3.down * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Test(Vector3 pos)
    {
        AnimalFind animalFind = m_currentTarget.GetComponent<AnimalFind>();
        //animalFind.m_animator.SetTrigger("Death");
        yield return Yielders.Get(0.1f);
        SimplePool.Spawn("SmokeSushi", pos /*+ Vector3.up * 0.2f*/, Quaternion.identity);
        yield return Yielders.Get(0.5f);
        if (!animalFind.m_checkDie)
        {
            animalFind.m_animator.Play("death");
            animalFind.OnDeath();
        }

    }

}
