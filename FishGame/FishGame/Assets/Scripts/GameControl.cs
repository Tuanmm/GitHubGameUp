using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LineDirection
{
    HORIZONTAL, VERTICAL, DIAGONAL, ANTIDIAGONAL
}

public enum HumanStyle
{
    STAND, MOVE
}

public class GameControl : MonoBehaviour
{
    public static GameControl Instance { get; set; }

    [HideInInspector] public LevelControl m_curentLevelControl;
    public PutSandControl m_putSandControl;
    public bool m_isComplete;
    public bool m_isFail;
    public List<LevelControl> m_listLevel;
    public int m_curentLevel;
    public GameObject m_fxComplete;
    public GameObject m_hand;
    public bool m_checkHint;

    public List<GameObject> m_listEnv;
    private int m_idEnv;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;

        if (Instance == null)
        {
            Instance = this;
        }

        foreach (var item in m_listLevel)
        {
            item.gameObject.SetActive(false);
        }
        m_fxComplete.SetActive(false);

        foreach (var item in m_listEnv)
        {
            item.SetActive(false);
        }
        //m_curentLevel = PlayerOptions.GetCurrentLevel();
    }

    private void Start()
    {
        StartGame();
    }

    public void LoadLevel(int level)
    {
        ShowEnv(m_idEnv);
        ResetSushi();
        ResetSmokeFighting();
        if (level <= 1)
        {
            m_checkHint = true;
        }
        else
        {
            m_checkHint = false;
        }
        m_fxComplete.SetActive(false);
        m_putSandControl.m_fishControl.gameObject.SetActive(false);
        if (m_curentLevelControl)
        {
            foreach (var item in m_curentLevelControl.m_listdeformSand)
            {
                foreach (var item1 in item.m_listObstacleDespawn)
                {
                    if (item1.activeSelf)
                    {
                        SimplePool.Despawn(item1);
                    }
                }
                foreach (var item1 in item.m_listObstacle)
                {
                    if (item1.activeSelf)
                    {
                        SimplePool.Despawn(item1);
                    }
                }
            }
            m_curentLevelControl.gameObject.SetActive(false);
        }
        m_curentLevelControl = m_listLevel[level];
        CameraFollower.Instance.ResetCamPos();
        UiManager.Instance.OpenPanel("Play");
        m_isComplete = false;
        m_isFail = false;
        PutSandControl.Instance.m_effectPutSand.Stop();
        m_putSandControl.m_emissionModule.rateOverDistance = 0;
        m_hand.SetActive(false);
        UiManager.Instance.m_txtLevel.text = "Level " + m_curentLevel;
        UiManager.Instance.m_txtOutLineLevel.text = "Level " + m_curentLevel;
        m_curentLevelControl.gameObject.SetActive(true);
    }

    public void OnclickRetry()
    {
        //if (!m_isComplete || !m_isFail)
        {
            LoadLevel(m_curentLevel);
        }
    }

    public void OnclickNext()
    {
        m_idEnv = UnityEngine.Random.Range(0, m_listEnv.Count);
        m_curentLevel++;
        if (m_curentLevel >= m_listLevel.Count)
        {
            m_curentLevel = 0;
        }
        LoadLevel(m_curentLevel);
        PlayerOptions.SetCurrentlevel(m_curentLevel);
    }

    public void OnClickStart()
    {
        //m_idEnv = UnityEngine.Random.Range(0, m_listEnv.Count);
        //LoadLevel(m_curentLevel);
        UiManager.Instance.OpenPanel("Play");
        GameControl.Instance.m_isComplete = false;
    }

    public void StartGame()
    {
        m_idEnv = UnityEngine.Random.Range(0, m_listEnv.Count);
        LoadLevel(m_curentLevel);
        UiManager.Instance.OpenPanel("Start");
    }

    public void LookAtTarget(GameObject _objectLook, Vector3 _target, float _speed)
    {
        _target.y = _objectLook.transform.position.y;
        Vector3 _lookDirection = _target - _objectLook.transform.position;
        if (_lookDirection != Vector3.zero)
        {
            Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
            _objectLook.transform.rotation = Quaternion.Lerp(_objectLook.transform.rotation, _rot, _speed);
        }
    }

    public IEnumerator SplineMove(GameObject _objectMove, Spline _spline, float speedMove = 0.2f, bool _checkLookAt = true)
    {
        while (_objectMove.activeSelf)
        {
            Vector3[] vectorData = new Vector3[_spline.SplineNodes.Length];
            int lengthVectorData = 0;
            float moveTime = 0;
            Vector3 targetLookAt = new Vector3();

            if (_spline.SplineNodes.Length > 0)
            {
                float start = _spline.SegmentNodes[0].Parameters[_spline].PosInSpline;
                vectorData = new Vector3[_spline.SplineNodes.Length];
                for (int i = 0; i < _spline.SplineNodes.Length; i++)
                {
                    float p = _spline.SegmentNodes[i].Parameters[_spline].PosInSpline;
                    Vector3 v = _spline.GetPositionOnSpline(p);
                    v.y = _objectMove.transform.position.y;
                    vectorData[i] = v;
                }
                lengthVectorData = vectorData.Length - 1;
                moveTime = 0;
            }

            _objectMove.transform.position = vectorData[0];

            while (moveTime <= 1f)
            {
                moveTime += Time.deltaTime * speedMove;
                float parameter = moveTime;
                int nodeIndex = Mathf.FloorToInt((vectorData.Length - 1) * parameter);
                float segmentLength = 1f / (vectorData.Length - 1);
                float segmentParameter = (parameter - (nodeIndex * segmentLength)) / segmentLength;
                SplineInterpolator splineInterpolator = new HermiteInterpolator();
                Vector3 positionOnSpline = splineInterpolator.InterpolateVector(segmentParameter, nodeIndex, false, vectorData, 0);
                _objectMove.transform.position = positionOnSpline;
                if (nodeIndex < lengthVectorData && _checkLookAt)
                {
                    targetLookAt = vectorData[nodeIndex + 1];
                    targetLookAt.y = _objectMove.transform.position.y;
                    Vector3 direction = (_objectMove.transform.position - targetLookAt).normalized;
                    if (direction != Vector3.zero)
                    {
                        Quaternion rotTo = Quaternion.LookRotation(-direction, Vector3.up);
                        _objectMove.transform.rotation = Quaternion.Slerp(_objectMove.transform.rotation, rotTo, Time.deltaTime * 2f);
                    }
                }
                yield return new WaitForFixedUpdate();
            }
            yield return Yielders.Get(0.25f);
        }
    }

    public void MoveHint(Spline _spline)
    {
        StopAllCoroutines();
        m_hand.SetActive(true);
        StartCoroutine(SplineMove(m_hand, _spline, 1f, false));
    }

    private List<GameObject> m_listEffectSmokeFighting = new List<GameObject>();

    public void OnSpawnSmokeFighting(Vector3 _pos)
    {
        _pos.y = 2f;
        GameObject objSpawn = SimplePool.Spawn("SmokeFighting", _pos, Quaternion.identity);
        m_listEffectSmokeFighting.Add(objSpawn);
    }

    private void ResetSmokeFighting()
    {
        if (m_listEffectSmokeFighting.Count >0)
        {
            foreach (var item in m_listEffectSmokeFighting)
            {
                SimplePool.Despawn(item);
            }
            m_listEffectSmokeFighting.Clear();
        }
    }


    private List<GameObject> m_listFxSushi = new List<GameObject>();

    public void OnSpawnSushi(GameObject obj, float time = 0f)
    {
        StartCoroutine(WaitSpawnSushi(obj, time));
    }

    IEnumerator WaitSpawnSushi(GameObject obj, float time = 0f)
    {
        yield return Yielders.Get(time);
        Vector3 _pos = obj.transform.position;
        _pos.y = 0;
        GameObject fx = SimplePool.Spawn("FxSushi", _pos, Quaternion.identity);
        fx.transform.localEulerAngles = new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        m_listFxSushi.Add(fx);
    }

    private void ResetSushi()
    {
        if (m_listFxSushi.Count > 0)
        {
            foreach (var item in m_listFxSushi)
            {
                SimplePool.Despawn(item);
            }
            m_listFxSushi.Clear();
        }
    }

    public void SpawnSmoke(GameObject objSpawn, float time = 0f)
    {
        StartCoroutine(SpawnSmokeProcess(objSpawn, time));
    }
    private IEnumerator SpawnSmokeProcess(GameObject objSpawn,float time)
    {
        yield return Yielders.Get(time);
        SimplePool.Spawn("SmokeSushi", objSpawn.transform.position, Quaternion.identity);
    }

    private void ShowEnv(int id)
    {
        for (int i = 0; i < m_listEnv.Count; i++)
        {
            if (i == id)
            {
                m_listEnv[i].SetActive(true);
            }
            else
            {
                m_listEnv[i].SetActive(false);
            }
        }
    }

}