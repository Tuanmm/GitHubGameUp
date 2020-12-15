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
        m_bearControl.EventPlaySoundAttack();
    }
}
