using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum BearState
{
    ON_SAND, ON_WATER
}
public class BearControl : MonoBehaviour
{
    [HideInInspector] public Rigidbody m_rigidbody;
    [HideInInspector] public Animator m_animator;
    [HideInInspector] public CapsuleCollider m_collider;
    [HideInInspector] public NavMeshAgent m_navmeshAgent;
    public List<GameObject> m_listTarget;
    [HideInInspector] public BearState m_bearState = BearState.ON_SAND;

    public float m_maxDistanceFind = 3f;

    public GameObject m_currentTarget;
    private bool m_checkPlay;

    private Vector3 m_startPos;
    bool m_checkAttack;
    private Vector3 m_tempTarget;

    public AudioSource m_audioSource;
    public AudioClip m_clipFall;
    public AudioClip m_clipAttack;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = transform.GetChild(0).GetComponent<Animator>();
        m_collider = GetComponent<CapsuleCollider>();
        m_navmeshAgent = GetComponent<NavMeshAgent>();
        m_startPos = transform.position;
        m_audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        ResetBear();
    }

    void Update()
    {
        if (!GameControl.Instance.m_isComplete && !GameControl.Instance.m_isFail && m_checkPlay)
        {
            switch (m_bearState)
            {
                case BearState.ON_SAND:
                    {
                        if (transform.position.y - m_startPos.y < -0.5f)
                        {
                            StartCoroutine(OnStartSwim());
                            m_bearState = BearState.ON_WATER;
                        }
                        else
                        {
                            if (m_checkAttack)
                            {
                                GameObject _target = FindTargetAround(m_maxDistanceFind, Vector3.down);
                                if (_target != null)
                                {
                                    m_currentTarget = _target;
                                    switch (_target.tag)
                                    {
                                        case "Player":
                                            {
                                                GameControl.Instance.LookAtTarget(gameObject, PutSandControl.Instance.m_fishControl.transform.position, 50f);
                                                m_animator.Play("Paw_Right");
                                                GameControl.Instance.m_isFail = true;
                                                UiManager.Instance.OpenPanel("Fail", 0.8f);
                                                //PutSandControl.Instance.m_fishControl.m_animator.SetTrigger("Death");
                                                PutSandControl.Instance.m_fishControl.OnDeath(0.3f);
                                                GameControl.Instance.SpawnSmoke(PutSandControl.Instance.m_fishControl.gameObject, 0.3f);
                                                //StartCoroutine(WaitShowSplash(PutSandControl.Instance.m_fishControl.gameObject));
                                            }
                                            break;
                                        case "Enemy":
                                            {
                                                AnimalFind _animalFind = _target.GetComponent<AnimalFind>();
                                                if (!_animalFind.m_checkDie)
                                                {
                                                    GameControl.Instance.LookAtTarget(gameObject, _target.transform.position, 50f);
                                                    m_animator.Play("Paw_Right");
                                                    _animalFind.OnDeath(0.2f);
                                                    m_listTarget.Remove(_target.gameObject);
                                                    StartCoroutine(WaitAttack());
                                                    //StartCoroutine(WaitShowSplash(_target.gameObject));
                                                    GameControl.Instance.SpawnSmoke(_animalFind.gameObject, 0.2f);
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case BearState.ON_WATER:
                    {
                        if (m_navmeshAgent.enabled)
                        {
                            if (Input.GetMouseButtonUp(0))
                            {
                                if (m_currentTarget == null)
                                {
                                    StartCoroutine(FindAndMoveToTarget());
                                    //FindAndMoveToTarget();
                                }
                            }

                            if (m_currentTarget != null)
                            {
                                if (Vector3.Distance(m_tempTarget, transform.position) < 1.5f)
                                {
                                    m_tempTarget = m_currentTarget.transform.position;
                                    m_navmeshAgent.SetDestination(m_tempTarget);
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
                                            if (_distance <= 3.5f)
                                            {
                                                NavMeshPath path = new NavMeshPath();
                                                m_tempTarget = m_currentTarget.transform.position;
                                                if (m_navmeshAgent.isOnNavMesh && m_navmeshAgent.CalculatePath(m_tempTarget, path) && path.status == NavMeshPathStatus.PathComplete)
                                                {
                                                    if (CalculatePathDistance(path) < 2.0f)
                                                    {
                                                        GameControl.Instance.m_isComplete = true;
                                                        UiManager.Instance.OpenPanel("Fail", 0.8f);
                                                        if (m_currentTarget == PutSandControl.Instance.m_fishControl.gameObject)
                                                        {
                                                            //StartCoroutine(GameControl.Instance.SpawnSmoke(m_currentTarget.transform.position, 0.1f));
                                                            PutSandControl.Instance.m_fishControl.OnDeath(0.3f);
                                                            GameControl.Instance.SpawnSmoke(PutSandControl.Instance.m_fishControl.gameObject, 0.3f);
                                                        }
                                                        else
                                                        {
                                                            PutSandControl.Instance.m_fishEndControl.OnDeath(0.3f);
                                                            GameControl.Instance.SpawnSmoke(PutSandControl.Instance.m_fishEndControl.gameObject, 0.3f);
                                                        }
                                                        m_animator.SetTrigger("Swim");
                                                        ProcessAttack();
                                                        GameControl.Instance.OnSpawnSmokeFighting(transform.position);
                                                    }
                                                    else
                                                    {
                                                        m_navmeshAgent.SetPath(path);
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Bear":
                                        {
                                            if (_distance <= 4.5f)
                                            {
                                                NavMeshPath path = new NavMeshPath();
                                                m_tempTarget = m_currentTarget.transform.position;
                                                if (m_navmeshAgent.isOnNavMesh && m_navmeshAgent.CalculatePath(m_tempTarget, path) && path.status == NavMeshPathStatus.PathComplete)
                                                {
                                                    if (CalculatePathDistance(path) < 2.5f)
                                                    {
                                                        GameControl.Instance.OnSpawnSmokeFighting(transform.position);
                                                        ProcessAttack();
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
                                            if (_distance <= 2.5f)
                                            {
                                                NavMeshPath path = new NavMeshPath();
                                                m_tempTarget = m_currentTarget.transform.position;
                                                if (m_navmeshAgent.isOnNavMesh && m_navmeshAgent.CalculatePath(m_tempTarget, path) && path.status == NavMeshPathStatus.PathComplete)
                                                {
                                                    if (CalculatePathDistance(path) < 2.0f)
                                                    {
                                                        m_currentTarget.GetComponent<AnimalFind>().OnDeath(0.3f);
                                                        m_animator.SetTrigger("Swim");
                                                        ProcessAttack();
                                                        //m_checkPlay = true;
                                                        GameControl.Instance.SpawnSmoke(m_currentTarget.gameObject, 0.3f);
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
                                            if (_distance <= 2.5f)
                                            {
                                                m_currentTarget.GetComponent<HumanControl>().OnDeath((transform.position + m_currentTarget.transform.position) / 2f);
                                                //m_animator.SetTrigger("Swim");
                                                ProcessAttack();
                                                //m_checkPlay = true;
                                                //m_navmeshAgent.enabled = true;
                                            }
                                        }
                                        break;

                                }

                            }
                        }
                    }
                    break;
            }
        }
    }

    void ProcessAttack()
    {
        m_navmeshAgent.enabled = false;
        GameControl.Instance.LookAtTarget(gameObject, m_currentTarget.transform.position, 10f);
        m_animator.Play("Paw_Left");
        m_checkPlay = false;
    }

    IEnumerator SetUpTarget()
    {
        m_listTarget = new List<GameObject>();
        yield return Yielders.Get(0.5f);
        m_listTarget.Add(PutSandControl.Instance.m_fishControl.gameObject);
        m_listTarget.Add(PutSandControl.Instance.m_fishEndControl.gameObject);
        foreach (var item in GameControl.Instance.m_curentLevelControl.m_listAnimalFind)
        {
            m_listTarget.Add(item.gameObject);
        }
        foreach (var item in GameControl.Instance.m_curentLevelControl.m_listHuman)
        {
            m_listTarget.Add(item.gameObject);
        }
        foreach (var item in GameControl.Instance.m_curentLevelControl.m_listBear)
        {
            if (!item.gameObject.Equals(gameObject))
            {
                m_listTarget.Add(item.gameObject);
            }
        }
        m_checkPlay = true;
    }

    IEnumerator OnStartSwim()
    {
        m_collider.isTrigger = true;
        m_audioSource.clip = m_clipFall;
        m_audioSource.Play();
        while (transform.position.y > -0.4f)
        {
            yield return null;
        }
        m_animator.Play("Idle_Swim");
        m_rigidbody.isKinematic = true;
        transform.position = new Vector3(transform.position.x, -0.48f, transform.position.z);
        SimplePool.Spawn("Splash", transform.position, Quaternion.identity);
        m_navmeshAgent.enabled = true;
        if (m_currentTarget == null)
        {
            StartCoroutine(FindAndMoveToTarget());
            //FindAndMoveToTarget();
        }
    }

    public IEnumerator FindAndMoveToTarget()
    {
        yield return Yielders.Get(0.3f);
        float distance = 1000f;
        bool check = false;
        for (int i = 0; i < m_listTarget.Count; i++)
        {
            if (m_listTarget[i] != gameObject)
            {
                NavMeshPath path = new NavMeshPath();
                Vector3 tempPos = m_listTarget[i].transform.position;
                if (m_navmeshAgent.isOnNavMesh && m_navmeshAgent.CalculatePath(tempPos, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    m_navmeshAgent.isStopped = false;
                    float d = CalculatePathDistance(path);
                    if (d < distance)
                    {
                        distance = d;
                        m_currentTarget = m_listTarget[i];
                        m_tempTarget = m_currentTarget.transform.position;
                        m_checkPlay = true;
                        //m_animator.Play("swim");
                        m_navmeshAgent.SetPath(path);
                    }
                    check = true;
                }
            }
            if (check && i == 1)
            {
                break;
            }
        }
        if (!check)
        {
            StartCoroutine(FindAndMoveToTarget());
        }
    }

    IEnumerator WaitAttack()
    {
        m_checkAttack = false;
        yield return Yielders.Get(1.0f);
        m_checkAttack = true;
        m_currentTarget = null;
    }

    public void ResetBear()
    {
        m_collider.enabled = true;
        m_checkAttack = true;
        m_checkPlay = false;
        m_bearState = BearState.ON_SAND;
        m_navmeshAgent.enabled = false;
        m_collider.isTrigger = false;
        m_rigidbody.isKinematic = false;
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;
        m_animator.Play("Idle");
        m_currentTarget = null;
        StartCoroutine(SetUpTarget());
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

    public void EventPlaySoundAttack()
    {
        m_audioSource.clip = m_clipAttack;
        m_audioSource.Play();
    }

    public void EventShowSplash()
    {
        Vector3 pos = m_currentTarget.transform.position;
        pos.y = 0f;
        SimplePool.Spawn("Splash", pos, Quaternion.identity);
    }

    RaycastHit hit;
    public GameObject FindTargetAround(float _radius, Vector3 dir)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radius, LayerMask.GetMask("Fish"));
        if (hitColliders.Length > 0)
        {
            return hitColliders[0].gameObject;
        }
        return null;
    }

    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position + Vector3.down * 1, m_maxDistanceFind);
    //}

    public void OnDeath()
    {
        m_collider.enabled = false;
        m_animator.Play("Death");
        m_checkPlay = false;
        SimplePool.Spawn("SmokeSushi", transform.position + transform.forward, Quaternion.identity);
        //GameControl.Instance.SpawnSmoke(gameObject);
    }

}
