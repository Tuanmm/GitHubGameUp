using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeformSand : MonoBehaviour
{
    private MeshFilter m_meshFilter;
    private Mesh m_planeMesh;
    private Vector3[] m_arrVerts;
    private MeshCollider m_meshCollider;

    public Vector3[] m_arrStartVerts;

    public Vector3 m_planeCenters;
    bool m_checkChangeMesh;

    public int m_numCol = 10;
    public int m_numRow = 10;
    [HideInInspector] public List<GameObject> m_listObstacleDespawn = new List<GameObject>();
    [HideInInspector] public List<GameObject> m_listObstacle = new List<GameObject>();

    private Ray m_ray;
    private Camera m_cam;
    private RaycastHit m_hit;
    bool m_checkPut;
    public Vector3 m_maxPosSize; 
    public Vector3 m_minPosSize; 

    void Awake()
    {
        m_meshFilter = GetComponent<MeshFilter>();
        m_planeMesh = m_meshFilter.mesh;
        m_arrStartVerts = m_planeMesh.vertices;
        m_meshCollider = transform.GetComponent<MeshCollider>();
        m_planeCenters = transform.GetComponent<Renderer>().bounds.center;
        m_cam = Camera.main;
        m_maxPosSize = m_meshCollider.bounds.center + m_meshCollider.bounds.size / 2 - Vector3.one * PutSandControl.Instance.m_radius * 1.2f;
        m_minPosSize = m_meshCollider.bounds.center - m_meshCollider.bounds.size / 2 + Vector3.one * PutSandControl.Instance.m_radius * 1.2f;
    }

    private void OnEnable()
    {
        m_planeMesh.vertices = m_arrStartVerts;
        m_meshCollider.sharedMesh = m_planeMesh;
        m_arrVerts = m_planeMesh.vertices;

        if (m_listObstacle.Count > 0)
        {
            foreach (var item in m_listObstacle)
            {
                if (item.activeSelf)
                {
                    SimplePool.Despawn(item);
                }
            }
        }

        if (m_listObstacleDespawn.Count > 0)
        {
            foreach (var item in m_listObstacleDespawn)
            {
                if (item.activeSelf)
                {
                    SimplePool.Despawn(item);
                }
            }
        }

        m_listObstacle.Clear();
        m_listObstacleDespawn.Clear();
        m_listObstacle = new List<GameObject>();
        m_listObstacleDespawn = new List<GameObject>();

        Vector3 pos = Vector3.down * 0.5f;
        for (int i = 0; i < m_numCol * 2.5f; i++)
        {
            for (int j = 0; j < m_numRow * 2.5f; j++)
            {
                pos.x = gameObject.transform.position.x - (float)m_numCol / 2f + (float)i / 2.5f + 0.2f;
                pos.z = gameObject.transform.position.z - (float)m_numRow / 2f + (float)j / 2.5f + 0.2f;
                GameObject obj = SimplePool.Spawn("Obstacle", pos, Quaternion.identity);
                //obj.transform.SetParent(transform);
                m_listObstacle.Add(obj);

            }
        }
        m_checkChangeMesh = false;
        //EasyTouch.On_TouchUp += On_SwipeEnd;
    }


    private void Update()
    {
        if (!GameControl.Instance.m_isComplete && !GameControl.Instance.m_isFail && !UiManager.Instance.m_groupSetting.activeSelf && UiManager.Instance.m_panelPlay.activeSelf)
        {
            switch (PutSandControl.Instance.m_playStyle)
            {
                case PlayStyle.RUSH:
                    CameraFollower.Instance.MoveToTarget();
                    break;
                case PlayStyle.NORMAL:

                    break;
            }

            if (Input.GetMouseButtonUp(0))
            {
                TouchUpProcess();
            }
            if (Input.GetMouseButton(0))
            {
                m_ray = m_cam.ScreenPointToRay(Input.mousePosition);
                DeformMesh();
            }
        }
    }

    //Vector3 m_OldPoint = Vector3.up;
    void DeformMesh()
    {
        if (Physics.Raycast(m_ray, out m_hit, LayerMask.GetMask("Sand")))
        {
            Vector3 _point = m_hit.point;
            _point.y = 0;
            {
                _point = LimitPut(_point);
                m_checkPut = true;
                Puthole(_point, PutSandControl.Instance.m_radius, PutSandControl.Instance.m_power);
                if (PutSandControl.Instance.m_playStyle == PlayStyle.RUSH)
                {
                    PutSandControl.Instance.m_fishControl.MoveFollowMouse(_point);

                }
                PutSandControl.Instance.m_effectPutSand.transform.position = _point;

                if (GameControl.Instance.m_hand.activeSelf)
                {
                    GameControl.Instance.m_hand.SetActive(false);
                }
                //if (PutSandControl.Instance.m_fishControl.m_bubble.activeSelf)
                //{
                //    PutSandControl.Instance.m_fishControl.m_bubble.SetActive(false);
                //}
            }
        }
        else
        {
            if (m_checkPut)
            {
                m_checkPut = false;
                StartCoroutine(UpdateMesh(0.5f));
            }
        }
    }

    public void TouchUpProcess()
    {
        if (m_checkChangeMesh)
        {
            m_checkChangeMesh = false;
            StartCoroutine(UpdateMesh(0.5f));
        }

        foreach (var item in m_listObstacleDespawn)
        {
            SimplePool.Despawn(item);
        }
        m_listObstacleDespawn.Clear();

        if (PutSandControl.Instance.m_fishControl.gameObject.activeSelf)
        {
            PutSandControl.Instance.m_fishControl.FindLove();
            PutSandControl.Instance.m_emissionModule.rateOverDistance = 0;
        }
        if (PutSandControl.Instance.m_effectPutSand.isPlaying)
        {
            PutSandControl.Instance.m_effectPutSand.Stop();
        }
        //PutSandControl.Instance.m_fishControl.m_animator.speed = 1;
    }

    public bool m_check = true;

    public void Puthole(Vector3 _positionToDeform, float _radius, float _power)
    {
        Vector3 _tempPos= transform.InverseTransformPoint(_positionToDeform);
        _tempPos.y = 0;
        bool somethingDeformed = false;
        for (int i = 0; i < m_arrVerts.Length; i++)
        {
            float dist = new Vector3(m_arrVerts[i].x - _tempPos.x, 0, m_arrVerts[i].z - _tempPos.z).sqrMagnitude;

            if (dist <= _radius)
            {
                somethingDeformed = true;
                if (m_check)
                {
                    m_arrVerts[i] -= Vector3.up * _power * (_radius - dist) * 2f;
                }
                else
                {
                    float temp = _power * (_radius - dist) * 0.1f;
                    StartCoroutine(ProcessPutHole(i, temp, 0.5f , _power * (_radius - dist) * 2f));
                }

            }
        }
        if (somethingDeformed)
        {
            m_checkChangeMesh = true;
            CreatePath(_positionToDeform, _radius * 0.8f);
            m_planeMesh.vertices = m_arrVerts;
            m_meshCollider.sharedMesh = m_planeMesh;

            if (!PutSandControl.Instance.m_effectPutSand.isPlaying)
            {
                PutSandControl.Instance.m_effectPutSand.Play();
            }
            PutSandControl.Instance.m_emissionModule.rateOverDistance = 10;
        }
        else
        {
            PutSandControl.Instance.m_emissionModule.rateOverDistance = 0;
            m_planeMesh.vertices = m_arrVerts;
            m_meshCollider.sharedMesh = m_planeMesh;
        }

    }

    IEnumerator UpdateMesh(float _time)
    {
        _time -= Time.deltaTime;
        while (_time > 0)
        {
            _time -= Time.deltaTime;
            m_planeMesh.vertices = m_arrVerts;
            m_meshCollider.sharedMesh = m_planeMesh;
            yield return null;
        }
    }

    IEnumerator ProcessPutHole(int i, float _delta , float _time, float _max = 0.5f)
    {
        if (_max > 0.05f)
        {
            _max = 0.5f;
        }
        else
        {
            _max /= 4f;
        }
        while ( _time  > 0)
        {
            if (m_arrVerts[i].y < -0.75f)
            {
                break;
            }
            if (_max > _delta)
            {
                m_arrVerts[i].y -=_delta;
                _max -= _delta;
            }
            else
            {
                break;
            }
            _time -= Time.deltaTime;
            yield return null;
        }
    }

    public void PutholeStart(Vector3 _positionToDeform, float _radius, float _power)
    {
        Vector3 _tempPos = transform.InverseTransformPoint(_positionToDeform);
        bool somethingDeformed = false;
        for (int i = 0; i < m_arrVerts.Length; i++)
        {
            float dist = (m_arrVerts[i] - _tempPos).sqrMagnitude;
            if (dist < _radius)
            {
                somethingDeformed = true;
                m_arrVerts[i] -= Vector3.up * _power * (_radius - dist) * 1.8f;
            }

        }
        if (somethingDeformed)
        {
            CreatePath(_positionToDeform ,_radius * 0.55f, true);
            m_planeMesh.vertices = m_arrVerts;
            m_meshCollider.sharedMesh = m_planeMesh;
        }
    }

    public void CreatePath(Vector3 _position, float _radius, bool _isStart = false)
    {
        for (int i = m_listObstacle.Count -1; i >- 0; i--)
        {
            if (m_listObstacle[i].activeSelf)
            {
                Vector3 tempPos = m_listObstacle[i].transform.position;
                if (Vector2.Distance(new Vector2(tempPos.x, tempPos.z), new Vector2(_position.x, _position.z)) < _radius)
                {
                    if (/*PutSandControl.Instance.m_playStyle == PlayStyle.RUSH ||*/ _isStart)
                    {
                        SimplePool.Despawn(m_listObstacle[i]);
                    }
                    else
                    {
                        m_listObstacleDespawn.Add(m_listObstacle[i]);
                    }

                    m_listObstacle.RemoveAt(i);
                }
            }
        }

    }

    public void PutLineStart(AnimalFind _animalFind ,Vector3 _point, float _distance, LineDirection _lineDirection, float _radius, float _power)
    {
        Vector3 _pointFirst = Vector3.zero;
        Vector3 _pointSecond = Vector3.zero;

        //_pointFirst = _point + _animalFind.transform.forward * _distance;
        //_pointSecond = _point - _animalFind.transform.forward * _distance;
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
        int count = (int)(_distance / (_radius / 6f));

        for (int i = 0; i <= count; i++)
        {
            PutholeStart(Vector3.Lerp(_pointFirst, _pointSecond, (float)i / count), _radius, _power);
        }
        _animalFind.m_firstPoint = _pointFirst;
        _animalFind.m_endPoint = _pointSecond;
    }

    public void PutLineHole(Vector3 firstPoint, Vector3 endPoint, float _radius, float _power)
    {
        float _distance = Vector3.Distance(firstPoint, endPoint);
        int count = (int)(_distance / (_radius / 2f));
        for (int i = 0; i <= count; i++)
        {
            Puthole(Vector3.Lerp(firstPoint, endPoint, (float)i / count), _radius, _power);
        }
    }

    public Vector3 LimitPut(Vector3 point)
    {
        if (point.x < m_minPosSize.x)
        {
            point.x = m_minPosSize.x;
        }
        if (point.z < m_minPosSize.z)
        {
            point.z = m_minPosSize.z;
        }

        if (point.x > m_maxPosSize.x)
        {
            point.x = m_maxPosSize.x;
        }
        if (point.z > m_maxPosSize.z)
        {
            point.z = m_maxPosSize.z;
        }
        return point;
    }
}