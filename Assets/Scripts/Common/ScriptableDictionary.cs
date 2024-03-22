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

    // serialize�ϱ����� dictionary�� �ִ� key���� value�� �Ű� �ִ´�.
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

    // Deserialize�ϰ� ���� List�� �ִ� key���� value�� dictionary�� �Ű� �ִ´�.
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

