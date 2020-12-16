using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEvent : MonoBehaviour
{
    public AnimalFind m_enemyControl;

    public void PlaySoundAttack()
    {
        m_enemyControl.m_audioSource.clip = m_enemyControl.m_clipAttack;
        m_enemyControl.m_audioSource.Play();
    }
}
