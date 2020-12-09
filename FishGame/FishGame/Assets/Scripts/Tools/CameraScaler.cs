using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScaler : MonoBehaviour
{
    public CanvasScaler m_Canvas;
    public List<GameObject> m_TopBar;
    public List<GameObject> m_BotBar;
    public List<GameObject> m_ScaleObjects;
    public List<GameObject> m_UpObjects;
    public List<GameObject> m_DownObjects;

    void Awake()
    {
        Camera camera = GetComponent<Camera>();
        float ratio = (float)Screen.height / (float)Screen.width;
        float origin = camera.orthographicSize;
        if (ratio > 1.8f)
        {
            if (ratio >= 2.05f)
            {
                m_Canvas.matchWidthOrHeight = 0;
                float scale = (ratio / (16.0f / 9.0f));
                //camera.orthographicSize = origin * scale;
                camera.fieldOfView = 65f * scale * 0.93f;

                RectTransform canvasRect = m_Canvas.GetComponent<RectTransform>();
                float k = (1080f / canvasRect.sizeDelta.x);
                float bunnyOffset = 100f;
                for (int i = 0; i < m_TopBar.Count; i++)
                {
                    Vector3 topBarPos = m_TopBar[i].transform.localPosition;
                    topBarPos.y -= bunnyOffset;
                    m_TopBar[i].transform.localPosition = topBarPos;
                }

                for (int i = 0; i < m_BotBar.Count; i++)
                {
                    Vector3 botBarPos = m_BotBar[i].transform.localPosition;
                    botBarPos.y += bunnyOffset;
                    m_BotBar[i].transform.localPosition = botBarPos;
                }

                for (int i = 0; i < m_ScaleObjects.Count; i++)
                {
                    Vector3 currentScale = m_ScaleObjects[i].transform.localScale;
                    currentScale = currentScale * scale;
                    m_ScaleObjects[i].transform.localScale = currentScale;
                }

                for (int i = 0; i < m_UpObjects.Count; i++)
                {
                    Vector3 currentPos = m_UpObjects[i].transform.localPosition;
                    //currentPos.y += camera.orthographicSize - 9.6f - bunnyOffset / 100f;
                    m_UpObjects[i].transform.localPosition = currentPos;
                }

                for (int i = 0; i < m_DownObjects.Count; i++)
                {
                    Vector3 currentPos = m_DownObjects[i].transform.localPosition;
                    //currentPos.y -= camera.orthographicSize - 9.6f;
                    currentPos.y -= 65f/ (scale* 0.63f * camera.fieldOfView);
                    m_DownObjects[i].transform.localPosition = currentPos;
                }
            }
            else
            {
                float scale = (ratio / (16.0f / 9.0f));
                //camera.orthographicSize = origin * scale;
                camera.fieldOfView = 65f * scale * 0.93f;
                m_Canvas.matchWidthOrHeight = 0;

                for (int i = 0; i < m_ScaleObjects.Count; i++)
                {
                    Vector3 currentScale = m_ScaleObjects[i].transform.localScale;
                    currentScale = currentScale * scale;
                    m_ScaleObjects[i].transform.localScale = currentScale;
                }

                for (int i = 0; i < m_UpObjects.Count; i++)
                {
                    Vector3 currentPos = m_UpObjects[i].transform.localPosition;
                    currentPos.y += camera.orthographicSize - 9.6f;
                    m_UpObjects[i].transform.localPosition = currentPos;
                }

                for (int i = 0; i < m_DownObjects.Count; i++)
                {
                    Vector3 currentPos = m_DownObjects[i].transform.localPosition;
                    //currentPos.y -= camera.orthographicSize - 9.6f;
                    currentPos.y -= 65f / (scale * 0.63f * camera.fieldOfView);
                    m_DownObjects[i].transform.localPosition = currentPos;
                }
            }
        }
        else
        {
            m_Canvas.matchWidthOrHeight = 1;
        }
    }
}