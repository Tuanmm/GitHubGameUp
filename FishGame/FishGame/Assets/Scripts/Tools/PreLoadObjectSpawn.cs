using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreLoadObjectSpawn : MonoBehaviour
{
    [System.Serializable]
    public struct InfoObjectSpawn
    {
        public GameObject _prefab;
        public string _strnName;
        public int _numSpawn;
    }

    public List<InfoObjectSpawn> m_listObjectSpawn;

    void Awake()
    {
        foreach (var item in m_listObjectSpawn)
        {
            SimplePool.Preload(item._prefab, item._numSpawn, item._strnName);
        }
    }
}
