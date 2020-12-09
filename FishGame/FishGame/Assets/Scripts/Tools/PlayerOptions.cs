using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerOptions
{
    public static string str_currentLevel = "CurrentLevel";
    public static string str_Sound = "Sound";
    public static string str_Vibrate = "Vibrate";
    public static string str_Money = "Money";
    public static string str_Name = "Name";

    public static int GetCurrentLevel()
    {
        int lv = PlayerPrefs.GetInt(str_currentLevel);
        return lv;
    }

    public static void SetCurrentlevel(int level)
    {
        PlayerPrefs.SetInt(str_currentLevel, level);
        PlayerPrefs.Save();
    }

    public static int GetMoney()
    {
        int money = PlayerPrefs.GetInt(str_Money);
        return money;
    }

    public static void AddMoney(int add)
    {
        int money = PlayerPrefs.GetInt(str_Money);
        money += add;
        PlayerPrefs.SetInt(str_Money, money);
    }

    public static int GetSound()
    {
        return PlayerPrefs.GetInt(str_Sound);
    }

    public static void SetSound(int value)
    {
        PlayerPrefs.SetInt(str_Sound, value);
        PlayerPrefs.Save();
    }

    public static int GetVibrate()
    {
        return PlayerPrefs.GetInt(str_Vibrate);
    }

    public static void SetVibrate(int value)
    {
        PlayerPrefs.SetInt(str_Vibrate, value);
        PlayerPrefs.Save();
    }

    public static string GetName()
    {
        return PlayerPrefs.GetString(str_Name);
    }

    public static void SetName(string str)
    {
        PlayerPrefs.SetString(str_Name, str);
        PlayerPrefs.Save();
    }
}