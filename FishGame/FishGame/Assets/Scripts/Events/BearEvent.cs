using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearEvent : MonoBehaviour
{
    public BearControl m_bearControl;

    public void ShowSplash()
    {
        m_bearControl.EventShowSplash();
    }

    public void PlaySoundAttack()
    {
        if (UiManager.Instance.m_panelPlay.activeSelf)
        {
            m_bearControl.EventPlaySoundAttack();
        }
    }

    public void ShowEmoji()
    {
        if (m_bearControl.m_bearState == BearState.ON_WATER && !GameControl.Instance.m_isFail && !GameControl.Instance.m_isComplete)
        {
            GameControl.Instance.ShowEmojiSad(transform.position + new Vector3());
        }
    }
}
