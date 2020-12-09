using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    public bool m_isDespawn = true;
    public float _time = 2.0f;

    private void OnEnable()
    {
        if (_time > 0)
        {
            StartCoroutine(DisableObject());
        }
    }

    private IEnumerator DisableObject()
    {
        float time = _time;
        while (time >0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        if (m_isDespawn)
        {
            SimplePool.Despawn(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
