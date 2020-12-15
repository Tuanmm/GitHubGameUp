using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSawControl : MonoBehaviour
{
    public float m_lineDistance;
    public float m_speedMove;
    public Vector3 m_pointFirst;
    public Vector3 m_pointSecond;
    public float m_speedRot = 400f;


    private Vector3 m_startPos;
    private Vector3 m_startRot;
    Vector3 m_tempPos;

    private void Awake()
    {
        m_startPos = transform.position;
        m_startRot = transform.localEulerAngles;
    }

    private void OnEnable()
    {
        transform.position = m_startPos;
        transform.localEulerAngles = m_startRot;
        InitLine(m_lineDistance);
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        Vector3 angle = m_startRot;
        while (gameObject.activeSelf)
        {
            angle.z += Time.deltaTime * m_speedRot;
            if (angle.z > 360f)
            {
                angle.z = 0;
            }
            transform.localEulerAngles = angle;
            yield return null;
        }
    }
    RaycastHit hit;
    void Update()
    {
        if (Vector3.Distance(transform.position, m_tempPos) > 0.25f)
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
        if (!GameControl.Instance.m_isComplete && !GameControl.Instance.m_isFail)
        {
            MyCollisions();
            //OnHit();
        }

    }

    void InitLine(float _distance)
    {
        m_pointFirst = transform.position + transform.right * _distance;
        m_pointSecond = transform.position - transform.right * _distance;
        m_tempPos = m_pointFirst;
    }

    void MyCollisions()
    {
        Quaternion quaternion = Quaternion.LookRotation(transform.right, Vector3.up);
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(0.1f, 1f, 1.5f), quaternion, LayerMask.GetMask("Fish", "Bear"));

        if (hitColliders.Length >0)
        {
            switch (hitColliders[0].transform.tag)
            {
                case "Player":
                    {
                        GameControl.Instance.m_isComplete = true;
                        UiManager.Instance.OpenPanel("Fail", 0.8f);
                        if (hitColliders[0].transform.gameObject == PutSandControl.Instance.m_fishControl.gameObject)
                        {
                            //StartCoroutine(GameControl.Instance.SpawnSmoke(m_currentTarget.transform.position, 0.1f));
                            PutSandControl.Instance.m_fishControl.OnDeath();
                            GameControl.Instance.SpawnSmoke(PutSandControl.Instance.m_fishControl.gameObject, 0.0f);
                        }
                        else
                        {
                            PutSandControl.Instance.m_fishEndControl.OnDeath();
                            GameControl.Instance.SpawnSmoke(PutSandControl.Instance.m_fishEndControl.gameObject, 0f);
                        }

                    }
                    break;
                case "Bear":
                    {
                        BearControl bearControl = hitColliders[0].transform.GetComponent<BearControl>();
                        bearControl.OnDeath();
                        //SimplePool.Spawn("SmokeSushi", hitColliders[0].ClosestPoint, Quaternion.identity);
                    }
                    break;

                case "Enemy":
                    {
                        GameControl.Instance.SpawnSmoke(hitColliders[0].gameObject);
                        AnimalFind animalFind = hitColliders[0].transform.GetComponent<AnimalFind>();
                        animalFind.OnDeath();
                        //animalFind.transform.position = hit.point;
                    }
                    break;
            }
        }
    }

    void OnHit()
    {
        Debug.DrawLine(transform.position, transform.position + transform.right * 2f, Color.blue);
        Debug.DrawLine(transform.position, transform.position - transform.right * 2f, Color.blue);
        if (Physics.Raycast(transform.position, transform.right, out hit, 2f, LayerMask.GetMask("Fish", "Bear")))
        {
            HitProcess();
        }
        if (Physics.Raycast(transform.position, -transform.right, out hit, 2f, LayerMask.GetMask("Fish", "Bear")))
        {
            HitProcess();
        }
    }

    void HitProcess()
    {
        switch (hit.transform.tag)
        {
            case "Player":
                {
                    GameControl.Instance.m_isComplete = true;
                    UiManager.Instance.OpenPanel("Fail", 0.8f);
                    if (hit.transform.gameObject == PutSandControl.Instance.m_fishControl.gameObject)
                    {
                        //StartCoroutine(GameControl.Instance.SpawnSmoke(m_currentTarget.transform.position, 0.1f));
                        PutSandControl.Instance.m_fishControl.OnDeath();
                        GameControl.Instance.SpawnSmoke(PutSandControl.Instance.m_fishControl.gameObject, 0.0f);
                    }
                    else
                    {
                        PutSandControl.Instance.m_fishEndControl.OnDeath();
                        GameControl.Instance.SpawnSmoke(PutSandControl.Instance.m_fishEndControl.gameObject, 0f);
                    }

                }
                break;
            case "Bear":
                {
                    BearControl bearControl = hit.transform.GetComponent<BearControl>();
                    bearControl.OnDeath();
                    //SimplePool.Spawn("SmokeSushi", hitColliders[0].ClosestPoint, Quaternion.identity);
                }
                break;

            case "Enemy":
                {
                    GameControl.Instance.SpawnSmoke(hit.transform.gameObject);
                    AnimalFind animalFind = hit.transform.GetComponent<AnimalFind>();
                    animalFind.OnDeath();
                    //animalFind.transform.position = hit.point;
                }
                break;
        }
        
    }
}
