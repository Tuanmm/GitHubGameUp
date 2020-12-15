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
        m_humanControl.EventPlaySoundAttack();
    }
}
