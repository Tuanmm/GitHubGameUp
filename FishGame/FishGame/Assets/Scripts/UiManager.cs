using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; set; }

    public GameObject m_panelStart;
    public GameObject m_panelPlay;
    public GameObject m_panelComplete;
    public GameObject m_panelFail;
    //public GameObject m_panelShop;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        OpenPanel("Start");
    }

    public void OpenPanel(string namePanel, float waittime = 0)
    {
        StartCoroutine(WaitToShowPanel(namePanel, waittime));
        //switch (namePanel)
        //{
        //    case "Start":
        //        {
        //            m_panelStart.SetActive(true);
        //            m_panelPlay.SetActive(false);
        //            m_panelComplete.SetActive(false);
        //            m_panelFail.SetActive(false);
        //            //m_panelShop.SetActive(false);
        //        }
        //        break;
        //    case "Play":
        //        {
        //            m_panelStart.SetActive(false);
        //            m_panelPlay.SetActive(true);
        //            m_panelComplete.SetActive(false);
        //            m_panelFail.SetActive(false);
        //            //m_panelShop.SetActive(false);   
        //        }
        //        break;

        //    case "Complete":
        //        {
        //            m_panelStart.SetActive(false);
        //            m_panelPlay.SetActive(false);
        //            m_panelComplete.SetActive(true);
        //            m_panelFail.SetActive(false);
        //            //m_panelShop.SetActive(false);
        //        }
        //        break;

        //    case "Fail":
        //        {
        //            m_panelStart.SetActive(false);
        //            m_panelPlay.SetActive(false);
        //            m_panelComplete.SetActive(false);
        //            m_panelFail.SetActive(true);
        //            //m_panelShop.SetActive(false);
        //        }
        //        break;

        //    case "Shop":
        //        {
        //            m_panelStart.SetActive(false);
        //            m_panelPlay.SetActive(false);
        //            m_panelComplete.SetActive(false);
        //            m_panelFail.SetActive(false);
        //            //m_panelShop.SetActive(true);
        //        }
        //        break;
        //}
    }

    IEnumerator WaitToShowPanel(string namePanel, float waittime)
    {
        yield return Yielders.Get(waittime);
        switch (namePanel)
        {
            case "Start":
                {
                    m_panelStart.SetActive(true);
                    m_panelPlay.SetActive(false);
                    m_panelComplete.SetActive(false);
                    m_panelFail.SetActive(false);
                    //m_panelShop.SetActive(false);
                }
                break;
            case "Play":
                {
                    m_panelStart.SetActive(false);
                    m_panelPlay.SetActive(true);
                    m_panelComplete.SetActive(false);
                    m_panelFail.SetActive(false);
                    //m_panelShop.SetActive(false);   
                }
                break;

            case "Complete":
                {
                    m_panelStart.SetActive(false);
                    m_panelPlay.SetActive(false);
                    m_panelComplete.SetActive(true);
                    m_panelFail.SetActive(false);
                    //m_panelShop.SetActive(false);
                }
                break;

            case "Fail":
                {
                    m_panelStart.SetActive(false);
                    m_panelPlay.SetActive(false);
                    m_panelComplete.SetActive(false);
                    m_panelFail.SetActive(true);
                    //m_panelShop.SetActive(false);
                }
                break;

            case "Shop":
                {
                    m_panelStart.SetActive(false);
                    m_panelPlay.SetActive(false);
                    m_panelComplete.SetActive(false);
                    m_panelFail.SetActive(false);
                    //m_panelShop.SetActive(true);
                }
                break;
        }
    }
}
