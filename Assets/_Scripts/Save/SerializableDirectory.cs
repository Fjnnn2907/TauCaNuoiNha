using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDirectory<Tkey, Tvalue> : Dictionary<Tkey, Tvalue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<Tkey> keys = new List<Tkey>();
    [SerializeField] private List<Tvalue> values = new List<Tvalue>();
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
            Debug.Log("khong co gia tri key va value");

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach(KeyValuePair<Tkey, Tvalue> pairs in this)
        {
            keys.Add(pairs.Key);
            values.Add(pairs.Value);
        }
    }
}
