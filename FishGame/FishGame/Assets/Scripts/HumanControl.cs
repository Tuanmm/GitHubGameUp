using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanControl : MonoBehaviour
{
    [HideInInspector] public Animator m_animator;
    [HideInInspector] public Rigidbody m_rigidbody;
    [HideInInspector] public CapsuleCollider m_collider;
    public float m_maxDistance = 5f;
    public GameObject m_spear;

    public bool m_checkSwim;
    bool m_checkAttack;

    public GameObject m_radar;
    public float m_radiusRadar;
    public float m_radarDistanceCast;
    public HumanStyle m_humanStyle = HumanStyle.STAND;
    private Vector3 m_startPos;

    public GameObject m_effectInWater;

    [Header("Human Stand")]
    public float m_maxAngleScan;
    public float m_speedScan;

    [Header("Human Move")]
    public LineDirection m_lineDirection;
    public float m_lineDistance;
    public float m_speedMove;
    private Vector3 m_pointFirst;
    private Vector3 m_pointSecond;
    private HumanStyle m_startStyle;

    public AudioSource m_audioSource;
    public AudioClip m_clipFall;
    public AudioClip m_clipAttack;

    private void Awake()
    {
        m_animator = transform.GetChild(0).GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();
        m_startPos = transform.position;
        m_startStyle = m_humanStyle;
        m_audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        m_humanStyle = m_startStyle;
        m_checkAttack = true;
        ResetHuman();
        transform.position = m_startPos;
        if (m_humanStyle == HumanStyle.MOVE)
        {
            InitLine(m_lineDirection, transform.position, m_lineDistance);

            m_animator.SetTrigger("Walk");
            m_animator.SetFloat("Speed", m_speedMove);
        }
        m_effectInWater.SetActive(false);
        //m_radar.SetActive(false);
        m_currentTarget = null;
    }

    private void Update()
    {
        if (!m_checkSwim)
        {
            if (transform.position.y < -0.5f)
            {
                StartCoroutine(OnStartSwim());
                m_checkSwim = true;
            }
            if (!GameControl.Instance.m_isComplete && !GameControl.Instance.m_isFail)
            {
                if (m_checkAttack)
                {
                    switch (m_humanStyle)
                    {
                        case HumanStyle.STAND:
                            ScanRadar();
                            FindTarget();
                            break;
                        case HumanStyle.MOVE:
                            MoveHuman();
                            FindTarget();
                            break;
                    }
                }
            }
        }
    }

    float m_deltaAngle = 0;
    float m_dirScan = 1;

    public void ScanRadar()
    {
        if (Mathf.Abs(m_deltaAngle) < m_maxAngleScan)
        {
            m_deltaAngle += m_speedScan * Time.deltaTime;
            transform.localEulerAngles += Vector3.up * m_speedScan * Time.deltaTime * m_dirScan;
        }
        else
        {
            m_deltaAngle = 0;
            m_dirScan *= -1;
        }
    }

    GameObject m_currentTarget;
    public void FindTarget()
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, m_radiusRadar, transform.forward, out hit, m_radarDistanceCast, LayerMask.GetMask("Fish")))
        {
            if (hit.transform.tag.Equals("Player"))
            {
                m_checkAttack = false;
                transform.LookAt(hit.point);
                m_animator.Play("Attack");
                GameControl.Instance.m_isComplete = true;
                PutSandControl.Instance.m_fishControl.OnDeath(0.2f);
                GameControl.Instance.SpawnSmoke(hit.transform.gameObject,0.2f);
                GameControl.Instance.OnSpawnSushi(hit.transform.gameObject, 0.2f);
                UiManager.Instance.OpenPanel("Fail", 0.8f);
                m_animator.SetTrigger("Idle");
                m_currentTarget = PutSandControl.Instance.m_fishControl.gameObject;
                return;
            }
            if (hit.transform.tag.Equals("Enemy"))
            {
                AnimalFind animalFind = hit.transform.GetComponent<AnimalFind>();
                if (!animalFind.m_checkDie)
                {
                    GameControl.Instance.LookAtTarget(gameObject, hit.point, 200f);
                    //transform.LookAt(hit.point);
                    m_animator.Play("Attack");
                    switch (m_humanStyle)
                    {
                        case HumanStyle.STAND:
                            m_animator.SetTrigger("Idle");
                            break;
                        case HumanStyle.MOVE:
                            m_animator.SetTrigger("Walk");
                            break;
                    }
                    animalFind.OnDeath(0.2f);
                    GameControl.Instance.SpawnSmoke(hit.transform.gameObject, 0.2f);
                    GameControl.Instance.OnSpawnSushi(hit.transform.gameObject, 0.2f);
                    StartCoroutine(WaitAttack());
                    m_currentTarget = animalFind.gameObject;
                }
            }
        }
    }

    public void ResetHuman()
    {
        m_spear.SetActive(true);
        m_collider.isTrigger = false;
        m_rigidbody.isKinematic = false;
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;
        StartCoroutine(SetUpTarget());
        m_deltaAngle = 0;
        m_dirScan = 1;
        m_radar.SetActive(true);
        //transform.position = m_startPos;
    }

    IEnumerator OnStartSwim()
    {
        m_collider.isTrigger = true;
        m_radar.SetActive(false);
        m_audioSource.clip = m_clipFall;
        m_audioSource.Play();
        while (transform.position.y > -1.5f)
        {
            yield return null;
        }
        m_rigidbody.isKinematic = true;
        transform.position = new Vector3(transform.position.x, -1.7f, transform.position.z);
        SimplePool.Spawn("Splash", transform.position + Vector3.up * 1.2f, Quaternion.identity);
        m_animator.Play("Swim");
        //m_spear.SetActive(false);
        m_effectInWater.SetActive(true);
        foreach (var item in GameControl.Instance.m_curentLevelControl.m_listAnimalFind)
        {
            item.OnActivated();
        }
        foreach (var item in GameControl.Instance.m_curentLevelControl.m_listBear)
        {
            if (item.m_bearState == BearState.ON_WATER)
            {
                StartCoroutine(item.FindAndMoveToTarget());
            }
        }
    }

    IEnumerator SetUpTarget()
    {
        m_checkSwim = true;
        yield return Yielders.Get(0.5f);
        m_checkSwim = false;
    }

    IEnumerator WaitAttack()
    {
        m_checkAttack = false;
        yield return Yielders.Get(1.0f);
        m_checkAttack = true;
    }

    void InitLine(LineDirection _lineDirection, Vector3 _point, float _distance)
    {
        Vector3 _pointFirst = Vector3.zero;
        Vector3 _pointSecond = Vector3.zero;
        switch (_lineDirection)
        {
            case LineDirection.HORIZONTAL:
                _pointFirst = new Vector3(_point.x - _distance, _point.y, _point.z);
                _pointSecond = new Vector3(_point.x + _distance, _point.y, _point.z);
                break;
            case LineDirection.VERTICAL:
                _pointFirst = new Vector3(_point.x, _point.y, _point.z - _distance);
                _pointSecond = new Vector3(_point.x, _point.y, _point.z + _distance);
                break;
            case LineDirection.DIAGONAL:
                _pointFirst = new Vector3(_point.x - _distance, _point.y, _point.z + _distance);
                _pointSecond = new Vector3(_point.x + _distance, _point.y, _point.z - _distance);
                break;
            case LineDirection.ANTIDIAGONAL:
                _pointFirst = new Vector3(_point.x - _distance, _point.y, _point.z - _distance);
                _pointSecond = new Vector3(_point.x + _distance, _point.y, _point.z + _distance);
                break;
        }
        m_pointFirst = _pointFirst;
        m_pointSecond = _pointSecond;
        m_tempPos = _pointFirst;
    }

    private Vector3 m_tempPos;

    void MoveHuman()
    {
        GameControl.Instance.LookAtTarget(gameObject, m_tempPos, 5f * Time.deltaTime);
        RaycastHit hit;
        if (!Physics.Raycast(transform.position + Vector3.up* 0.1f, (m_tempPos- transform.position).normalized + Vector3.down, out hit, 0.25f, LayerMask.GetMask("Sand")))
        {
            if (m_tempPos == m_pointFirst)
            {
                m_pointFirst = transform.position;
                m_tempPos = m_pointSecond;
            }
            else
            {
                m_pointSecond = transform.position;
                m_tempPos = m_pointFirst;
            }

            if (Vector3.Distance(transform.position, m_tempPos) < 1.0f)
            {
                m_animator.SetTrigger("Idle");
                m_humanStyle = HumanStyle.STAND;
            }
            return;
        }

        if (Vector3.Distance(transform.position , m_tempPos) > 0.25f)
        {
            Vector3 dir = (m_tempPos - transform.position).normalized;
            transform.position += dir * Time.deltaTime * m_speedMove;
        }
        else
        {
            if (m_tempPos == m_pointFirst)
            {
                m_tempPos = m_pointSecond;
            }
            else
            {
                m_tempPos = m_pointFirst;
            }
        }

    }

    public void OnDeath(Vector3 target)
    {
        GameControl.Instance.OnSpawnSmokeFighting(transform.position);
        GameControl.Instance.LookAtTarget(gameObject, target, 200f);
        m_effectInWater.SetActive(false);
        m_animator.Play("Attack_On_Water");
    }

    public void EventShowSplash()
    {
        Vector3 posSpawn = transform.position;
        if (m_currentTarget != null)
        {
            posSpawn = m_currentTarget.transform.position;
            m_currentTarget.SetActive(false);
            m_currentTarget = null;
        }
        else
        {
            posSpawn = transform.position + transform.forward * 1.5f;
        }
        posSpawn.y = 0;
        SimplePool.Spawn("Splash", posSpawn, Quaternion.identity);
    }

    public void EventPlaySoundAttack()
    {
        m_audioSource.clip = m_clipAttack;
        m_audioSource.Play();
    }


    //public float m_maxAngleCast = 60;
    //public int m_numRay = 10;
    //public void TestCast()
    //{
    //    for (int i = 0; i < m_numRay; i++)
    //    {
    //        float angle = i * (m_maxAngleCast / (m_numRay - 1));

    //        Vector3 dir = Quaternion.AngleAxis(angle + (180 - (m_maxAngleCast / 2)), transform.up) * new Vector3(0, 0, -360);
    //        Debug.DrawRay(transform.position , dir * m_maxDistance, Color.blue);
    //    }
    //} 
}
