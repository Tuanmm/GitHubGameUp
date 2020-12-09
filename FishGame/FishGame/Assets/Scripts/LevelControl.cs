using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    public Transform m_Start;
    public Transform m_End;
    public PlayStyle m_playStyle;
    public List<DeformSand> m_listdeformSand;
    public List<AnimalFind> m_listAnimalFind;
    public List<HumanControl> m_listHuman;
    public List<BearControl> m_listBear;
    public List<Spline> m_listSpline;

    private List<Vector3> m_listPosAnimalFind;

    [System.Serializable]
    public struct ObjectCanChangePos
    {
        public Rigidbody m_rigidbody;
        public Vector3 m_pos;
        public Quaternion m_rotation;
        public ObjectCanChangePos(Rigidbody _newRigidbody, Vector3 _newPos, Quaternion _newRot)
        {
            m_rigidbody = _newRigidbody;
            m_pos = _newPos;
            m_rotation = _newRot;
        }
    }

    private List<Rigidbody> m_listObjectHasRigidbody;
    private List<ObjectCanChangePos> m_listObjectCanChangePos;

    private void Awake()
    {
        m_listObjectHasRigidbody = new List<Rigidbody>();
        m_listObjectCanChangePos = new List<ObjectCanChangePos>();

        Rigidbody[] tempRig = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < tempRig.Length; i++)
        {
            m_listObjectHasRigidbody.Add(tempRig[i]);
            m_listObjectCanChangePos.Add(new ObjectCanChangePos(tempRig[i], tempRig[i].transform.position, tempRig[i].transform.rotation));
        }

        m_listPosAnimalFind = new List<Vector3>();
        foreach (var item in m_listAnimalFind)
        {
            item.gameObject.SetActive(false);
            Vector3 pos = item.transform.position;
            pos.y = -0.5f;
            m_listPosAnimalFind.Add(pos);
        }
        m_Start.gameObject.SetActive(false);
        m_Start.transform.position = new Vector3(m_Start.transform.position.x, -0.5f, m_Start.transform.position.z);
        m_End.gameObject.SetActive(false);
        m_End.transform.position = new Vector3(m_End.transform.position.x, -0.5f, m_End.transform.position.z);
    }

    private void OnEnable()
    {
        StartCoroutine(ResetLevel());
    }

    public IEnumerator ResetLevel()
    {
        yield return Yielders.EndOfFrame;
        PutSandControl.Instance.m_playStyle = m_playStyle;
        foreach (var item in m_listObjectCanChangePos)
        {
            item.m_rigidbody.velocity = Vector3.zero;
            item.m_rigidbody.angularVelocity = Vector3.zero;
            item.m_rigidbody.transform.position = item.m_pos;
            item.m_rigidbody.transform.rotation = item.m_rotation;
        }
        yield return Yielders.EndOfFrame;
        foreach (var item in m_listdeformSand)
        {
            item.PutholeStart(m_Start.position, PutSandControl.Instance.m_radius * 3, PutSandControl.Instance.m_power);
            item.PutholeStart(m_End.position, PutSandControl.Instance.m_radius * 3, PutSandControl.Instance.m_power);
        }

        yield return Yielders.EndOfFrame;
        PutSandControl.Instance.m_fishControl.gameObject.SetActive(false);
        PutSandControl.Instance.m_fishControl.transform.position = m_Start.position;
        PutSandControl.Instance.m_fishControl.m_listTarget = new List<Vector3>();
        PutSandControl.Instance.m_fishControl.m_listTarget.Add(m_End.position);
        PutSandControl.Instance.m_fishControl.gameObject.SetActive(true);

        PutSandControl.Instance.m_fishEndControl.gameObject.SetActive(false);
        PutSandControl.Instance.m_fishEndControl.transform.position = m_End.position;
        PutSandControl.Instance.m_fishEndControl.gameObject.SetActive(true);
        GameControl.Instance.LookAtTarget(PutSandControl.Instance.m_fishEndControl.gameObject, m_Start.position,360f);
        GameControl.Instance.LookAtTarget(PutSandControl.Instance.m_fishControl.gameObject, m_End.position,360f);
   

        int id = 0;
        foreach (var item in m_listAnimalFind)
        {
            item.gameObject.SetActive(false);
            item.transform.position = m_listPosAnimalFind[id];
            foreach (var sand in m_listdeformSand)
            {
                sand.PutLineStart(item, m_listPosAnimalFind[id], item.m_lineDistance, item.m_lineDirection,PutSandControl.Instance.m_radius * 2, PutSandControl.Instance.m_power);
            }

            PutSandControl.Instance.m_fishControl.m_listTarget.Add(m_listPosAnimalFind[id]);
            item.gameObject.SetActive(true);
            id++;
        }

        yield return Yielders.Get(0.5f);

        if (GameControl.Instance.m_checkHint && m_listSpline.Count >0)
        {
            GameControl.Instance.MoveHint(m_listSpline[0]);
        }
        //PutSandControl.Instance.m_curentSand = m_listdeformSand;
    }
}
