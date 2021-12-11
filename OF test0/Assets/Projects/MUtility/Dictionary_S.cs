using System;
using System.Collections.Generic;
using UnityEngine;
namespace MUtility
{
    [Serializable]
    public class Dictionary_S<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {

        [Serializable]
        public struct Pair
        {
            public Pair(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }
            public TKey key;
            public TValue value;
        }

        public List<Pair> pair_list = new List<Pair>();

        public void OnBeforeSerialize()
        {
            pair_list.Clear();

            foreach (var kvp in this)
            {
                pair_list.Add( new Pair( kvp.Key, kvp.Value ) );
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            for (int i = 0; i < pair_list.Count; i++)
            {
                this[pair_list[i].key] = pair_list[i].value;
            }
        }
    }
}
