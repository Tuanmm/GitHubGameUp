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
    public GameObject m_groupSetting;

    public Button m_btnOpenSetting;
    public Button m_btnCloseSetting;
    public Button m_btnSound;
    public Button m_btnVibrate;
    public Button m_btnRetryOnplay;
    public Button m_btnRetryOnFail;
    public Button m_btnNext;

    public Sprite m_spOnSound;
    public Sprite m_spOffSound;
    public Sprite m_spOnVibrate;
    public Sprite m_spOffvibrate;

    public Image m_loadLv;
    public Text m_txtLevel;
    public Text m_txtOutLineLevel;

    bool m_checkOnClick;

    //public GameObject m_panelShop;

    private void Awake()
    {
        Instance = this;
        m_btnOpenSetting.onClick.AddListener(() => OnClickOpenSetting());
        m_btnCloseSetting.onClick.AddListener(() => OnClickCloseSetting());
        m_btnSound.onClick.AddListener(() => OnClickSound());
        m_btnVibrate.onClick.AddListener(() => OnClickVibrate());
        m_btnRetryOnplay.onClick.AddListener(() => OnClickRetryOnPlay());
        m_btnRetryOnFail.onClick.AddListener(() => OnClickRetryOnFail());
        m_btnNext.onClick.AddListener(() => OnClickNext());
        m_checkOnClick = true;
        if (PlayerPrefs.GetInt("FirstPlay") == 0)
        {
            PlayerOptions.SetSound(1);
            PlayerOptions.SetVibrate(1);
            PlayerPrefs.SetInt("FirstPlay", 1);
        }
    }

    private void Start()
    {
        SetSound(PlayerOptions.GetSound() == 1);
        SetVibrate(PlayerOptions.GetVibrate() == 1);
    }

    private void OnClickNext()
    {
        if (m_checkOnClick)
        {
            StartCoroutine(EffectButton(m_btnNext));
            StartCoroutine(WaitNext(0.3f));
        }
    }
    IEnumerator WaitNext(float time)
    {
        yield return Yielders.Get(time);
        GameControl.Instance.OnclickNext();
    }

    IEnumerator WaitRetry(float time)
    {
        yield return Yielders.Get(time);
        GameControl.Instance.OnclickRetry();
    }

    private void OnClickRetryOnFail()
    {
        if (m_checkOnClick)
        {
            StartCoroutine(EffectButton(m_btnRetryOnFail));
            StartCoroutine(WaitRetry(0.3f));
        }
    }

    private void OnClickRetryOnPlay()
    {
        if (m_checkOnClick)
        {
            StartCoroutine(EffectButton(m_btnRetryOnplay));
            StartCoroutine(WaitRetry(0.0f));
        }
    }

    private void OnClickOpenSetting()
    {
        if (m_checkOnClick)
        {
            StartCoroutine(EffectButton(m_btnOpenSetting));
            StartCoroutine(WaitOpenSetting(0.2f));
        }
    }

    IEnumerator WaitOpenSetting(float time)
    {
        yield return Yielders.Get(time);
        SetSound(PlayerOptions.GetSound() == 1);
        SetVibrate(PlayerOptions.GetVibrate() == 1);
        m_groupSetting.SetActive(true);
    }

    private void OnClickCloseSetting()
    {
        m_groupSetting.SetActive(false);
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
                    m_loadLv.gameObject.SetActive(false);
                    m_panelStart.SetActive(true);
                    m_panelPlay.SetActive(false);
                    m_panelComplete.SetActive(false);
                    m_panelFail.SetActive(false);
                    //m_panelShop.SetActive(false);
                }
                break;
            case "Play":
                {
                    StartCoroutine(LoadLevel());
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

    public void SetSound(bool isOn)
    {
        if (isOn)
        {
            m_btnSound.image.sprite = m_spOnSound;
            AudioListener.volume = 1.0f;
        }
        else
        {
            AudioListener.volume = 0.0f;
            m_btnSound.image.sprite = m_spOffSound;
        }
    }

    public void SetVibrate(bool isOn)
    {
        if (isOn)
        {
            m_btnVibrate.image.sprite = m_spOnVibrate;
        }
        else
        {
            m_btnVibrate.image.sprite = m_spOffvibrate;
        }
    }

    private void OnClickSound()
    {
        if (m_checkOnClick)
        {
            StartCoroutine(EffectButton(m_btnSound));
            if (PlayerOptions.GetSound() == 0)
            {
                SetSound(true);
                PlayerOptions.SetSound(1);
            }
            else
            {
                SetSound(false);
                PlayerOptions.SetSound(0);
            }
        }
    }

    private void OnClickVibrate()
    {
        if (m_checkOnClick)
        {
            StartCoroutine(EffectButton(m_btnVibrate));
            if (PlayerOptions.GetVibrate() == 0)
            {
                SetVibrate(true);
                PlayerOptions.SetVibrate(1);
            }
            else
            {
                SetVibrate(false);
                PlayerOptions.SetVibrate(0);
            }
        }
    }

    IEnumerator LoadLevel()
    {
        m_loadLv.gameObject.SetActive(true);
        Color color = m_loadLv.color;
        color.a = 1f;
        m_loadLv.color = color;
        while (color.a > 0.1f)
        {
            color.a -= Time.deltaTime* 2f;
            m_loadLv.color = color;
            yield return null;
        }
        m_loadLv.gameObject.SetActive(false);
    }

    public IEnumerator EffectButton(Button _objectScale)
    {
        m_checkOnClick = false;
        LeanTween.scale(_objectScale.gameObject, new Vector3(1.1f, 0.9f, 1f), 0.1f).setIgnoreTimeScale(false);
        yield return Yielders.Get(0.1f);
        LeanTween.scale(_objectScale.gameObject, Vector3.one, 0.15f).setIgnoreTimeScale(false);
        yield return Yielders.Get(0.2f);
        m_checkOnClick = true;
    }

}
