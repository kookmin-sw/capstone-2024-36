using System;
using System.Collections.Generic;
using UnityEngine;

// key: registerId
// value: prefab in project

[CreateAssetMenu(fileName = "Network Register List", menuName = "Network/RegisterList", order = 1)]
public class NetworkRegisterList : ScriptableObject
{
    [Serializable]
    public struct Item {
        [SerializeField]
        public int RegisterId;

        [SerializeField]
        public GameObject Prefab;
    };

    public List<Item> List;
}
