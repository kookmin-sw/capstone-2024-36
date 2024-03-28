using System.Collections.Generic;
using UnityEngine;
using System;

// REF: https://pinelike.tistory.com/271
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    public List<TKey> keys = new List<TKey>();
    [SerializeField]
    public List<TValue> values = new List<TValue>();

    // serialize하기전에 dictionary에 있는 key값과 value를 옮겨 넣는다.
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        var enumer = GetEnumerator();
        while (enumer.MoveNext())
        {
            keys.Add(enumer.Current.Key);
            values.Add(enumer.Current.Value);
        }
    }

    // Deserialize하고 나서 List에 있는 key값과 value를 dictionary에 옮겨 넣는다.
    public void OnAfterDeserialize()
    {
        this.Clear();
        int keysCount = keys.Count;
        int valuesCount = values.Count;

        if (keysCount == 0)
        {
            return;
        }

        for (int i = 0; i < keysCount; i++)
        {
            if (i >= values.Count)
                continue;

            this.Add(keys[i], values[i]);
        }
    }
}

