using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEndControl : MonoBehaviour
{
    [HideInInspector] public Animator m_animator;

    //public ParticleSystem m_effectSplash;

    private void Awake()
    {
        m_animator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void OnEnable()
    {
        //m_effectSplash.gameObject.SetActive(true);
        m_animator.Play("Idle");
        //m_animator.SetTrigger("Idle");

    }
    private void Update()
    {
        if (!GameControl.Instance.m_isComplete)
        {
            GameControl.Instance.LookAtTarget(gameObject, PutSandControl.Instance.m_fishControl.transform.position, 80f*Time.deltaTime);
        }
    }

    public IEnumerator WaitTimeToEat(Vector3 startPos)
    {
        transform.LookAt(PutSandControl.Instance.m_fishControl.transform);
        GameControl.Instance.m_fxComplete.transform.position = transform.position + Vector3.up;
        GameControl.Instance.m_fxComplete.SetActive(true);
        yield return Yielders.Get(0.1f);
        m_animator.Play("Happy");
        UiManager.Instance.OpenPanel("Complete", 1.0f);
        yield return Yielders.Get(0.5f);
        SimplePool.Spawn("Splash", transform.position, Quaternion.identity);
        m_animator.ResetTrigger("Happy");
        m_animator.Play("Swim");
        m_animator.SetFloat("Speed", 2);

        Vector3 _point = transform.position;
        LeanTween.move(gameObject, startPos + (_point - startPos).normalized * 1f, 0.8f);
        //yield return Yielders.Get(0.9f);
        //float speedRot = 2f;
        //while (gameObject.activeSelf)
        //{
        //    if (speedRot < 200f)
        //    {
        //        speedRot += Time.deltaTime;
        //    }
        //    transform.RotateAround(_point, Vector3.up, 200f * Time.deltaTime);
        //    Vector3 _lookDirection = _point - transform.position;
        //    _lookDirection = Vector3.Cross(_lookDirection.normalized, Vector3.up);
        //    Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, _rot, speedRot * Time.deltaTime);
        //    yield return null;
        //}
    }

    public IEnumerator Test(Vector3 _point)
    {
        float speedRot = 2f;
        while (gameObject.activeSelf)
        {
            if (speedRot < 200f)
            {
                speedRot += Time.deltaTime;
            }
            transform.RotateAround(_point, Vector3.up, 200f * Time.deltaTime);
            Vector3 _lookDirection = _point - transform.position;
            _lookDirection = Vector3.Cross(_lookDirection.normalized, Vector3.up);
            Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, _rot, speedRot * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDeath()
    {
        GameControl.Instance.m_isFail = true;
        //m_effectSplash.gameObject.SetActive(false);
        m_animator.Play("Death");
        m_animator.SetTrigger("Death");
    }
}
