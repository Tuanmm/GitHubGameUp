using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMoveTutorial : MonoBehaviour
{
    public Spline m_StartSpline;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StopAllCoroutines();
            StartCoroutine(SplineMove(gameObject, m_StartSpline, 1f, false));
        }
    }

    public IEnumerator SplineMove(GameObject _objectMove, Spline _spline, float speedMove = 0.2f, bool _checkLookAt = true)
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

        //float t = 0.5f;
        //Vector3 dir = (_objectMove.transform.position - vectorData[1]).normalized;
        _objectMove.transform.position = vectorData[0];
        //Quaternion rotTo1 = Quaternion.LookRotation(-dir, Vector3.up);
        //_objectMove.transform.rotation = rotTo1;
        //if (dir != Vector3.zero)
        //{
        //    while (t > 0f)
        //    {
        //        t -= Time.deltaTime;
        //        Quaternion rotTo = Quaternion.LookRotation(-dir, Vector3.up);
        //        _objectMove.transform.rotation = Quaternion.Slerp(_objectMove.transform.rotation, rotTo, Time.deltaTime * 2f);
        //        yield return null;
        //    }
        //}

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
    }
}
