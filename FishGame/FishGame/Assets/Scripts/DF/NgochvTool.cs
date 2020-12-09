#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MenuItems
{
    [MenuItem("Ngochv/Clear PlayerPrefs")]
    private static void NewMenuOption()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Ngochv/Down Timescale")]
    private static void DownTimeScale()
    {
        Time.timeScale = 0.2f;
    }

    [MenuItem("Ngochv/Reset Timescale")]
    private static void ResettTimeScale()
    {
        Time.timeScale = 1.0f;
    }

    [MenuItem("Ngochv/Double Timescale")]
    private static void DoubleTimeScale()
    {
        Time.timeScale += 1.0f;
    }
}
#endif