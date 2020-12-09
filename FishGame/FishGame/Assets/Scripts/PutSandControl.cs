using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayStyle
{
    RUSH, NORMAL
}

public class PutSandControl : MonoBehaviour
{
    public static PutSandControl Instance { get; set; }

    public float m_planeDistance = 20;
    public float m_radius = 0.5f;
    public float m_power = 0.25f;
    public FishControl m_fishControl;
    public FishEndControl m_fishEndControl;
    public ParticleSystem m_effectPutSand;


    [HideInInspector] public PlayStyle m_playStyle;
    [HideInInspector] public DeformSand m_curentSand;
    [HideInInspector] public ParticleSystem.EmissionModule m_emissionModule;

    private void Awake()
    {
        Instance = this;
        m_emissionModule = m_effectPutSand.emission;
    }
}