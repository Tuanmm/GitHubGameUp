using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanEvent : MonoBehaviour
{
    public HumanControl m_humanControl;
    public void ShowSpash()
    {
        m_humanControl.EventShowSplash();
    }

    public void PlaySoundAttack()
    {
        if (UiManager.Instance.m_panelPlay.activeSelf)
        {
            m_humanControl.EventPlaySoundAttack();
        }
    }

    public void ShowEmoji()
    {
        if (m_humanControl.m_checkSwim && !GameControl.Instance.m_isFail && !GameControl.Instance.m_isComplete)
        {
            GameControl.Instance.ShowEmojiSad(transform.position + new Vector3());
        }
    }
}
