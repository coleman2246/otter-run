using System;
using System.Collections.Generic;
using UnityEngine;

// Needed a way to serialize a dict. Dont want to use a list because lookup will take take a while
// For some reason, unity does not let you do that out of the box
// Anyways, this entire class is taken from:
// https://answers.unity.com/questions/460727/how-to-serialize-dictionary-with-unity-serializati.html

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        if(keys.Count != values.Count)
        {
             throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
        }
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}

